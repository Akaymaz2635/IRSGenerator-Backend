using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Defect;
using MES.Domain.Entities;
using MES.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Services;

public class DefectService : IDefectService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DefectService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DefectReadDto>> GetAllAsync(long? inspectionId = null)
    {
        IEnumerable<Defect> items = inspectionId.HasValue
            ? _uow.Defects.Filter(d => d.InspectionId == inspectionId.Value)
            : await _uow.Defects.GetAllAsync();
        return _mapper.Map<IEnumerable<DefectReadDto>>(items);
    }

    public async Task<DefectReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Defects.GetByIdAsync(id, q => q
            .Include(d => d.DefectType)
            .Include(d => d.Dispositions.OrderByDescending(disp => disp.CreatedAt))
            .Include(d => d.ChildDefects));
        return entity is null ? null : _mapper.Map<DefectReadDto>(entity);
    }

    public async Task<DefectReadDto> CreateAsync(DefectCreateDto dto)
    {
        var entity = _mapper.Map<Defect>(dto);
        var created = await _uow.Defects.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<DefectReadDto>(created);
    }

    public async Task UpdateAsync(long id, DefectUpdateDto dto)
    {
        var entity = await _uow.Defects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Defect>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.Defects.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Defect>(id);
        await _uow.Defects.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
