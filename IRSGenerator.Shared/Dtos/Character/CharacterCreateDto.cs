namespace IRSGenerator.Shared.Dtos.Character;

public class CharacterCreateDto
{
    public string ItemNo { get; set; } = default!;
    public string Dimension { get; set; } = default!;
    public string? Badge { get; set; }
    public string? Tooling { get; set; }
    public string? BPZone { get; set; }
    public string? InspectionLevel { get; set; }
    public string? Remarks { get; set; }
    public long? IRSProjectId { get; set; }
    public long? InspectionId { get; set; }
}
