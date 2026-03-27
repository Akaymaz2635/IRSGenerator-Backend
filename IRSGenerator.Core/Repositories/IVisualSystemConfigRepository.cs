using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IVisualSystemConfigRepository
{
    ValueTask<VisualSystemConfig?> GetByIdAsync(long id, Func<IQueryable<VisualSystemConfig>, IIncludableQueryable<VisualSystemConfig, object>>? include = null);
    Task<IEnumerable<VisualSystemConfig>> GetAllAsync(Func<IQueryable<VisualSystemConfig>, IIncludableQueryable<VisualSystemConfig, object>>? include = null);
    Task UpdateAsync(VisualSystemConfig entity);
    Task<VisualSystemConfig?> GetByKeyAsync(string key);
    Task<IEnumerable<VisualSystemConfig>> GetAllConfigsAsync();
}
