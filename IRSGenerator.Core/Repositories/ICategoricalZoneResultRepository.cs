using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface ICategoricalZoneResultRepository : IBaseRepository<CategoricalZoneResult>
{
    Task<IEnumerable<CategoricalZoneResult>> GetByCharacterIdAsync(long characterId);
}
