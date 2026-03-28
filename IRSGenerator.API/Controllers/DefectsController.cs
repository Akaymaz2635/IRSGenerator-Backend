using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Defect;
using IRSGenerator.Shared.Dtos.Disposition;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DefectsController : ControllerBase
{
    private readonly IDefectRepository _repo;

    public DefectsController(IDefectRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    // GET /api/defects?inspection_id=X
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DefectReadDto>>> GetAll(
        [FromQuery] long? inspection_id = null)
    {
        IEnumerable<Defect> items;
        if (inspection_id.HasValue)
            items = await _repo.GetByInspectionAsync(inspection_id.Value);
        else
            items = await _repo.GetAllAsync();

        return Ok(items.Select(ToReadDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DefectReadDto>> GetById(long id)
    {
        var entity = await _repo.GetWithDispositionsAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDetailReadDto(entity));
    }

    [HttpPost]
    public async Task<ActionResult<DefectReadDto>> Create([FromBody] DefectCreateDto dto)
    {
        var entity = new Defect
        {
            InspectionId = dto.InspectionId,
            DefectTypeId = dto.DefectTypeId,
            OriginDefectId = dto.OriginDefectId,
            Depth = dto.Depth,
            Width = dto.Width,
            Length = dto.Length,
            Radius = dto.Radius,
            Angle = dto.Angle,
            Height = dto.Height,
            Color = dto.Color,
            Notes = dto.Notes,
            HighMetal = dto.HighMetal ?? false
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpPut("{id:long}")]
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] DefectUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.DefectTypeId.HasValue) entity.DefectTypeId = dto.DefectTypeId.Value;
        if (dto.Depth.HasValue) entity.Depth = dto.Depth.Value;
        if (dto.Width.HasValue) entity.Width = dto.Width.Value;
        if (dto.Length.HasValue) entity.Length = dto.Length.Value;
        if (dto.Radius.HasValue) entity.Radius = dto.Radius.Value;
        if (dto.Angle.HasValue) entity.Angle = dto.Angle.Value;
        if (dto.Height.HasValue) entity.Height = dto.Height.Value;
        if (dto.Color is not null) entity.Color = dto.Color;
        if (dto.Notes is not null) entity.Notes = dto.Notes;
        if (dto.HighMetal.HasValue) entity.HighMetal = dto.HighMetal.Value;

        await _repo.UpdateAsync(entity);
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

    private static DefectReadDto ToReadDto(Defect d) => new()
    {
        Id             = d.Id,
        InspectionId   = d.InspectionId,
        DefectTypeId   = d.DefectTypeId,
        DefectTypeName = d.DefectType?.Name,
        OriginDefectId = d.OriginDefectId,
        Depth          = d.Depth,
        Width          = d.Width,
        Length         = d.Length,
        Radius         = d.Radius,
        Angle          = d.Angle,
        Height         = d.Height,
        Color          = d.Color,
        Notes          = d.Notes,
        HighMetal      = d.HighMetal,
        CreatedAt      = d.CreatedAt,
        UpdatedAt      = d.UpdatedAt
    };

    private static DefectReadDto ToDetailReadDto(Defect d)
    {
        var dto = ToReadDto(d);
        var dispsSorted = d.Dispositions
            .OrderByDescending(disp => disp.CreatedAt)
            .ToList();

        dto.ChildDefectIds   = d.ChildDefects.Select(c => c.Id).ToList();
        dto.Dispositions     = dispsSorted.Select(MapDisposition).ToList();
        dto.ActiveDisposition = dispsSorted.FirstOrDefault(disp => disp.Decision != "VOID") is { } active
            ? MapDisposition(active)
            : null;
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
