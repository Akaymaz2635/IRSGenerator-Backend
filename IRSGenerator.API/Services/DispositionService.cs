using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Disposition;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class DispositionService : IDispositionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DispositionService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<DispositionReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Dispositions.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<DispositionReadDto>(entity);
    }

    public async Task<IEnumerable<DispositionReadDto>> GetByDefectAsync(long defectId)
    {
        var items = _uow.Dispositions.Filter(d => d.DefectId == defectId);
        return _mapper.Map<IEnumerable<DispositionReadDto>>(items);
    }

    public async Task<DispositionReadDto> CreateAsync(DispositionCreateDto dto)
    {
        // DispositionType kodu geçerliliğini kontrol et
        if (!string.IsNullOrWhiteSpace(dto.Decision))
        {
            var validType = await _uow.DispositionTypes.FindAsync(dt => dt.Code == dto.Decision);
            if (validType is null)
                throw new InvalidOperationException($"Geçersiz disposition kodu: '{dto.Decision}'.");
        }

        var entity = _mapper.Map<Disposition>(dto);
        var created = await _uow.Dispositions.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<DispositionReadDto>(created);
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.Dispositions.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Disposition>(id);
        await _uow.Dispositions.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
