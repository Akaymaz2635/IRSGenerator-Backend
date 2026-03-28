using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<Permission?> GetByCodeAsync(string code)
        => await Context.Permissions.FirstOrDefaultAsync(p => p.Code == code);
}
