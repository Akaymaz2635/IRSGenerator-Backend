using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.DispositionType;

namespace IRSGenerator.API.Controllers;

[Route("api/disposition-types")]
[ApiController]
public class DispositionTypesController : ControllerBase
{
    private readonly IDispositionTypeRepository _repo;

    public DispositionTypesController(IDispositionTypeRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionTypeReadDto>>> GetAll([FromQuery] bool all = false)
    {
        var items = all
            ? await _repo.GetAllAsync()
            : await _repo.GetActiveAsync();

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DispositionTypeReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<DispositionTypeReadDto>> Create([FromBody] DispositionTypeCreateDto dto)
    {
        var entity = new DispositionType
        {
            Code          = dto.Code,
            Label         = dto.Label,
            CssClass      = dto.CssClass,
            IsNeutralizing = dto.IsNeutralizing,
            IsInitial     = dto.IsInitial,
            SortOrder     = dto.SortOrder,
            Active        = true
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] DispositionTypeUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        // Code is immutable — never updated
        if (dto.Label         is not null) entity.Label         = dto.Label;
        if (dto.CssClass      is not null) entity.CssClass      = dto.CssClass;
        if (dto.IsNeutralizing.HasValue)   entity.IsNeutralizing = dto.IsNeutralizing.Value;
        if (dto.IsInitial.HasValue)        entity.IsInitial     = dto.IsInitial.Value;
        if (dto.SortOrder.HasValue)        entity.SortOrder     = dto.SortOrder.Value;
        if (dto.Active.HasValue)           entity.Active        = dto.Active.Value;

        await _repo.UpdateAsync(entity);
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

    private static DispositionTypeReadDto ToReadDto(DispositionType dt) => new()
    {
        Id            = dt.Id,
        Code          = dt.Code,
        Label         = dt.Label,
        CssClass      = dt.CssClass,
        IsNeutralizing = dt.IsNeutralizing,
        IsInitial     = dt.IsInitial,
        SortOrder     = dt.SortOrder,
        Active        = dt.Active,
        CreatedAt     = dt.CreatedAt,
        UpdatedAt     = dt.UpdatedAt
    };
}
