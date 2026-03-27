namespace IRSGenerator.Core.Entities;

public class PhotoDefect
{
    public long PhotoId { get; set; }
    public long DefectId { get; set; }

    // Navigation
    public Photo Photo { get; set; } = null!;
    public Defect Defect { get; set; } = null!;
}
