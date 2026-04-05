using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Services;

/// <summary>
/// Generates a Word report by writing measurement results into the op sheet template.
/// Port of Python word_save_as.py logic.
/// </summary>
public class WordReportWriter
{
    public async Task<byte[]> GenerateAsync(
        Inspection inspection,
        List<Character> characters,
        string webRootPath,
        bool includeDetailSection = false)
    {
        // Load template op sheet
        string? templatePath = null;
        if (!string.IsNullOrEmpty(inspection.OpSheetPath))
            templatePath = Path.Combine(webRootPath, inspection.OpSheetPath.Replace('/', Path.DirectorySeparatorChar));

        using var ms = new MemoryStream();

        if (templatePath != null && File.Exists(templatePath))
        {
            // Clone op sheet and write back actual values
            var templateBytes = await File.ReadAllBytesAsync(templatePath);
            ms.Write(templateBytes, 0, templateBytes.Length);
            ms.Position = 0;

            using var doc = WordprocessingDocument.Open(ms, true);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body != null)
                WriteActualValues(body, characters);

            AppendDefectSection(doc, inspection);
            AppendNonConformanceSection(doc, inspection, characters);
            if (includeDetailSection)
                AppendDimensionalDetailSection(doc, characters);
            doc.Save();
        }
        else
        {
            // Create minimal report from scratch
            using var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document);
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            var body = mainPart.Document.Body!;

