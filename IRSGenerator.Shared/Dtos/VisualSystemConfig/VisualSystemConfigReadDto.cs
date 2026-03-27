namespace IRSGenerator.Shared.Dtos.VisualSystemConfig;

public class VisualSystemConfigReadDto
{
    public long Id { get; set; }
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
    public DateTime? UpdatedAt { get; set; }
}
