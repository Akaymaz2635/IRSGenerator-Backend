namespace MES.Domain.Dtos.DispositionTransition;

public class DispositionTransitionReadDto
{
    public long    Id       { get; set; }
    public string? FromCode { get; set; }
    public string  ToCode   { get; set; } = "";
}
