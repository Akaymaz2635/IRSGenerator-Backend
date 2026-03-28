namespace IRSGenerator.Shared.Dtos.CategoricalPartResult;

public class CategoricalPartResultReadDto
{
    public long Id { get; set; }
    public string? Index { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }
    public long CharacterId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
