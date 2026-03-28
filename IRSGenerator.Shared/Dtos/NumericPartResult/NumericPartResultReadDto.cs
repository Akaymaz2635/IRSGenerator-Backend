namespace IRSGenerator.Shared.Dtos.NumericPartResult;

public class NumericPartResultReadDto
{
    public long Id { get; set; }
    public double Actual { get; set; }
    public long CharacterId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
