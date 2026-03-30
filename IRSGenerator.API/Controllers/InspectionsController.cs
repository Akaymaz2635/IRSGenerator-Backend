using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Shared.Dtos.Character;
using IRSGenerator.Shared.Dtos.Defect;
using IRSGenerator.Shared.Dtos.Disposition;
using IRSGenerator.Shared.Dtos.Inspection;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InspectionsController : ControllerBase
{
    private readonly IInspectionRepository _repo;
    private readonly ICharacterRepository _charRepo;
    private readonly IPhotoRepository _photoRepo;
    private readonly WordOpSheetParser _parser;
    private readonly WordReportWriter _reportWriter;
    private readonly IWebHostEnvironment _env;

    public InspectionsController(
        IInspectionRepository repo,
        ICharacterRepository charRepo,
        IPhotoRepository photoRepo,
        WordOpSheetParser parser,
        WordReportWriter reportWriter,
        IWebHostEnvironment env)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _charRepo = charRepo ?? throw new ArgumentNullException(nameof(charRepo));
        _photoRepo = photoRepo ?? throw new ArgumentNullException(nameof(photoRepo));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _reportWriter = reportWriter ?? throw new ArgumentNullException(nameof(reportWriter));
        _env = env ?? throw new ArgumentNullException(nameof(env));
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
            IrsProjectId    = dto.IrsProjectId,
            PartNumber      = dto.PartNumber,
            SerialNumber    = dto.SerialNumber,
            OperationNumber = dto.OperationNumber,
            Inspector       = dto.Inspector,
            Status          = dto.Status,
            Notes           = dto.Notes
        };
        try
        {
            var created = await _repo.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToReadDto(created));
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("foreign key") == true
                                || ex.InnerException?.Message.Contains("violates") == true)
        {
            return BadRequest(new { detail = "Geçersiz project_id veya irs_project_id." });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] InspectionUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return NotFound();

        // Block completion if Not Conform dimensional characters lack disposition
        if (dto.Status == "completed")
        {
            var neutralizingDecisions = new HashSet<string> { "USE_AS_IS", "SCRAP", "MRB_ACCEPTED", "MRB_REJECTED", "REPAIR" };
            var chars = (await _charRepo.GetByInspectionIdWithDispositionsAsync(id)).ToList();
            var pendingChars = chars
                .Where(c => c.InspectionResult == "Not Conform")
                .Where(c => !c.Dispositions.Any(d =>
                    d.Decision != "VOID" && neutralizingDecisions.Contains(d.Decision)))
                .ToList();
            if (pendingChars.Any())
                return BadRequest(new { detail = $"{pendingChars.Count} Not Conform dimensional character(s) require disposition before closing." });
        }

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

    // ── POST /api/inspections/{id}/parse-op-sheet ──────────────────────────────
    [HttpPost("{id:long}/parse-op-sheet")]
    public async Task<ActionResult> ParseOpSheet(long id, IFormFile file)
    {
        var inspection = await _repo.GetByIdAsync(id);
        if (inspection is null) return NotFound();

        if (inspection.Status == "completed")
            return BadRequest(new { detail = "Tamamlanmış bir inspection'a op-sheet yüklenemez." });

        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "Dosya boş veya yüklenmedi." });

        if (!file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { detail = "Sadece .docx dosyaları desteklenmektedir." });

        // Save op sheet file
        var opSheetsDir = Path.Combine(_env.WebRootPath, "op-sheets");
        Directory.CreateDirectory(opSheetsDir);
        var filePath = Path.Combine(opSheetsDir, $"{id}.docx");

        using (var fs = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(fs);

        inspection.OpSheetPath = $"op-sheets/{id}.docx";
        await _repo.UpdateAsync(inspection);

        // Delete existing characters for this inspection before re-parsing
        var existingChars = (await _charRepo.GetByInspectionIdAsync(id)).ToList();
        if (existingChars.Any())
            _charRepo.RemoveRange(existingChars.Select(c => c.Id));

        // Parse characters from docx
        using var stream = System.IO.File.OpenRead(filePath);
        var characters = _parser.Parse(stream);

        var created = new List<Character>();
        foreach (var ch in characters)
        {
            ch.InspectionId = id;
            created.Add(await _charRepo.AddAsync(ch));
        }

        return Ok(new
        {
            characters_created = created.Count,
            characters = created.Select(c => new CharacterReadDto
            {
                Id              = c.Id,
                ItemNo          = c.ItemNo,
                Dimension       = c.Dimension,
                Badge           = c.Badge,
                Tooling         = c.Tooling,
                BPZone          = c.BPZone,
                InspectionLevel = c.InspectionLevel,
                Remarks         = c.Remarks,
                LowerLimit      = c.LowerLimit,
                UpperLimit      = c.UpperLimit,
                InspectionResult = c.InspectionResult,
                InspectionId    = c.InspectionId,
            }).ToList()
        });
    }

    // ── GET /api/inspections/{id}/report-data ─────────────────────────────────
    [HttpGet("{id:long}/report-data")]
    public async Task<ActionResult> GetReportData(long id)
    {
        var inspection = await _repo.GetWithDetailsAsync(id);
        if (inspection is null) return NotFound();

        var allPhotos  = (await _photoRepo.GetByInspectionAsync(id)).ToList();
        var characters = (await _charRepo.GetByInspectionIdAsync(id)).ToList();

        // Group photo ids by defect id
        var photosByDefect = new Dictionary<long, List<long>>();
        foreach (var photo in allPhotos)
        {
            foreach (var pd in photo.PhotoDefects)
            {
                if (!photosByDefect.ContainsKey(pd.DefectId))
                    photosByDefect[pd.DefectId] = new List<long>();
                photosByDefect[pd.DefectId].Add(photo.Id);
            }
        }

        // Neutralizing decisions (closed defects)
        var neutralizingDecisions = new HashSet<string> { "USE_AS_IS", "SCRAP", "MRB_ACCEPTED", "MRB_REJECTED", "VOID", "REPAIR" };

        // Summary
        int totalDefects   = inspection.Defects.Count;
        int neutralized    = inspection.Defects.Count(d =>
            d.Dispositions.Any(disp => disp.Decision != "VOID" && neutralizingDecisions.Contains(disp.Decision)));
        int pending        = totalDefects - neutralized;
        var byType         = inspection.Defects
            .GroupBy(d => d.DefectType?.Name ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());

        var defectsPayload = inspection.Defects.Select(d =>
        {
            var dispsSorted  = d.Dispositions.OrderBy(disp => disp.CreatedAt).ToList();
            var activeDisp   = d.Dispositions
                .OrderByDescending(disp => disp.CreatedAt)
                .FirstOrDefault(disp => disp.Decision != "VOID");
            var photoIds     = photosByDefect.TryGetValue(d.Id, out var pids) ? pids : new List<long>();

            return new
            {
                id               = d.Id,
                defect_type_name = d.DefectType?.Name,
                depth            = d.Depth,
                width            = d.Width,
                length           = d.Length,
                radius           = d.Radius,
                angle            = d.Angle,
                color            = d.Color,
                high_metal       = d.HighMetal,
                notes            = d.Notes,
                origin_defect_id = d.OriginDefectId,
                child_defect_ids = d.ChildDefects.Select(c => c.Id).ToList(),
                active_disposition = activeDisp == null ? null : new
                {
                    id          = activeDisp.Id,
                    decision    = activeDisp.Decision,
                    note        = activeDisp.Note,
                    decided_at  = activeDisp.DecidedAt,
                    entered_by  = activeDisp.EnteredBy,
                    measurements_snapshot = activeDisp.MeasurementsSnapshot,
                },
                dispositions = dispsSorted.Select(disp => new
                {
                    id          = disp.Id,
                    decision    = disp.Decision,
                    note        = disp.Note,
                    decided_at  = disp.DecidedAt,
                    entered_by  = disp.EnteredBy,
                    measurements_snapshot = disp.MeasurementsSnapshot,
                }).ToList(),
                photos = photoIds.Select(pid => new { id = pid }).ToList(),
            };
        }).ToList();

        var charsPayload = characters.Select(c => new
        {
            id               = c.Id,
            item_no          = c.ItemNo,
            dimension        = c.Dimension,
            badge            = c.Badge,
            tooling          = c.Tooling,
            remarks          = c.Remarks,
            bp_zone          = c.BPZone,
            inspection_level = c.InspectionLevel,
            lower_limit      = c.LowerLimit,
            upper_limit      = c.UpperLimit,
            inspection_result = c.InspectionResult,
        }).ToList();

        return Ok(new
        {
            id               = inspection.Id,
            part_number      = inspection.PartNumber,
            serial_number    = inspection.SerialNumber,
            operation_number = inspection.OperationNumber,
            inspector        = inspection.Inspector,
            status           = inspection.Status,
            notes            = inspection.Notes,
            created_at       = inspection.CreatedAt,
            summary = new
            {
                total       = totalDefects,
                neutralized,
                pending,
                by_type     = byType,
            },
            defects    = defectsPayload,
            characters = charsPayload,
        });
    }

    // ── GET /api/inspections/{id}/report ──────────────────────────────────────
    [HttpGet("{id:long}/report")]
    public async Task<ActionResult> GetReport(long id, [FromQuery] string? type = null)
    {
        var inspection = await _repo.GetWithDetailsAsync(id);
        if (inspection is null) return NotFound();

        var characters = await _charRepo.GetByInspectionIdAsync(id);
        var includeDetail = type == "full";

        byte[] reportBytes;
        try
        {
            reportBytes = await _reportWriter.GenerateAsync(inspection, characters.ToList(), _env.WebRootPath, includeDetail);
        }
        catch (Exception ex)
        {
            return BadRequest(new { detail = ex.Message });
        }

        var filename = includeDetail ? $"combined_report_{id}.docx" : $"report_{id}.docx";
        return File(
            reportBytes,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            filename);
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
        Id              = i.Id,
        ProjectId       = i.VisualProjectId,
        IrsProjectId    = i.IrsProjectId,
        PartNumber      = i.PartNumber,
        SerialNumber    = i.SerialNumber,
        OperationNumber = i.OperationNumber,
        Inspector       = i.Inspector,
        Status          = i.Status,
        Notes           = i.Notes,
        OpSheetPath     = i.OpSheetPath,
        CreatedAt       = i.CreatedAt,
        UpdatedAt       = i.UpdatedAt
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
