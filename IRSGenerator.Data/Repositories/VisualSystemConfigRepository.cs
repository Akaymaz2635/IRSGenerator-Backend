using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class VisualSystemConfigRepository : BaseRepository<VisualSystemConfig>, IVisualSystemConfigRepository
{
    public VisualSystemConfigRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<VisualSystemConfig?> GetByKeyAsync(string key)
        => await Context.Set<VisualSystemConfig>()
            .FirstOrDefaultAsync(c => c.Key == key);

    public async Task<IEnumerable<VisualSystemConfig>> GetAllConfigsAsync()
        => await Context.Set<VisualSystemConfig>()
            .OrderBy(c => c.Key)
            .ToListAsync();
}
