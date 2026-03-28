using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDispositionTypeRepository
{
    ValueTask<DispositionType?> GetByIdAsync(long id);
    Task<IEnumerable<DispositionType>> GetAllAsync();
    Task<IEnumerable<DispositionType>> GetActiveAsync();
    Task<DispositionType?> GetByCodeAsync(string code);
    Task<DispositionType> AddAsync(DispositionType entity);
    Task UpdateAsync(DispositionType entity);
    Task DeleteAsync(DispositionType entity);
}
