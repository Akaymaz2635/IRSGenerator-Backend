using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DispositionTransition;

namespace MES.API.Controllers;

[Route("api/disposition-transitions")]
[ApiController]
[Authorize]
public class DispositionTransitionsController : ControllerBase
{
    private readonly IDispositionTransitionService _service;
    public DispositionTransitionsController(IDispositionTransitionService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionTransitionReadDto>>> GetAll(
        [FromQuery] string? from_code = null)
        => Ok(await _service.GetAllAsync(from_code));

    [HttpGet("allowed")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllowed(
        [FromQuery] string? current_code = null)
        => Ok(await _service.GetAllowedNextCodesAsync(current_code));

    [HttpPost("bulk-set")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> BulkSet([FromBody] DispositionTransitionBulkSetDto dto)
    {
        await _service.BulkSetAsync(dto);
        return NoContent();
    }
}
