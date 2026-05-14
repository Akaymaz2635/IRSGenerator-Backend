using MES.Domain.Dtos.DispositionType;

namespace MES.Application.Interfaces;

public interface IDispositionTypeService
{
    Task<IEnumerable<DispositionTypeReadDto>> GetAllAsync();
    Task<IEnumerable<DispositionTypeReadDto>> GetActiveAsync();
    Task<DispositionTypeReadDto?> GetByIdAsync(long id);
    Task<DispositionTypeReadDto> CreateAsync(DispositionTypeCreateDto dto);
    Task UpdateAsync(long id, DispositionTypeUpdateDto dto);
    Task DeleteAsync(long id);
}
