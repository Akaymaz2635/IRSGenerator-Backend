using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Shared.Dtos.Disposition;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DispositionsController : ControllerBase
{
    private readonly IDispositionRepository _repo;
    private readonly IDispositionTypeRepository _typeRepo;

    public DispositionsController(IDispositionRepository repo, IDispositionTypeRepository typeRepo)
    {
        _repo     = repo     ?? throw new ArgumentNullException(nameof(repo));
        _typeRepo = typeRepo ?? throw new ArgumentNullException(nameof(typeRepo));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DispositionReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToReadDto(entity));
    }

    // GET /api/dispositions?defect_id=X
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispositionReadDto>>> GetByDefect(
        [FromQuery] long? defect_id = null)
    {
        if (!defect_id.HasValue)
            return BadRequest(new { detail = "defect_id gereklidir." });

        var items = await _repo.GetByDefectAsync(defect_id.Value);
        return Ok(items.Select(ToReadDto));
    }

    [HttpPost]
    public async Task<ActionResult<DispositionReadDto>> Create([FromBody] DispositionCreateDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Decision))
        {
            var validType = await _typeRepo.GetByCodeAsync(dto.Decision);
            if (validType is null)
                return BadRequest(new { detail = $"Geçersiz disposition kodu: '{dto.Decision}'." });
        }

        var entity = new Disposition
        {
            DefectId = dto.DefectId,
            Decision = dto.Decision,
            EnteredBy = dto.EnteredBy,
            DecidedAt = dto.DecidedAt,
            Note = dto.Note ?? "",
            SpecRef = dto.SpecRef,
            Engineer = dto.Engineer,
            Reinspector = dto.Reinspector,
            ConcessionNo = dto.ConcessionNo,
            VoidReason = dto.VoidReason,
            RepairRef = dto.RepairRef,
            ScrapReason = dto.ScrapReason,
            MeasurementsSnapshot = dto.MeasurementsSnapshot
        };
        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    private static DispositionReadDto ToReadDto(Disposition d) => new()
    {
        Id = d.Id,
        DefectId = d.DefectId,
        Decision = d.Decision,
        EnteredBy = d.EnteredBy,
        DecidedAt = d.DecidedAt,
        Note = d.Note,
        SpecRef = d.SpecRef,
        Engineer = d.Engineer,
        Reinspector = d.Reinspector,
        ConcessionNo = d.ConcessionNo,
        VoidReason = d.VoidReason,
        RepairRef = d.RepairRef,
        ScrapReason = d.ScrapReason,
        MeasurementsSnapshot = d.MeasurementsSnapshot,
        CreatedAt = d.CreatedAt
    };
}
