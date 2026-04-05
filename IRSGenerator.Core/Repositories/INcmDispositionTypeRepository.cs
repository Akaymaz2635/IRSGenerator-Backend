using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface INcmDispositionTypeRepository
{
    ValueTask<NcmDispositionType?> GetByIdAsync(long id);
    Task<IEnumerable<NcmDispositionType>> GetAllAsync();
    Task<IEnumerable<NcmDispositionType>> GetActiveAsync();
    Task<NcmDispositionType> AddAsync(NcmDispositionType entity);
    Task UpdateAsync(NcmDispositionType entity);
    Task DeleteAsync(NcmDispositionType entity);
}
