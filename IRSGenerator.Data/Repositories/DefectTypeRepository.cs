using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class DefectTypeRepository : BaseRepository<DefectType>, IDefectTypeRepository
{
    public DefectTypeRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<DefectType>> GetActiveAsync()
        => await Context.Set<DefectType>()
            .Where(dt => dt.Active)
            .OrderBy(dt => dt.Name)
            .ToListAsync();

    public async Task<DefectType?> GetWithFieldsAsync(long id)
        => await Context.Set<DefectType>()
            .Include(dt => dt.DefectFields.OrderBy(f => f.SortOrder))
            .FirstOrDefaultAsync(dt => dt.Id == id);
}
