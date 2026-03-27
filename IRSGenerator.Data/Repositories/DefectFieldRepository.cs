using Microsoft.EntityFrameworkCore;
using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;

namespace IRSGenerator.Data.Repositories;

public class DefectFieldRepository : BaseRepository<DefectField>, IDefectFieldRepository
{
    public DefectFieldRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<DefectField>> GetByDefectTypeAsync(long defectTypeId)
        => await Context.Set<DefectField>()
            .Where(f => f.DefectTypeId == defectTypeId)
            .OrderBy(f => f.SortOrder)
            .ToListAsync();
}
