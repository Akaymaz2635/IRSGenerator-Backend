using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class VisualProjectRepository : BaseRepository<VisualProject>, IVisualProjectRepository
{
    public VisualProjectRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<VisualProject>> GetActiveAsync()
        => await Context.Set<VisualProject>()
            .Where(p => p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task UpdateAsync(VisualProject entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<VisualProject>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(VisualProject entity)
    {
        Context.Set<VisualProject>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
