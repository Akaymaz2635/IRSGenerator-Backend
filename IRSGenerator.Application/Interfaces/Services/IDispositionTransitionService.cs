using MES.Domain.Dtos.DispositionTransition;

namespace MES.Application.Interfaces;

public interface IDispositionTransitionService
{
    Task<IEnumerable<DispositionTransitionReadDto>> GetAllAsync(string? fromCode = null);
    Task<IEnumerable<string>> GetAllowedNextCodesAsync(string? currentCode = null);
    Task BulkSetAsync(DispositionTransitionBulkSetDto dto);
}
