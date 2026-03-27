using IRSGenerator.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace IRSGenerator.Core.Repositories;

public interface IDispositionRepository
{
    ValueTask<Disposition?> GetByIdAsync(long id, Func<IQueryable<Disposition>, IIncludableQueryable<Disposition, object>>? include = null);
    Task<IEnumerable<Disposition>> GetAllAsync(Func<IQueryable<Disposition>, IIncludableQueryable<Disposition, object>>? include = null);
    Task<Disposition> AddAsync(Disposition entity);
    Task DeleteAsync(Disposition entity);
    Task<IEnumerable<Disposition>> GetByDefectAsync(long defectId);
    Task<Disposition?> GetActiveByDefectAsync(long defectId);
}
