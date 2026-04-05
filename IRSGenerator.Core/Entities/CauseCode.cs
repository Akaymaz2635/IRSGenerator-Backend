namespace IRSGenerator.Core.Entities;

public class CauseCode : BaseEntity
{
    public string Code        { get; set; } = "";
    public string Description { get; set; } = "";
    public bool   Active      { get; set; } = true;
    public int    SortOrder   { get; set; } = 0;
}
