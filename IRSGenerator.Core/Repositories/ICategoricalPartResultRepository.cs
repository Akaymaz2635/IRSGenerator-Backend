using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface ICategoricalPartResultRepository : IBaseRepository<CategoricalPartResult>
{
    Task<IEnumerable<CategoricalPartResult>> GetByCharacterIdAsync(long characterId);
}
