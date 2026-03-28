namespace IRSGenerator.Shared.Dtos.CategoricalZoneResult;

public class CategoricalZoneResultCreateDto
{
    public string? ZoneName { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }
    public long CharacterId { get; set; }
}
