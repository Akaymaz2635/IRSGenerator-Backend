using MES.Domain.Dtos.IRSProject;

namespace MES.Application.Interfaces;

public interface IIRSProjectService
{
    Task<IEnumerable<IRSProjectReadDto>> GetAllAsync();
    Task<IRSProjectReadDto?> GetByIdAsync(long id);
    Task<IRSProjectReadDto> CreateAsync(IRSProjectCreateDto dto);
    Task UpdateAsync(long id, IRSProjectCreateDto dto);
    Task DeleteAsync(long id);
}
