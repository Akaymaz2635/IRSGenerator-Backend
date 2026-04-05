using System.IO.Compression;
using System.Text.RegularExpressions;
using IRSGenerator.Shared.Dtos.Ncm;

namespace IRSGenerator.Core.Services;

/// <summary>
/// Fills NCR-101 .docx templates with inspection NC data.
/// Returns file contents in memory — no disk writes.
/// </summary>
public class NcmSheetGenerator
{
    private const int SlotsPerRow  = 3;
    private const int RowsPerSheet = 7;
    private const int NcPerSheet   = SlotsPerRow * RowsPerSheet; // 21

    private const string NcPlaceholder = "[NONCONFROMANCE PLACE HOLDER]";

    private readonly string _templatesDir;

    public NcmSheetGenerator(string templatesDir)
    {
        _templatesDir = templatesDir;
    }

    public string TemplatesDir => _templatesDir;

    /// <summary>
    /// Generates one or more filled .docx files in memory.
    /// Returns list of (FileName, FileBytes).
    /// </summary>
    public List<(string FileName, byte[] Content)> Generate(
        GenerateDispositionSheetDto request,
        string templateFileName,
        string causeCodeText,
        string projectName,
        string partNumber,
        string serialNumber,
        string operationNumber)
    {
        var templatePath = Path.Combine(_templatesDir, templateFileName);
        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Template not found: {templateFileName}", templatePath);

        var templateBytes = File.ReadAllBytes(templatePath);
        var items         = request.Items;
        var sheetData     = SplitIntoSheets(items, NcPerSheet);
        var results       = new List<(string, byte[])>();

        var baseName = Path.GetFileNameWithoutExtension(templateFileName);
        var ext      = Path.GetExtension(templateFileName);

        for (int i = 0; i < sheetData.Count; i++)
        {
            var suffix  = i == 0 ? "" : $"_{i + 1}";
            var fileName = $"{baseName}{suffix}{ext}";
            var content  = FillTemplate(
                templateBytes,
                serialNumber,
                request.Oper, request.CauseOper,
                request.Qty.ToString(), causeCodeText,
                sheetData[i]);

            results.Add((fileName, content));
        }

        return results;
    }

    // ── Private helpers ──────────────────────────────────────────────────

    private static List<List<NcmSheetItemDto>> SplitIntoSheets(
        List<NcmSheetItemDto> items, int maxPerSheet)
    {
        var result = new List<List<NcmSheetItemDto>>();
        for (int i = 0; i < items.Count; i += maxPerSheet)
            result.Add(items.Skip(i).Take(maxPerSheet).ToList());
        if (result.Count == 0)
            result.Add([]);
        return result;
    }

    private static byte[] FillTemplate(
        byte[] templateBytes,
        string serialNumber,
        string oper, string causeOper,
        string qty, string causeCode,
        List<NcmSheetItemDto> ncItems)
    {
        var ms = new MemoryStream(templateBytes.Length);
        ms.Write(templateBytes, 0, templateBytes.Length);
        ms.Position = 0;

        using (var zip = new ZipArchive(ms, ZipArchiveMode.Update, leaveOpen: true))
        {
            var entry = zip.GetEntry("word/document.xml")
                ?? throw new InvalidOperationException("word/document.xml not found in template.");

            string xml;
            using (var reader = new StreamReader(entry.Open()))
                xml = reader.ReadToEnd();

            xml = xml.Replace("[SERIAL NUMBER]", EscapeXml(serialNumber));
            xml = xml.Replace("[OPER]",          EscapeXml(oper));
            xml = xml.Replace("[C-OP]",          EscapeXml(causeOper));
            xml = xml.Replace("[QTY]",           EscapeXml(qty));
            xml = xml.Replace("[C.CODE]",        EscapeXml(causeCode));

            int ncIdx = 0;
            xml = Regex.Replace(
                xml,
                Regex.Escape(NcPlaceholder),
                _ =>
                {
                    string replacement = ncIdx < ncItems.Count
                        ? EscapeXml(ncItems[ncIdx].Description)
                        : "";
                    ncIdx++;
                    return replacement;
                });

            entry.Delete();
            var newEntry = zip.CreateEntry("word/document.xml", CompressionLevel.Optimal);
            using var writer = new StreamWriter(newEntry.Open());
            writer.Write(xml);
        }

        return ms.ToArray();
    }

    private static string EscapeXml(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
