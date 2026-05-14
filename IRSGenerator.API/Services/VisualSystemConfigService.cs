using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.VisualSystemConfig;
using MES.Domain.Entities;
using MES.Domain.Exceptions;

namespace MES.API.Services;

public class VisualSystemConfigService : IVisualSystemConfigService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VisualSystemConfigService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VisualSystemConfigReadDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<VisualSystemConfigReadDto>>(await _uow.VisualSystemConfigs.GetAllAsync());

    public async Task<VisualSystemConfigReadDto?> GetByKeyAsync(string key)
    {
        var entity = await _uow.VisualSystemConfigs.FindAsync(c => c.Key == key);
        return entity is null ? null : _mapper.Map<VisualSystemConfigReadDto>(entity);
    }

    public async Task UpdateAsync(VisualSystemConfigUpdateDto dto)
    {
        var configs = await _uow.VisualSystemConfigs.GetAllAsync();
        var configList = configs.ToList();

        void SetKey(string key, string? value)
        {
            if (value is null) return;
            var cfg = configList.FirstOrDefault(c => c.Key == key);
            if (cfg is not null) cfg.Value = value;
        }

        SetKey("PhotoRootFolder",  dto.PhotoRootFolder);
        SetKey("ReportRootFolder", dto.ReportRootFolder);
        SetKey("BackupRootFolder", dto.BackupRootFolder);
        await _uow.CommitAsync();
    }
}
