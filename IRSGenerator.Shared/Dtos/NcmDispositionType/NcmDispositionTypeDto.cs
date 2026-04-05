namespace IRSGenerator.Shared.Dtos.NcmDispositionType;

public class NcmDispositionTypeReadDto
{
    public long   Id               { get; set; }
    public string Code             { get; set; } = "";
    public string Label            { get; set; } = "";
    public string Description      { get; set; } = "";
    public string TemplateFileName { get; set; } = "";
    public bool   Active           { get; set; }
}

public class NcmDispositionTypeCreateDto
{
    public string Code             { get; set; } = "";
    public string Label            { get; set; } = "";
    public string Description      { get; set; } = "";
    public string TemplateFileName { get; set; } = "";
    public bool   Active           { get; set; } = true;
}

public class NcmDispositionTypeUpdateDto
{
    public string Code             { get; set; } = "";
    public string Label            { get; set; } = "";
    public string Description      { get; set; } = "";
    public string TemplateFileName { get; set; } = "";
    public bool   Active           { get; set; }
}
