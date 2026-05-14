using MES.Domain.Dtos.CauseCode;

namespace MES.Application.Interfaces;

public interface ICauseCodeService
{
    Task<IEnumerable<CauseCodeReadDto>> GetAllAsync();
    Task<IEnumerable<CauseCodeReadDto>> GetActiveAsync();
    Task<CauseCodeReadDto?> GetByIdAsync(long id);
    Task<CauseCodeReadDto> CreateAsync(CauseCodeCreateDto dto);
    Task UpdateAsync(long id, CauseCodeUpdateDto dto);
    Task DeleteAsync(long id);
}
