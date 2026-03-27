namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionCreateDto
{
    public long IrsProjectId { get; set; }
    public long? InspectorId { get; set; }
    public string Status { get; set; } = "open";
    public string? Notes { get; set; }
}
