using IRSGenerator.Core.Entities;
using IRSGenerator.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IRSGenerator.Data.Repositories;

public class NumericPartResultRepository : BaseRepository<NumericPartResult>, INumericPartResultRepository
{
    public NumericPartResultRepository(IRSGeneratorDbContext context) : base(context) { }

    public async Task<IEnumerable<NumericPartResult>> GetByCharacterIdAsync(long characterId)
        => await Context.NumericPartResults
            .Where(r => r.CharacterId == characterId)
            .ToListAsync();
}
