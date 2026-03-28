namespace IRSGenerator.Shared.Dtos.Character;

public class CharacterReadDto
{
    public long Id { get; set; }
    public string ItemNo { get; set; } = default!;
    public string Dimension { get; set; } = default!;
    public string? Badge { get; set; }
    public string? Tooling { get; set; }
    public string? BPZone { get; set; }
    public string? InspectionLevel { get; set; }
    public string? Remarks { get; set; }
    public double LowerLimit { get; set; }
    public double UpperLimit { get; set; }
    public string InspectionResult { get; set; } = "Unidentified";
    public long IRSProjectId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
