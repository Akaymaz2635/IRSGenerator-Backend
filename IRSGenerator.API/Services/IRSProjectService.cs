using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.IRSProject;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class IRSProjectService : IIRSProjectService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public IRSProjectService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<IRSProjectReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<IRSProjectReadDto>>(await _uow.IRSProjects.GetAllAsync());

    public async Task<IRSProjectReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.IRSProjects.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<IRSProjectReadDto>(entity);
    }

    public async Task<IRSProjectReadDto> CreateAsync(IRSProjectCreateDto dto)
    {
        var entity = _mapper.Map<IRSProject>(dto);
        var created = await _uow.IRSProjects.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<IRSProjectReadDto>(created);
    }

    public async Task UpdateAsync(long id, IRSProjectCreateDto dto)
    {
        var entity = await _uow.IRSProjects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<IRSProject>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.IRSProjects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<IRSProject>(id);
        await _uow.IRSProjects.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
