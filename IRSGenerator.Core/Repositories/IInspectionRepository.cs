using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IInspectionRepository
{
    ValueTask<Inspection?> GetByIdAsync(long id, Func<IQueryable<Inspection>, IIncludableQueryable<Inspection, object>>? include = null);
    Task<IEnumerable<Inspection>> GetAllAsync(Func<IQueryable<Inspection>, IIncludableQueryable<Inspection, object>>? include = null);
    Task<Inspection> AddAsync(Inspection entity);
    Task UpdateAsync(Inspection entity);
    Task DeleteAsync(Inspection entity);
    Task<Inspection?> GetWithDetailsAsync(long id);
    Task<IEnumerable<Inspection>> GetByIrsProjectAsync(long irsProjectId);
    Task<IEnumerable<Inspection>> GetFilteredAsync(string? status, long? visualProjectId, string? search);
    Task<bool> SetStatusCompletedAsync(long id);
}
