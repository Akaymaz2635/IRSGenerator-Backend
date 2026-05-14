using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Disposition;
namespace MES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DispositionsController : ControllerBase
{
    private readonly IDispositionService _service;
    public DispositionsController(IDispositionService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DispositionReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionReadDto>>> GetByDefect(
        [FromQuery] long? defect_id = null)
    {
        if (!defect_id.HasValue) return BadRequest(new { detail = "defect_id gereklidir." });
        return Ok(await _service.GetByDefectAsync(defect_id.Value));
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<DispositionReadDto>> Create([FromBody] DispositionCreateDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }
}
