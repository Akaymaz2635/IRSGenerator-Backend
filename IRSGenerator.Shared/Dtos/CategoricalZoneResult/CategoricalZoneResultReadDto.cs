namespace IRSGenerator.Shared.Dtos.CategoricalZoneResult;

public class CategoricalZoneResultReadDto
{
    public long Id { get; set; }
    public string? ZoneName { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }
    public long CharacterId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