            AddReportHeader(body, inspection);
            AddDimensionalTable(body, characters);
            AppendDefectSection(doc, inspection);
            AppendNonConformanceSection(doc, inspection, characters);
            if (includeDetailSection)
                AppendDimensionalDetailSection(doc, characters);
            doc.Save();
        }

        return ms.ToArray();
    }

    private static void WriteActualValues(Body body, List<Character> characters)
    {
        // Build a lookup: normalized itemNo → latest actual per part_label (joined by " / ")
        // Grouping by part_label ensures post-REWORK re-measurement overwrites the old value,
        // while multi-part entries (P1/P2/P3) are still shown together.
        var actuals = characters
            .Where(c => c.NumericPartResults.Any())
            .ToDictionary(
                c => (c.ItemNo ?? "").Replace(" ", "").ToUpperInvariant(),
                c =>
                {
                    var allActuals = c.NumericPartResults
                        .GroupBy(r => r.PartLabel)
                        .Select(g => g.OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue).First().Actual)
                        .ToList();
                    var nums = allActuals
                        .SelectMany(a => a.Split('/', System.StringSplitOptions.TrimEntries | System.StringSplitOptions.RemoveEmptyEntries))
                        .Select(s => double.TryParse(s, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var d) ? (double?)d : null)
                        .Where(d => d.HasValue).Select(d => d!.Value).ToList();
                    if (!nums.Any()) return string.Join(" / ", allActuals);
                    var min = nums.Min(); var max = nums.Max();
                    return Math.Abs(max - min) < 1e-9 ? min.ToString("G") : $"{min} / {max}";
                });

        // Also include attribute (categorical) characters — their result is stored in InspectionResult
        foreach (var c in characters.Where(c =>
            !c.NumericPartResults.Any() &&
            !string.IsNullOrEmpty(c.InspectionResult) &&
            c.InspectionResult != "Unidentified"))
        {
            var key = (c.ItemNo ?? "").Replace(" ", "").ToUpperInvariant();
            if (!actuals.ContainsKey(key))
                actuals[key] = c.InspectionResult;  // "Conform" or "Not Conform"
        }

        foreach (var table in body.Elements<Table>())
        {
            // Check all rows for a header row (not just first — some tables have merged header rows)
            TableRow? headerRow = null;
            int headerRowIdx = 0;
            var allRows = table.Elements<TableRow>().ToList();
            for (int ri = 0; ri < Math.Min(allRows.Count, 5); ri++)
            {
                var txt = GetRowText(allRows[ri]).ToUpper();
                if ((txt.Contains("ITEM") || txt.Contains("NO")) && txt.Contains("ACTUAL"))
                {
                    headerRow = allRows[ri];
                    headerRowIdx = ri;
                    break;
                }
            }
            if (headerRow is null) continue;

            // Find ACTUAL and ITEM NO column indices
            var headerCells = headerRow.Elements<TableCell>().ToList();
            int actualCol = -1, itemNoCol = -1;
            for (int i = 0; i < headerCells.Count; i++)
            {
                var t = GetCellText(headerCells[i]).ToUpper().Trim();
                if (t.Contains("ACTUAL")) actualCol = i;
                if (t.Contains("ITEM") || t == "NO" || t == "NO.") itemNoCol = i;
            }
            if (actualCol < 0) continue;
            if (itemNoCol < 0) itemNoCol = 0; // fallback

            for (int ri = headerRowIdx + 1; ri < allRows.Count; ri++)
            {
                var cells = allRows[ri].Elements<TableCell>().ToList();
                if (cells.Count <= Math.Max(itemNoCol, actualCol)) continue;

                var itemNoRaw = GetCellText(cells[itemNoCol]).Trim();
                if (string.IsNullOrWhiteSpace(itemNoRaw)) continue;
                var itemNo = itemNoRaw.Replace(" ", "").ToUpperInvariant();
                if (!actuals.TryGetValue(itemNo, out var actual)) continue;

                // Write actual value into cell, preserving existing run formatting
                var cell = cells[actualCol];
                var para = cell.Elements<Paragraph>().FirstOrDefault()
                    ?? cell.AppendChild(new Paragraph());
                var existingRuns = para.Elements<Run>().ToList();
                var existingProps = existingRuns.FirstOrDefault()
                    ?.Elements<RunProperties>().FirstOrDefault()
                    ?.CloneNode(true) as RunProperties;
                foreach (var r in existingRuns) r.Remove();

                var ch = characters.FirstOrDefault(c =>
                    (c.ItemNo ?? "").Replace(" ", "").ToUpperInvariant() == itemNo);
                bool isNc = ch != null && IsNonConformChar(ch);
                bool hasLimits = ch != null && (ch.LowerLimit != 0 || ch.UpperLimit != 0);
                var parts = actual.Split('/').Select(s => s.Trim()).ToList();

                if (isNc && hasLimits && parts.Count > 1)
                {
                    // Multi-value (e.g. "12 / 145"): highlight only the OOT values
                    for (int pi = 0; pi < parts.Count; pi++)
                    {
                        if (pi > 0)
                            para.AppendChild(new Run(new Text(" / ") { Space = SpaceProcessingModeValues.Preserve }));

                        var partRun = new Run(new Text(parts[pi]));
                        if (existingProps != null)
                            partRun.PrependChild((RunProperties)existingProps.CloneNode(true));

                        bool oot = double.TryParse(parts[pi],
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var pv) &&
                            ((ch!.LowerLimit != 0 && pv < ch.LowerLimit) ||
                             (ch.UpperLimit != 0 && pv > ch.UpperLimit));

                        if (oot)
                        {
                            var rp = partRun.GetFirstChild<RunProperties>()
                                     ?? partRun.PrependChild(new RunProperties());
                            rp.AppendChild(new Highlight { Val = HighlightColorValues.Yellow });
                        }
                        para.AppendChild(partRun);
                    }
                }
                else
                {
                    // Single value or no limits: one run, highlight whole text if OOT
                    var newRun = new Run(new Text(actual));
                    if (existingProps != null) newRun.PrependChild(existingProps);

                    if (isNc)
                    {
                        var rp = newRun.GetFirstChild<RunProperties>()
                                 ?? newRun.PrependChild(new RunProperties());
                        rp.AppendChild(new Highlight { Val = HighlightColorValues.Yellow });
                    }
                    para.AppendChild(newRun);
                }
            }
        }
    }

    private static void AddReportHeader(Body body, Inspection inspection)
    {
        var title = new Paragraph(new Run(new Text($"Inspection Report #{inspection.Id}")));
        SetBold(title);
        body.AppendChild(title);
        body.AppendChild(new Paragraph(new Run(new Text($"Part: {inspection.PartNumber}  SN: {inspection.SerialNumber}  Op: {inspection.OperationNumber}"))));
        body.AppendChild(new Paragraph(new Run(new Text($"Inspector: {inspection.Inspector}  Status: {inspection.Status}"))));
        body.AppendChild(new Paragraph(new Run(new Text(" "))));
    }

    private static void AddDimensionalTable(Body body, List<Character> characters)
    {
        if (!characters.Any()) return;

        body.AppendChild(new Paragraph(new Run(new Text("Dimensional Results"))));

        var table = new Table();
        var headerRow = new TableRow();
        foreach (var h in new[] { "Item No", "Dimension", "Lower", "Upper", "Actual", "Result" })
            headerRow.AppendChild(MakeCell(h, bold: true));
        table.AppendChild(headerRow);

        foreach (var ch in characters)
        {
            var actual = ch.NumericPartResults.Any()
                ? string.Join(" / ", ch.NumericPartResults
                    .GroupBy(r => r.PartLabel)
                    .Select(g => g.OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue).First().Actual))
                : "—";

            var row = new TableRow();
            row.AppendChild(MakeCell(ch.ItemNo));
            row.AppendChild(MakeCell(ch.Dimension));
            row.AppendChild(MakeCell(ch.LowerLimit.ToString("G")));
            row.AppendChild(MakeCell(ch.UpperLimit.ToString("G")));
            row.AppendChild(MakeCell(actual));
            row.AppendChild(MakeCell(ch.InspectionResult));
            table.AppendChild(row);
        }

        body.AppendChild(table);
        body.AppendChild(new Paragraph(new Run(new Text(" "))));
    }

    private static void AppendNonConformanceSection(WordprocessingDocument doc, Inspection inspection, List<Character> characters)
    {
        var body = doc.MainDocumentPart!.Document.Body!;

        var notConformChars = characters.Where(c => c.InspectionResult == "Not Conform").ToList();
        var notConformDefects = inspection.Defects
            .Where(d => {
                var active = d.Dispositions.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.Decision != "VOID");
                return active != null && active.Decision != "USE_AS_IS";
            }).ToList();

        if (!notConformChars.Any() && !notConformDefects.Any()) return;

        body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        var title = new Paragraph(new Run(new Text("Non-Conformances")));
        SetBold(title);
        body.AppendChild(title);

        if (notConformChars.Any())
        {
            body.AppendChild(new Paragraph(new Run(new Text("Dimensional Non-Conformances:"))));
            var table = new Table();
            var hrow = new TableRow();
            foreach (var h in new[] { "Item No", "Dimension", "B/P Zone", "Actual", "Disposition" })
                hrow.AppendChild(MakeCell(h, bold: true));
            table.AppendChild(hrow);

            foreach (var c in notConformChars)
            {
                var actual = c.NumericPartResults.Any()
                    ? string.Join(" / ", c.NumericPartResults
                        .GroupBy(r => r.PartLabel)
                        .Select(g => g.OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue).First().Actual))
                    : "N/A";
                var activeDisp = c.Dispositions
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefault(d => d.Decision != "VOID");

                var row = new TableRow();
                row.AppendChild(MakeCell(c.ItemNo));
                row.AppendChild(MakeCell(c.Dimension));
                row.AppendChild(MakeCell(c.BPZone ?? ""));
                row.AppendChild(MakeCell(actual));
                row.AppendChild(MakeCell(activeDisp?.Decision ?? "Pending"));
                table.AppendChild(row);
            }
            body.AppendChild(table);
            body.AppendChild(new Paragraph(new Run(new Text(" "))));
        }

        if (notConformDefects.Any())
        {
            body.AppendChild(new Paragraph(new Run(new Text("Visual Non-Conformances:"))));
            foreach (var defect in notConformDefects)
            {
                var dims = new List<string>();
                if (defect.Depth.HasValue)  dims.Add($"D:{defect.Depth:G}");
                if (defect.Width.HasValue)  dims.Add($"W:{defect.Width:G}");
                if (defect.Length.HasValue) dims.Add($"L:{defect.Length:G}");
                var dimsStr = dims.Any() ? string.Join(", ", dims) : "";
                var defNo = notConformDefects.IndexOf(defect) + 1;
                var text = $"There is {defect.DefectType?.Name ?? "defect"} on part{(dimsStr.Length > 0 ? $" with value {dimsStr}" : "")} defect number #{defNo}.";
                body.AppendChild(new Paragraph(new Run(new Text(text))));
            }
        }
    }

    private static void AppendDefectSection(WordprocessingDocument doc, Inspection inspection)
    {
        var body = doc.MainDocumentPart!.Document.Body!;

        if (!inspection.Defects.Any()) return;

        // Page break before defect section
        body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        var sectionTitle = new Paragraph(new Run(new Text("Visual Inspection — Defects")));
        SetBold(sectionTitle);
        body.AppendChild(sectionTitle);

        var table = new Table();
        var headerRow = new TableRow();
        foreach (var h in new[] { "Defect Type", "Dimensions", "Notes", "Disposition" })
            headerRow.AppendChild(MakeCell(h, bold: true));
        table.AppendChild(headerRow);

        foreach (var defect in inspection.Defects)
        {
            var dims = new List<string>();
            if (defect.Depth.HasValue)  dims.Add($"D:{defect.Depth:G}");
            if (defect.Width.HasValue)  dims.Add($"W:{defect.Width:G}");
            if (defect.Length.HasValue) dims.Add($"L:{defect.Length:G}");

            var activeDisp = defect.Dispositions
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault(d => d.Decision != "VOID");

            var row = new TableRow();
            row.AppendChild(MakeCell(defect.DefectType?.Name ?? $"#{defect.DefectTypeId}"));
            row.AppendChild(MakeCell(string.Join(", ", dims)));
            row.AppendChild(MakeCell(defect.Notes ?? ""));
            row.AppendChild(MakeCell(activeDisp?.Decision ?? "—"));
            table.AppendChild(row);
        }

        body.AppendChild(table);
    }

    private static TableCell MakeCell(string text, bool bold = false)
    {
        var run = new Run(new Text(text));
        if (bold)
            run.PrependChild(new RunProperties(new Bold()));

        var para = new Paragraph(run);
        var props = new TableCellProperties(
            new TableCellWidth { Type = TableWidthUnitValues.Auto });

        return new TableCell(props, para);
    }

    private static void SetBold(Paragraph para)
    {
        var pPr = para.PrependChild(new ParagraphProperties());
        var rPr = pPr.AppendChild(new ParagraphMarkRunProperties());
        rPr.AppendChild(new Bold());
    }

    private static string GetRowText(TableRow row)
        => string.Join(" ", row.Elements<TableCell>().Select(GetCellText));

    private static string GetCellText(TableCell cell)
        => string.Join(" ", cell.Elements<Paragraph>()
            .SelectMany(p => p.Elements<Run>())
            .Select(r => r.InnerText)).Trim();

    private static bool IsNonConformChar(Character c)
    {
        if (string.IsNullOrEmpty(c.InspectionResult)) return false;
        var ncWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "Not Conform", "Manual Reject", "Fail", "Failed" };
        if (ncWords.Contains(c.InspectionResult)) return true;
        if (c.LowerLimit == 0 && c.UpperLimit == 0) return false;
        return c.InspectionResult
            .Split('/', System.StringSplitOptions.TrimEntries | System.StringSplitOptions.RemoveEmptyEntries)
            .Any(seg => double.TryParse(seg, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var v) &&
                ((c.LowerLimit != 0 && v < c.LowerLimit) || (c.UpperLimit != 0 && v > c.UpperLimit)));
    }

    private static void AppendDimensionalDetailSection(WordprocessingDocument doc, List<Character> characters)
    {
        var body = doc.MainDocumentPart!.Document.Body!;

        if (!characters.Any()) return;

        body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        var title = new Paragraph(new Run(new Text("Dimensional Inspection Detail")));
        SetBold(title);
        body.AppendChild(title);
        body.AppendChild(new Paragraph(new Run(new Text(" "))));

        // Per-part measurement table
        var measureTable = new Table();
        var hrow = new TableRow();
        foreach (var h in new[] { "Item No", "Dimension", "B/P Zone", "Lower", "Upper", "Part Label", "Actual", "Status" })
            hrow.AppendChild(MakeCell(h, bold: true));
        measureTable.AppendChild(hrow);

        foreach (var c in characters)
        {
            if (c.NumericPartResults.Any())
            {
                foreach (var r in c.NumericPartResults)
                {
                    var vals = r.Actual?.Split('/').Select(v => { double.TryParse(v.Trim(), out var d); return d; }).ToList() ?? new List<double>();
                    bool ok = !vals.Any() || (c.LowerLimit == 0 && c.UpperLimit == 0) ||
                              vals.All(v => v >= c.LowerLimit && v <= c.UpperLimit);
                    var row = new TableRow();
                    row.AppendChild(MakeCell(c.ItemNo ?? ""));
                    row.AppendChild(MakeCell(c.Dimension ?? ""));
                    row.AppendChild(MakeCell(c.BPZone ?? ""));
                    row.AppendChild(MakeCell(c.LowerLimit.ToString("G")));
                    row.AppendChild(MakeCell(c.UpperLimit.ToString("G")));
                    row.AppendChild(MakeCell(r.PartLabel ?? ""));
                    row.AppendChild(MakeCell(r.Actual ?? ""));
                    row.AppendChild(MakeCell(ok ? "OK" : "OOT"));
                    measureTable.AppendChild(row);
                }
            }
            else if (c.CategoricalPartResults.Any())
            {
                foreach (var r in c.CategoricalPartResults)
                {
                    var row = new TableRow();
                    row.AppendChild(MakeCell(c.ItemNo ?? ""));
                    row.AppendChild(MakeCell(c.Dimension ?? ""));
                    row.AppendChild(MakeCell(c.BPZone ?? ""));
                    row.AppendChild(MakeCell("—"));
                    row.AppendChild(MakeCell("—"));
                    row.AppendChild(MakeCell(""));
                    row.AppendChild(MakeCell(r.AdditionalInfo ?? ""));
                    row.AppendChild(MakeCell(r.IsConfirmed ? "Conform" : "Not Conform"));
                    measureTable.AppendChild(row);
                }
            }
        }
        body.AppendChild(measureTable);
        body.AppendChild(new Paragraph(new Run(new Text(" "))));

        // Zone results table (if any)
        var allZones = characters.SelectMany(c => c.CategoricalZoneResults.Select(z => new { c, z })).ToList();
        if (allZones.Any())
        {
            var zoneTitle = new Paragraph(new Run(new Text("Zone Results")));
            SetBold(zoneTitle);
            body.AppendChild(zoneTitle);

            var zoneTable = new Table();
            var zhrow = new TableRow();
            foreach (var h in new[] { "Item No", "Zone Name", "Value", "OK?" })
                zhrow.AppendChild(MakeCell(h, bold: true));
            zoneTable.AppendChild(zhrow);

            foreach (var entry in allZones)
            {
                var row = new TableRow();
                row.AppendChild(MakeCell(entry.c.ItemNo ?? ""));
                row.AppendChild(MakeCell(entry.z.ZoneName ?? ""));
                row.AppendChild(MakeCell(entry.z.AdditionalInfo ?? ""));
                row.AppendChild(MakeCell(entry.z.IsConfirmed ? "OK" : "NOK"));
                zoneTable.AppendChild(row);
            }
            body.AppendChild(zoneTable);
            body.AppendChild(new Paragraph(new Run(new Text(" "))));
        }

        // Dispositions for not-conform characters
        var notConformChars = characters.Where(c => c.InspectionResult == "Not Conform").ToList();
        if (notConformChars.Any())
        {
            var dispTitle = new Paragraph(new Run(new Text("Dimensional Dispositions")));
            SetBold(dispTitle);
            body.AppendChild(dispTitle);

            var dispTable = new Table();
            var dhrow = new TableRow();
            foreach (var h in new[] { "Item No", "Result", "Decision", "Entered By", "Date", "Note" })
                dhrow.AppendChild(MakeCell(h, bold: true));
            dispTable.AppendChild(dhrow);

            foreach (var c in notConformChars)
            {
                var activeDisp = c.Dispositions
                    .OrderByDescending(d => d.CreatedAt)
                    .FirstOrDefault(d => d.Decision != "VOID");
                var row = new TableRow();
                row.AppendChild(MakeCell(c.ItemNo ?? ""));
                row.AppendChild(MakeCell(c.InspectionResult ?? ""));
                row.AppendChild(MakeCell(activeDisp?.Decision ?? "Pending"));
                row.AppendChild(MakeCell(activeDisp?.EnteredBy ?? ""));
                row.AppendChild(MakeCell(activeDisp?.DecidedAt?.ToString("yyyy-MM-dd") ?? ""));
                row.AppendChild(MakeCell(activeDisp?.Note ?? ""));
                dispTable.AppendChild(row);
            }
            body.AppendChild(dispTable);
        }
    }
}
