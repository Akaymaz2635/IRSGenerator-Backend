using MES.Domain.Dtos.Project;

namespace MES.Application.Interfaces;

public interface IVisualProjectService
{
    Task<IEnumerable<ProjectReadDto>> GetAllAsync(bool includeInactive = false);
    Task<ProjectReadDto?> GetByIdAsync(long id);
    Task<ProjectReadDto> CreateAsync(ProjectCreateDto dto);
    Task UpdateAsync(long id, ProjectUpdateDto dto);
    Task DeleteAsync(long id);
}
