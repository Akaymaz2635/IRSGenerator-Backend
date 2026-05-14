using MES.Domain.Dtos.VisualSystemConfig;

namespace MES.Application.Interfaces;

public interface IVisualSystemConfigService
{
    Task<IEnumerable<VisualSystemConfigReadDto>> GetAllAsync();
    Task<VisualSystemConfigReadDto?> GetByKeyAsync(string key);
    Task UpdateAsync(VisualSystemConfigUpdateDto dto);
}
