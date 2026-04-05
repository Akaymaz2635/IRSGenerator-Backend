namespace IRSGenerator.Shared.Dtos.CauseCode;

public class CauseCodeReadDto
{
    public long   Id          { get; set; }
    public string Code        { get; set; } = "";
    public string Description { get; set; } = "";
    public bool   Active      { get; set; }
}

public class CauseCodeCreateDto
{
    public string Code        { get; set; } = "";
    public string Description { get; set; } = "";
    public bool   Active      { get; set; } = true;
}

public class CauseCodeUpdateDto
{
    public string Code        { get; set; } = "";
    public string Description { get; set; } = "";
    public bool   Active      { get; set; }
}
