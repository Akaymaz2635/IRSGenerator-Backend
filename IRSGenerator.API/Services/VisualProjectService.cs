using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Project;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class VisualProjectService : IVisualProjectService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VisualProjectService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectReadDto>> GetAllAsync(bool includeInactive = false)
    {
        var items = includeInactive
            ? await _uow.VisualProjects.GetAllAsync()
            : _uow.VisualProjects.Filter(p => p.Active);
        return _mapper.Map<IEnumerable<ProjectReadDto>>(items);
    }

    public async Task<ProjectReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.VisualProjects.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<ProjectReadDto>(entity);
    }

    public async Task<ProjectReadDto> CreateAsync(ProjectCreateDto dto)
    {
        var entity = _mapper.Map<VisualProject>(dto);
        var created = await _uow.VisualProjects.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<ProjectReadDto>(created);
    }

    public async Task UpdateAsync(long id, ProjectUpdateDto dto)
    {
        var entity = await _uow.VisualProjects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<VisualProject>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.VisualProjects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<VisualProject>(id);
        entity.Active = false;
        await _uow.CommitAsync();
    }
}
