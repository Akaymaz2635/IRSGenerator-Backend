namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionCreateDto
{
    public long? ProjectId { get; set; }        // → VisualProjectId  (snake: project_id)
    public long? IrsProjectId { get; set; }     // → IrsProjectId     (snake: irs_project_id)
    public string? PartNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? OperationNumber { get; set; }
    public string? Inspector { get; set; }
    public string Status { get; set; } = "open";
    public string? Notes { get; set; }
}
