namespace IRSGenerator.Shared.Dtos.DispositionType;

public class DispositionTypeReadDto
{
    public long   Id            { get; set; }
    public string Code          { get; set; } = "";
    public string Label         { get; set; } = "";
    public string CssClass      { get; set; } = "";
    public bool   IsNeutralizing { get; set; }
    public bool   IsInitial     { get; set; }
    public int    SortOrder     { get; set; }
    public bool   Active        { get; set; }
    public DateTime? CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
}
