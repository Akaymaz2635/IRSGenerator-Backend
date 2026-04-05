namespace IRSGenerator.Shared.Dtos.Ncm;

// ── GET /api/inspections/{id}/ncm ─────────────────────────────────────────
public class NcmInspectionDataDto
{
    public long   InspectionId    { get; set; }
    public string PartNumber      { get; set; } = "";
    public string SerialNumber    { get; set; } = "";
    public string OperationNumber { get; set; } = "";
    public string Inspector       { get; set; } = "";
    public string Status          { get; set; } = "";
    public string ProjectName     { get; set; } = "";

    public List<NcmDimensionalItemDto> Dimensional { get; set; } = [];
    public List<NcmVisualItemDto>      Visual      { get; set; } = [];
}

public class NcmDimensionalItemDto
{
    public long   CharacterId  { get; set; }
    public string ItemNo       { get; set; } = "";
    public string Description  { get; set; } = "";
}

public class NcmVisualItemDto
{
    public long   DefectId    { get; set; }
    public int    Index       { get; set; }
    public string Description { get; set; } = "";
}

// ── POST /api/ncm/generate ───────────────────────────────────────────────��
public class GenerateDispositionSheetDto
{
    public long   InspectionId        { get; set; }
    public string Oper                { get; set; } = "";
    public string CauseOper           { get; set; } = "";
    public int    Qty                 { get; set; } = 1;
    public long   CauseCodeId         { get; set; }
    public long   DispositionTypeId   { get; set; }

    /// <summary>Ordered list of NC items to place in the sheet.</summary>
    public List<NcmSheetItemDto> Items { get; set; } = [];
}

public class NcmSheetItemDto
{
    public string Type        { get; set; } = "";  // "dimensional" | "visual"
    public long   SourceId    { get; set; }        // CharacterId or DefectId
    public string Description { get; set; } = "";
}

// ── Response ──────────────────────────────────────────────────────────────
public class GenerateDispositionResultDto
{
    public List<string> FilePaths  { get; set; } = [];
    public int          SheetCount { get; set; }
}
