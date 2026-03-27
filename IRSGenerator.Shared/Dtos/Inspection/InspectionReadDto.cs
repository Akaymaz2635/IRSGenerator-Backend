namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionReadDto
{
    public long Id { get; set; }
    public long IrsProjectId { get; set; }
    public string? PartNumber { get; set; }
    public string? SerialNumber { get; set; }
    public int? Operation { get; set; }
    public string? ProjectType { get; set; }
    public long? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public string Status { get; set; } = "";
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
