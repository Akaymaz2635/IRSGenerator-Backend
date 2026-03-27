namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionReadDto
{
    public long Id { get; set; }
    public long? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string PartNumber { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string OperationNumber { get; set; } = "";
    public string Inspector { get; set; } = "";
    public string Status { get; set; } = "open";
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
