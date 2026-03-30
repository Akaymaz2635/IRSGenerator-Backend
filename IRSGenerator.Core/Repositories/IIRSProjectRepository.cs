using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface IIRSProjectRepository : IBaseRepository<IRSProject>
{
    Task<IEnumerable<IRSProject>> GetByOwnerIdAsync(long ownerId);
}
