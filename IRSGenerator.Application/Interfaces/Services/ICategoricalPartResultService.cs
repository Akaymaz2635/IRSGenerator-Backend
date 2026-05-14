using MES.Domain.Dtos.CategoricalPartResult;

namespace MES.Application.Interfaces;

public interface ICategoricalPartResultService
{
    Task<IEnumerable<CategoricalPartResultReadDto>> GetAllAsync(long? characterId = null);
    Task<CategoricalPartResultReadDto?> GetByIdAsync(long id);
    Task<CategoricalPartResultReadDto> CreateAsync(CategoricalPartResultCreateDto dto);
    Task UpdateAsync(long id, CategoricalPartResultCreateDto dto);
    Task DeleteAsync(long id);
    Task DeleteByCharacterAsync(long characterId);
}
