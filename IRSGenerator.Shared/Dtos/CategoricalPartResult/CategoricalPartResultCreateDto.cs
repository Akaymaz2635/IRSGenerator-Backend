namespace IRSGenerator.Shared.Dtos.CategoricalPartResult;

public class CategoricalPartResultCreateDto
{
    public string? Index { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }
    public long CharacterId { get; set; }
    public string? UpdateReason { get; set; }
    public string? UpdateNote { get; set; }
}
