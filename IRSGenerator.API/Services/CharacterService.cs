using AutoMapper;
using MES.Application.Interfaces;
using MES.Application.Interfaces;
using MES.Domain.Dtos.Character;
using MES.Domain.Dtos.Disposition;
using MES.Domain.Entities;
using MES.Domain.Exceptions;
using MES.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace MES.API.Services;

public class CharacterService : ICharacterService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    private static readonly HashSet<string> ValidResults = new(StringComparer.OrdinalIgnoreCase)
        { "Unidentified", "Conform", "Not Conform", "Pass", "Fail", "Passed", "Failed" };

    public CharacterService(IUnitOfWork uow, IMapper mapper)
    {
        _uow    = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CharacterReadDto>> GetAllAsync(long? irsProjectId = null, long? inspectionId = null)
    {
        IEnumerable<Character> items;
        if (inspectionId.HasValue)
        {
            // Use GetAllAsync with include; the Where on the IIncludableQueryable acts as a filter
            items = await _uow.Characters.GetAllAsync(q => q
                .Include(c => c.NumericPartResults)
                .Include(c => c.CategoricalPartResults)
                .Include(c => c.CategoricalZoneResults)
                .Include(c => c.Dispositions));
            items = items.Where(c => c.InspectionId == inspectionId.Value).OrderBy(c => c.Id);
        }
        else if (irsProjectId.HasValue)
        {
            items = _uow.Characters.Filter(c => c.IRSProjectId == irsProjectId.Value);
        }
        else
        {
            items = await _uow.Characters.GetAllAsync();
        }

        // Limit otomatik tespiti
        var itemList = items.ToList();
        bool anyUpdated = false;
        foreach (var c in itemList)
        {
            if (c.LowerLimit == 0 && c.UpperLimit == 0 && !string.IsNullOrEmpty(c.Dimension))
            {
                var limits = LimitCatcherService.CatchMeasurement(c.Dimension);
                if (limits.Length >= 2 && (limits[0] != 0 || limits[1] != 0))
                {
                    c.LowerLimit = limits[0];
                    c.UpperLimit = limits[1];
                    anyUpdated = true;
                }
            }
        }
        if (anyUpdated)
            await _uow.CommitAsync();

        return _mapper.Map<IEnumerable<CharacterReadDto>>(itemList);
    }

    public async Task<CharacterReadDto?> GetByIdAsync(long id)
    {
        var entity = await _uow.Characters.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<CharacterReadDto>(entity);
    }

    public async Task<CharacterReadDto> CreateAsync(CharacterCreateDto dto)
    {
        var limits = LimitCatcherService.CatchMeasurement(dto.Dimension);
        var entity = _mapper.Map<Character>(dto);
        entity.LowerLimit = limits.Length > 0 ? limits[0] : 0;
        entity.UpperLimit = limits.Length > 1 ? limits[1] : 0;
        var created = await _uow.Characters.AddAsync(entity);
        await _uow.CommitAsync();
        return _mapper.Map<CharacterReadDto>(created);
    }

    public async Task UpdateAsync(long id, CharacterUpdateDto dto)
    {
        var entity = await _uow.Characters.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Character>(id);

        if (dto.ItemNo is not null) entity.ItemNo = dto.ItemNo;
        if (dto.Badge is not null) entity.Badge = dto.Badge;
        if (dto.Tooling is not null) entity.Tooling = dto.Tooling;
        if (dto.BPZone is not null) entity.BPZone = dto.BPZone;
        if (dto.InspectionLevel is not null) entity.InspectionLevel = dto.InspectionLevel;
        if (dto.Remarks is not null) entity.Remarks = dto.Remarks;
        if (dto.Note is not null) entity.Note = dto.Note;
        if (dto.InspectionResult is not null)
        {
            var isNumeric = System.Text.RegularExpressions.Regex.IsMatch(dto.InspectionResult.Trim(), @"^[\d.,\s/\-]+$");
            if (!isNumeric && !ValidResults.Contains(dto.InspectionResult))
                throw new InvalidOperationException($"Geçersiz inspection_result: '{dto.InspectionResult}'.");
            entity.InspectionResult = dto.InspectionResult;
        }
        if (dto.Dimension is not null)
        {
            entity.Dimension = dto.Dimension;
            var limits = LimitCatcherService.CatchMeasurement(dto.Dimension);
            entity.LowerLimit = limits.Length > 0 ? limits[0] : 0;
            entity.UpperLimit = limits.Length > 1 ? limits[1] : 0;
        }

        await _uow.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _uow.Characters.GetByIdAsync(id)
            ?? throw new EntityNotFoundException<Character>(id);
        await _uow.Characters.RemoveAsync(entity.Id);
        await _uow.CommitAsync();
    }

    public async Task<IEnumerable<object>> GetDispositionsAsync(long characterId)
    {
        _ = await _uow.Characters.GetByIdAsync(characterId)
            ?? throw new EntityNotFoundException<Character>(characterId);

        var disps = _uow.Dispositions.Filter(d => d.CharacterId == characterId);
        return disps.Select(d => (object)new
        {
            id                    = d.Id,
            character_id          = d.CharacterId,
            decision              = d.Decision,
            entered_by            = d.EnteredBy,
            decided_at            = d.DecidedAt,
            note                  = d.Note,
            spec_ref              = d.SpecRef,
            created_at            = d.CreatedAt,
            measurements_snapshot = d.MeasurementsSnapshot,
        });
    }

    public async Task<object> AddDispositionAsync(long characterId, DispositionCreateDto dto)
    {
        _ = await _uow.Characters.GetByIdAsync(characterId)
            ?? throw new EntityNotFoundException<Character>(characterId);

        if (!string.IsNullOrWhiteSpace(dto.Decision))
        {
            var validType = await _uow.DispositionTypes.FindAsync(dt => dt.Code == dto.Decision);
            if (validType is null)
                throw new InvalidOperationException($"Geçersiz disposition kodu: '{dto.Decision}'.");
        }

        var disp = new Disposition
        {
            CharacterId          = characterId,
            DefectId             = null,
            Decision             = dto.Decision,
            EnteredBy            = dto.EnteredBy,
            DecidedAt            = dto.DecidedAt.HasValue
                ? DateTime.SpecifyKind(dto.DecidedAt.Value, DateTimeKind.Utc)
                : DateTime.UtcNow,
            Note                 = dto.Note ?? "",
            SpecRef              = dto.SpecRef,
            Engineer             = dto.Engineer,
            MeasurementsSnapshot = dto.MeasurementsSnapshot,
        };

        var created = await _uow.Dispositions.AddAsync(disp);
        await _uow.CommitAsync();

        return new
        {
            id           = created.Id,
            character_id = created.CharacterId,
            decision     = created.Decision,
            entered_by   = created.EnteredBy,
            decided_at   = created.DecidedAt,
            note         = created.Note,
            created_at   = created.CreatedAt,
        };
    }
}
