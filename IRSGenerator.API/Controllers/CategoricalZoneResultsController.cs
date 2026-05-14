using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.CategoricalZoneResult;
namespace MES.API.Controllers;

[Route("api/categorical-zone-results")]
[ApiController]
[Authorize]
public class CategoricalZoneResultsController : ControllerBase
{
    private readonly ICategoricalZoneResultService _service;
    public CategoricalZoneResultsController(ICategoricalZoneResultService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoricalZoneResultReadDto>>> GetAll(
        [FromQuery(Name = "character_id")] long? characterId = null)
        => Ok(await _service.GetAllAsync(characterId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoricalZoneResultReadDto>> GetById(long id)
    { var dto = await _service.GetByIdAsync(id); return dto is null ? NotFound() : Ok(dto); }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<CategoricalZoneResultReadDto>> Create([FromBody] CategoricalZoneResultCreateDto dto)
    { var c = await _service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = c.Id }, c); }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoricalZoneResultCreateDto dto)
    { try { await _service.UpdateAsync(id, dto); return NoContent(); } catch (Exception) { return NotFound(); } }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    { try { await _service.DeleteAsync(id); return NoContent(); } catch (Exception) { return NotFound(); } }

    [HttpDelete]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> DeleteByCharacter([FromQuery(Name = "character_id")] long? characterId)
    { if (!characterId.HasValue) return BadRequest("character_id is required"); await _service.DeleteByCharacterAsync(characterId.Value); return NoContent(); }
}
