namespace IRSGenerator.Core.Entities;

public class NumericPartResult : BaseEntity
{
    public string Actual { get; set; } = "";
    public string? PartLabel { get; set; }

    // Parent
    public long CharacterId { get; set; }
    public Character? Character { get; set; }
}
