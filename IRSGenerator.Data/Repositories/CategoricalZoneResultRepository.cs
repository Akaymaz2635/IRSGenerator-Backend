using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class CategoricalZoneResultRepository : BaseRepository<CategoricalZoneResult>, ICategoricalZoneResultRepository
{
    public CategoricalZoneResultRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<CategoricalZoneResult>> GetByCharacterIdAsync(long characterId)
        => await Context.CategoricalZoneResults
            .Where(r => r.CharacterId == characterId)
            .ToListAsync();
}
