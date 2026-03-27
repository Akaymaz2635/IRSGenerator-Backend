using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(Project entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<Project>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project entity)
    {
        Context.Set<Project>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Project>> GetActiveAsync()
        => await Context.Set<Project>().Where(p => p.Active).ToListAsync();
}
