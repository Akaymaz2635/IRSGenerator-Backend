namespace IRSGenerator.Core.Entities;

public class Disposition : BaseEntity
{
    public long? DefectId { get; set; }
    public long? CharacterId { get; set; }
    public string Decision { get; set; } = "";
    public string EnteredBy { get; set; } = "";
    public DateTime? DecidedAt { get; set; }
    public string Note { get; set; } = "";
    public string? SpecRef { get; set; }
    public string? Engineer { get; set; }
    public string? Reinspector { get; set; }
    public string? ConcessionNo { get; set; }
    public string? VoidReason { get; set; }
    public string? RepairRef { get; set; }
    public string? ScrapReason { get; set; }
    public string? MeasurementsSnapshot { get; set; }

    // Navigation
    public Defect? Defect { get; set; }
    public Character? Character { get; set; }
}
