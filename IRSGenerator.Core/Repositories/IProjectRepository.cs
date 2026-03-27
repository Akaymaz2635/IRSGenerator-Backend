using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IProjectRepository
{
    ValueTask<Project?> GetByIdAsync(long id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project> AddAsync(Project entity);
    Task UpdateAsync(Project entity);
    Task DeleteAsync(Project entity);
    Task<IEnumerable<Project>> GetActiveAsync();
}
