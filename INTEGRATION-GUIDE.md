# QualiSight UI Entegrasyon Rehberi

Bu klasördeki dosyalar IRSGenerator projesine kopyalanarak QualiSight görsel muayene
UI'sini backend'e bağlar. Her OS'ta tarayıcıdan erişim sağlar.

---

## AŞAMA 1 — Önceki entegrasyondan gelen dosyalar (değişmeden geçerli)

### IRSGenerator.API/Controllers/ klasörüne kopyala (replace):
Aşağıdaki dosyalar önceki entegrasyonda eklenmiş olmalı:
- DefectFieldsController.cs, DefectsController.cs, DefectTypesController.cs
- DispositionsController.cs, InspectionsController.cs, PhotosController.cs
- VisualSystemConfigController.cs

### IRSGenerator.Core/Entities/ klasörüne kopyala:
- Defect.cs, DefectField.cs, DefectType.cs, Disposition.cs
- Photo.cs, PhotoDefect.cs, VisualSystemConfig.cs

### IRSGenerator.Core/Repositories/ klasörüne kopyala:
- IDefectRepository.cs, IDefectFieldRepository.cs, IDefectTypeRepository.cs
- IDispositionRepository.cs, IPhotoRepository.cs, IVisualSystemConfigRepository.cs

### IRSGenerator.Data/Repositories/ klasörüne kopyala:
- DefectRepository.cs, DefectFieldRepository.cs, DefectTypeRepository.cs
- DispositionRepository.cs, PhotoRepository.cs, VisualSystemConfigRepository.cs

### IRSGenerator.Shared/Dtos/ klasörüne kopyala:
- Defect/, DefectField/, DefectType/, Disposition/

---

## AŞAMA 2 — Bu entegrasyon (QualiSight UI bağlantısı)

### 2.1 Tüm mevcut Controllers'ları replace et:
```
Backend/IRSGenerator.API/Controllers/  →  IRSGenerator.API/Controllers/
```
(Tüm dosyalar dahil: yeni ProjectsController, AuthController, UsersController)

### 2.2 Tüm Entities'leri güncelle:

**Inspection.cs** — Tamamen replace et (yeni alanlar: VisualProjectId, PartNumber, SerialNumber, OperationNumber, Inspector)

**VisualProject.cs** — YENİ dosya, kopyala:
```
Backend/IRSGenerator.Core/Entities/VisualProject.cs
```

**User.cs — KRİTİK:**
Mevcut User.cs'i silme. Sadece şu 3 alanı ekle:
```csharp
public string? PasswordHash { get; set; }
public string Role { get; set; } = "inspector";
public bool Active { get; set; } = true;
```

### 2.3 Repository arayüzlerini güncelle:
```
Backend/IRSGenerator.Core/Repositories/  →  IRSGenerator.Core/Repositories/
```
(IInspectionRepository ve IPhotoRepository değişti, IVisualProjectRepository ve IUserRepository eklendi)

### 2.4 Repository implementasyonlarını güncelle:
```
Backend/IRSGenerator.Data/Repositories/  →  IRSGenerator.Data/Repositories/
```
(InspectionRepository ve PhotoRepository değişti, VisualProjectRepository ve UserRepository eklendi)

### 2.5 DTOs'leri güncelle:
```
Backend/IRSGenerator.Shared/Dtos/  →  IRSGenerator.Shared/Dtos/
```
(Inspection/ değişti, Project/, User/, Auth/ eklendi)

### 2.6 IRSGeneratorDbContext.cs — Şunları ekle:
```
Backend/IRSGenerator.Data/IRSGeneratorDbContext.cs
```
Bu dosyayı tamamen replace et. (VisualProject DbSet + yeni model konfigürasyonları eklendi)

### 2.7 Program.cs — Tamamen replace et:
```
Backend/IRSGenerator.API/Program.cs
```
(UseStaticFiles, snake_case JSON, IVisualProjectRepository, IUserRepository kayıtları)

### 2.8 wwwroot klasörünü kopyala:
```
Backend/IRSGenerator.API/wwwroot/  →  IRSGenerator.API/wwwroot/
```
(QualiSight UI statik dosyaları — index.html, static/, photos/)

---

## AŞAMA 3 — UserConfiguration.cs güncelleme

`IRSGenerator.Data/Configurations/UserConfiguration.cs` dosyasına yeni sütunları ekle:

```csharp
builder.Property(u => u.PasswordHash).HasMaxLength(64).IsRequired(false);
builder.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("inspector").IsRequired();
builder.Property(u => u.Active).HasDefaultValue(true).IsRequired();
```

