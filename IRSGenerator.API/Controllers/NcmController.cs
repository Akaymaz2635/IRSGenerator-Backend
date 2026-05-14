using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using MES.API.Utils;
using MES.Domain.Dtos.Ncm;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Controllers;

[Route("api/ncm")]
[ApiController]
[Authorize]
public class NcmController : ControllerBase
{
    private const string DocxMime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    private const string ZipMime  = "application/zip";

    private readonly IUnitOfWork       _uow;
    private readonly NcmSheetGenerator _generator;

    public NcmController(IUnitOfWork uow, NcmSheetGenerator generator)
    {
        _uow       = uow       ?? throw new ArgumentNullException(nameof(uow));
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
    }

    [HttpPost("generate")]
    [Authorize(Policy = "CanWriteNcm")]
    public async Task<IActionResult> Generate([FromBody] GenerateDispositionSheetDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest(new { detail = "En az bir NC seçilmeli." });

        var inspection = await _uow.Inspections.GetByIdAsync(dto.InspectionId, q => q
            .Include(i => i.VisualProject)
            .Include(i => i.IrsProject)
            .Include(i => i.Defects).ThenInclude(d => d.DefectType)
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions));
        if (inspection is null) return NotFound(new { detail = "Inspection bulunamadı." });

        var causeCode = await _uow.CauseCodes.GetByIdAsync(dto.CauseCodeId);
        if (causeCode is null) return BadRequest(new { detail = "Geçersiz cause code." });

        var dispType = await _uow.NcmDispositionTypes.GetByIdAsync(dto.DispositionTypeId);
        if (dispType is null) return BadRequest(new { detail = "Geçersiz disposition type." });

        var sheets = _generator.Generate(
            dto,
            dispType.TemplateFileName,
            $"{causeCode.Code} - {causeCode.Description}",
            inspection.VisualProject?.Name ?? "",
            inspection.PartNumber ?? "",
            inspection.SerialNumber ?? "",
            inspection.OperationNumber ?? "");

        if (sheets.Count == 0) return StatusCode(500, new { detail = "Sayfa oluşturulamadı." });

        if (sheets.Count == 1)
        {
            var (_, bytes) = sheets[0];
            return File(bytes, DocxMime, $"ncm_{dto.InspectionId}.docx");
        }

        using var ms  = new MemoryStream();
        using var zip = new ZipArchive(ms, ZipArchiveMode.Create, true);
        foreach (var (name, bytes) in sheets)
        {
            var entry = zip.CreateEntry(name);
            await using var entryStream = entry.Open();
            await entryStream.WriteAsync(bytes);
        }
        zip.Dispose();
        return File(ms.ToArray(), ZipMime, $"ncm_{dto.InspectionId}.zip");
    }

    [HttpPost("upload-template")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UploadTemplate(string fileName, IFormFile file)
    {
        if (file.Length == 0) return BadRequest(new { detail = "Dosya boş." });

        var savePath = Path.Combine(_generator.TemplatesDir, fileName);
        Directory.CreateDirectory(_generator.TemplatesDir);
        await using var fs = System.IO.File.Create(savePath);
        await file.CopyToAsync(fs);
        return Ok(new { message = "Şablon yüklendi." });
    }
}
