namespace IRSGenerator.Shared.Dtos.Defect;

public class DefectReadDto
{
    public long Id { get; set; }
    public long InspectionId { get; set; }
    public long DefectTypeId { get; set; }
    public string? DefectTypeName { get; set; }
    public long? OriginDefectId { get; set; }
    public double? Depth { get; set; }
    public double? Width { get; set; }
    public double? Length { get; set; }
    public double? Radius { get; set; }
    public double? Angle { get; set; }
    public double? Height { get; set; }
    public string? Color { get; set; }
    public string? Notes { get; set; }
    public bool HighMetal { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
