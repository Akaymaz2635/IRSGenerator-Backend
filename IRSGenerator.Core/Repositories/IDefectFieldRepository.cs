using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectFieldRepository
{
    ValueTask<DefectField?> GetByIdAsync(long id);
    Task<IEnumerable<DefectField>> GetAllAsync();
    Task<DefectField> AddAsync(DefectField entity);
    Task UpdateAsync(DefectField entity);
    Task DeleteAsync(DefectField entity);
    Task<IEnumerable<DefectField>> GetByDefectTypeAsync(long defectTypeId);
}
