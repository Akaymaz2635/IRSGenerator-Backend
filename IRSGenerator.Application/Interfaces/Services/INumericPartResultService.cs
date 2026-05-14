using MES.Domain.Dtos.NumericPartResult;

namespace MES.Application.Interfaces;

public interface INumericPartResultService
{
    Task<IEnumerable<NumericPartResultReadDto>> GetAllAsync(long? characterId = null);
    Task<NumericPartResultReadDto?> GetByIdAsync(long id);
    Task<NumericPartResultReadDto> CreateAsync(NumericPartResultCreateDto dto);
    Task UpdateAsync(long id, NumericPartResultCreateDto dto);
    Task DeleteAsync(long id);
    Task DeleteByCharacterAsync(long characterId);
}
