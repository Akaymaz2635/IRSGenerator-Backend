using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<IEnumerable<Project>> GetActiveAsync();
}
