using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.NumericPartResult;

namespace IRSGenerator.API.Controllers;

[Route("api/numeric-part-results")]
[ApiController]
public class NumericPartResultsController : ControllerBase
{
    private readonly INumericPartResultRepository _repo;

    public NumericPartResultsController(INumericPartResultRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NumericPartResultReadDto>>> GetAll(
        [FromQuery(Name = "character_id")] long? characterId = null)
    {
        IEnumerable<NumericPartResult> items = characterId.HasValue
            ? await _repo.GetByCharacterIdAsync(characterId.Value)
            : await _repo.GetAllAsync();

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<NumericPartResultReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<NumericPartResultReadDto>> Create(
        [FromBody] NumericPartResultCreateDto dto)
    {
        var entity = new NumericPartResult
        {
            Actual      = dto.Actual,
            PartLabel   = dto.PartLabel,
            CharacterId = dto.CharacterId,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] NumericPartResultCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        entity.Actual    = dto.Actual;
        entity.PartLabel = dto.PartLabel;
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

    [HttpDelete]
    public async Task<IActionResult> DeleteByCharacter(
        [FromQuery(Name = "character_id")] long? characterId)
    {
        if (!characterId.HasValue) return BadRequest("character_id is required");
        var items = await _repo.GetByCharacterIdAsync(characterId.Value);
        foreach (var item in items)
            await _repo.DeleteAsync(item);
        return NoContent();
    }

    private static NumericPartResultReadDto ToDto(NumericPartResult e) => new()
    {
        Id          = e.Id,
        Actual      = e.Actual,
        PartLabel   = e.PartLabel,
        CharacterId = e.CharacterId,
        CreatedAt   = e.CreatedAt,
        UpdatedAt   = e.UpdatedAt,
    };
}
