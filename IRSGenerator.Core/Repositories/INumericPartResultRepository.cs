using IRSGenerator.Core.Entities;

namespace IRSGenerator.Core.Repositories;

public interface INumericPartResultRepository : IBaseRepository<NumericPartResult>
{
    Task<IEnumerable<NumericPartResult>> GetByCharacterIdAsync(long characterId);
}
