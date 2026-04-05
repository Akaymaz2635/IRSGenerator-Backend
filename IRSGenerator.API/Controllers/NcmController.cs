using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IRSGenerator.Core.Repositories;
using IRSGenerator.Core.Services;
using IRSGenerator.Shared.Dtos.Ncm;

namespace IRSGenerator.API.Controllers;

[Route("api/ncm")]
[ApiController]
[Authorize]
public class NcmController : ControllerBase
{
    private const string DocxMime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    private const string ZipMime  = "application/zip";

    private readonly IInspectionRepository         _inspRepo;
    private readonly ICauseCodeRepository          _causeCodeRepo;
    private readonly INcmDispositionTypeRepository _dispTypeRepo;
    private readonly NcmSheetGenerator             _generator;

    public NcmController(
        IInspectionRepository inspRepo,
        ICauseCodeRepository causeCodeRepo,
        INcmDispositionTypeRepository dispTypeRepo,
        NcmSheetGenerator generator)
    {
        _inspRepo      = inspRepo      ?? throw new ArgumentNullException(nameof(inspRepo));
        _causeCodeRepo = causeCodeRepo ?? throw new ArgumentNullException(nameof(causeCodeRepo));
        _dispTypeRepo  = dispTypeRepo  ?? throw new ArgumentNullException(nameof(dispTypeRepo));
        _generator     = generator     ?? throw new ArgumentNullException(nameof(generator));
    }

    // ── POST /api/ncm/generate ───────────────────────────────────────────────
    // Returns the filled .docx (single sheet) or a .zip (multiple sheets) as a download.
    [HttpPost("generate")]
    [Authorize(Policy = "CanWriteNcm")]
    public async Task<IActionResult> Generate([FromBody] GenerateDispositionSheetDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest(new { detail = "En az bir NC seçilmeli." });

        var inspection = await _inspRepo.GetWithDetailsAsync(dto.InspectionId);
        if (inspection is null) return NotFound(new { detail = "Inspection bulunamadı." });

        var causeCode = await _causeCodeRepo.GetByIdAsync(dto.CauseCodeId);
        if (causeCode is null) return BadRequest(new { detail = "Geçersiz cause code." });

        var dispType = await _dispTypeRepo.GetByIdAsync(dto.DispositionTypeId);
        if (dispType is null) return BadRequest(new { detail = "Geçersiz disposition type." });

        if (string.IsNullOrWhiteSpace(dispType.TemplateFileName))
            return BadRequest(new { detail = "Bu karar tipine ait şablon dosyası tanımlı değil." });

        var causeCodeText   = $"{causeCode.Code} — {causeCode.Description}";
        var projectName     = inspection.VisualProject?.Name ?? inspection.IrsProject?.ProjectType ?? "UNKNOWN";
        var partNumber      = inspection.PartNumber      ?? "UNKNOWN";
        var serialNumber    = inspection.SerialNumber    ?? "UNKNOWN";
        var operationNumber = inspection.OperationNumber ?? "UNKNOWN";

        List<(string FileName, byte[] Content)> sheets;
        try
        {
            sheets = _generator.Generate(
                dto,
                dispType.TemplateFileName,
                causeCodeText,
                projectName,
                partNumber,
                serialNumber,
                operationNumber);
        }
        catch (FileNotFoundException ex)
        {
            return BadRequest(new { detail = $"Şablon dosyası bulunamadı: {ex.FileName}" });
        }

        if (sheets.Count == 1)
        {
            return File(sheets[0].Content, DocxMime, sheets[0].FileName);
        }

        // Multiple sheets → bundle as .zip
        var zipMs = new MemoryStream();
        using (var zip = new ZipArchive(zipMs, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (fileName, content) in sheets)
            {
                var entry = zip.CreateEntry(fileName, CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                entryStream.Write(content, 0, content.Length);
            }
        }
        zipMs.Position = 0;

        var zipName = $"{Path.GetFileNameWithoutExtension(dispType.TemplateFileName)}_sheets.zip";
        return File(zipMs.ToArray(), ZipMime, zipName);
    }

    // ── POST /api/ncm/templates/{fileName} ──────────────────────────────────
    // Upload a .docx template file (admin only).
    [HttpPost("templates/{fileName}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UploadTemplate(string fileName, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "Dosya boş." });

        if (!fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { detail = "Sadece .docx dosyaları yüklenebilir." });

        Directory.CreateDirectory(_generator.TemplatesDir);
        var savePath = Path.Combine(_generator.TemplatesDir, fileName);

        await using var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
        await file.CopyToAsync(stream);

        return Ok(new { file_name = fileName });
    }

    // ── GET /api/ncm/templates ───────────────────────────────────────────────
    // List available template files (admin only).
    [HttpGet("templates")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult ListTemplates()
    {
        if (!Directory.Exists(_generator.TemplatesDir))
            return Ok(new { files = Array.Empty<string>() });

        var files = Directory.GetFiles(_generator.TemplatesDir, "*.docx")
            .Select(Path.GetFileName)
            .OrderBy(f => f)
            .ToArray();

        return Ok(new { files });
    }
}
