using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<Project>> GetActiveAsync()
        => await Context.Set<Project>()
            .Where(p => p.Active)
            .OrderBy(p => p.Name)
            .ToListAsync();
}
