using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class VisualSystemConfigRepository : BaseRepository<VisualSystemConfig>, IVisualSystemConfigRepository
{
    public VisualSystemConfigRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(VisualSystemConfig entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<VisualSystemConfig>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<VisualSystemConfig?> GetByKeyAsync(string key)
        => await Context.Set<VisualSystemConfig>()
            .FirstOrDefaultAsync(c => c.Key == key);

    public async Task<IEnumerable<VisualSystemConfig>> GetAllConfigsAsync()
        => await Context.Set<VisualSystemConfig>()
            .OrderBy(c => c.Key)
            .ToListAsync();
}
