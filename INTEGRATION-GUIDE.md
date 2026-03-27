# Integration Guide

## Adım 1: Kopyalanacak Dosyalar

### IRSGenerator.API/Controllers/ klasörüne kopyala (replace):
- DefectFieldsController.cs
- DefectsController.cs
- DefectTypesController.cs
- DispositionsController.cs
- InspectionsController.cs
- PhotosController.cs
- VisualSystemConfigController.cs

### IRSGenerator.Core/Entities/ klasörüne kopyala (yeni dosyalar):
- Inspection.cs
- Defect.cs
- DefectType.cs
- DefectField.cs
- Disposition.cs
- Photo.cs
- PhotoDefect.cs
- VisualSystemConfig.cs

### IRSGenerator.Core/Repositories/ klasörüne kopyala (yeni dosyalar):
- IInspectionRepository.cs
- IDefectRepository.cs
- IDefectTypeRepository.cs
- IDefectFieldRepository.cs
- IDispositionRepository.cs
- IPhotoRepository.cs
- IVisualSystemConfigRepository.cs

### IRSGenerator.Data/Repositories/ klasörüne kopyala (yeni dosyalar):
- InspectionRepository.cs
- DefectRepository.cs
- DefectTypeRepository.cs
- DefectFieldRepository.cs
- DispositionRepository.cs
- PhotoRepository.cs
- VisualSystemConfigRepository.cs

### IRSGenerator.Shared/Dtos/ klasörüne kopyala (tüm alt klasörlerle birlikte):
- Defect/
- DefectField/
- DefectType/
- Disposition/
- Inspection/
- Photo/
- VisualSystemConfig/

---

## Adım 2: IRSProject.cs'e navigation ekle

IRSGenerator.Core/Entities/IRSProject.cs dosyasına şunu ekle:

```csharp
// Children - QualiSight görsel kontroller
public ICollection<Inspection> Inspections { get; set; } = new Collection<Inspection>();
```

---

## Adım 3: IRSGeneratorDbContext.cs'e ekle

Mevcut DbSet'lerin altına ekle:
```csharp
public DbSet<Inspection> Inspections { get; set; }
public DbSet<Defect> Defects { get; set; }
public DbSet<DefectType> DefectTypes { get; set; }
public DbSet<DefectField> DefectFields { get; set; }
public DbSet<Disposition> Dispositions { get; set; }
public DbSet<Photo> Photos { get; set; }
public DbSet<PhotoDefect> PhotoDefects { get; set; }
public DbSet<VisualSystemConfig> VisualSystemConfigs { get; set; }
```

OnModelCreating'e ekle:
```csharp
modelBuilder.Entity<PhotoDefect>().HasKey(pd => new { pd.PhotoId, pd.DefectId });

modelBuilder.Entity<Defect>()
    .HasOne(d => d.OriginDefect)
    .WithMany(d => d.ChildDefects)
    .HasForeignKey(d => d.OriginDefectId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Inspection>()
    .HasOne(i => i.IrsProject)
    .WithMany(p => p.Inspections)
    .HasForeignKey(i => i.IrsProjectId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Inspection>()
    .HasOne(i => i.Inspector)
    .WithMany()
    .HasForeignKey(i => i.InspectorId)
    .OnDelete(DeleteBehavior.SetNull);
```

---

## Adım 4: Program.cs'e DI kayıtları ekle

```csharp
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IDefectRepository, DefectRepository>();
builder.Services.AddScoped<IDefectTypeRepository, DefectTypeRepository>();
builder.Services.AddScoped<IDefectFieldRepository, DefectFieldRepository>();
builder.Services.AddScoped<IDispositionRepository, DispositionRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IVisualSystemConfigRepository, VisualSystemConfigRepository>();
```

---

## Adım 5: Migration oluştur

```
Add-Migration AddQualiSightEntities
Update-Database
```

---

## Mimari Özet

```
IRSProject (mevcut)
  ├── Characters[]        → boyutsal kontrol (IRSGenerator)
  └── Inspections[]       → görsel kontrol (QualiSight) ← YENİ
        ├── Defects[]
        │     ├── DefectType
        │     └── Dispositions[]
        └── Photos[]
```

User entity'si her iki sistem tarafından ortak kullanılır.
Project, PartNumber, SerialNumber, Operation bilgileri IRSProject'ten gelir.
