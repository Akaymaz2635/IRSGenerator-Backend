namespace MES.Domain.Entities;

public class CategoricalZoneResult : BaseEntity
{
    public string? ZoneName { get; set; }
    public bool IsConfirmed { get; set; }
    public string? AdditionalInfo { get; set; }

    // Parent
    public long CharacterId { get; set; }
    public Character? Character { get; set; }
}
