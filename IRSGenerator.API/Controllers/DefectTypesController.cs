using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.DefectType;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DefectTypesController : ControllerBase
{
    private readonly IDefectTypeRepository _repo;

    public DefectTypesController(IDefectTypeRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectTypeReadDto>>> GetAll([FromQuery] bool all = false)
    {
        var items = all
            ? await _repo.GetAllAsync()
            : await _repo.GetActiveAsync();

        return Ok(items.Select(dt => new DefectTypeReadDto
        {
            Id = dt.Id,
            Name = dt.Name,
            Description = dt.Description,
            Active = dt.Active,
            CreatedAt = dt.CreatedAt,
            UpdatedAt = dt.UpdatedAt
        }));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectTypeReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        return Ok(new DefectTypeReadDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Active = entity.Active,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<DefectTypeReadDto>> Create([FromBody] DefectTypeCreateDto dto)
    {
        var entity = new DefectType
        {
            Name = dto.Name,
            Description = dto.Description,
            Active = true
        };
        var created = await _repo.AddAsync(entity);
        var read = new DefectTypeReadDto
        {
            Id = created.Id,
            Name = created.Name,
            Description = created.Description,
            Active = created.Active,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, read);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectTypeUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.Name is not null) entity.Name = dto.Name;
        if (dto.Description is not null) entity.Description = dto.Description;
        if (dto.Active.HasValue) entity.Active = dto.Active.Value;

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
}
