using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DefectType;
namespace MES.API.Controllers;

[Route("api/defect-types")]
[ApiController]
[Authorize]
public class DefectTypesController : ControllerBase
{
    private readonly IDefectTypeService _service;
    public DefectTypesController(IDefectTypeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectTypeReadDto>>> GetAll(
        [FromQuery] bool active_only = false)
        => Ok(active_only ? await _service.GetActiveAsync() : await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectTypeReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DefectTypeReadDto>> Create([FromBody] DefectTypeCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectTypeUpdateDto dto)
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
