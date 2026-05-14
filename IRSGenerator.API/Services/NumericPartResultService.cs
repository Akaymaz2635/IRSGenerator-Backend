using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.NumericPartResult;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class NumericPartResultService : INumericPartResultService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NumericPartResultService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NumericPartResultReadDto>> GetAllAsync(long? characterId = null)
    {
        IEnumerable<NumericPartResult> items = characterId.HasValue
            ? _uow.NumericPartResults.Filter(r => r.CharacterId == characterId.Value)
            : await _uow.NumericPartResults.GetAllAsync();
        return _mapper.Map<IEnumerable<NumericPartResultReadDto>>(items);
    }

    public async Task<NumericPartResultReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.NumericPartResults.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<NumericPartResultReadDto>(entity);
    }

    public async Task<NumericPartResultReadDto> CreateAsync(NumericPartResultCreateDto dto)
    {
        var entity = _mapper.Map<NumericPartResult>(dto);
        var created = await _uow.NumericPartResults.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<NumericPartResultReadDto>(created);
    }

    public async Task UpdateAsync(long id, NumericPartResultCreateDto dto)
    {
        var entity = await _uow.NumericPartResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<NumericPartResult>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.NumericPartResults.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<NumericPartResult>(id);
        await _uow.NumericPartResults.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }

    public async Task DeleteByCharacterAsync(long characterId)
    {
        var items = _uow.NumericPartResults.Filter(r => r.CharacterId == characterId).ToList();
        foreach (var item in items)
            await _uow.NumericPartResults.RemoveAsync(item.Id);
        await _uow.CommitAsync();
    }
}
