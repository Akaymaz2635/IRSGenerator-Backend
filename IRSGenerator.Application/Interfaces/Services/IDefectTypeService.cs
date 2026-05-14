using MES.Domain.Dtos.DefectType;

namespace MES.Application.Interfaces;

public interface IDefectTypeService
{
    Task<IEnumerable<DefectTypeReadDto>> GetAllAsync();
    Task<IEnumerable<DefectTypeReadDto>> GetActiveAsync();
    Task<DefectTypeReadDto?> GetByIdAsync(long id);
    Task<DefectTypeReadDto> CreateAsync(DefectTypeCreateDto dto);
    Task UpdateAsync(long id, DefectTypeUpdateDto dto);
    Task DeleteAsync(long id);
}
