namespace IRSGenerator.Core.Entities;

public class DispositionTransition : BaseEntity
{
    // null = "entry point" — henüz karar yokken seçilebilir
    public string? FromCode { get; set; }
    public string  ToCode   { get; set; } = "";

    // Navigation
    public DispositionType? FromType { get; set; }
    public DispositionType  ToType   { get; set; } = null!;
}
