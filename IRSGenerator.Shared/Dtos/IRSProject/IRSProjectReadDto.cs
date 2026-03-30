namespace IRSGenerator.Shared.Dtos.IRSProject;

public class IRSProjectReadDto
{
    public long Id { get; set; }
    public string ProjectType { get; set; } = default!;
    public string PartNumber { get; set; } = default!;
    public int Operation { get; set; }
    public string SerialNumber { get; set; } = default!;
    public string OpSheetPath { get; set; } = "";
    public long OwnerId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
