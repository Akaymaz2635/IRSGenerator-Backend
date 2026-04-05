namespace IRSGenerator.Core.Entities;

public class NumericPartResult : BaseEntity
{
    public string Actual { get; set; } = "";
    public string? PartLabel { get; set; }

    public string? UpdateReason { get; set; }   // "typo" | "re-inspect" | null
    public string? UpdateNote { get; set; }     // serbest metin not

    // Parent
    public long CharacterId { get; set; }
    public Character? Character { get; set; }
}
