namespace IRSGenerator.Core.Entities;

public class DefectType : BaseEntity
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool Active { get; set; } = true;

    // Navigation
    public ICollection<Defect> Defects { get; set; } = new List<Defect>();
    public ICollection<DefectField> DefectFields { get; set; } = new List<DefectField>();
}
