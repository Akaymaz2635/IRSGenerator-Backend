using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IDefectFieldRepository
{
    ValueTask<DefectField?> GetByIdAsync(long id, Func<IQueryable<DefectField>, IIncludableQueryable<DefectField, object>>? include = null);
    Task<IEnumerable<DefectField>> GetAllAsync();
    Task<DefectField> AddAsync(DefectField entity);
    Task UpdateAsync(DefectField entity);
    Task DeleteAsync(DefectField entity);
    Task<IEnumerable<DefectField>> GetByDefectTypeAsync(long defectTypeId);
}
