using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Services;

/// <summary>
/// Parses a .docx op sheet and returns a list of Character entities.
/// Port of Python word_reader.py logic.
/// </summary>
public class WordOpSheetParser
{
    // Item No values to skip
    private static readonly string[] SkipItemSuffixes =
        ["ITEM NO KC", "ITEM NO", "RECORD", "INSPECTION", "INSPECTOR", "/0", "**", "PAGE NO"];

    // Dimension values to skip
    private static readonly string[] SkipDimSuffixes = ["INCH", "INCHES"];

    // Dimension keywords that indicate a categorical (LOT) character
    private static readonly string[] LotKeywords =
        ["VISUAL", "CHECK", "MARKING", "SURFACE", "COATING"];

    public List<Character> Parse(Stream docxStream)
    {
        var results = new List<Character>();

        using var doc = WordprocessingDocument.Open(docxStream, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body is null) return results;

        // Find ALL tables that have both "ITEM NO" and "DIMENSION" in their header row
        var targetTables = new List<Table>();
        foreach (var table in body.Elements<Table>())
        {
            var firstRow = table.Elements<TableRow>().FirstOrDefault();
            if (firstRow is null) continue;
            var headerText = GetRowText(firstRow).ToUpper();
            if (headerText.Contains("ITEM NO") && headerText.Contains("DIMENSION"))
                targetTables.Add(table);
        }

        if (targetTables.Count == 0) return results;

        foreach (var targetTable in targetTables)
        {
            // Determine column indices from this table's header row
            var headerRow   = targetTable.Elements<TableRow>().First();
            var headerCells = headerRow.Elements<TableCell>().ToList();
            int itemNoCol = -1, dimensionCol = -1, badgeCol = -1, toolingCol = -1,
                remarksCol = -1, bpZoneCol = -1, inspLevelCol = -1;

            for (int i = 0; i < headerCells.Count; i++)
            {
                var cellText = GetCellText(headerCells[i]).ToUpper().Trim();
                if (cellText.Contains("ITEM NO"))      itemNoCol    = i;
                if (cellText.Contains("DIMENSION"))    dimensionCol = i;
                if (cellText == "BADGE")               badgeCol     = i;
                if (cellText.Contains("TOOLING"))      toolingCol   = i;
                if (cellText.Contains("REMARK"))       remarksCol   = i;
                if (cellText.Contains("B/P") || cellText.Contains("ZONE")) bpZoneCol = i;
                if (cellText.Contains("INSP") && cellText.Contains("LEVEL")) inspLevelCol = i;
            }

            if (itemNoCol < 0 || dimensionCol < 0) continue;

            // Process data rows (skip header)
            bool isFirstRow = true;
            foreach (var row in targetTable.Elements<TableRow>())
            {
                if (isFirstRow) { isFirstRow = false; continue; }

                var cells = row.Elements<TableCell>().ToList();
                if (cells.Count <= Math.Max(itemNoCol, dimensionCol)) continue;

                var itemNo    = GetCellText(cells[itemNoCol]).Trim();
                var dimension = dimensionCol < cells.Count ? GetCellText(cells[dimensionCol]).Trim() : "";

                if (string.IsNullOrWhiteSpace(itemNo) && string.IsNullOrWhiteSpace(dimension))
                    continue;

                if (ShouldSkipItemNo(itemNo))        continue;
                if (ShouldSkipDimension(dimension))  continue;

                var badge     = badgeCol >= 0 && badgeCol < cells.Count        ? GetCellText(cells[badgeCol]).Trim()     : null;
                var tooling   = toolingCol >= 0 && toolingCol < cells.Count    ? GetCellText(cells[toolingCol]).Trim()   : null;
                var remarks   = remarksCol >= 0 && remarksCol < cells.Count    ? GetCellText(cells[remarksCol]).Trim()   : null;
                var bpZone    = bpZoneCol >= 0 && bpZoneCol < cells.Count      ? GetCellText(cells[bpZoneCol]).Trim()    : null;
                var inspLevel = inspLevelCol >= 0 && inspLevelCol < cells.Count? GetCellText(cells[inspLevelCol]).Trim() : null;

                bool isLot = IsLotDimension(dimension);
                if (isLot) badge = "LOT";

                var limits = LimitCatcherService.CatchMeasurement(dimension);

                results.Add(new Character
                {
                    ItemNo          = Regex.Replace(itemNo, @"\s+", ""),
                    Dimension       = dimension,
                    Badge           = string.IsNullOrEmpty(badge)     ? null : badge,
                    Tooling         = string.IsNullOrEmpty(tooling)   ? null : tooling,
                    Remarks         = string.IsNullOrEmpty(remarks)   ? null : remarks,
                    BPZone          = string.IsNullOrEmpty(bpZone)    ? null : bpZone,
                    InspectionLevel = string.IsNullOrEmpty(inspLevel) ? null : inspLevel,
                    LowerLimit      = limits.Length > 0 ? limits[0] : 0,
                    UpperLimit      = limits.Length > 1 ? limits[1] : 0,
                    InspectionResult = "Unidentified",
                });
            }
        }

        return results;
    }

    private static bool ShouldSkipItemNo(string itemNo)
    {
        if (string.IsNullOrWhiteSpace(itemNo)) return true;
        var upper = itemNo.ToUpper().Trim();
        return SkipItemSuffixes.Any(s => upper.Contains(s));
    }

    private static bool ShouldSkipDimension(string dimension)
    {
        if (string.IsNullOrWhiteSpace(dimension)) return false;
        var upper = dimension.ToUpper().Trim();
        return SkipDimSuffixes.Any(s => upper.EndsWith(s) || upper == s);
    }

    private static bool IsLotDimension(string dimension)
    {
        var upper = dimension.ToUpper();
        return LotKeywords.Any(k => upper.Contains(k));
    }

    private static string GetRowText(TableRow row)
        => string.Join(" ", row.Elements<TableCell>().Select(GetCellText));

    private static string GetCellText(TableCell cell)
        => string.Join(" ", cell.Elements<Paragraph>()
            .SelectMany(p => p.Elements<Run>())
            .Select(r => r.InnerText)).Trim();
}
