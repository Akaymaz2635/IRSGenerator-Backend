using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IVisualProjectRepository
{
    ValueTask<VisualProject?> GetByIdAsync(long id, Func<IQueryable<VisualProject>, IIncludableQueryable<VisualProject, object>>? include = null);
    Task<IEnumerable<VisualProject>> GetAllAsync(Func<IQueryable<VisualProject>, IIncludableQueryable<VisualProject, object>>? include = null);
    Task<IEnumerable<VisualProject>> GetActiveAsync();
    Task<VisualProject> AddAsync(VisualProject entity);
    Task UpdateAsync(VisualProject entity);
    Task DeleteAsync(VisualProject entity);
}
