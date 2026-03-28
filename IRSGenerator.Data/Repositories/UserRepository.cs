using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<User?> GetByEmployeeIdAsync(string employeeId)
        => await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

    public async Task UpdateAsync(User entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<User>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        Context.Set<User>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
