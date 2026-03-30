using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.CategoricalPartResult;

namespace IRSGenerator.API.Controllers;

[Route("api/categorical-part-results")]
[ApiController]
public class CategoricalPartResultsController : ControllerBase
{
    private readonly ICategoricalPartResultRepository _repo;

    public CategoricalPartResultsController(ICategoricalPartResultRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoricalPartResultReadDto>>> GetAll(
        [FromQuery(Name = "character_id")] long? characterId = null)
    {
        IEnumerable<CategoricalPartResult> items = characterId.HasValue
            ? await _repo.GetByCharacterIdAsync(characterId.Value)
            : await _repo.GetAllAsync();

        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoricalPartResultReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<CategoricalPartResultReadDto>> Create(
        [FromBody] CategoricalPartResultCreateDto dto)
    {
        var entity = new CategoricalPartResult
        {
            Index          = dto.Index,
            IsConfirmed    = dto.IsConfirmed,
            AdditionalInfo = dto.AdditionalInfo,
            CharacterId    = dto.CharacterId,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoricalPartResultCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        entity.Index          = dto.Index;
        entity.IsConfirmed    = dto.IsConfirmed;
        entity.AdditionalInfo = dto.AdditionalInfo;
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

    private static CategoricalPartResultReadDto ToDto(CategoricalPartResult e) => new()
    {
        Id             = e.Id,
        Index          = e.Index,
        IsConfirmed    = e.IsConfirmed,
        AdditionalInfo = e.AdditionalInfo,
        CharacterId    = e.CharacterId,
        CreatedAt      = e.CreatedAt,
        UpdatedAt      = e.UpdatedAt,
    };
}
