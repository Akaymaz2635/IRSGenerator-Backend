using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Photo;
namespace MES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _service;
    private readonly IWebHostEnvironment _env;
    public PhotosController(IPhotoService service, IWebHostEnvironment env)
    {
        _service = service;
        _env     = env;
    }

    private string UploadPath => Path.Combine(_env.ContentRootPath, "uploads");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoReadDto>>> GetAll(
        [FromQuery] long? inspection_id = null,
        [FromQuery] long? defect_id     = null)
        => Ok(await _service.GetAllAsync(inspection_id, defect_id));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PhotoReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:long}/file")]
    public async Task<IActionResult> GetFile(long id)
    {
        try
        {
            var (content, contentType, fileName) = await _service.GetFileAsync(id, UploadPath);
            return File(content, contentType, fileName);
        }
        catch (Exception) { return NotFound(); }
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<PhotoReadDto>> Upload(
        IFormFile file,
        [FromQuery] long inspection_id)
    {
        if (file.Length == 0) return BadRequest(new { detail = "Dosya boş." });
        await using var stream = file.OpenReadStream();
        var created = await _service.UploadAsync(stream, file.FileName, file.ContentType, inspection_id, UploadPath);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}/defects")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> SetDefects(long id, [FromBody] long[] defectIds)
    {
        try { await _service.SetDefectsAsync(id, defectIds); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id, UploadPath); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }
}
