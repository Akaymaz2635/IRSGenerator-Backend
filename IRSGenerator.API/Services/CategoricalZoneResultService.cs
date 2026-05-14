using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.CategoricalZoneResult;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class CategoricalZoneResultService : ICategoricalZoneResultService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoricalZoneResultService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoricalZoneResultReadDto>> GetAllAsync(long? characterId = null)
    {
        IEnumerable<CategoricalZoneResult> items = characterId.HasValue
            ? _uow.CategoricalZoneResults.Filter(r => r.CharacterId == characterId.Value)
            : await _uow.CategoricalZoneResults.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoricalZoneResultReadDto>>(items);
    }

    public async Task<CategoricalZoneResultReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.CategoricalZoneResults.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<CategoricalZoneResultReadDto>(entity);
    }

    public async Task<CategoricalZoneResultReadDto> CreateAsync(CategoricalZoneResultCreateDto dto)
    {
        var entity = _mapper.Map<CategoricalZoneResult>(dto);
        var created = await _uow.CategoricalZoneResults.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<CategoricalZoneResultReadDto>(created);
    }

    public async Task UpdateAsync(long id, CategoricalZoneResultCreateDto dto)
    {
        var entity = await _uow.CategoricalZoneResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CategoricalZoneResult>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.CategoricalZoneResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CategoricalZoneResult>(id);
        await _uow.CategoricalZoneResults.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }

    public async Task DeleteByCharacterAsync(long characterId)
    {
        var items = _uow.CategoricalZoneResults.Filter(r => r.CharacterId == characterId).ToList();
        foreach (var item in items)
            await _uow.CategoricalZoneResults.RemoveAsync(item.Id);
        await _uow.CommitAsync();
    }
}
