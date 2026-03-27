using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class DefectFieldRepository : BaseRepository<DefectField>, IDefectFieldRepository
{
    public DefectFieldRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task UpdateAsync(DefectField entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        Context.Set<DefectField>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DefectField entity)
    {
        Context.Set<DefectField>().Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DefectField>> GetByDefectTypeAsync(long defectTypeId)
        => await Context.Set<DefectField>()
            .Where(f => f.DefectTypeId == defectTypeId)
            .OrderBy(f => f.SortOrder)
            .ToListAsync();
}
