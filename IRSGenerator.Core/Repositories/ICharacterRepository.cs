using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface ICharacterRepository : IBaseRepository<Character>
{
    Task<IEnumerable<Character>> GetByProjectIdAsync(long projectId);
    Task<IEnumerable<Character>> GetByInspectionIdAsync(long inspectionId);
    Task<IEnumerable<Character>> GetByInspectionIdWithDispositionsAsync(long inspectionId);
}
