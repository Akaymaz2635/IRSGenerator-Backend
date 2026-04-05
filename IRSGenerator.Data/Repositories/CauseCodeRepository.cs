using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class CauseCodeRepository : BaseRepository<CauseCode>, ICauseCodeRepository
{
    public CauseCodeRepository(IRSGeneratorDbContext context) : base(context) { }

    public new async ValueTask<CauseCode?> GetByIdAsync(long id)
        => await Context.Set<CauseCode>().FirstOrDefaultAsync(e => e.Id == id);

    public new async Task<IEnumerable<CauseCode>> GetAllAsync()
        => await Context.Set<CauseCode>()
            .OrderBy(c => c.Description)
            .ToListAsync();

    public async Task<IEnumerable<CauseCode>> GetActiveAsync()
        => await Context.Set<CauseCode>()
            .Where(c => c.Active)
            .OrderBy(c => c.Description)
            .ToListAsync();

    public async Task UpdateAsync(CauseCode entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<CauseCode>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CauseCode entity)
    {
        Context.Set<CauseCode>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
