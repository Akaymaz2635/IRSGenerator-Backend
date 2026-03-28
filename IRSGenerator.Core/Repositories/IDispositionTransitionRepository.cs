using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDispositionTransitionRepository
{
    ValueTask<DispositionTransition?> GetByIdAsync(long id);
    Task<IEnumerable<DispositionTransition>> GetAllAsync();
    Task<IEnumerable<DispositionTransition>> GetByFromCodeAsync(string? fromCode);
    Task<IEnumerable<string>> GetAllowedNextCodesAsync(string? currentCode);
    Task<DispositionTransition> AddAsync(DispositionTransition entity);
    Task DeleteAsync(DispositionTransition entity);
    Task BulkSetAsync(string? fromCode, IEnumerable<string> toCodes);
}
