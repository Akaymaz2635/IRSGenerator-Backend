using MES.Domain.Dtos.DefectField;

namespace MES.Application.Interfaces;

public interface IDefectFieldService
{
    Task<IEnumerable<DefectFieldReadDto>> GetAllAsync(long? defectTypeId = null);
    Task<DefectFieldReadDto?> GetByIdAsync(long id);
    Task<DefectFieldReadDto> CreateAsync(DefectFieldCreateDto dto);
    Task UpdateAsync(long id, DefectFieldUpdateDto dto);
    Task DeleteAsync(long id);
}
