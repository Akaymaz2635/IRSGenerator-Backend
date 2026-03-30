namespace IRSGenerator.Shared.Dtos.IRSProject;

public class IRSProjectCreateDto
{
    public string ProjectType { get; set; } = default!;
    public string PartNumber { get; set; } = default!;
    public int Operation { get; set; }
    public string SerialNumber { get; set; } = default!;
    public string OpSheetPath { get; set; } = "";
    public long OwnerId { get; set; }
}
