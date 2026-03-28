namespace IRSGenerator.Core.Entities;

public class DispositionType : BaseEntity
{
    public string Code          { get; set; } = "";   // "USE_AS_IS", "REWORK" vb.
    public string Label         { get; set; } = "";   // "Kabul (Spec)"
    public string CssClass      { get; set; } = "";   // "disp-accepted"
    public bool   IsNeutralizing { get; set; }        // defekti kapatır mı?
    public bool   IsInitial     { get; set; }         // ilk karar olarak seçilebilir mi?
    public int    SortOrder     { get; set; }
    public bool   Active        { get; set; } = true;

    // Navigation
    public ICollection<DispositionTransition> TransitionsFrom { get; set; } = new List<DispositionTransition>();
    public ICollection<DispositionTransition> TransitionsTo   { get; set; } = new List<DispositionTransition>();
}
