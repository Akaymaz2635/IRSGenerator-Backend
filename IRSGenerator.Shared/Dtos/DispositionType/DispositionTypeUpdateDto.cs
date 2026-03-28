namespace IRSGenerator.Shared.Dtos.DispositionType;

/// Code güncellenemez — mevcut Disposition kayıtlarında string olarak saklı.
public class DispositionTypeUpdateDto
{
    public string? Label         { get; set; }
    public string? CssClass      { get; set; }
    public bool?   IsNeutralizing { get; set; }
    public bool?   IsInitial     { get; set; }
    public int?    SortOrder     { get; set; }
    public bool?   Active        { get; set; }
}
