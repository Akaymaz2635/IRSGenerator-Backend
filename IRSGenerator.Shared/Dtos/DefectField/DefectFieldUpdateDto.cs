namespace IRSGenerator.Shared.Dtos.DefectField;

public class DefectFieldUpdateDto
{
    public string? Label { get; set; }
    public string? FieldType { get; set; }
    public bool? Required { get; set; }
    public string? Unit { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
    public int? SortOrder { get; set; }
}
