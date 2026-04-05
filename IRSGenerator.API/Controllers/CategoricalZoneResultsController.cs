using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.CategoricalZoneResult;

namespace IRSGenerator.API.Controllers;

[Route("api/categorical-zone-results")]
[ApiController]
[Authorize]
public class CategoricalZoneResultsController : ControllerBase
{
    private readonly ICategoricalZoneResultRepository _repo;

    public CategoricalZoneResultsController(ICategoricalZoneResultRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoricalZoneResultReadDto>>> GetAll(
        [FromQuery(Name = "character_id")] long? characterId = null)
    {
        IEnumerable<CategoricalZoneResult> items = characterId.HasValue
            ? await _repo.GetByCharacterIdAsync(characterId.Value)
            : await _repo.GetAllAsync();

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoricalZoneResultReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<CategoricalZoneResultReadDto>> Create(
        [FromBody] CategoricalZoneResultCreateDto dto)
    {
        var entity = new CategoricalZoneResult
        {
            ZoneName       = dto.ZoneName,
            IsConfirmed    = dto.IsConfirmed,
            AdditionalInfo = dto.AdditionalInfo,
            CharacterId    = dto.CharacterId,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoricalZoneResultCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        entity.ZoneName       = dto.ZoneName;
        entity.IsConfirmed    = dto.IsConfirmed;
        entity.AdditionalInfo = dto.AdditionalInfo;
        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    [HttpDelete]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> DeleteByCharacter(
        [FromQuery(Name = "character_id")] long? characterId)
    {
        if (!characterId.HasValue) return BadRequest("character_id is required");
        var items = await _repo.GetByCharacterIdAsync(characterId.Value);
        foreach (var item in items)
            await _repo.DeleteAsync(item);
        return NoContent();
    }

    private static CategoricalZoneResultReadDto ToDto(CategoricalZoneResult e) => new()
    {
        Id             = e.Id,
        ZoneName       = e.ZoneName,
        IsConfirmed    = e.IsConfirmed,
        AdditionalInfo = e.AdditionalInfo,
        CharacterId    = e.CharacterId,
        CreatedAt      = e.CreatedAt,
        UpdatedAt      = e.UpdatedAt,
    };
}
