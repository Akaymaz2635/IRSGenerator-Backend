namespace IRSGenerator.Core.Entities;

public class CategoricalPartResult : BaseEntity
{
    public string? Index { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? UpdateReason { get; set; }   // "typo" | "re-inspect" | null
    public string? UpdateNote { get; set; }

    // Parent
    public long CharacterId { get; set; }
    public Character? Character { get; set; }
}
