using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DispositionType;
namespace MES.API.Controllers;

[Route("api/disposition-types")]
[ApiController]
[Authorize]
public class DispositionTypesController : ControllerBase
{
    private readonly IDispositionTypeService _service;
    public DispositionTypesController(IDispositionTypeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionTypeReadDto>>> GetAll(
        [FromQuery] bool active_only = false)
        => Ok(active_only ? await _service.GetActiveAsync() : await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DispositionTypeReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DispositionTypeReadDto>> Create([FromBody] DispositionTypeCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] DispositionTypeUpdateDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }
}
