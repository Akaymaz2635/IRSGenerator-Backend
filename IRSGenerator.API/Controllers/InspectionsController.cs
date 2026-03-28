using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Defect;
using IRSGenerator.Shared.Dtos.Disposition;
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
    public async Task<ActionResult<IEnumerable<InspectionReadDto>>> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] long? project_id = null,
        [FromQuery] string? search = null)
    {
        var items = await _repo.GetFilteredAsync(status, project_id, search);
        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<InspectionReadDto>> GetById(long id)
    {
        var entity = await _repo.GetWithDetailsAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDetailReadDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<InspectionReadDto>> Create([FromBody] InspectionCreateDto dto)
    {
        var entity = new Inspection
        {
            VisualProjectId = dto.ProjectId,
            PartNumber = dto.PartNumber,
            SerialNumber = dto.SerialNumber,
            OperationNumber = dto.OperationNumber,
            Inspector = dto.Inspector,
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

        if (dto.ProjectId.HasValue) entity.VisualProjectId = dto.ProjectId.Value;
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
        var success = await _repo.SetStatusCompletedAsync(id);
        if (!success)
            return BadRequest(new { detail = "Tüm defektlerin disposition'ı tamamlanmadan inspection kapatılamaz." });
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
        ProjectId = i.VisualProjectId,
        PartNumber = i.PartNumber,
        SerialNumber = i.SerialNumber,
        OperationNumber = i.OperationNumber,
        Inspector = i.Inspector,
        Status = i.Status,
        Notes = i.Notes,
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };

    // Detail variant — populates Defects list (only used in GetById)
    private static InspectionReadDto ToDetailReadDto(Inspection i)
    {
        var dto = ToReadDto(i);
        dto.Defects = i.Defects.Select(d =>
        {
            var dispsSorted = d.Dispositions
                .OrderByDescending(disp => disp.CreatedAt)
                .ToList();

            return new DefectReadDto
            {
                Id              = d.Id,
                InspectionId    = d.InspectionId,
                DefectTypeId    = d.DefectTypeId,
                DefectTypeName  = d.DefectType?.Name,
                OriginDefectId  = d.OriginDefectId,
                Depth           = d.Depth,
                Width           = d.Width,
                Length          = d.Length,
                Radius          = d.Radius,
                Angle           = d.Angle,
                Height          = d.Height,
                Color           = d.Color,
                Notes           = d.Notes,
                HighMetal       = d.HighMetal,
                CreatedAt       = d.CreatedAt,
                UpdatedAt       = d.UpdatedAt,
                ChildDefectIds  = d.ChildDefects.Select(c => c.Id).ToList(),
                Dispositions    = dispsSorted.Select(MapDisposition).ToList(),
                ActiveDisposition = dispsSorted.FirstOrDefault(disp => disp.Decision != "VOID") is { } active
                    ? MapDisposition(active)
                    : null
            };
        }).ToList();
        return dto;
    }

    private static DispositionReadDto MapDisposition(Disposition disp) => new()
    {
        Id                   = disp.Id,
        DefectId             = disp.DefectId,
        Decision             = disp.Decision,
        EnteredBy            = disp.EnteredBy,
        DecidedAt            = disp.DecidedAt,
        Note                 = disp.Note,
        SpecRef              = disp.SpecRef,
        Engineer             = disp.Engineer,
        Reinspector          = disp.Reinspector,
        ConcessionNo         = disp.ConcessionNo,
        VoidReason           = disp.VoidReason,
        RepairRef            = disp.RepairRef,
        ScrapReason          = disp.ScrapReason,
        MeasurementsSnapshot = disp.MeasurementsSnapshot,
        CreatedAt            = disp.CreatedAt
    };
}
