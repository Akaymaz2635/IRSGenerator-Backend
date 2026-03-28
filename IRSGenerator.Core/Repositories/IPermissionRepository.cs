using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<Permission?> GetByCodeAsync(string code);
}
