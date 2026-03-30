using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class IRSProjectRepository : BaseRepository<IRSProject>, IIRSProjectRepository
{
    public IRSProjectRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<IRSProject>> GetByOwnerIdAsync(long ownerId)
        => await Context.Projects
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync();
}
