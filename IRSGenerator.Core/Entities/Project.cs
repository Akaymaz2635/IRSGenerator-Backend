namespace IRSGenerator.Core.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public bool Active { get; set; } = true;

    // Navigation
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
