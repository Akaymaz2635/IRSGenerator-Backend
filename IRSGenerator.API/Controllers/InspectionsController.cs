using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Shared.Dtos.Character;
using IRSGenerator.Shared.Dtos.Defect;
using IRSGenerator.Shared.Dtos.Disposition;
using IRSGenerator.Shared.Dtos.Inspection;
using IRSGenerator.Shared.Dtos.Ncm;

namespace IRSGenerator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
    [Authorize(Policy = "CanWrite")]
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
    [Authorize(Policy = "CanWrite")]
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
    [Authorize(Policy = "CanWrite")]
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
    [Authorize(Policy = "CanWrite")]
    public async Task<IActionResult> Complete(long id)
    {
        var success = await _repo.SetStatusCompletedAsync(id);
        if (!success)
            return BadRequest(new { detail = "Tüm defektlerin disposition'ı tamamlanmadan inspection kapatılamaz." });
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

    // ── GET /api/inspections/{id}/ncm ────────────────────────────────────────
    // Returns full NC data for the NCM page (with entity IDs for disposition tracking)
    [HttpGet("{id:long}/ncm")]
    public async Task<ActionResult<NcmInspectionDataDto>> GetNcmData(long id)
    {
        var inspection = await _repo.GetWithDetailsAsync(id);
        if (inspection is null) return NotFound();

        var characters = (await _charRepo.GetByInspectionIdAsync(id)).ToList();

        var dimensional = characters
            .Where(IsNonConformChar)
            .Select(c => new NcmDimensionalItemDto
            {
                CharacterId = c.Id,
                ItemNo      = c.ItemNo ?? "",
                Description = BuildDimensionalDescription(c),
            })
            .ToList();

        int idx = 0;
        var visual = inspection.Defects
            .OrderBy(d => d.Id)
            .Select(d =>
            {
                idx++;
                return new NcmVisualItemDto
                {
                    DefectId    = d.Id,
                    Index       = idx,
                    Description = BuildVisualDescription(d, idx),
                };
            })
            .ToList();

        var projectName = inspection.VisualProject?.Name
                       ?? inspection.IrsProject?.ProjectType
                       ?? "";

        return Ok(new NcmInspectionDataDto
        {
            InspectionId    = inspection.Id,
            PartNumber      = inspection.PartNumber  ?? "",
            SerialNumber    = inspection.SerialNumber ?? "",
            OperationNumber = inspection.OperationNumber ?? "",
            Inspector       = inspection.Inspector ?? "",
            Status          = inspection.Status,
            ProjectName     = projectName,
            Dimensional     = dimensional,
            Visual          = visual,
        });
    }

    // ── GET /api/inspections/{id}/nonconformance-descriptions ────────────────
    [HttpGet("{id:long}/nonconformance-descriptions")]
    public async Task<ActionResult> GetNonconformanceDescriptions(long id)
    {
        var inspection = await _repo.GetWithDetailsAsync(id);
        if (inspection is null) return NotFound();

        var characters = (await _charRepo.GetByInspectionIdAsync(id)).ToList();

        // Dimensional — characters marked as non-conform (including numeric OOT)
        var dimensional = characters
            .Where(IsNonConformChar)
            .Select(c => new { item_no = c.ItemNo, description = BuildDimensionalDescription(c) })
            .ToList();

        // Visual — all defects (each defect is a nonconformance by definition)
        int idx = 0;
        var visual = inspection.Defects
            .OrderBy(d => d.Id)
            .Select(d => new { defect_id = d.Id, index = ++idx, description = BuildVisualDescription(d, idx) })
            .ToList();

        return Ok(new
        {
            inspection_id    = inspection.Id,
            part_number      = inspection.PartNumber,
            serial_number    = inspection.SerialNumber,
            operation_number = inspection.OperationNumber,
            inspector        = inspection.Inspector,
            status           = inspection.Status,
            dimensional,
            visual,
        });
    }

    private static string BuildDimensionalDescription(Core.Entities.Character c)
    {
        var itemNo = c.ItemNo?.Trim() ?? "?";
        var dim    = c.Dimension?.Trim() ?? "?";
        var zone   = string.IsNullOrWhiteSpace(c.BPZone) ? "" : $" ({c.BPZone.Trim()})";

        // Collect out-of-tolerance value strings from NumericPartResults
        var ootParts = new List<string>();
        foreach (var npr in c.NumericPartResults)
        {
            var segments = npr.Actual.Split('/',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var seg in segments)
            {
                if (!double.TryParse(seg, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    continue;
                if (c.LowerLimit != 0 && val < c.LowerLimit)
                    ootParts.Add($"{seg} U/MIN");
                else if (c.UpperLimit != 0 && val > c.UpperLimit)
                    ootParts.Add($"{seg} O/T");
            }
        }

        if (ootParts.Count > 0)
            return $"[{itemNo}] {dim}{zone} checks {string.Join(" or ", ootParts)}";

        // Attribute / LOT character or manual reject — no numeric OOT values
        var suffix = c.NumericPartResults.Count == 0 ? "Attribute FAIL" : "Non-Conform";
        return $"[{itemNo}] {dim}{zone} — {suffix}";
    }

    private static string BuildVisualDescription(Core.Entities.Defect d, int index)
    {
        var typeName = d.DefectType?.Name ?? "Unknown";
        var parts    = new List<string>();

        if (d.Depth.HasValue  && d.Depth  > 0) parts.Add($"D:{d.Depth:0.###}");
        if (d.Width.HasValue  && d.Width  > 0) parts.Add($"W:{d.Width:0.###}");
        if (d.Length.HasValue && d.Length > 0) parts.Add($"L:{d.Length:0.###}");
        if (d.Radius.HasValue && d.Radius > 0) parts.Add($"R:{d.Radius:0.###}");
        if (d.Angle.HasValue  && d.Angle  > 0) parts.Add($"A:{d.Angle:0.###}");
        if (d.Height.HasValue && d.Height > 0) parts.Add($"H:{d.Height:0.###}");
        if (!string.IsNullOrWhiteSpace(d.Color)) parts.Add($"Color:{d.Color.Trim()}");
        if (d.HighMetal) parts.Add("High Metal");

        var fieldStr = parts.Count > 0 ? " " + string.Join(", ", parts) : "";
        var noteStr  = !string.IsNullOrWhiteSpace(d.Notes) ? $" ({d.Notes.Trim()})" : "";

        return $"There is {typeName}{fieldStr}{noteStr} see defect-{index}";
    }

    private static bool IsNonConformChar(Character c)
    {
        if (string.IsNullOrEmpty(c.InspectionResult)) return false;
        var ncWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Not Conform", "Manual Reject", "Fail", "Failed" };
        if (ncWords.Contains(c.InspectionResult)) return true;
        if (c.LowerLimit == 0 && c.UpperLimit == 0) return false;
        return c.InspectionResult
            .Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(seg => double.TryParse(seg, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) &&
                ((c.LowerLimit != 0 && v < c.LowerLimit) || (c.UpperLimit != 0 && v > c.UpperLimit)));
    }

    // ── GET /api/inspections/{id}/detail-report ──────────────────────────────
    [HttpGet("{id:long}/detail-report")]
    public async Task<ActionResult> GetDetailReport(long id)
    {
        var inspection = await _repo.GetWithDetailsAsync(id);
        if (inspection is null) return NotFound();
        var characters = (await _charRepo.GetByInspectionIdAsync(id)).ToList();
        var html = BuildDetailReportHtml(inspection, characters);
        return Content(html, "text/html");
    }

    private static string BuildDetailReportHtml(Inspection inspection, List<Character> characters)
    {
        string H(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
        string FmtLimit(double v) => (v == 0) ? "—" : v.ToString("G");
        bool IsAttr(Character c) => (c.Badge ?? "").ToUpperInvariant() is "LOT" or "ATTRIBUTE";

        // ── Filter: only characters with detail-modal entries ──────────────────
        // (note, multi-part, zones, multi-categorical, categorical zones, disposition)
        var baseSerial = inspection.SerialNumber ?? "Part";
        var serialRe   = new System.Text.RegularExpressions.Regex(
            $@"^{System.Text.RegularExpressions.Regex.Escape(baseSerial)}-\d+$");

        bool HasDetailEntry(Character c) =>
            !string.IsNullOrEmpty(c.Note)
            || c.NumericPartResults.Count > 1
            || c.NumericPartResults.Any(r => !string.IsNullOrEmpty(r.PartLabel) && !serialRe.IsMatch(r.PartLabel))
            || c.CategoricalPartResults.Count > 1
            || c.CategoricalZoneResults.Any()
            || c.Dispositions.Any(d => d.Decision != "VOID");

        var detailChars = characters.Where(HasDetailEntry).ToList();

        // ── Stats (over ALL characters, not just detailed ones) ────────────────
        int total     = characters.Count;
        int okCount   = characters.Count(c => !IsNonConformChar(c) &&
                            !string.IsNullOrEmpty(c.InspectionResult) &&
                            c.InspectionResult != "Unidentified");
        int nokCount  = characters.Count(IsNonConformChar);
        int pending   = characters.Count(c => string.IsNullOrEmpty(c.InspectionResult) ||
                            c.InspectionResult == "Unidentified");
        int attrCount = characters.Count(IsAttr);
        double pct    = total > 0 ? Math.Round((double)(total - pending) / total * 100, 1) : 0;
        var generatedAt = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        // ── Per-character rows ──────────────────────────────────���───────────────
        string CharRows(Character c, int idx)
        {
            bool isAttr   = IsAttr(c);
            bool isNok    = IsNonConformChar(c);
            bool hasResult = !string.IsNullOrEmpty(c.InspectionResult) && c.InspectionResult != "Unidentified";
            string rowClass = isNok ? "row-nok" : (hasResult ? "row-ok" : "row-pending");
            string detailId = $"detail-{idx}";

            // ── Summary result cell ──────────────────────────────────────────
            string resultSummary;
            if (!hasResult)
            {
                resultSummary = "<span class='no-data'>Girilmedi</span>";
            }
            else if (isAttr)
            {
                var res = c.InspectionResult!;
                var cls = res == "Conform" ? "badge-ok" : "badge-nok";
                resultSummary = $"<span class='badge {cls}'>{H(res)}</span>";
            }
            else
            {
                // Per-value OOT highlighting for summary
                var sb2 = new System.Text.StringBuilder();
                var segs = (c.InspectionResult ?? "").Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                bool hasLim = c.LowerLimit != 0 || c.UpperLimit != 0;
                foreach (var seg in segs)
                {
                    bool oot = hasLim && double.TryParse(seg, NumberStyles.Any, CultureInfo.InvariantCulture, out var vv)
                               && (vv < c.LowerLimit || vv > c.UpperLimit);
                    sb2.Append(oot ? $"<span class='val-oot'>{H(seg)}</span>" : $"<span class='val-ok'>{H(seg)}</span>");
                    sb2.Append("<span class='val-sep'> / </span>");
                }
                if (sb2.Length > 0) sb2.Length -= "<span class='val-sep'> / </span>".Length;
                resultSummary = sb2.ToString();
            }

            // ── Active disposition ──────────────────────────────────────────
            var activeDisp = c.Dispositions.OrderByDescending(d => d.CreatedAt).FirstOrDefault(d => d.Decision != "VOID");
            string dispSummary = activeDisp != null
                ? $"<span class='badge badge-disp'>{H(activeDisp.Decision)}</span>"
                : "";

            // ── Has extra detail? ────────────────────────────────────────────
            bool hasExtra = !string.IsNullOrEmpty(c.Note)
                || !string.IsNullOrEmpty(c.Tooling)
                || !string.IsNullOrEmpty(c.BPZone)
                || !string.IsNullOrEmpty(c.Remarks)
                || !string.IsNullOrEmpty(c.InspectionLevel)
                || c.NumericPartResults.Count > 1
                || c.CategoricalPartResults.Any()
                || c.CategoricalZoneResults.Any()
                || c.Dispositions.Any(d => d.Decision != "VOID");
            string expandIcon = hasExtra ? "<span class='expand-icon'>▶</span>" : "";

            // ── Main row ─────────────────────────────────────────────────────
            var mainRow = $@"<tr class='{rowClass} main-row{(hasExtra ? " clickable" : "")}' onclick=""{(hasExtra ? $"toggleDetail('{detailId}')" : "")}"">
  <td class='col-item'>{H(c.ItemNo ?? "")}{expandIcon}</td>
  <td class='col-dim'>{H(c.Dimension ?? "")}{(string.IsNullOrEmpty(c.Badge) ? "" : $" <span class='char-badge'>{H(c.Badge)}</span>")}</td>
  <td class='col-num'>{FmtLimit(c.LowerLimit)}</td>
  <td class='col-num'>{FmtLimit(c.UpperLimit)}</td>
  <td class='col-result'>{resultSummary}</td>
  <td class='col-disp'>{dispSummary}</td>
</tr>";

            if (!hasExtra) return mainRow;

            // ── Detail row ───────────────────────────────────────────────────
            var det = new System.Text.StringBuilder();
            det.Append($"<tr class='detail-row' id='{detailId}' style='display:none'><td colspan='6'><div class='detail-panel'>");

            // Metadata chips
            var metaChips = new List<string>();
            if (!string.IsNullOrEmpty(c.Tooling))         metaChips.Add($"<span class='meta-chip'><b>Tooling:</b> {H(c.Tooling)}</span>");
            if (!string.IsNullOrEmpty(c.BPZone))          metaChips.Add($"<span class='meta-chip'><b>B/P Zone:</b> {H(c.BPZone)}</span>");
            if (!string.IsNullOrEmpty(c.Remarks))         metaChips.Add($"<span class='meta-chip'><b>Remarks:</b> {H(c.Remarks)}</span>");
            if (!string.IsNullOrEmpty(c.InspectionLevel)) metaChips.Add($"<span class='meta-chip'><b>Insp. Level:</b> {H(c.InspectionLevel)}</span>");
            if (metaChips.Count > 0)
                det.Append($"<div class='meta-row'>{string.Join("", metaChips)}</div>");

            // Note
            if (!string.IsNullOrEmpty(c.Note))
                det.Append($"<div class='detail-note'><span class='note-icon'>📝</span> <em>{H(c.Note)}</em></div>");

            // ── Numeric measurements ─────────────────────────────────────────
            if (!isAttr && c.NumericPartResults.Any())
            {
                // baseSerial / serialRe defined at method scope
                var parts = c.NumericPartResults.Where(r => string.IsNullOrEmpty(r.PartLabel) || serialRe.IsMatch(r.PartLabel))
                                                .OrderBy(r => r.PartLabel).ToList();
                var zones = c.NumericPartResults.Where(r => !string.IsNullOrEmpty(r.PartLabel) && !serialRe.IsMatch(r.PartLabel))
                                                .OrderBy(r => r.PartLabel).ToList();

                void WriteNumericTable(List<NumericPartResult> items, string sectionLabel)
                {
                    if (!items.Any()) return;
                    bool hasLim = c.LowerLimit != 0 || c.UpperLimit != 0;
                    det.Append($"<div class='sub-section'><div class='sub-title'>{sectionLabel}</div>");
                    det.Append("<table class='detail-table'><thead><tr><th>Etiket</th><th>Ölçüm</th><th>Durum</th><th>Güncelleme</th></tr></thead><tbody>");
                    foreach (var r in items)
                    {
                        var segs2 = r.Actual.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                        bool oot = hasLim && segs2.Any(s => double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var vv2) && (vv2 < c.LowerLimit || vv2 > c.UpperLimit));
                        string statusHtml = !hasLim ? "" : oot ? "<span class='status-nok'>✗ OOT</span>" : "<span class='status-ok'>✓ OK</span>";
                        string updInfo = "";
                        if (!string.IsNullOrEmpty(r.UpdateReason))
                            updInfo = $"<span class='upd-badge'>{H(r.UpdateReason)}</span>{(string.IsNullOrEmpty(r.UpdateNote) ? "" : $" <span class='upd-note'>{H(r.UpdateNote)}</span>")}";
                        var measHtml = new System.Text.StringBuilder();
                        foreach (var seg2 in segs2)
                        {
                            bool segOot = hasLim && double.TryParse(seg2, NumberStyles.Any, CultureInfo.InvariantCulture, out var sv) && (sv < c.LowerLimit || sv > c.UpperLimit);
                            measHtml.Append(segOot ? $"<span class='val-oot'>{H(seg2)}</span>" : $"<span>{H(seg2)}</span>");
                            measHtml.Append(" / ");
                        }
                        if (measHtml.Length > 3) measHtml.Length -= 3;
                        det.Append($"<tr class='{(oot ? "tr-oot" : "")}'><td class='td-lbl'>{H(r.PartLabel ?? "—")}</td><td class='td-meas'>{measHtml}</td><td>{statusHtml}</td><td>{updInfo}</td></tr>");
                    }
                    det.Append("</tbody></table></div>");
                }

                WriteNumericTable(parts, "📦 Parçalar");
                WriteNumericTable(zones, "📐 Bölgeler");
            }

            // ── Categorical (attribute) results ──────────────────────────────
            if (isAttr)
            {
                if (c.CategoricalPartResults.Any())
                {
                    det.Append("<div class='sub-section'><div class='sub-title'>📦 Parça Sonuçları</div>");
                    det.Append("<table class='detail-table'><thead><tr><th>Parça</th><th>Sonuç</th><th>Bilgi</th><th>Güncelleme</th></tr></thead><tbody>");
                    int pi = 1;
                    foreach (var pr in c.CategoricalPartResults.OrderBy(r => r.CreatedAt))
                    {
                        string sc = pr.IsConfirmed ? "status-ok" : "status-nok";
                        string sv2 = pr.IsConfirmed ? "✓ Conform" : "✗ Not Conform";
                        string updInfo2 = "";
                        if (!string.IsNullOrEmpty(pr.UpdateReason))
                            updInfo2 = $"<span class='upd-badge'>{H(pr.UpdateReason)}</span>{(string.IsNullOrEmpty(pr.UpdateNote) ? "" : $" <span class='upd-note'>{H(pr.UpdateNote)}</span>")}";
                        det.Append($"<tr class='{(pr.IsConfirmed ? "" : "tr-oot")}'><td class='td-lbl'>{inspection.SerialNumber}-{pi++}</td><td><span class='{sc}'>{sv2}</span></td><td>{H(pr.AdditionalInfo ?? "")}</td><td>{updInfo2}</td></tr>");
                    }
                    det.Append("</tbody></table></div>");
                }
                if (c.CategoricalZoneResults.Any())
                {
                    det.Append("<div class='sub-section'><div class='sub-title'>📐 Bölge Sonuçları</div>");
                    det.Append("<table class='detail-table'><thead><tr><th>Bölge</th><th>Sonuç</th><th>Bilgi</th></tr></thead><tbody>");
                    foreach (var zr in c.CategoricalZoneResults.OrderBy(r => r.ZoneName))
                    {
                        string sc2 = zr.IsConfirmed ? "status-ok" : "status-nok";
                        string sv3 = zr.IsConfirmed ? "✓ Conform" : "✗ Not Conform";
                        det.Append($"<tr class='{(zr.IsConfirmed ? "" : "tr-oot")}'><td class='td-lbl'>{H(zr.ZoneName ?? "—")}</td><td><span class='{sc2}'>{sv3}</span></td><td>{H(zr.AdditionalInfo ?? "")}</td></tr>");
                    }
                    det.Append("</tbody></table></div>");
                }
            }

            // ── Dispositions ─────────────────────────────────────────────────
            var nonVoidDisps = c.Dispositions.Where(d => d.Decision != "VOID").OrderByDescending(d => d.CreatedAt).ToList();
            if (nonVoidDisps.Any())
            {
                det.Append("<div class='sub-section'><div class='sub-title'>⚖ Dispositions</div>");
                det.Append("<table class='detail-table'><thead><tr><th>Karar</th><th>Kişi</th><th>Tarih</th><th>Not</th></tr></thead><tbody>");
                foreach (var d in nonVoidDisps)
                {
                    var dateStr = d.DecidedAt.HasValue ? d.DecidedAt.Value.ToString("dd.MM.yyyy") : (d.CreatedAt?.ToString("dd.MM.yyyy") ?? "");
                    det.Append($"<tr><td><span class='badge badge-disp'>{H(d.Decision)}</span></td><td>{H(d.EnteredBy ?? "")}</td><td>{dateStr}</td><td>{H(d.Note ?? "")}</td></tr>");
                }
                det.Append("</tbody></table></div>");
            }

            det.Append("</div></td></tr>");
            return mainRow + det.ToString();
        }

        var rows = string.Join("\n", detailChars.Select((c, i) => CharRows(c, i)));

        return $@"<!DOCTYPE html>
<html lang='tr'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width,initial-scale=1'>
<title>Detay Raporu — Muayene #{inspection.Id}</title>
<style>
@media print {{
  .no-print {{ display:none !important }}
  .detail-row {{ display:table-row !important }}
  .report-container {{ box-shadow:none;margin:0 }}
  body {{ font-size:10px }}
  .expand-icon {{ display:none }}
}}
* {{ margin:0;padding:0;box-sizing:border-box }}
body {{ font-family:'Segoe UI',Arial,sans-serif;background:#f0f2f5;color:#222 }}
.report-container {{ max-width:1400px;margin:24px auto;background:#fff;border-radius:10px;box-shadow:0 4px 24px rgba(0,0,0,.12);overflow:hidden }}
.header {{ background:linear-gradient(135deg,#1a237e,#1565c0);color:#fff;padding:28px 36px }}
.header h1 {{ font-size:1.6em;letter-spacing:.5px;margin-bottom:6px }}
.header-meta {{ display:flex;gap:28px;flex-wrap:wrap;font-size:.88em;opacity:.9;margin-top:10px }}
.header-meta span b {{ font-weight:600 }}
.stats-section {{ padding:20px 36px;background:#f8f9fc;border-bottom:1px solid #e0e4ef }}
.stats-grid {{ display:grid;grid-template-columns:repeat(auto-fit,minmax(120px,1fr));gap:12px;margin-top:12px }}
.stat-card {{ background:#fff;border-radius:8px;padding:14px;text-align:center;box-shadow:0 2px 8px rgba(0,0,0,.07) }}
.stat-number {{ font-size:1.9em;font-weight:700;display:block }}
.stat-label {{ color:#666;font-size:.8em;margin-top:3px }}
.stat-ok .stat-number {{ color:#2e7d32 }}
.stat-nok .stat-number {{ color:#c62828 }}
.stat-pending .stat-number {{ color:#e65100 }}
.stat-total .stat-number {{ color:#1565c0 }}
.stat-pct .stat-number {{ color:#6a1b9a }}
.table-section {{ padding:20px 36px }}
.section-title {{ font-size:1.05em;font-weight:700;color:#1a237e;margin-bottom:12px;padding-bottom:7px;border-bottom:2px solid #e0e4ef }}
table {{ width:100%;border-collapse:collapse;font-size:.86em }}
thead tr {{ background:linear-gradient(135deg,#263238,#37474f);color:#fff }}
th {{ padding:9px 11px;text-align:left;font-weight:600;white-space:nowrap }}
.col-item {{ width:95px }}
.col-dim  {{ min-width:180px }}
.col-num  {{ width:72px;text-align:center }}
.col-result {{ min-width:130px }}
.col-disp {{ width:120px }}
td {{ padding:8px 11px;border-bottom:1px solid #eceff1;vertical-align:middle }}
.main-row td {{ border-bottom:none }}
.row-ok {{ background:#fff }}
.row-ok.clickable:hover {{ background:#f1f8e9;cursor:pointer }}
.row-nok {{ background:#fff8f8 }}
.row-nok.clickable:hover {{ background:#ffebee;cursor:pointer }}
.row-pending {{ background:#fffde7 }}
.row-pending.clickable:hover {{ background:#fff9c4;cursor:pointer }}
.expand-icon {{ float:right;font-size:.75em;color:#90a4ae;transition:transform .2s }}
.expand-icon.open {{ transform:rotate(90deg) }}
.val-ok {{ color:#2e7d32 }}
.val-oot {{ color:#c62828;font-weight:700 }}
.val-sep {{ color:#90a4ae }}
.no-data {{ color:#9e9e9e;font-style:italic }}
.char-badge {{ display:inline-block;padding:1px 6px;border-radius:10px;font-size:.75em;font-weight:600;background:#e3f2fd;color:#1565c0;margin-left:4px }}
.badge {{ display:inline-block;padding:3px 8px;border-radius:10px;font-size:.78em;font-weight:600;color:#fff }}
.badge-ok {{ background:#2e7d32 }}
.badge-nok {{ background:#c62828 }}
.badge-pending {{ background:#e65100 }}
.badge-disp {{ background:#1565c0 }}
/* Detail panel */
.detail-row td {{ padding:0;background:#fafbff;border-top:none;border-bottom:2px solid #e0e4ef }}
.detail-panel {{ padding:16px 20px 20px 36px }}
.meta-row {{ display:flex;flex-wrap:wrap;gap:8px;margin-bottom:10px }}
.meta-chip {{ background:#eceff1;border-radius:5px;padding:3px 10px;font-size:.8em;color:#455a64 }}
.detail-note {{ background:#fffde7;border-left:3px solid #fbc02d;padding:6px 12px;border-radius:0 4px 4px 0;font-size:.85em;color:#5d4037;margin-bottom:10px }}
.note-icon {{ margin-right:4px }}
.sub-section {{ margin-bottom:14px }}
.sub-title {{ font-weight:700;font-size:.82em;color:#546e7a;text-transform:uppercase;letter-spacing:.5px;margin-bottom:6px }}
.detail-table {{ width:auto;min-width:500px;font-size:.83em;border:1px solid #eceff1;border-radius:6px;overflow:hidden }}
.detail-table thead tr {{ background:#eceff1;color:#37474f }}
.detail-table th {{ padding:6px 10px;font-weight:600 }}
.detail-table td {{ padding:5px 10px;background:#fff;border-bottom:1px solid #f5f5f5;vertical-align:middle }}
.detail-table tr:last-child td {{ border-bottom:none }}
.tr-oot td {{ background:#fff5f5 }}
.td-lbl {{ color:#546e7a;font-weight:600;white-space:nowrap }}
.td-meas {{ font-family:monospace;font-size:.95em }}
.status-ok {{ color:#2e7d32;font-weight:600 }}
.status-nok {{ color:#c62828;font-weight:600 }}
.upd-badge {{ display:inline-block;padding:1px 6px;border-radius:8px;background:#fff3e0;color:#e65100;font-size:.78em;font-weight:600 }}
.upd-note {{ font-size:.78em;color:#78909c;font-style:italic }}
.footer {{ background:#263238;color:#cfd8dc;text-align:center;padding:14px;font-size:.8em }}
.print-btn {{ position:fixed;bottom:24px;right:24px;background:#1565c0;color:#fff;border:none;padding:11px 20px;border-radius:22px;cursor:pointer;font-size:.93em;box-shadow:0 4px 16px rgba(21,101,192,.4);transition:all .2s }}
.print-btn:hover {{ background:#0d47a1;transform:translateY(-2px) }}
.expand-all-btn {{ background:#546e7a;color:#fff;border:none;padding:6px 14px;border-radius:16px;cursor:pointer;font-size:.82em;margin-bottom:12px }}
</style>
</head>
<body>
<button class='print-btn no-print' onclick='window.print()'>🖨 Yazdır / PDF</button>
<div class='report-container'>

  <div class='header'>
    <h1>📋 Boyutsal Detay Raporu — Muayene #{inspection.Id}</h1>
    <div class='header-meta'>
      <span><b>Parça No:</b> {H(inspection.PartNumber)}</span>
      <span><b>Seri No:</b> {H(inspection.SerialNumber)}</span>
      <span><b>Operasyon:</b> {H(inspection.OperationNumber)}</span>
      <span><b>Muayeneci:</b> {H(inspection.Inspector)}</span>
      <span><b>Durum:</b> {H(inspection.Status)}</span>
      <span><b>Oluşturuldu:</b> {generatedAt}</span>
    </div>
  </div>

  <div class='stats-section'>
    <div class='section-title'>📊 Özet İstatistikler</div>
    <div class='stats-grid'>
      <div class='stat-card stat-total'><span class='stat-number'>{total}</span><div class='stat-label'>Toplam Karakter</div></div>
      <div class='stat-card stat-ok'><span class='stat-number'>{okCount}</span><div class='stat-label'>OK / Conform</div></div>
      <div class='stat-card stat-nok'><span class='stat-number'>{nokCount}</span><div class='stat-label'>NOK / Uygunsuz</div></div>
      <div class='stat-card stat-pending'><span class='stat-number'>{pending}</span><div class='stat-label'>Girilmedi</div></div>
      <div class='stat-card'><span class='stat-number'>{attrCount}</span><div class='stat-label'>Attribute</div></div>
      <div class='stat-card stat-pct'><span class='stat-number'>{pct}%</span><div class='stat-label'>Tamamlanma</div></div>
    </div>
  </div>

  <div class='table-section'>
    <div class='section-title'>📐 Detay Girişi Yapılan Karakterler</div>
    <div style='margin-bottom:10px;display:flex;align-items:center;gap:16px;flex-wrap:wrap'>
      <button class='expand-all-btn no-print' onclick='toggleAll()'>▶ Tümünü Aç / Kapat</button>
      <span style='font-size:.82em;color:#546e7a'>{detailChars.Count} / {total} karakter · Not, çoklu parça/bölge veya disposition içerenler</span>
    </div>
    <table>
      <thead><tr>
        <th class='col-item'>Madde No</th>
        <th class='col-dim'>Ölçü / Tanım</th>
        <th class='col-num'>Alt Limit</th>
        <th class='col-num'>Üst Limit</th>
        <th class='col-result'>Sonuç</th>
        <th class='col-disp'>Karar</th>
      </tr></thead>
      <tbody>{rows}</tbody>
    </table>
  </div>

  <div class='footer'>
    Detay Raporu · Muayene #{inspection.Id} · {generatedAt}
  </div>
</div>
<script>
function toggleDetail(id) {{
  var el = document.getElementById(id);
  if (!el) return;
  var visible = el.style.display !== 'none';
  el.style.display = visible ? 'none' : 'table-row';
  var mainRow = el.previousElementSibling;
  if (mainRow) {{
    var icon = mainRow.querySelector('.expand-icon');
    if (icon) icon.classList.toggle('open', !visible);
  }}
}}
var _allOpen = false;
function toggleAll() {{
  _allOpen = !_allOpen;
  document.querySelectorAll('.detail-row').forEach(function(el) {{
    el.style.display = _allOpen ? 'table-row' : 'none';
  }});
  document.querySelectorAll('.expand-icon').forEach(function(icon) {{
    icon.classList.toggle('open', _allOpen);
  }});
}}
</script>
</body></html>";
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
