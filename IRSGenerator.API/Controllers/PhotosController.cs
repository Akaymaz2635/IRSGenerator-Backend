using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Photo;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhotosController : ControllerBase
{
    private readonly IPhotoRepository _repo;

    public PhotosController(IPhotoRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PhotoReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpGet("by-inspection/{inspectionId:long}")]
    public async Task<ActionResult<IEnumerable<PhotoReadDto>>> GetByInspection(long inspectionId)
    {
        var items = await _repo.GetByInspectionAsync(inspectionId);
        return Ok(items.Select(ToReadDto));
    }

    [HttpPost]
    public async Task<ActionResult<PhotoReadDto>> Create([FromBody] PhotoCreateDto dto)
    {
        var entity = new Photo
        {
            InspectionId = dto.InspectionId,
            Filename = dto.Filename,
            Filepath = dto.Filepath
        };
        var created = await _repo.AddAsync(entity);

        foreach (var defectId in dto.DefectIds)
            await _repo.LinkDefectAsync(created.Id, defectId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPost("{id:long}/link-defect/{defectId:long}")]
    public async Task<IActionResult> LinkDefect(long id, long defectId)
    {
        await _repo.LinkDefectAsync(id, defectId);
        return NoContent();
    }

    [HttpDelete("{id:long}/unlink-defect/{defectId:long}")]
    public async Task<IActionResult> UnlinkDefect(long id, long defectId)
    {
        await _repo.UnlinkDefectAsync(id, defectId);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

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
