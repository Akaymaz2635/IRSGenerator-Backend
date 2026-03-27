using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class DefectRepository : BaseRepository<Defect>, IDefectRepository
{
    public DefectRepository(IRSGeneratorDbContext context) : base(context) { }

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
            .Include(d => d.PhotoDefects)
                .ThenInclude(pd => pd.Photo)
            .FirstOrDefaultAsync(d => d.Id == id);
}
