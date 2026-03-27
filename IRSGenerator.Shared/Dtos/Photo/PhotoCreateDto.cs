namespace IRSGenerator.Shared.Dtos.Photo;

public class PhotoCreateDto
{
    public long InspectionId { get; set; }
    public string Filename { get; set; } = "";
    public string Filepath { get; set; } = "";
    public List<long> DefectIds { get; set; } = new();
}
