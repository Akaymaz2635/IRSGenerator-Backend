using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.VisualSystemConfig;

namespace MES.API.Controllers;

[Route("api/visual-system-config")]
[ApiController]
[Authorize]
public class VisualSystemConfigController : ControllerBase
{
    private readonly IVisualSystemConfigService _service;
    public VisualSystemConfigController(IVisualSystemConfigService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisualSystemConfigReadDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{key}")]
    public async Task<ActionResult<VisualSystemConfigReadDto>> GetByKey(string key)
    { var dto = await _service.GetByKeyAsync(key); return dto is null ? NotFound() : Ok(dto); }

    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update([FromBody] VisualSystemConfigUpdateDto dto)
    { await _service.UpdateAsync(dto); return NoContent(); }
}
