using MES.Domain.Dtos.CategoricalZoneResult;

namespace MES.Application.Interfaces;

public interface ICategoricalZoneResultService
{
    Task<IEnumerable<CategoricalZoneResultReadDto>> GetAllAsync(long? characterId = null);
    Task<CategoricalZoneResultReadDto?> GetByIdAsync(long id);
    Task<CategoricalZoneResultReadDto> CreateAsync(CategoricalZoneResultCreateDto dto);
    Task UpdateAsync(long id, CategoricalZoneResultCreateDto dto);
    Task DeleteAsync(long id);
    Task DeleteByCharacterAsync(long characterId);
}
