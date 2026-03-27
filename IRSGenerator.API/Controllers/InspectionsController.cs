using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Inspection;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InspectionsController : ControllerBase
{
    private readonly IInspectionRepository _repo;

    public InspectionsController(IInspectionRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InspectionReadDto>>> GetAll()
    {
        var items = await _repo.GetAllAsync();
        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<InspectionReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    [HttpGet("by-irs-project/{irsProjectId:long}")]
    public async Task<ActionResult<IEnumerable<InspectionReadDto>>> GetByIrsProject(long irsProjectId)
    {
        var items = await _repo.GetByIrsProjectAsync(irsProjectId);
        return Ok(items.Select(ToReadDto));
    }

    [HttpPost]
    public async Task<ActionResult<InspectionReadDto>> Create([FromBody] InspectionCreateDto dto)
    {
        var entity = new Inspection
        {
            IrsProjectId = dto.IrsProjectId,
            InspectorId = dto.InspectorId,
            Status = dto.Status,
            Notes = dto.Notes
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] InspectionUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.InspectorId.HasValue) entity.InspectorId = dto.InspectorId.Value;
        if (dto.Status is not null) entity.Status = dto.Status;
        if (dto.Notes is not null) entity.Notes = dto.Notes;

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpPost("{id:long}/complete")]
    public async Task<IActionResult> Complete(long id)
    {
        var success = await _repo.SetStatusCompletedAsync(id);
        if (!success)
            return BadRequest(new { error = "Tüm defektlerin disposition'ı tamamlanmadan inspection kapatılamaz." });
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static InspectionReadDto ToReadDto(Inspection i) => new()
    {
        Id = i.Id,
        IrsProjectId = i.IrsProjectId,
        PartNumber = i.IrsProject?.PartNumber,
        SerialNumber = i.IrsProject?.SerialNumber,
        Operation = i.IrsProject?.Operation,
        ProjectType = i.IrsProject?.ProjectType,
        InspectorId = i.InspectorId,
        InspectorName = i.Inspector?.DisplayName,
        Status = i.Status,
        Notes = i.Notes,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };
}
