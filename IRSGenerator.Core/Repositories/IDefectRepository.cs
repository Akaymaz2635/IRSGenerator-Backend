using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectRepository : IBaseRepository<Defect>
{
    Task<IEnumerable<Defect>> GetByInspectionAsync(long inspectionId);
    Task<Defect?> GetWithDispositionsAsync(long id);
}
