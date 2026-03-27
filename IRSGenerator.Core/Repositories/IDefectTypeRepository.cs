using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IDefectTypeRepository
{
    ValueTask<DefectType?> GetByIdAsync(long id, Func<IQueryable<DefectType>, IIncludableQueryable<DefectType, object>>? include = null);
    Task<IEnumerable<DefectType>> GetAllAsync(Func<IQueryable<DefectType>, IIncludableQueryable<DefectType, object>>? include = null);
    Task<DefectType> AddAsync(DefectType entity);
    Task UpdateAsync(DefectType entity);
    Task DeleteAsync(DefectType entity);
    Task<IEnumerable<DefectType>> GetActiveAsync();
    Task<DefectType?> GetWithFieldsAsync(long id);
}
