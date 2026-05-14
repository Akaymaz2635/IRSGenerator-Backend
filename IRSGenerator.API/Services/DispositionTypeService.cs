using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DispositionType;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class DispositionTypeService : IDispositionTypeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DispositionTypeService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DispositionTypeReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<DispositionTypeReadDto>>(await _uow.DispositionTypes.GetAllAsync());

    public async Task<IEnumerable<DispositionTypeReadDto>> GetActiveAsync()
        => _mapper.Map<IEnumerable<DispositionTypeReadDto>>(_uow.DispositionTypes.Filter(dt => dt.Active));

    public async Task<DispositionTypeReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.DispositionTypes.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<DispositionTypeReadDto>(entity);
    }

    public async Task<DispositionTypeReadDto> CreateAsync(DispositionTypeCreateDto dto)
    {
        var entity = _mapper.Map<DispositionType>(dto);
        entity.Active = true;
        var created = await _uow.DispositionTypes.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<DispositionTypeReadDto>(created);
    }

    public async Task UpdateAsync(long id, DispositionTypeUpdateDto dto)
    {
        var entity = await _uow.DispositionTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DispositionType>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.DispositionTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DispositionType>(id);
        await _uow.DispositionTypes.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
