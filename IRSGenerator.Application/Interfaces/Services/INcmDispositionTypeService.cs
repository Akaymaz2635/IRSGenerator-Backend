using MES.Domain.Dtos.NcmDispositionType;

namespace MES.Application.Interfaces;

public interface INcmDispositionTypeService
{
    Task<IEnumerable<NcmDispositionTypeReadDto>> GetAllAsync();
    Task<IEnumerable<NcmDispositionTypeReadDto>> GetActiveAsync();
    Task<NcmDispositionTypeReadDto?> GetByIdAsync(long id);
    Task<NcmDispositionTypeReadDto> CreateAsync(NcmDispositionTypeCreateDto dto);
    Task UpdateAsync(long id, NcmDispositionTypeUpdateDto dto);
    Task DeleteAsync(long id);
}
