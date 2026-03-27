namespace IRSGenerator.Core.Entities;

public class Photo : BaseEntity
{
    public long InspectionId { get; set; }
    public string Filename { get; set; } = "";
    public string Filepath { get; set; } = "";

    // Navigation
    public Inspection Inspection { get; set; } = null!;
    public ICollection<PhotoDefect> PhotoDefects { get; set; } = new List<PhotoDefect>();
}
