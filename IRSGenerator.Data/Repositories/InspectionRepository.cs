using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class InspectionRepository : BaseRepository<Inspection>, IInspectionRepository
{
    public InspectionRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<Inspection?> GetWithDetailsAsync(long id)
        => await Context.Set<Inspection>()
            .Include(i => i.Project)
            .Include(i => i.Defects)
                .ThenInclude(d => d.DefectType)
            .Include(i => i.Defects)
                .ThenInclude(d => d.Dispositions)
            .Include(i => i.Photos)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Inspection>> GetByProjectAsync(long projectId)
        => await Context.Set<Inspection>()
            .Where(i => i.ProjectId == projectId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

    public async Task SetStatusCompletedAsync(long id)
    {
        var inspection = await Context.Set<Inspection>()
            .Include(i => i.Defects)
                .ThenInclude(d => d.Dispositions)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new InvalidOperationException($"Inspection with id {id} not found.");

        var openDefects = inspection.Defects
            .Where(d =>
            {
                var active = d.Dispositions
                    .OrderByDescending(disp => disp.CreatedAt)
                    .FirstOrDefault();
                return active == null
                    || string.Equals(active.Decision, "VOID", StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        if (openDefects.Count > 0)
            throw new InvalidOperationException(
                $"Cannot complete inspection: {openDefects.Count} defect(s) have no valid disposition.");

        inspection.Status = "completed";
        inspection.UpdatedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
    }
}
