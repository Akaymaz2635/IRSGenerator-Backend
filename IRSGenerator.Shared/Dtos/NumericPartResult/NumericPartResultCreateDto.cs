namespace IRSGenerator.Shared.Dtos.NumericPartResult;

public class NumericPartResultCreateDto
{
    public string Actual { get; set; } = "";
    public string? PartLabel { get; set; }
    public long CharacterId { get; set; }
    public string? UpdateReason { get; set; }
    public string? UpdateNote { get; set; }
}
