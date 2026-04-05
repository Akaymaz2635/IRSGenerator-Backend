using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.NcmDispositionType;

namespace IRSGenerator.API.Controllers;

[Route("api/ncm-disposition-types")]
[ApiController]
[Authorize]
public class NcmDispositionTypesController : ControllerBase
{
    private readonly INcmDispositionTypeRepository _repo;

    public NcmDispositionTypesController(INcmDispositionTypeRepository repo)
        => _repo = repo ?? throw new ArgumentNullException(nameof(repo));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NcmDispositionTypeReadDto>>> GetAll(
        [FromQuery] bool active_only = false)
    {
        var items = active_only
            ? await _repo.GetActiveAsync()
            : await _repo.GetAllAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<NcmDispositionTypeReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<NcmDispositionTypeReadDto>> Create(
        [FromBody] NcmDispositionTypeCreateDto dto)
    {
        var entity = new NcmDispositionType
        {
            Code             = dto.Code.Trim().ToUpper(),
            Label            = dto.Label.Trim(),
            Description      = dto.Description.Trim(),
            TemplateFileName = dto.TemplateFileName.Trim(),
            Active           = dto.Active,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<NcmDispositionTypeReadDto>> Update(
        long id, [FromBody] NcmDispositionTypeUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Code)) entity.Code = dto.Code.Trim().ToUpper();
        entity.Label            = dto.Label.Trim();
        entity.Description      = dto.Description.Trim();
        entity.TemplateFileName = dto.TemplateFileName.Trim();
        entity.Active           = dto.Active;

        await _repo.UpdateAsync(entity);
        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static NcmDispositionTypeReadDto ToDto(NcmDispositionType t) => new()
    {
        Id               = t.Id,
        Code             = t.Code,
        Label            = t.Label,
        Description      = t.Description,
        TemplateFileName = t.TemplateFileName,
        Active           = t.Active,
    };
}
