using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class CharacterRepository : BaseRepository<Character>, ICharacterRepository
{
    public CharacterRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<Character>> GetByProjectIdAsync(long projectId)
        => await Context.Characters
            .Where(c => c.IRSProjectId == projectId)
            .ToListAsync();

    public async Task<IEnumerable<Character>> GetByInspectionIdAsync(long inspectionId)
        => await Context.Characters
            .Include(c => c.NumericPartResults)
            .Include(c => c.CategoricalPartResults)
            .Include(c => c.CategoricalZoneResults)
            .Include(c => c.Dispositions)
            .Where(c => c.InspectionId == inspectionId)
            .OrderBy(c => c.Id)
            .ToListAsync();

    public async Task<IEnumerable<Character>> GetByInspectionIdWithDispositionsAsync(long inspectionId)
        => await Context.Characters
            .Include(c => c.Dispositions)
            .Where(c => c.InspectionId == inspectionId)
            .OrderBy(c => c.Id)
            .ToListAsync();
}
