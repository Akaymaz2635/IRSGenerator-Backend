using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.CategoricalPartResult;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class CategoricalPartResultService : ICategoricalPartResultService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoricalPartResultService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoricalPartResultReadDto>> GetAllAsync(long? characterId = null)
    {
        IEnumerable<CategoricalPartResult> items = characterId.HasValue
            ? _uow.CategoricalPartResults.Filter(r => r.CharacterId == characterId.Value)
            : await _uow.CategoricalPartResults.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoricalPartResultReadDto>>(items);
    }

    public async Task<CategoricalPartResultReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.CategoricalPartResults.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<CategoricalPartResultReadDto>(entity);
    }

    public async Task<CategoricalPartResultReadDto> CreateAsync(CategoricalPartResultCreateDto dto)
    {
        var entity = _mapper.Map<CategoricalPartResult>(dto);
        var created = await _uow.CategoricalPartResults.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<CategoricalPartResultReadDto>(created);
    }

    public async Task UpdateAsync(long id, CategoricalPartResultCreateDto dto)
    {
        var entity = await _uow.CategoricalPartResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CategoricalPartResult>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.CategoricalPartResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CategoricalPartResult>(id);
        await _uow.CategoricalPartResults.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }

    public async Task DeleteByCharacterAsync(long characterId)
    {
        var items = _uow.CategoricalPartResults.Filter(r => r.CharacterId == characterId).ToList();
        foreach (var item in items)
            await _uow.CategoricalPartResults.RemoveAsync(item.Id);
        await _uow.CommitAsync();
    }
}
