using MES.Domain.Dtos.Character;
using MES.Domain.Dtos.Disposition;

namespace MES.Application.Interfaces;

public interface ICharacterService
{
    Task<IEnumerable<CharacterReadDto>> GetAllAsync(long? irsProjectId = null, long? inspectionId = null);
    Task<CharacterReadDto?> GetByIdAsync(long id);
    Task<CharacterReadDto> CreateAsync(CharacterCreateDto dto);
    Task UpdateAsync(long id, CharacterUpdateDto dto);
    Task DeleteAsync(long id);
    Task<IEnumerable<object>> GetDispositionsAsync(long characterId);
    Task<object> AddDispositionAsync(long characterId, DispositionCreateDto dto);
}
