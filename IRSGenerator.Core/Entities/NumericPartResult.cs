namespace IRSGenerator.Core.Entities;

public class NumericPartResult : BaseEntity
{
    public double Actual { get; set; }

    // Parent
    public long CharacterId { get; set; }
    public Character? Character { get; set; }
}
