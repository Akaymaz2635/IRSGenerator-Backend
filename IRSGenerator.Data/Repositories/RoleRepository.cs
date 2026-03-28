using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name)
        => await Context.Roles.FirstOrDefaultAsync(r => r.Name == name);
}
