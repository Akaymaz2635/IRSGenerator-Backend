using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.NcmDispositionType;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class NcmDispositionTypeService : INcmDispositionTypeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NcmDispositionTypeService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NcmDispositionTypeReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<NcmDispositionTypeReadDto>>(await _uow.NcmDispositionTypes.GetAllAsync());

    public async Task<IEnumerable<NcmDispositionTypeReadDto>> GetActiveAsync()
        => _mapper.Map<IEnumerable<NcmDispositionTypeReadDto>>(_uow.NcmDispositionTypes.Filter(n => n.Active));

    public async Task<NcmDispositionTypeReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.NcmDispositionTypes.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<NcmDispositionTypeReadDto>(entity);
    }

    public async Task<NcmDispositionTypeReadDto> CreateAsync(NcmDispositionTypeCreateDto dto)
    {
        var entity = _mapper.Map<NcmDispositionType>(dto);
        var created = await _uow.NcmDispositionTypes.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<NcmDispositionTypeReadDto>(created);
    }

    public async Task UpdateAsync(long id, NcmDispositionTypeUpdateDto dto)
    {
        var entity = await _uow.NcmDispositionTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<NcmDispositionType>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.NcmDispositionTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<NcmDispositionType>(id);
        await _uow.NcmDispositionTypes.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
