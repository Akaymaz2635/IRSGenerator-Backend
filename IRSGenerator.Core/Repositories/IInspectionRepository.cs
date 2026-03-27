using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IInspectionRepository : IBaseRepository<Inspection>
{
    Task<Inspection?> GetWithDetailsAsync(long id);
    Task<IEnumerable<Inspection>> GetByProjectAsync(long projectId);
    Task SetStatusCompletedAsync(long id);
}
