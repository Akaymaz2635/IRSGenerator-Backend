namespace IRSGenerator.Core.Entities;

public class DefectField : BaseEntity
{
    public long DefectTypeId { get; set; }
    public string FieldName { get; set; } = "";
    public string Label { get; set; } = "";
    public string FieldType { get; set; } = "number";
    public bool Required { get; set; }
    public string? Unit { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public DefectType DefectType { get; set; } = null!;
}
