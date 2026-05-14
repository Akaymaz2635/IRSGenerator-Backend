using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.CauseCode;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class CauseCodeService : ICauseCodeService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CauseCodeService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CauseCodeReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<CauseCodeReadDto>>(await _uow.CauseCodes.GetAllAsync());

    public async Task<IEnumerable<CauseCodeReadDto>> GetActiveAsync()
        => _mapper.Map<IEnumerable<CauseCodeReadDto>>(_uow.CauseCodes.Filter(c => c.Active));

    public async Task<CauseCodeReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.CauseCodes.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<CauseCodeReadDto>(entity);
    }

    public async Task<CauseCodeReadDto> CreateAsync(CauseCodeCreateDto dto)
    {
        var entity = _mapper.Map<CauseCode>(dto);
        var created = await _uow.CauseCodes.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<CauseCodeReadDto>(created);
    }

    public async Task UpdateAsync(long id, CauseCodeUpdateDto dto)
    {
        var entity = await _uow.CauseCodes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CauseCode>(id);
        _mapper.Map(dto, entity);
        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.CauseCodes.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<CauseCode>(id);
        await _uow.CauseCodes.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }
}
