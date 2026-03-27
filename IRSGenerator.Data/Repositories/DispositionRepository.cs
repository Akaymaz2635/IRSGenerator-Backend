using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class DispositionRepository : BaseRepository<Disposition>, IDispositionRepository
{
    public DispositionRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<Disposition>> GetByDefectAsync(long defectId)
        => await Context.Set<Disposition>()
            .Where(d => d.DefectId == defectId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    public async Task<Disposition?> GetActiveByDefectAsync(long defectId)
        => await Context.Set<Disposition>()
            .Where(d => d.DefectId == defectId)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync();
}
