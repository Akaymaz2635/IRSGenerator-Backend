using AutoMapper;
using MES.Domain.Dtos.CategoricalPartResult;
using MES.Domain.Dtos.CategoricalZoneResult;
using MES.Domain.Dtos.CauseCode;
using MES.Domain.Dtos.Character;
using MES.Domain.Dtos.Defect;
using MES.Domain.Dtos.DefectField;
using MES.Domain.Dtos.DefectType;
using MES.Domain.Dtos.Disposition;
using MES.Domain.Dtos.DispositionTransition;
using MES.Domain.Dtos.DispositionType;
using MES.Domain.Dtos.Inspection;
using MES.Domain.Dtos.IRSProject;
using MES.Domain.Dtos.NcmDispositionType;
using MES.Domain.Dtos.NumericPartResult;
using MES.Domain.Dtos.Photo;
using MES.Domain.Dtos.Project;
using MES.Domain.Dtos.User;
using MES.Domain.Dtos.VisualSystemConfig;
using MES.Domain.Entities;

namespace MES.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── VisualProject ──────────────────────────────────────────────────
        CreateMap<VisualProject, ProjectReadDto>();
        CreateMap<ProjectCreateDto, VisualProject>()
            .ForMember(d => d.Active, o => o.MapFrom(_ => true));
        CreateMap<ProjectUpdateDto, VisualProject>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── Inspection ─────────────────────────────────────────────────────
        CreateMap<Inspection, InspectionReadDto>()
            .ForMember(d => d.ProjectId, o => o.MapFrom(s => s.VisualProjectId));
        CreateMap<InspectionCreateDto, Inspection>()
            .ForMember(d => d.VisualProjectId, o => o.MapFrom(s => s.ProjectId));
        CreateMap<InspectionUpdateDto, Inspection>()
            .ForMember(d => d.VisualProjectId, o => o.MapFrom(s => s.ProjectId))
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── Defect ─────────────────────────────────────────────────────────
        CreateMap<Defect, DefectReadDto>()
            .ForMember(d => d.DefectTypeName,  o => o.MapFrom(s => s.DefectType != null ? s.DefectType.Name : null))
            .ForMember(d => d.ChildDefectIds,  o => o.MapFrom(s => s.ChildDefects.Select(c => c.Id).ToList()))
            .ForMember(d => d.Dispositions,    o => o.MapFrom(s => s.Dispositions))
            .ForMember(d => d.ActiveDisposition, o => o.MapFrom(s =>
                s.Dispositions.OrderByDescending(d => d.CreatedAt).FirstOrDefault()));
        CreateMap<DefectCreateDto, Defect>();
        CreateMap<DefectUpdateDto, Defect>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── Disposition ────────────────────────────────────────────────────
        CreateMap<Disposition, DispositionReadDto>();
        CreateMap<DispositionCreateDto, Disposition>()
            .ForMember(d => d.DecidedAt, o => o.MapFrom(s =>
                s.DecidedAt.HasValue
                    ? DateTime.SpecifyKind(s.DecidedAt.Value, DateTimeKind.Utc)
                    : (DateTime?)null));

        // ── Character ──────────────────────────────────────────────────────
        CreateMap<Character, CharacterReadDto>();
        CreateMap<CharacterCreateDto, Character>();
        CreateMap<CharacterUpdateDto, Character>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── Photo ──────────────────────────────────────────────────────────
        CreateMap<Photo, PhotoReadDto>()
            .ForMember(d => d.DefectIds, o => o.MapFrom(s =>
                s.PhotoDefects.Select(pd => pd.DefectId).ToList()));
        CreateMap<PhotoCreateDto, Photo>();

        // ── User ───────────────────────────────────────────────────────────
        CreateMap<User, UserReadDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.DisplayName));
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── IRSProject ─────────────────────────────────────────────────────
        CreateMap<IRSProject, IRSProjectReadDto>();
        CreateMap<IRSProjectCreateDto, IRSProject>();

        // ── CauseCode ──────────────────────────────────────────────────────
        CreateMap<CauseCode, CauseCodeReadDto>();
        CreateMap<CauseCodeCreateDto, CauseCode>();
        CreateMap<CauseCodeUpdateDto, CauseCode>();

        // ── DefectType ─────────────────────────────────────────────────────
        CreateMap<DefectType, DefectTypeReadDto>();
        CreateMap<DefectTypeCreateDto, DefectType>();
        CreateMap<DefectTypeUpdateDto, DefectType>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── DefectField ────────────────────────────────────────────────────
        CreateMap<DefectField, DefectFieldReadDto>();
        CreateMap<DefectFieldCreateDto, DefectField>();
        CreateMap<DefectFieldUpdateDto, DefectField>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── DispositionType ────────────────────────────────────────────────
        CreateMap<DispositionType, DispositionTypeReadDto>();
        CreateMap<DispositionTypeCreateDto, DispositionType>();
        CreateMap<DispositionTypeUpdateDto, DispositionType>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));

        // ── DispositionTransition ──────────────────────────────────────────
        CreateMap<DispositionTransition, DispositionTransitionReadDto>();

        // ── NcmDispositionType ─────────────────────────────────────────────
        CreateMap<NcmDispositionType, NcmDispositionTypeReadDto>();
        CreateMap<NcmDispositionTypeCreateDto, NcmDispositionType>();
        CreateMap<NcmDispositionTypeUpdateDto, NcmDispositionType>();

        // ── NumericPartResult ──────────────────────────────────────────────
        CreateMap<NumericPartResult, NumericPartResultReadDto>();
        CreateMap<NumericPartResultCreateDto, NumericPartResult>();

        // ── CategoricalPartResult ──────────────────────────────────────────
        CreateMap<CategoricalPartResult, CategoricalPartResultReadDto>();
        CreateMap<CategoricalPartResultCreateDto, CategoricalPartResult>();

        // ── CategoricalZoneResult ──────────────────────────────────────────
        CreateMap<CategoricalZoneResult, CategoricalZoneResultReadDto>();
        CreateMap<CategoricalZoneResultCreateDto, CategoricalZoneResult>();

        // ── VisualSystemConfig ─────────────────────────────────────────────
        CreateMap<VisualSystemConfig, VisualSystemConfigReadDto>();
        CreateMap<VisualSystemConfigUpdateDto, VisualSystemConfig>()
            .ForAllMembers(o => o.Condition((src, dest, val) => val != null));
    }
}
