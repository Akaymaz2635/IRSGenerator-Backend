using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.DispositionTransition;

namespace IRSGenerator.API.Controllers;

[Route("api/disposition-transitions")]
[ApiController]
public class DispositionTransitionsController : ControllerBase
{
    private readonly IDispositionTransitionRepository _repo;

    public DispositionTransitionsController(IDispositionTransitionRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    // GET /api/disposition-transitions?from_code=REWORK
    // GET /api/disposition-transitions               — all transitions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionTransitionReadDto>>> GetAll(
        [FromQuery] string? from_code = null)
    {
        if (from_code is not null)
        {
            // "null" string → null (initial transitions)
            string? code = from_code.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : from_code;
            var items = await _repo.GetByFromCodeAsync(code);
            return Ok(items.Select(ToReadDto));
        }

        var all = await _repo.GetAllAsync();
        return Ok(all.Select(ToReadDto));
    }

    // GET /api/disposition-transitions/allowed?current_code=REWORK
    // Returns string[] of allowed next codes
    [HttpGet("allowed")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllowed(
        [FromQuery] string? current_code = null)
    {
        string? code = current_code == null || current_code.Equals("null", StringComparison.OrdinalIgnoreCase)
            ? null : current_code;

        var codes = await _repo.GetAllowedNextCodesAsync(code);
        return Ok(codes);
    }

    // POST /api/disposition-transitions/bulk-set
    // Atomically replaces all transitions for a given fromCode
    [HttpPost("bulk-set")]
    public async Task<IActionResult> BulkSet([FromBody] DispositionTransitionBulkSetDto dto)
    {
        await _repo.BulkSetAsync(dto.FromCode, dto.ToCodes);
        return NoContent();
    }

    private static DispositionTransitionReadDto ToReadDto(Core.Entities.DispositionTransition t) => new()
    {
        Id       = t.Id,
        FromCode = t.FromCode,
        ToCode   = t.ToCode
    };
}
