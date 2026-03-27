using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDispositionRepository : IBaseRepository<Disposition>
{
    Task<IEnumerable<Disposition>> GetByDefectAsync(long defectId);
    Task<Disposition?> GetActiveByDefectAsync(long defectId);
}
