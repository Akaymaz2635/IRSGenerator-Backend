using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.DispositionTransition;
using MES.Domain.Entities;

namespace MES.API.Services;

public class DispositionTransitionService : IDispositionTransitionService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DispositionTransitionService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DispositionTransitionReadDto>> GetAllAsync(string? fromCode = null)
    {
        if (fromCode is not null)
        {
            string? code = fromCode.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : fromCode;
            var items = _uow.DispositionTransitions.Filter(t => t.FromCode == code);
            return _mapper.Map<IEnumerable<DispositionTransitionReadDto>>(items);
        }
        var all = await _uow.DispositionTransitions.GetAllAsync();
        return _mapper.Map<IEnumerable<DispositionTransitionReadDto>>(all);
    }

    public async Task<IEnumerable<string>> GetAllowedNextCodesAsync(string? currentCode = null)
    {
        string? code = currentCode == null || currentCode.Equals("null", StringComparison.OrdinalIgnoreCase)
            ? null : currentCode;
        return _uow.DispositionTransitions
            .Filter(t => t.FromCode == code)
            .Select(t => t.ToCode)
            .ToList();
    }

    public async Task BulkSetAsync(DispositionTransitionBulkSetDto dto)
    {
        // Remove existing transitions for this fromCode
        var existing = _uow.DispositionTransitions.Filter(t => t.FromCode == dto.FromCode).ToList();
        foreach (var e in existing)
        {
            await _uow.DispositionTransitions.RemoveAsync(e.Id);
        }

        // Add new transitions
        var newRows = dto.ToCodes.Distinct().Select(code => new DispositionTransition
        {
            FromCode = dto.FromCode,
            ToCode   = code,
        });
        await _uow.DispositionTransitions.AddRangeAsync(newRows);
        await _uow.CommitAsync();
    }
}
