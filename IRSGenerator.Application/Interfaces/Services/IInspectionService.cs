using MES.Domain.Dtos.Inspection;

namespace MES.Application.Interfaces;

public interface IInspectionService
{
    Task<IEnumerable<InspectionReadDto>> GetAllAsync(string? status = null, long? visualProjectId = null, string? search = null);
    Task<InspectionReadDto?> GetByIdAsync(long id);
    Task<InspectionReadDto> CreateAsync(InspectionCreateDto dto);
    Task UpdateAsync(long id, InspectionUpdateDto dto);
    Task DeleteAsync(long id);
    Task<bool> CompleteAsync(long id);
}
