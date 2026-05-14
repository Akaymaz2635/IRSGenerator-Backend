using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.API.Utils;
using MES.Domain.Dtos.Inspection;
using MES.Domain.Entities;
using MES.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Services;

public class InspectionService : IInspectionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public InspectionService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InspectionReadDto>> GetAllAsync(string? status = null, long? visualProjectId = null, string? search = null)
    {
        var predicate = PredicateBuilder.True<Inspection>();

        if (!string.IsNullOrEmpty(status))
            predicate = predicate.And(e => e.Status == status);

        if (visualProjectId.HasValue)
            predicate = predicate.And(e => e.VisualProjectId == visualProjectId.Value);

        if (!string.IsNullOrEmpty(search))
        {
            var s = search.ToLower();
            predicate = predicate.And(e =>
                (e.SerialNumber != null && e.SerialNumber.ToLower().Contains(s)) ||
                (e.PartNumber != null && e.PartNumber.ToLower().Contains(s)) ||
                (e.Inspector != null && e.Inspector.ToLower().Contains(s)));
        }

        var entities = _uow.Inspections.Filter(predicate);
        return _mapper.Map<IEnumerable<InspectionReadDto>>(entities);
    }

    public async Task<InspectionReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Inspections.GetByIdAsync(id, q => q
            .Include(i => i.VisualProject)
            .Include(i => i.IrsProject)
            .Include(i => i.InspectorUser)
            .Include(i => i.Defects).ThenInclude(d => d.DefectType)
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions)
            .Include(i => i.Defects).ThenInclude(d => d.ChildDefects)
            .Include(i => i.Photos).ThenInclude(p => p.PhotoDefects));
        return entity is null ? null : _mapper.Map<InspectionReadDto>(entity);
    }

    public async Task<InspectionReadDto> CreateAsync(InspectionCreateDto dto)
    {
        var entity = _mapper.Map<Inspection>(dto);
        var created = await _uow.Inspections.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<InspectionReadDto>(created);
    }

    public async Task UpdateAsync(long id, InspectionUpdateDto dto)
    {
        var entity = await _uow.Inspections.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Inspection>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.Inspections.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Inspection>(id);
        await _uow.Inspections.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }

    public async Task<bool> CompleteAsync(long id)
    {
        var inspection = await _uow.Inspections.GetByIdAsync(id, q => q
            .Include(i => i.Defects).ThenInclude(d => d.Dispositions));

        if (inspection == null) return false;

        var allDisposed = inspection.Defects.All(d =>
            d.Dispositions.Any(disp => disp.Decision != "VOID"));

        if (!allDisposed) return false;

        inspection.Status = "completed";
        await _uow.CommitAsync();
        return true;
    }
}
