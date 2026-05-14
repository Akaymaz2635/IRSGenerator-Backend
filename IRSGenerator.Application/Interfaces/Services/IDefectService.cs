using MES.Domain.Dtos.Defect;

namespace MES.Application.Interfaces;

public interface IDefectService
{
    Task<IEnumerable<DefectReadDto>> GetAllAsync(long? inspectionId = null);
    Task<DefectReadDto?> GetByIdAsync(long id);
    Task<DefectReadDto> CreateAsync(DefectCreateDto dto);
    Task UpdateAsync(long id, DefectUpdateDto dto);
    Task DeleteAsync(long id);
}
