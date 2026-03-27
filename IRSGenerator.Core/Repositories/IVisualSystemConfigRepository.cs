using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IVisualSystemConfigRepository : IBaseRepository<VisualSystemConfig>
{
    Task<VisualSystemConfig?> GetByKeyAsync(string key);
    Task<IEnumerable<VisualSystemConfig>> GetAllConfigsAsync();
}
