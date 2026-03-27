using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IVisualSystemConfigRepository
{
    ValueTask<VisualSystemConfig?> GetByIdAsync(long id);
    Task<IEnumerable<VisualSystemConfig>> GetAllAsync();
    Task UpdateAsync(VisualSystemConfig entity);
    Task<VisualSystemConfig?> GetByKeyAsync(string key);
    Task<IEnumerable<VisualSystemConfig>> GetAllConfigsAsync();
}
