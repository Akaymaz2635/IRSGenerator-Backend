using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectTypeRepository
{
    ValueTask<DefectType?> GetByIdAsync(long id);
    Task<IEnumerable<DefectType>> GetAllAsync();
    Task<DefectType> AddAsync(DefectType entity);
    Task UpdateAsync(DefectType entity);
    Task DeleteAsync(DefectType entity);
    Task<IEnumerable<DefectType>> GetActiveAsync();
    Task<DefectType?> GetWithFieldsAsync(long id);
}
