using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DefectField;
namespace MES.API.Controllers;

[Route("api/defect-fields")]
[ApiController]
[Authorize]
public class DefectFieldsController : ControllerBase
{
    private readonly IDefectFieldService _service;
    public DefectFieldsController(IDefectFieldService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectFieldReadDto>>> GetAll(
        [FromQuery] long? defect_type_id = null)
        => Ok(await _service.GetAllAsync(defect_type_id));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectFieldReadDto>> GetById(long id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<DefectFieldReadDto>> Create([FromBody] DefectFieldCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectFieldUpdateDto dto)
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
