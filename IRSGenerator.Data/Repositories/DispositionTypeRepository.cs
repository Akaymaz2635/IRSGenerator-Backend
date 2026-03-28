using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class DispositionTypeRepository : BaseRepository<DispositionType>, IDispositionTypeRepository
{
    public DispositionTypeRepository(IRSGeneratorDbContext context) : base(context) { }

    public new async ValueTask<DispositionType?> GetByIdAsync(long id)
        => await Context.Set<DispositionType>().FirstOrDefaultAsync(e => e.Id == id);

    public new async Task<IEnumerable<DispositionType>> GetAllAsync()
        => await Context.Set<DispositionType>()
            .OrderBy(dt => dt.SortOrder)
            .ToListAsync();

    public async Task<IEnumerable<DispositionType>> GetActiveAsync()
        => await Context.Set<DispositionType>()
            .Where(dt => dt.Active)
            .OrderBy(dt => dt.SortOrder)
            .ToListAsync();

    public async Task<DispositionType?> GetByCodeAsync(string code)
        => await Context.Set<DispositionType>()
            .FirstOrDefaultAsync(dt => dt.Code == code);

    public async Task UpdateAsync(DispositionType entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<DispositionType>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DispositionType entity)
    {
        Context.Set<DispositionType>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
