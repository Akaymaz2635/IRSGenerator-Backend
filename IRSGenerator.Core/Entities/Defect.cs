namespace IRSGenerator.Core.Entities;

public class Defect : BaseEntity
{
    public long InspectionId { get; set; }
    public long DefectTypeId { get; set; }
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

    // Navigation
    public Inspection Inspection { get; set; } = null!;
    public DefectType DefectType { get; set; } = null!;
    public Defect? OriginDefect { get; set; }
    public ICollection<Defect> ChildDefects { get; set; } = new List<Defect>();
    public ICollection<Disposition> Dispositions { get; set; } = new List<Disposition>();
    public ICollection<PhotoDefect> PhotoDefects { get; set; } = new List<PhotoDefect>();
}
