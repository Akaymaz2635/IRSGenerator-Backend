using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities;

public class Character : BaseEntity
{
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

    // Parent
    public long IRSProjectId { get; set; }
    public IRSProject? IRSProject { get; set; }

    // Children
    public ICollection<NumericPartResult> NumericPartResults { get; set; }
        = new Collection<NumericPartResult>();
    public ICollection<CategoricalPartResult> CategoricalPartResults { get; set; }
        = new Collection<CategoricalPartResult>();
    public ICollection<CategoricalZoneResult> CategoricalZoneResults { get; set; }
        = new Collection<CategoricalZoneResult>();
}
