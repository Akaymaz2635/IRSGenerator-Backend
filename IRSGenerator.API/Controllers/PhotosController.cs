using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Photo;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotosController : ControllerBase
{
    private readonly IPhotoRepository _repo;
    private readonly IWebHostEnvironment _env;

    public PhotosController(IPhotoRepository repo, IWebHostEnvironment env)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    // GET /api/photos?inspection_id=X   veya   ?defect_id=X
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoReadDto>>> GetAll(
        [FromQuery] long? inspection_id = null,
        [FromQuery] long? defect_id = null)
    {
        IEnumerable<Photo> items;

        if (inspection_id.HasValue)
            items = await _repo.GetByInspectionAsync(inspection_id.Value);
        else if (defect_id.HasValue)
            items = await _repo.GetByDefectAsync(defect_id.Value);
        else
            items = await _repo.GetAllAsync();

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PhotoReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    // GET /api/photos/{id}/file  →  fotoğraf dosyasını yönlendir
    [HttpGet("{id:long}/file")]
    public async Task<IActionResult> GetFile(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Redirect(entity.Filepath);
    }

    // POST /api/photos?inspection_id=X&defect_ids=Y&defect_ids=Z
    // Body: multipart/form-data  →  file=<binary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PhotoReadDto>> Upload(
        [FromQuery] long inspection_id,
        [FromQuery] long[]? defect_ids,
        IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "Dosya gönderilmedi." });

        // wwwroot/photos/ klasörüne kaydet
        var photosDir = Path.Combine(_env.WebRootPath, "photos");
        Directory.CreateDirectory(photosDir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";
        var filename = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(photosDir, filename);

        await using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream);

        var filepath = $"/photos/{filename}";

        var entity = new Photo
        {
            InspectionId = inspection_id,
            Filename = filename,
            Filepath = filepath
        };
        var created = await _repo.AddAsync(entity);

        if (defect_ids is { Length: > 0 })
            foreach (var did in defect_ids)
                await _repo.LinkDefectAsync(created.Id, did);

        // Tekrar getir (PhotoDefects yüklü)
        var refreshed = await _repo.GetByIdAsync(created.Id,
            q => q.Include(p => p.PhotoDefects)) ?? created;

        return CreatedAtAction(nameof(GetById), new { id = refreshed.Id }, ToReadDto(refreshed));
    }

    // PUT /api/photos/{id}/defects
    [HttpPut("{id:long}/defects")]
    public async Task<IActionResult> SetDefects(long id, [FromBody] long[] defectIds)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        await _repo.SetDefectsAsync(id, defectIds);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        // Fiziksel dosyayı da sil (varsa)
        if (!string.IsNullOrEmpty(entity.Filepath))
        {
            var fullPath = Path.Combine(_env.WebRootPath, entity.Filepath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }

        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static PhotoReadDto ToReadDto(Photo p) => new()
    {
        Id = p.Id,
        InspectionId = p.InspectionId,
        Filename = p.Filename,
        Filepath = p.Filepath,
        DefectIds = p.PhotoDefects.Select(pd => pd.DefectId).ToList(),
        CreatedAt = p.CreatedAt
    };
}
