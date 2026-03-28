namespace IRSGenerator.Shared.Dtos.Project;

public class ProjectReadDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public bool Active { get; set; }
    public DateTime? CreatedAt { get; set; }
}
