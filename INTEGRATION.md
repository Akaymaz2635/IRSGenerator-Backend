# MES Entegrasyon Rehberi

Bu branch IRSGenerator modülünün MES projesine entegrasyonu için hazırlanmıştır.
Tüm namespace'ler `IRSGenerator.*` → `MES.*` olarak güncellenmiştir.

## Dosya Yapısı

Klasör yapısı MES'tekiyle birebir uyumludur:
- `IRSGenerator.Domain/` → `MES.Domain/` içine kopyala
- `IRSGenerator.Application/` → `MES.Application/` içine kopyala
- `IRSGenerator.Infrastructure/` → `MES.Infrastructure/` içine kopyala
- `IRSGenerator.API/` → `MES.API/` içine kopyala

## ⚠️ Manuel Merge Gerektirenler

Aşağıdaki entity'ler MES'te zaten mevcut — property farklılıklarını karşılaştır:
- `Character.cs` — IRSGenerator versiyonunda InspectionId, Badge, Tooling, vb. var
- `IRSProject.cs` — IRSGenerator versiyonunda LocalOpSheetPath, vb. var
- `NumericPartResult.cs`
- `CategoricalPartResult.cs`
- `CategoricalZoneResult.cs`

## ApplicationDbContext'e Eklenecekler

`MES.Infrastructure/Persistence/ApplicationDbContext.cs` dosyasına şu DbSet'leri ekle:

```csharp
// QualiSight / IRSGenerator entities
public DbSet<VisualProject> VisualProjects { get; set; }
public DbSet<Inspection> Inspections { get; set; }
public DbSet<Defect> Defects { get; set; }
public DbSet<DefectType> DefectTypes { get; set; }
public DbSet<DefectField> DefectFields { get; set; }
public DbSet<Disposition> Dispositions { get; set; }
public DbSet<Photo> Photos { get; set; }
public DbSet<PhotoDefect> PhotoDefects { get; set; }
public DbSet<VisualSystemConfig> VisualSystemConfigs { get; set; }
public DbSet<DispositionType> DispositionTypes { get; set; }
public DbSet<DispositionTransition> DispositionTransitions { get; set; }

// NCM entities
public DbSet<CauseCode> CauseCodes { get; set; }
public DbSet<NcmDispositionType> NcmDispositionTypes { get; set; }
```

OnModelCreating içine şunları ekle:

```csharp
// PhotoDefect composite key
modelBuilder.Entity<PhotoDefect>().HasKey(pd => new { pd.PhotoId, pd.DefectId });

// Inspection → VisualProject
modelBuilder.Entity<Inspection>()
    .HasOne(i => i.VisualProject)
    .WithMany(p => p.Inspections)
    .HasForeignKey(i => i.VisualProjectId)
    .OnDelete(DeleteBehavior.SetNull);

// Inspection → User (inspector)
modelBuilder.Entity<Inspection>()
    .HasOne(i => i.InspectorUser)
    .WithMany()
    .HasForeignKey(i => i.InspectorId)
    .OnDelete(DeleteBehavior.SetNull);

// Defect self-referencing
modelBuilder.Entity<Defect>()
    .HasOne(d => d.OriginDefect)
    .WithMany(d => d.ChildDefects)
    .HasForeignKey(d => d.OriginDefectId)
    .OnDelete(DeleteBehavior.Restrict);

// Inspection → IRSProject
modelBuilder.Entity<Inspection>()
    .HasOne(i => i.IrsProject)
    .WithMany(p => p.Inspections)
    .HasForeignKey(i => i.IrsProjectId)
    .OnDelete(DeleteBehavior.Restrict)
    .IsRequired(false);

// DispositionType — unique index on Code
modelBuilder.Entity<DispositionType>()
    .HasIndex(dt => dt.Code)
    .IsUnique();

// DispositionTransition — string FK
modelBuilder.Entity<DispositionTransition>()
    .HasOne(t => t.FromType)
    .WithMany(dt => dt.TransitionsFrom)
    .HasForeignKey(t => t.FromCode)
    .HasPrincipalKey(dt => dt.Code)
    .OnDelete(DeleteBehavior.Restrict)
    .IsRequired(false);

modelBuilder.Entity<DispositionTransition>()
    .HasOne(t => t.ToType)
    .WithMany(dt => dt.TransitionsTo)
    .HasForeignKey(t => t.ToCode)
    .HasPrincipalKey(dt => dt.Code)
    .OnDelete(DeleteBehavior.Restrict);

// EF Configurations
modelBuilder.ApplyConfiguration(new VisualProjectConfiguration()); // if exists
```

## IUnitOfWork'e Eklenecekler

`MES.Application/Interfaces/IUnitOfWork.cs` dosyasına şu property'leri ekle:

```csharp
IBaseRepository<VisualProject> VisualProjectRepository { get; }
IBaseRepository<Inspection> InspectionRepository { get; }
IBaseRepository<Defect> DefectRepository { get; }
IBaseRepository<DefectType> DefectTypeRepository { get; }
IBaseRepository<DefectField> DefectFieldRepository { get; }
IBaseRepository<Disposition> DispositionRepository { get; }
IBaseRepository<DispositionType> DispositionTypeRepository { get; }
IBaseRepository<DispositionTransition> DispositionTransitionRepository { get; }
IBaseRepository<Photo> PhotoRepository { get; }
IBaseRepository<VisualSystemConfig> VisualSystemConfigRepository { get; }
IBaseRepository<CauseCode> CauseCodeRepository { get; }
IBaseRepository<NcmDispositionType> NcmDispositionTypeRepository { get; }
```

## UnitOfWork'e Eklenecekler

`MES.Infrastructure/Persistence/UnitOfWork.cs` dosyasına private field ve lazy property ekle (her biri için):

```csharp
// Private fields
private BaseRepository<VisualProject>? _visualProjectRepository;
// ... diğerleri

// Lazy properties
public IBaseRepository<VisualProject> VisualProjectRepository =>
    _visualProjectRepository ??= new BaseRepository<VisualProject>(_context);
// ... diğerleri
```

## Migration

Dosyalar eklendikten sonra MES.Infrastructure projesinde:

```bash
dotnet ef migrations add AddIRSGeneratorModule --project MES.Infrastructure --startup-project MES.API
dotnet ef database update --project MES.Infrastructure --startup-project MES.API
```

## ServiceExtensions / Program.cs

MES.API'deki ServiceExtensions'a IRSGenerator servislerini kaydet:

```csharp
// IRSGenerator Services
services.AddScoped<IIRSProjectService, IRSProjectService>();
services.AddScoped<IInspectionService, InspectionService>();
services.AddScoped<IDefectService, DefectService>();
// ... diğer servisler
```
