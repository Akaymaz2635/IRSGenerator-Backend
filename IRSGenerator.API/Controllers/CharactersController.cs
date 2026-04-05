using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Shared.Dtos.Character;
using IRSGenerator.Shared.Dtos.Disposition;
using System.Text.RegularExpressions;

namespace IRSGenerator.API.Controllers;

[Route("api/characters")]
[ApiController]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly ICharacterRepository _repo;
    private readonly IDispositionRepository _dispRepo;
    private readonly IDispositionTypeRepository _dispTypeRepo;

    private static readonly HashSet<string> ValidResults = new(StringComparer.OrdinalIgnoreCase)
        { "Unidentified", "Conform", "Not Conform", "Pass", "Fail", "Passed", "Failed" };

    public CharactersController(ICharacterRepository repo, IDispositionRepository dispRepo,
                                IDispositionTypeRepository dispTypeRepo)
    {
        _repo         = repo         ?? throw new ArgumentNullException(nameof(repo));
        _dispRepo     = dispRepo     ?? throw new ArgumentNullException(nameof(dispRepo));
        _dispTypeRepo = dispTypeRepo ?? throw new ArgumentNullException(nameof(dispTypeRepo));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetAll(
        [FromQuery(Name = "irs_project_id")] long? irsProjectId = null,
        [FromQuery(Name = "inspection_id")] long? inspectionId = null)
    {
        IEnumerable<Character> items;
        if (inspectionId.HasValue)
            items = await _repo.GetByInspectionIdAsync(inspectionId.Value);
        else if (irsProjectId.HasValue)
            items = await _repo.GetByProjectIdAsync(irsProjectId.Value);
        else
            items = await _repo.GetAllAsync();

        var itemList = items.ToList();
        foreach (var c in itemList)
        {
            if (c.LowerLimit == 0 && c.UpperLimit == 0 && !string.IsNullOrEmpty(c.Dimension))
            {
                var limits = LimitCatcherService.CatchMeasurement(c.Dimension);
                if (limits.Length >= 2 && (limits[0] != 0 || limits[1] != 0))
                {
                    c.LowerLimit = limits[0];
                    c.UpperLimit = limits[1];
                    await _repo.UpdateAsync(c);
                }
            }
        }

        return Ok(itemList.Select(ToDto));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CharacterReadDto>> GetById(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        return Ok(ToDto(entity));
    }

    [HttpPost]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult<CharacterReadDto>> Create([FromBody] CharacterCreateDto dto)
    {
        var limits = LimitCatcherService.CatchMeasurement(dto.Dimension);

        var entity = new Character
        {
            ItemNo          = dto.ItemNo,
            Dimension       = dto.Dimension,
            Badge           = dto.Badge,
            Tooling         = dto.Tooling,
            BPZone          = dto.BPZone,
            InspectionLevel = dto.InspectionLevel,
            Remarks         = dto.Remarks,
            LowerLimit      = limits.Length > 0 ? limits[0] : 0,
            UpperLimit      = limits.Length > 1 ? limits[1] : 0,
            IRSProjectId    = dto.IRSProjectId,
            InspectionId    = dto.InspectionId,
        };

        var created = await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Update(long id, [FromBody] CharacterUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (dto.ItemNo is not null)          entity.ItemNo          = dto.ItemNo;
        if (dto.Badge is not null)           entity.Badge           = dto.Badge;
        if (dto.Tooling is not null)         entity.Tooling         = dto.Tooling;
        if (dto.BPZone is not null)          entity.BPZone          = dto.BPZone;
        if (dto.InspectionLevel is not null) entity.InspectionLevel = dto.InspectionLevel;
        if (dto.Remarks is not null)         entity.Remarks         = dto.Remarks;
        if (dto.InspectionResult is not null)
        {
            // Allow free-form numeric values (e.g. "17.72", "17.60 / 17.75") or known status words
            var isNumeric = Regex.IsMatch(dto.InspectionResult.Trim(), @"^[\d.,\s/\-]+$");
            if (!isNumeric && !ValidResults.Contains(dto.InspectionResult))
                return BadRequest(new { detail = $"Geçersiz inspection_result: '{dto.InspectionResult}'." });
            entity.InspectionResult = dto.InspectionResult;
        }
        if (dto.Note is not null)            entity.Note            = dto.Note;

        if (dto.Dimension is not null)
        {
            entity.Dimension = dto.Dimension;
            var limits = LimitCatcherService.CatchMeasurement(dto.Dimension);
            entity.LowerLimit = limits.Length > 0 ? limits[0] : 0;
            entity.UpperLimit = limits.Length > 1 ? limits[1] : 0;
        }

        await _repo.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Delete(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();
        await _repo.DeleteAsync(entity);
        return NoContent();
    }

    // ── GET /api/characters/{id}/dispositions ─────────────────────────────────
    [HttpGet("{id:long}/dispositions")]
    public async Task<ActionResult> GetDispositions(long id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        var disps = await _dispRepo.GetByCharacterIdAsync(id);
        return Ok(disps.Select(d => new
        {
            id                    = d.Id,
            character_id          = d.CharacterId,
            decision              = d.Decision,
            entered_by            = d.EnteredBy,
            decided_at            = d.DecidedAt,
            note                  = d.Note,
            spec_ref              = d.SpecRef,
            created_at            = d.CreatedAt,
            measurements_snapshot = d.MeasurementsSnapshot,
        }));
    }

    // ── POST /api/characters/{id}/dispositions ────────────────────────────────
    [HttpPost("{id:long}/dispositions")]
    [Authorize(Policy = "CanWrite")]
    public async Task<ActionResult> AddDisposition(long id, [FromBody] DispositionCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Decision))
        {
            var validType = await _dispTypeRepo.GetByCodeAsync(dto.Decision);
            if (validType is null)
                return BadRequest(new { detail = $"Geçersiz disposition kodu: '{dto.Decision}'." });
        }

        var disp = new Disposition
        {
            CharacterId          = id,
            DefectId             = null,
            Decision             = dto.Decision,
            EnteredBy            = dto.EnteredBy,
            DecidedAt            = dto.DecidedAt.HasValue ? DateTime.SpecifyKind(dto.DecidedAt.Value, DateTimeKind.Utc) : DateTime.UtcNow,
            Note                 = dto.Note ?? "",
            SpecRef              = dto.SpecRef,
            Engineer             = dto.Engineer,
            MeasurementsSnapshot = dto.MeasurementsSnapshot,
        };
        var created = await _dispRepo.AddAsync(disp);
        return Ok(new
        {
            id           = created.Id,
            character_id = created.CharacterId,
            decision     = created.Decision,
            entered_by   = created.EnteredBy,
            decided_at   = created.DecidedAt,
            note         = created.Note,
            created_at   = created.CreatedAt,
        });
    }

    private static CharacterReadDto ToDto(Character e) => new()
    {
        Id              = e.Id,
        ItemNo          = e.ItemNo,
        Dimension       = e.Dimension,
        Badge           = e.Badge,
        Tooling         = e.Tooling,
        BPZone          = e.BPZone,
        InspectionLevel = e.InspectionLevel,
        Remarks         = e.Remarks,
        LowerLimit      = e.LowerLimit,
        UpperLimit      = e.UpperLimit,
        InspectionResult = e.InspectionResult,
        Note            = e.Note,
        IRSProjectId    = e.IRSProjectId,
        InspectionId    = e.InspectionId,
        CreatedAt       = e.CreatedAt,
        UpdatedAt       = e.UpdatedAt,
    };
}
