using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class DefectTypeRepository : BaseRepository<DefectType>, IDefectTypeRepository
{
    public DefectTypeRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(DefectType entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<DefectType>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DefectType entity)
    {
        Context.Set<DefectType>().Remove(entity);
        await Context.SaveChangesAsync();
    }

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
