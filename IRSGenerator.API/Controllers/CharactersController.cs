using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Shared.Dtos.Character;

namespace IRSGenerator.API.Controllers;

[Route("api/characters")]
[ApiController]
public class CharactersController : ControllerBase
{
    private readonly ICharacterRepository _repo;

    public CharactersController(ICharacterRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetAll(
        [FromQuery] long? projectId = null)
    {
        IEnumerable<Character> items = projectId.HasValue
            ? await _repo.GetByProjectIdAsync(projectId.Value)
            : await _repo.GetAllAsync();

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CharacterReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<CharacterReadDto>> Create([FromBody] CharacterCreateDto dto)
    {
        var limits = LimitCatcher.CatchMeasurement(dto.Dimension);

        var entity = new Character
        {
            ItemNo          = dto.ItemNo,
            Dimension       = dto.Dimension,
            Badge           = dto.Badge,
            Tooling         = dto.Tooling,
            BPZone          = dto.BPZone,
            InspectionLevel = dto.InspectionLevel,
            Remarks         = dto.Remarks,
            LowerLimit      = limits.Length > 0 ? limits[0] : 0,
            UpperLimit      = limits.Length > 1 ? limits[1] : 0,
            IRSProjectId    = dto.IRSProjectId,
        };

        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CharacterUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.ItemNo is not null)          entity.ItemNo          = dto.ItemNo;
        if (dto.Badge is not null)           entity.Badge           = dto.Badge;
        if (dto.Tooling is not null)         entity.Tooling         = dto.Tooling;
        if (dto.BPZone is not null)          entity.BPZone          = dto.BPZone;
        if (dto.InspectionLevel is not null) entity.InspectionLevel = dto.InspectionLevel;
        if (dto.Remarks is not null)         entity.Remarks         = dto.Remarks;
        if (dto.InspectionResult is not null) entity.InspectionResult = dto.InspectionResult;

        if (dto.Dimension is not null)
        {
            entity.Dimension = dto.Dimension;
            var limits = LimitCatcher.CatchMeasurement(dto.Dimension);
            entity.LowerLimit = limits.Length > 0 ? limits[0] : 0;
            entity.UpperLimit = limits.Length > 1 ? limits[1] : 0;
        }

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

    private static CharacterReadDto ToDto(Character e) => new()
    {
        Id              = e.Id,
        ItemNo          = e.ItemNo,
        Dimension       = e.Dimension,
        Badge           = e.Badge,
        Tooling         = e.Tooling,
        BPZone          = e.BPZone,
        InspectionLevel = e.InspectionLevel,
        Remarks         = e.Remarks,
        LowerLimit      = e.LowerLimit,
        UpperLimit      = e.UpperLimit,
        InspectionResult = e.InspectionResult,
        IRSProjectId    = e.IRSProjectId,
        CreatedAt       = e.CreatedAt,
        UpdatedAt       = e.UpdatedAt,
    };
}
