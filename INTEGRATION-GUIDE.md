# Integration Guide

## Adım 1: Kopyalanacak Dosyalar

### IRSGenerator.Core/Entities/ klasörüne kopyala:
- Project.cs
- Inspection.cs
- Defect.cs
- DefectType.cs
- DefectField.cs
- Disposition.cs
- Photo.cs
- PhotoDefect.cs
- VisualSystemConfig.cs

### IRSGenerator.Core/Repositories/ klasörüne kopyala:
- IProjectRepository.cs
- IInspectionRepository.cs
- IDefectRepository.cs
- IDefectTypeRepository.cs
- IDefectFieldRepository.cs
- IDispositionRepository.cs
- IPhotoRepository.cs
- IVisualSystemConfigRepository.cs

### IRSGenerator.Data/Repositories/ klasörüne kopyala:
- ProjectRepository.cs
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
- Project/
- VisualSystemConfig/

## Adım 2: IRSGeneratorDbContext.cs'e ekle

```csharp
// Mevcut DbSet'lerin altına ekle:
public DbSet<Project> InspectionProjects { get; set; }
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
modelBuilder.Entity<Defect>().HasOne(d => d.OriginDefect).WithMany(d => d.ChildDefects)
    .HasForeignKey(d => d.OriginDefectId).OnDelete(DeleteBehavior.Restrict);
```

## Adım 3: Program.cs'e DI kayıtları ekle

```csharp
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IDefectRepository, DefectRepository>();
builder.Services.AddScoped<IDefectTypeRepository, DefectTypeRepository>();
builder.Services.AddScoped<IDefectFieldRepository, DefectFieldRepository>();
builder.Services.AddScoped<IDispositionRepository, DispositionRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IVisualSystemConfigRepository, VisualSystemConfigRepository>();
```

## Adım 4: Migration oluştur
```bash
Add-Migration AddInspectionEntities
Update-Database
```
