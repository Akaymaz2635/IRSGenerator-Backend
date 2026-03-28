namespace IRSGenerator.Shared.Dtos.DispositionType;

public class DispositionTypeCreateDto
{
    public string Code          { get; set; } = "";
    public string Label         { get; set; } = "";
    public string CssClass      { get; set; } = "";
    public bool   IsNeutralizing { get; set; }
    public bool   IsInitial     { get; set; }
    public int    SortOrder     { get; set; }
}
