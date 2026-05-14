using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Character;
using MES.Domain.Dtos.Disposition;
namespace MES.API.Controllers;

[Route("api/characters")]
[ApiController]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly ICharacterService _service;
    public CharactersController(ICharacterService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetAll(
        [FromQuery(Name = "irs_project_id")] long? irsProjectId = null,
        [FromQuery(Name = "inspection_id")]  long? inspectionId = null)
        => Ok(await _service.GetAllAsync(irsProjectId, inspectionId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CharacterReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<CharacterReadDto>> Create([FromBody] CharacterCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] CharacterUpdateDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { detail = ex.Message }); }
        catch (Exception) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpGet("{id:long}/dispositions")]
    public async Task<ActionResult> GetDispositions(long id)
    {
        try { return Ok(await _service.GetDispositionsAsync(id)); }
        catch (Exception) { return NotFound(); }
    }

    [HttpPost("{id:long}/dispositions")]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult> AddDisposition(long id, [FromBody] DispositionCreateDto dto)
    {
        try { return Ok(await _service.AddDispositionAsync(id, dto)); }
        catch (InvalidOperationException ex) { return BadRequest(new { detail = ex.Message }); }
        catch (Exception) { return NotFound(); }
    }
}
