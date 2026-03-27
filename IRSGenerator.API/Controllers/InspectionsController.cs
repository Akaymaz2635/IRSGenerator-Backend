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

    [HttpGet("by-project/{projectId:long}")]
    public async Task<ActionResult<IEnumerable<InspectionReadDto>>> GetByProject(long projectId)
    {
        var items = await _repo.GetByProjectAsync(projectId);
        return Ok(items.Select(ToReadDto));
    }

    [HttpPost]
    public async Task<ActionResult<InspectionReadDto>> Create([FromBody] InspectionCreateDto dto)
    {
        var entity = new Inspection
        {
            ProjectId = dto.ProjectId,
            PartNumber = dto.PartNumber ?? "",
            SerialNumber = dto.SerialNumber ?? "",
            OperationNumber = dto.OperationNumber ?? "",
            Inspector = dto.Inspector ?? "",
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

        if (dto.ProjectId.HasValue) entity.ProjectId = dto.ProjectId.Value;
        if (dto.PartNumber is not null) entity.PartNumber = dto.PartNumber;
        if (dto.SerialNumber is not null) entity.SerialNumber = dto.SerialNumber;
        if (dto.OperationNumber is not null) entity.OperationNumber = dto.OperationNumber;
        if (dto.Inspector is not null) entity.Inspector = dto.Inspector;
        if (dto.Status is not null) entity.Status = dto.Status;
        if (dto.Notes is not null) entity.Notes = dto.Notes;

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpPost("{id:long}/complete")]
    public async Task<IActionResult> Complete(long id)
    {
        try
        {
            await _repo.SetStatusCompletedAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
        ProjectId = i.ProjectId,
        ProjectName = i.Project?.Name,
        PartNumber = i.PartNumber,
        SerialNumber = i.SerialNumber,
        OperationNumber = i.OperationNumber,
        Inspector = i.Inspector,
        Status = i.Status,
        Notes = i.Notes,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };
}
