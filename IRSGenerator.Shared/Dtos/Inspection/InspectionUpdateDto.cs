namespace IRSGenerator.Shared.Dtos.Inspection;

public class InspectionUpdateDto
{
    public long? ProjectId { get; set; }
    public string? PartNumber { get; set; }
    public string? SerialNumber { get; set; }
    public string? OperationNumber { get; set; }
    public string? Inspector { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}
