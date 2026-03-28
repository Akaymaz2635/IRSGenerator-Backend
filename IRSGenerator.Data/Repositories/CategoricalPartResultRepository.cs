using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class CategoricalPartResultRepository : BaseRepository<CategoricalPartResult>, ICategoricalPartResultRepository
{
    public CategoricalPartResultRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<CategoricalPartResult>> GetByCharacterIdAsync(long characterId)
        => await Context.CategoricalPartResults
            .Where(r => r.CharacterId == characterId)
            .ToListAsync();
}
