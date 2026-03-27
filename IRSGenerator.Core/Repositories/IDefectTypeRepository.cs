using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectTypeRepository : IBaseRepository<DefectType>
{
    Task<IEnumerable<DefectType>> GetActiveAsync();
    Task<DefectType?> GetWithFieldsAsync(long id);
}
