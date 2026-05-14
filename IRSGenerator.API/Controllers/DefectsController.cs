using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Defect;
namespace MES.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DefectsController : ControllerBase
{
    private readonly IDefectService _service;
    public DefectsController(IDefectService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectReadDto>>> GetAll(
        [FromQuery] long? inspection_id = null)
        => Ok(await _service.GetAllAsync(inspection_id));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<DefectReadDto>> Create([FromBody] DefectCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectUpdateDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpPatch("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Patch(long id, [FromBody] DefectUpdateDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (Exception) { return NotFound(); }
    }
}
