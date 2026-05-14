using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DefectType;
using MES.Domain.Entities;
using MES.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Services;

public class DefectTypeService : IDefectTypeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DefectTypeService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DefectTypeReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<DefectTypeReadDto>>(await _uow.DefectTypes.GetAllAsync());

    public async Task<IEnumerable<DefectTypeReadDto>> GetActiveAsync()
        => _mapper.Map<IEnumerable<DefectTypeReadDto>>(_uow.DefectTypes.Filter(dt => dt.Active == true));

    public async Task<DefectTypeReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.DefectTypes.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<DefectTypeReadDto>(entity);
    }

    public async Task<DefectTypeReadDto> CreateAsync(DefectTypeCreateDto dto)
    {
        var entity = _mapper.Map<DefectType>(dto);
        var created = await _uow.DefectTypes.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<DefectTypeReadDto>(created);
    }

    public async Task UpdateAsync(long id, DefectTypeUpdateDto dto)
    {
        var entity = await _uow.DefectTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DefectType>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.DefectTypes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DefectType>(id);
        await _uow.DefectTypes.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
