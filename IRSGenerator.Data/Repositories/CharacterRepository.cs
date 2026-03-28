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
}
