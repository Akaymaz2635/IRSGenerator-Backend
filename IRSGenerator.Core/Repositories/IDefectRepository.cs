using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectRepository
{
    ValueTask<Defect?> GetByIdAsync(long id);
    Task<IEnumerable<Defect>> GetAllAsync();
    Task<Defect> AddAsync(Defect entity);
    Task UpdateAsync(Defect entity);
    Task DeleteAsync(Defect entity);
    Task<IEnumerable<Defect>> GetByInspectionAsync(long inspectionId);
    Task<Defect?> GetWithDispositionsAsync(long id);
}
