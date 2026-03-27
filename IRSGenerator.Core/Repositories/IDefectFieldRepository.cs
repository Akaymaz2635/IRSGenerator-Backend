using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IDefectFieldRepository : IBaseRepository<DefectField>
{
    Task<IEnumerable<DefectField>> GetByDefectTypeAsync(long defectTypeId);
}
