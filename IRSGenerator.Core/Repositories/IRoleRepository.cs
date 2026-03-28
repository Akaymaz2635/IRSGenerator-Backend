using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
