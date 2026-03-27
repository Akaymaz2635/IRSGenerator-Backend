namespace IRSGenerator.Shared.Dtos.DefectField;

public class DefectFieldReadDto
{
    public long Id { get; set; }
    public long DefectTypeId { get; set; }
    public string FieldName { get; set; } = "";
    public string Label { get; set; } = "";
    public string FieldType { get; set; } = "number";
    public bool Required { get; set; }
    public string? Unit { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public int SortOrder { get; set; }
    public DateTime? CreatedAt { get; set; }
}
