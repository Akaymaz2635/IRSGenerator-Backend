using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DefectField;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class DefectFieldService : IDefectFieldService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DefectFieldService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DefectFieldReadDto>> GetAllAsync(long? defectTypeId = null)
    {
        IEnumerable<DefectField> items = defectTypeId.HasValue
            ? _uow.DefectFields.Filter(f => f.DefectTypeId == defectTypeId.Value)
            : await _uow.DefectFields.GetAllAsync();
        return _mapper.Map<IEnumerable<DefectFieldReadDto>>(items);
    }

    public async Task<DefectFieldReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.DefectFields.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<DefectFieldReadDto>(entity);
    }

    public async Task<DefectFieldReadDto> CreateAsync(DefectFieldCreateDto dto)
    {
        var entity = _mapper.Map<DefectField>(dto);
        var created = await _uow.DefectFields.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<DefectFieldReadDto>(created);
    }

    public async Task UpdateAsync(long id, DefectFieldUpdateDto dto)
    {
        var entity = await _uow.DefectFields.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DefectField>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.DefectFields.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<DefectField>(id);
        await _uow.DefectFields.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
