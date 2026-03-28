using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.VisualSystemConfig;

namespace IRSGenerator.API.Controllers;

// Her iki route da çalışır: /api/system-config  ve  /api/VisualSystemConfig
[Route("api/system-config")]
[ApiController]
public class VisualSystemConfigController : ControllerBase
{
    private readonly IVisualSystemConfigRepository _repo;

    public VisualSystemConfigController(IVisualSystemConfigRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisualSystemConfigReadDto>>> GetAll()
    {
        var items = await _repo.GetAllConfigsAsync();
        return Ok(items.Select(c => new VisualSystemConfigReadDto
        {
            Id = c.Id,
            Key = c.Key,
            Value = c.Value,
            UpdatedAt = c.UpdatedAt
        }));
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<VisualSystemConfigReadDto>> GetByKey(string key)
    {
        var entity = await _repo.GetByKeyAsync(key);
        if (entity is null) return NotFound();

        return Ok(new VisualSystemConfigReadDto
        {
            Id = entity.Id,
            Key = entity.Key,
            Value = entity.Value,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] VisualSystemConfigUpdateDto dto)
    {
        if (dto.PhotoRootFolder is not null)
            await UpdateKey("photo_root_folder", dto.PhotoRootFolder);
        if (dto.ReportRootFolder is not null)
            await UpdateKey("report_root_folder", dto.ReportRootFolder);
        if (dto.BackupRootFolder is not null)
            await UpdateKey("backup_root_folder", dto.BackupRootFolder);

        return NoContent();
    }

    private async Task UpdateKey(string key, string value)
    {
        var entity = await _repo.GetByKeyAsync(key);
        if (entity is null) return;
        entity.Value = value;
        await _repo.UpdateAsync(entity);
    }
}
