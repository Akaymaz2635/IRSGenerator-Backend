using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class DefectRepository : BaseRepository<Defect>, IDefectRepository
{
    public DefectRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(Defect entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<Defect>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Defect entity)
    {
        Context.Set<Defect>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Defect>> GetByInspectionAsync(long inspectionId)
        => await Context.Set<Defect>()
            .Include(d => d.DefectType)
            .Include(d => d.Dispositions)
            .Where(d => d.InspectionId == inspectionId)
            .ToListAsync();

    public async Task<Defect?> GetWithDispositionsAsync(long id)
        => await Context.Set<Defect>()
            .Include(d => d.DefectType)
            .Include(d => d.Dispositions.OrderByDescending(disp => disp.CreatedAt))
            .FirstOrDefaultAsync(d => d.Id == id);
}
