using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDispositionRepository
{
    ValueTask<Disposition?> GetByIdAsync(long id);
    Task<IEnumerable<Disposition>> GetAllAsync();
    Task<Disposition> AddAsync(Disposition entity);
    Task DeleteAsync(Disposition entity);
    Task<IEnumerable<Disposition>> GetByDefectAsync(long defectId);
    Task<Disposition?> GetActiveByDefectAsync(long defectId);
}
