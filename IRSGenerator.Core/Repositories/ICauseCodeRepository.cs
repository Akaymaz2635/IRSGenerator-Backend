using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface ICauseCodeRepository
{
    ValueTask<CauseCode?> GetByIdAsync(long id);
    Task<IEnumerable<CauseCode>> GetAllAsync();
    Task<IEnumerable<CauseCode>> GetActiveAsync();
    Task<CauseCode> AddAsync(CauseCode entity);
    Task UpdateAsync(CauseCode entity);
    Task DeleteAsync(CauseCode entity);
}
