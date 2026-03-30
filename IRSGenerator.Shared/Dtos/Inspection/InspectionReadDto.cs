using IRSGenerator.Shared.Dtos.Defect;

namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionReadDto
{
    public long    Id              { get; set; }
    public long?   ProjectId       { get; set; }        // snake: project_id
    public long?   IrsProjectId    { get; set; }        // snake: irs_project_id
    public string? PartNumber      { get; set; }
    public string? SerialNumber    { get; set; }
    public string? OperationNumber { get; set; }
    public string? Inspector       { get; set; }
    public string  Status          { get; set; } = "";
    public string? Notes           { get; set; }
    public string? OpSheetPath     { get; set; }
    public DateTime? CreatedAt     { get; set; }
    public DateTime? UpdatedAt     { get; set; }

    // Sadece GetById'de dolu gelir (inspection-detail sayfası için)
    public List<DefectReadDto>? Defects { get; set; }
}
