using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IInspectionRepository
{
    ValueTask<Inspection?> GetByIdAsync(long id);
    Task<IEnumerable<Inspection>> GetAllAsync();
    Task<Inspection> AddAsync(Inspection entity);
    Task UpdateAsync(Inspection entity);
    Task DeleteAsync(Inspection entity);
    Task<Inspection?> GetWithDetailsAsync(long id);
    Task<IEnumerable<Inspection>> GetByProjectAsync(long projectId);
    Task<bool> SetStatusCompletedAsync(long id);
}
