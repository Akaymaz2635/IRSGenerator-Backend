using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.CauseCode;

namespace IRSGenerator.API.Controllers;

[Route("api/cause-codes")]
[ApiController]
[Authorize]
public class CauseCodesController : ControllerBase
{
    private readonly ICauseCodeRepository _repo;

    public CauseCodesController(ICauseCodeRepository repo)
        => _repo = repo ?? throw new ArgumentNullException(nameof(repo));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CauseCodeReadDto>>> GetAll(
        [FromQuery] bool active_only = false)
    {
        var items = active_only
            ? await _repo.GetActiveAsync()
            : await _repo.GetAllAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CauseCodeReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CauseCodeReadDto>> Create([FromBody] CauseCodeCreateDto dto)
    {
        var entity = new CauseCode
        {
            Code        = dto.Code.Trim().ToUpper(),
            Description = dto.Description.Trim(),
            Active      = dto.Active,
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CauseCodeReadDto>> Update(long id, [FromBody] CauseCodeUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Code)) entity.Code = dto.Code.Trim().ToUpper();
        entity.Description = dto.Description.Trim();
        entity.Active      = dto.Active;

        await _repo.UpdateAsync(entity);
        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static CauseCodeReadDto ToDto(CauseCode c) => new()
    {
        Id          = c.Id,
        Code        = c.Code,
        Description = c.Description,
        Active      = c.Active,
    };
}
