namespace IRSGenerator.Core.Entities;

public class Inspection : BaseEntity
{
    public long? ProjectId { get; set; }
    public string PartNumber { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string OperationNumber { get; set; } = "";
    public string Inspector { get; set; } = "";
    public string Status { get; set; } = "open";
    public string? Notes { get; set; }

    // Navigation
    public Project? Project { get; set; }
    public ICollection<Defect> Defects { get; set; } = new List<Defect>();
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
