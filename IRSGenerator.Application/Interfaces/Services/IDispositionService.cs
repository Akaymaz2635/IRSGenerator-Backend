using MES.Domain.Dtos.Disposition;

namespace MES.Application.Interfaces;

public interface IDispositionService
{
    Task<DispositionReadDto?> GetByIdAsync(long id);
    Task<IEnumerable<DispositionReadDto>> GetByDefectAsync(long defectId);
    Task<DispositionReadDto> CreateAsync(DispositionCreateDto dto);
    Task DeleteAsync(long id);
}
