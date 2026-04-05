namespace IRSGenerator.Core.Entities;

public class NcmDispositionType : BaseEntity
{
    public string Code             { get; set; } = "";  // "ACCEPT", "CTP&MRB", "SCRAP-IND" …
    public string Label            { get; set; } = "";  // Display name
    public string Description      { get; set; } = "";  // Admin-provided description / notes
    public string TemplateFileName { get; set; } = "";  // "ACCEPT.docx"
    public bool   Active           { get; set; } = true;
    public int    SortOrder        { get; set; } = 0;
}
