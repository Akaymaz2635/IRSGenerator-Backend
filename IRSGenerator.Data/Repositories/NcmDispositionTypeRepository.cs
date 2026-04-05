using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class NcmDispositionTypeRepository : BaseRepository<NcmDispositionType>, INcmDispositionTypeRepository
{
    public NcmDispositionTypeRepository(IRSGeneratorDbContext context) : base(context) { }

    public new async ValueTask<NcmDispositionType?> GetByIdAsync(long id)
        => await Context.Set<NcmDispositionType>().FirstOrDefaultAsync(e => e.Id == id);

    public new async Task<IEnumerable<NcmDispositionType>> GetAllAsync()
        => await Context.Set<NcmDispositionType>()
            .OrderBy(t => t.Label)
            .ToListAsync();

    public async Task<IEnumerable<NcmDispositionType>> GetActiveAsync()
        => await Context.Set<NcmDispositionType>()
            .Where(t => t.Active)
            .OrderBy(t => t.Label)
            .ToListAsync();

    public async Task UpdateAsync(NcmDispositionType entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<NcmDispositionType>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(NcmDispositionType entity)
    {
        Context.Set<NcmDispositionType>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
