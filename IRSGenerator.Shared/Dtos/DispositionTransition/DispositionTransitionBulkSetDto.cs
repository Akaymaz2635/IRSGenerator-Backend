namespace IRSGenerator.Shared.Dtos.DispositionTransition;

/// Belirli bir FromCode için tüm geçişleri atomik olarak günceller.
/// null FromCode = başlangıç geçişleri.
public class DispositionTransitionBulkSetDto
{
    public string?      FromCode { get; set; }
    public List<string> ToCodes  { get; set; } = new();
}