Ayrıca seed data'daki User nesnelerine `Role = "admin"` ve `Active = true` ekle.

---

## AŞAMA 4 — Disposition Tipleri (Dinamik Workflow)

### 4.1 Yeni entity dosyalarını kopyala:
```
Backend/IRSGenerator.Core/Entities/DispositionType.cs     → IRSGenerator.Core/Entities/
Backend/IRSGenerator.Core/Entities/DispositionTransition.cs → IRSGenerator.Core/Entities/
```

### 4.2 Yeni repository arayüzlerini kopyala:
```
Backend/IRSGenerator.Core/Repositories/IDispositionTypeRepository.cs → IRSGenerator.Core/Repositories/
Backend/IRSGenerator.Core/Repositories/IDispositionTransitionRepository.cs → IRSGenerator.Core/Repositories/
```

### 4.3 Yeni repository implementasyonlarını kopyala:
```
Backend/IRSGenerator.Data/Repositories/DispositionTypeRepository.cs → IRSGenerator.Data/Repositories/
Backend/IRSGenerator.Data/Repositories/DispositionTransitionRepository.cs → IRSGenerator.Data/Repositories/
```

### 4.4 Yeni DTO klasörlerini kopyala:
```
Backend/IRSGenerator.Shared/Dtos/DispositionType/     → IRSGenerator.Shared/Dtos/
Backend/IRSGenerator.Shared/Dtos/DispositionTransition/ → IRSGenerator.Shared/Dtos/
```

### 4.5 Yeni Controller dosyalarını kopyala:
```
Backend/IRSGenerator.API/Controllers/DispositionTypesController.cs → IRSGenerator.API/Controllers/
Backend/IRSGenerator.API/Controllers/DispositionTransitionsController.cs → IRSGenerator.API/Controllers/
```

---

## AŞAMA 5 — Migration oluştur

```powershell
Add-Migration AddQualiSightWithDispositionTypes
Update-Database
```

Migration şunları ekleyecek:
- `VisualProjects` tablosu (yeni)
- `DispositionTypes` tablosu + seed (13 kayıt)
- `DispositionTransitions` tablosu + seed (39 geçiş kuralı)
- `Inspections` tablosuna: `VisualProjectId`, `PartNumber`, `SerialNumber`, `OperationNumber`, `Inspector` sütunları
- `Inspections.IrsProjectId` artık nullable
- `Users` tablosuna: `PasswordHash`, `Role`, `Active` sütunları

---

## AŞAMA 6 — Seed data güncelleme (isteğe bağlı)

DbContext'teki User seed data'sına şifre eklemek için AuthController.HashPassword kullanabilirsiniz:
```csharp
User user1 = new() { ..., Role = "admin", Active = true, PasswordHash = "..." };
```

Veya ilk giriş için şifresiz bırakın (PasswordHash = null → şifresiz giriş kabul edilir).

---

## AŞAMA 7 — Test

1. Proje başlat: `http://localhost:[port]` → QualiSight UI açılmalı
2. Admin: `http://localhost:[port]/admin.html`
3. Swagger: `http://localhost:[port]/swagger`

---

## Mimari Özet

```
[Tarayıcı — Chrome/Firefox/Edge]
        │
        ▼
IRSGenerator.API (ASP.NET Core 8)
  ├── /                  → wwwroot/index.html  (QualiSight UI)
  ├── /api/projects      → ProjectsController  (VisualProject CRUD)
  ├── /api/auth/login    → AuthController      (sicil + şifre)
  ├── /api/users         → UsersController
  ├── /api/inspections   → InspectionsController
  ├── /api/defects       → DefectsController
  ├── /api/defect-types  → DefectTypesController
  ├── /api/defect-fields → DefectFieldsController
  ├── /api/dispositions              → DispositionsController
  ├── /api/disposition-types        → DispositionTypesController  (YENİ)
  ├── /api/disposition-transitions  → DispositionTransitionsController (YENİ)
  ├── /api/photos        → PhotosController (file upload)
  └── /api/system-config → VisualSystemConfigController
        │
        ▼
SQL Server / PostgreSQL
```

---

## JSON Değişikliği Notu

Program.cs'e `JsonNamingPolicy.SnakeCaseLower` eklendi.
Bu tüm API endpoint'lerini etkiler. WPF client'ı kullanıyorsanız
HttpClient konfigürasyonuna şunu ekleyin:
```csharp
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
```
