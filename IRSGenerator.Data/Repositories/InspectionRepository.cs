using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class InspectionRepository : BaseRepository<Inspection>, IInspectionRepository
{
    public InspectionRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(Inspection entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<Inspection>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Inspection entity)
    {
        Context.Set<Inspection>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<Inspection?> GetWithDetailsAsync(long id)
        => await Context.Set<Inspection>()
            .Include(i => i.IrsProject)
            .Include(i => i.Inspector)
            .Include(i => i.Defects).ThenInclude(d => d.DefectType)
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions)
            .Include(i => i.Photos)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Inspection>> GetByIrsProjectAsync(long irsProjectId)
        => await Context.Set<Inspection>()
            .Include(i => i.Inspector)
            .Where(i => i.IrsProjectId == irsProjectId)
            .ToListAsync();

    public async Task<bool> SetStatusCompletedAsync(long id)
    {
        var inspection = await Context.Set<Inspection>()
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inspection == null) return false;

        var allDisposed = inspection.Defects.All(d =>
            d.Dispositions.Any(disp => disp.Decision != "VOID"));

        if (!allDisposed) return false;

        inspection.Status = "completed";
        inspection.UpdatedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync();
        return true;
    }
}
