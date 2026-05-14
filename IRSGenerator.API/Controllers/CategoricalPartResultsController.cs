using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.CategoricalPartResult;
namespace MES.API.Controllers;

[Route("api/categorical-part-results")]
[ApiController]
[Authorize]
public class CategoricalPartResultsController : ControllerBase
{
    private readonly ICategoricalPartResultService _service;
    public CategoricalPartResultsController(ICategoricalPartResultService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoricalPartResultReadDto>>> GetAll(
        [FromQuery(Name = "character_id")] long? characterId = null)
        => Ok(await _service.GetAllAsync(characterId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoricalPartResultReadDto>> GetById(long id)
    { var dto = await _service.GetByIdAsync(id); return dto is null ? NotFound() : Ok(dto); }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<CategoricalPartResultReadDto>> Create([FromBody] CategoricalPartResultCreateDto dto)
    { var c = await _service.CreateAsync(dto); return CreatedAtAction(nameof(GetById), new { id = c.Id }, c); }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] CategoricalPartResultCreateDto dto)
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
