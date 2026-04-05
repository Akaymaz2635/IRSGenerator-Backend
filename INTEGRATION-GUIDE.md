# QualiSight — Mimari Referans & Güncelleme Kılavuzu

Bu belge, sistemin tam entegre hâlini ve gelecekte herhangi bir bileşen güncellendiğinde
**hangi dosyaların değişmesi gerektiğini** gösteren referans kılavuzudur.

---

## Bölümler

1. [Mevcut Sistem Mimarisi](#1-mevcut-sistem-mimarisi)
2. [Katman → Dosya Haritası](#2-katman--dosya-haritası)
3. [Özellik Alanlarına Göre Dosya Grupları](#3-özellik-alanlarına-göre-dosya-grupları)
4. [Güncelleme Senaryoları](#4-güncelleme-senaryoları)
5. [NuGet Paket Bağımlılıkları](#5-nuget-paket-bağımlılıkları)
6. [Veritabanı Migration Zinciri](#6-veritabanı-migration-zinciri)
7. [Frontend Modül Bağımlılıkları](#7-frontend-modül-bağımlılıkları)

---

## 1. Mevcut Sistem Mimarisi

QualiSight, tek bir ASP.NET Core 8 uygulaması içinde iki ana iş modülü barındırır:

```
┌──────────────────────────────────────────────────────────────────┐
│  IRSGenerator.API  (ASP.NET Core 8 — Web API + Statik SPA)       │
│                                                                  │
│  Modül A: IRS (Boyutsal Muayene & Ölçüm)                         │
│  Modül B: QualiSight (Görsel Muayene & Defekt)                   │
│  Modül C: NCM (Uygunsuzluk Yönetimi & Disposition Sheet)         │
├──────────────────────────────────────────────────────────────────┤
│  IRSGenerator.Core  (Domain — Entity, Repository Arayüzü, Servis)│
├──────────────────────────────────────────────────────────────────┤
│  IRSGenerator.Data  (EF Core — DbContext, Repository Impl, Migration)│
├──────────────────────────────────────────────────────────────────┤
│  IRSGenerator.Shared  (DTO'lar — yalnızca POCO sınıflar)         │
├──────────────────────────────────────────────────────────────────┤
│  IRSGenerator.Tests  (xUnit — servis unit testleri)              │
└──────────────────────────────────────────────────────────────────┘
                              │
                    PostgreSQL 14+
```

**Kimlik Doğrulama:** HttpOnly cookie (`qc_auth`), SHA256 şifre hash.
**JSON:** snake_case (`PropertyNamingPolicy.SnakeCaseLower`).
**Frontend:** Vanilla JS ES Modules, hash tabanlı routing, build aracı yok.

---

## 2. Katman → Dosya Haritası

### IRSGenerator.API/Controllers/ (20 controller)

| Dosya | Route | Yetki |
|---|---|---|
| `AuthController.cs` | `/api/auth` | Herkese açık (login/logout/me) |
| `ProjectsController.cs` | `/api/projects` | GET: herkes; CUD: AdminOnly |
| `InspectionsController.cs` | `/api/inspections` | GET: herkes; CUD: CanWrite |
| `CharactersController.cs` | `/api/characters` | GET: herkes; CUD: CanWrite |
| `NumericPartResultsController.cs` | `/api/numeric-part-results` | GET: herkes; CUD: CanWrite |
| `CategoricalPartResultsController.cs` | `/api/categorical-part-results` | GET: herkes; CUD: CanWrite |
| `CategoricalZoneResultsController.cs` | `/api/categorical-zone-results` | GET: herkes; CUD: CanWrite |
| `DefectsController.cs` | `/api/defects` | GET: herkes; CUD: CanWrite |
| `DefectTypesController.cs` | `/api/defect-types` | GET: herkes; CUD: AdminOnly |
| `DefectFieldsController.cs` | `/api/defect-fields` | GET: herkes; CUD: AdminOnly |
| `DispositionsController.cs` | `/api/dispositions` | GET: herkes; CUD: CanWrite |
| `DispositionTypesController.cs` | `/api/disposition-types` | GET: herkes; CUD: AdminOnly |
| `DispositionTransitionsController.cs` | `/api/disposition-transitions` | GET: herkes; bulk-set: AdminOnly |
| `PhotosController.cs` | `/api/photos` | GET: herkes; CUD: CanWrite |
| `VisualSystemConfigController.cs` | `/api/system-config` | GET: herkes; PUT: AdminOnly |
| `UsersController.cs` | `/api/users` | AdminOnly |
| `NcmController.cs` | `/api/ncm` | generate: CanWriteNcm; templates: AdminOnly |
| `CauseCodesController.cs` | `/api/cause-codes` | GET: herkes; CUD: AdminOnly |
| `NcmDispositionTypesController.cs` | `/api/ncm-disposition-types` | GET: herkes; CUD: AdminOnly |
| *(yok — IRSProjects API doğrudan InspectionsController üzerinden)* | — | — |

> **Yetki Politikaları:** `AdminOnly` = admin rolü; `CanWrite` = inspector hariç herkes;
> `CanWriteNcm` = engineer + admin.

### IRSGenerator.Core/Entities/ (24 entity)

| Dosya | Açıklama |
|---|---|
| `BaseEntity.cs` | `Id`, `CreatedAt`, `UpdatedAt`, `CreatedById`, `UpdatedById` |
| `User.cs` | EmployeeId, DisplayName, PasswordHash, Role, Active |
| `Role.cs`, `Permission.cs`, `UserRole.cs`, `RolePermission.cs` | RBAC tabloları |
| `IRSProject.cs` | IRS proje şablonu |
| `Character.cs` | Muayene kalemi (dimensional / LOT) |
| `NumericPartResult.cs` | Sayısal ölçüm + `UpdateReason` + `UpdateNote` |
| `CategoricalPartResult.cs` | Kategorik sonuç + `UpdateReason` |
| `CategoricalZoneResult.cs` | Bölge bazlı kategorik sonuç |
| `VisualProject.cs` | Görsel muayene proje tanımı |
| `Inspection.cs` | Muayene örneği |
| `Defect.cs` | Görsel kusur |
| `DefectType.cs` | Kusur tipi tanımı |
| `DefectField.cs` | Kusur tipi özel alanı |
| `Disposition.cs` | Karar kaydı (defect veya character bağlantılı) |
| `DispositionType.cs` | Karar tipi (code, label, cssClass) |
| `DispositionTransition.cs` | State machine geçiş kuralı |
| `Photo.cs`, `PhotoDefect.cs` | Fotoğraf ve defekt eşlemesi |
| `VisualSystemConfig.cs` | Key-value konfigürasyon |
| `CauseCode.cs` | NCM kök neden kodu |
| `NcmDispositionType.cs` | NCM disposition tipi + Word şablon dosyası |

### IRSGenerator.Core/Repositories/ (21 interface)

Her entity için birer arayüz dosyası: `I{Entity}Repository.cs`.
Temel arayüz: `IBaseRepository<T>` — `GetAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `RemoveAsync`, `RemoveRange`.

### IRSGenerator.Core/Services/ (5 servis)

| Dosya | Görev |
|---|---|
| `OlcuParser.cs` | Dimension string → alt/üst limit (15+ format) |
| `LimitCatcher.cs` | OlcuParser çıktısını `double[2]` formatına normalize eder |
| `WordOpSheetParser.cs` | `.docx` op-sheet → `Character[]` |
| `WordReportWriter.cs` | Muayene verisi → Word raporu (.docx) |
| `NcmSheetGenerator.cs` | NCM disposition Word şablonu doldurur → .docx/.zip |

### IRSGenerator.Data/Repositories/ (21 implementasyon)

`IRSGenerator.Core/Repositories/` içindeki her interface için EF Core implementasyonu.
Dosya adı deseni: `{Entity}Repository.cs`.

### IRSGenerator.Data/Migrations/ (8 migration)

Ayrıntı için bkz. [Bölüm 6](#6-veritabanı-migration-zinciri).

### IRSGenerator.Shared/Dtos/ (klasör yapısı)

```
Dtos/
├── Auth/          LoginRequestDto, LoginResponseDto, LoginUserDto
├── Project/       ProjectCreateDto, ProjectReadDto, ProjectUpdateDto
├── Inspection/    InspectionCreateDto, InspectionReadDto, InspectionUpdateDto
├── Character/     CharacterCreateDto, CharacterReadDto, CharacterUpdateDto
├── NumericPartResult/     Create + Read Dto
├── CategoricalPartResult/ Create + Read Dto
├── CategoricalZoneResult/ Create + Read Dto
├── Defect/        DefectCreateDto, DefectReadDto, DefectUpdateDto
├── DefectType/    DefectTypeCreateDto, DefectTypeReadDto, DefectTypeUpdateDto
├── DefectField/   DefectFieldCreateDto, DefectFieldReadDto, DefectFieldUpdateDto
├── Disposition/   DispositionCreateDto, DispositionReadDto
├── DispositionType/       Create + Read + Update Dto
├── DispositionTransition/ Read + BulkSet Dto
├── Photo/         PhotoCreateDto, PhotoReadDto
├── User/          UserCreateDto, UserReadDto, UserUpdateDto
├── VisualSystemConfig/    Read + Update Dto
├── CauseCode/     CauseCodeCreateDto, CauseCodeReadDto, CauseCodeUpdateDto
├── NcmDispositionType/    Create + Read + Update Dto
└── Ncm/           GenerateDispositionSheetDto, NcmSheetItemDto, NcmInspectionDataDto, vb.
```

### IRSGenerator.API/wwwroot/static/js/pages/ (22 sayfa)

| Dosya | Sayfa |
|---|---|
| `dashboard.js` | Ana panel |
| `inspections.js` | Muayene listesi |
| `inspection-form.js` | Muayene oluştur/düzenle |
| `inspection-detail.js` | Muayene detayı (defekt + disposition + dimensional panel) |
| `settings.js` | Kullanıcı ayarları |
| `irs-projects.js` | IRS proje listesi |
| `irs-project-detail.js` | IRS proje detayı |
| `character-detail.js` | Karakter detayı |
| `ncm.js` | NCM listesi |
| `ncm-detail.js` | NCM detay + disposition sheet üretme |
| `nonconformance-descriptions.js` | NC açıklama listesi |
| `analytics.js` | Analitik panel |
| `admin-dashboard.js` | Yönetici paneli |
| `admin-users.js` | Kullanıcı yönetimi |
| `admin-projects.js` | Proje yönetimi |
| `admin-defect-types.js` | Kusur tipi yönetimi |
| `admin-defect-fields.js` | Kusur alanı yönetimi |
| `admin-disposition-types.js` | Disposition tipi yönetimi |
| `admin-ncm-disposition-types.js` | NCM disposition tipi yönetimi |
| `admin-cause-codes.js` | Kök neden kodu yönetimi |
| `admin-config.js` | Sistem konfigürasyonu |
| `admin-backup.js` | Yedekleme yönetimi |

---

## 3. Özellik Alanlarına Göre Dosya Grupları

### Modül A — IRS (Boyutsal Muayene)

```
Core/Entities/         IRSProject.cs, Character.cs,
                       NumericPartResult.cs, CategoricalPartResult.cs, CategoricalZoneResult.cs
Core/Repositories/     IIRSProjectRepository.cs, ICharacterRepository.cs,
                       INumericPartResultRepository.cs, ICategoricalPartResultRepository.cs,
                       ICategoricalZoneResultRepository.cs
Core/Services/         OlcuParser.cs, LimitCatcher.cs, WordOpSheetParser.cs, WordReportWriter.cs
Data/Repositories/     IRSProjectRepository.cs, CharacterRepository.cs,
                       NumericPartResultRepository.cs, CategoricalPartResultRepository.cs,
                       CategoricalZoneResultRepository.cs
Shared/Dtos/           Character/, NumericPartResult/, CategoricalPartResult/, CategoricalZoneResult/
API/Controllers/       CharactersController.cs, NumericPartResultsController.cs,
                       CategoricalPartResultsController.cs, CategoricalZoneResultsController.cs
wwwroot/pages/         irs-projects.js, irs-project-detail.js, character-detail.js,
                       inspection-detail.js (Dimensional panel)
```

### Modül B — QualiSight (Görsel Muayene)

```
Core/Entities/         VisualProject.cs, Inspection.cs, Defect.cs, DefectType.cs,
                       DefectField.cs, Disposition.cs, DispositionType.cs,
                       DispositionTransition.cs, Photo.cs, PhotoDefect.cs, VisualSystemConfig.cs
Core/Repositories/     IVisualProjectRepository.cs, IInspectionRepository.cs,
                       IDefectRepository.cs, IDefectTypeRepository.cs, IDefectFieldRepository.cs,
                       IDispositionRepository.cs, IDispositionTypeRepository.cs,
                       IDispositionTransitionRepository.cs, IPhotoRepository.cs,
                       IVisualSystemConfigRepository.cs
Data/Repositories/     VisualProjectRepository.cs, InspectionRepository.cs,
                       DefectRepository.cs, DefectTypeRepository.cs, DefectFieldRepository.cs,
                       DispositionRepository.cs, DispositionTypeRepository.cs,
                       DispositionTransitionRepository.cs, PhotoRepository.cs,
                       VisualSystemConfigRepository.cs
Shared/Dtos/           Project/, Inspection/, Defect/, DefectType/, DefectField/,
                       Disposition/, DispositionType/, DispositionTransition/, Photo/
API/Controllers/       ProjectsController.cs, InspectionsController.cs, DefectsController.cs,
                       DefectTypesController.cs, DefectFieldsController.cs,
                       DispositionsController.cs, DispositionTypesController.cs,
                       DispositionTransitionsController.cs, PhotosController.cs,
                       VisualSystemConfigController.cs
wwwroot/pages/         inspections.js, inspection-form.js, inspection-detail.js,
                       analytics.js
```

### Modül C — NCM (Uygunsuzluk Yönetimi)

```
Core/Entities/         CauseCode.cs, NcmDispositionType.cs
Core/Repositories/     ICauseCodeRepository.cs, INcmDispositionTypeRepository.cs
Core/Services/         NcmSheetGenerator.cs
Data/Repositories/     CauseCodeRepository.cs, NcmDispositionTypeRepository.cs
Shared/Dtos/           CauseCode/, NcmDispositionType/, Ncm/
API/Controllers/       NcmController.cs, CauseCodesController.cs, NcmDispositionTypesController.cs
API/DispositionTemplates/ ACCEPT.docx, CTP&MRB.docx, CTP&R-I.docx, CTP&R-W.docx,
                          DEBURR R-W.docx, EMPTY.docx, MRB.docx, RETURN-TO-VENDOR.docx,
                          SCRAP-IND.docx, SCRAP-LOT.docx, STD-OP-R-W.docx, WELD R-W.docx
wwwroot/pages/         ncm.js, ncm-detail.js, nonconformance-descriptions.js,
                       admin-ncm-disposition-types.js, admin-cause-codes.js
```

### Auth & Kullanıcı Yönetimi

```
Core/Entities/         User.cs, Role.cs, Permission.cs, UserRole.cs, RolePermission.cs
Core/Repositories/     IUserRepository.cs, IRoleRepository.cs, IPermissionRepository.cs
Data/Repositories/     UserRepository.cs, RoleRepository.cs, PermissionRepository.cs
Shared/Dtos/           Auth/, User/
API/Controllers/       AuthController.cs, UsersController.cs
API/Extensions/        ServiceExtensions.cs  (politika tanımları: AdminOnly, CanWrite, CanWriteNcm)
API/Program.cs         Cookie auth konfigürasyonu
```

---

## 4. Güncelleme Senaryoları

### 4.1 Yeni bir entity (tablo) eklemek

Aşağıdaki dosyalar sırasıyla değişir/oluşturulur:

```
1. Core/Entities/{YeniEntity}.cs                        YENİ
2. Core/Repositories/I{YeniEntity}Repository.cs         YENİ
3. Data/Repositories/{YeniEntity}Repository.cs          YENİ
4. Data/IRSGeneratorDbContext.cs                        DEĞİŞİR  (DbSet ekle)
5. Shared/Dtos/{YeniEntity}/...Dto.cs                   YENİ
6. API/Controllers/{YeniEntity}sController.cs           YENİ
7. API/Extensions/ServiceExtensions.cs                  DEĞİŞİR  (DI kaydı)
8. dotnet ef migrations add ...                         YENİ migration
9. wwwroot/static/js/pages/{yeni-sayfa}.js              YENİ (gerekirse)
10. wwwroot/static/js/api.js                            DEĞİŞİR  (yeni API grubu)
11. wwwroot/index.html veya app.js                      DEĞİŞİR  (route/link ekle)
```

### 4.2 Mevcut entity'ye alan eklemek

```
1. Core/Entities/{Entity}.cs                            DEĞİŞİR  (property ekle)
2. Shared/Dtos/{Entity}/...Dto.cs                       DEĞİŞİR  (DTO'ya ekle)
3. API/Controllers/{Entity}sController.cs               DEĞİŞİR  (mapping güncelle)
4. dotnet ef migrations add Add{Alan}To{Entity}         YENİ migration
5. wwwroot/static/js/pages/{ilgili-sayfa}.js            DEĞİŞİR  (UI güncelle)
```

### 4.3 Yeni NCM disposition şablonu eklemek

```
1. API/DispositionTemplates/{YeniSablon}.docx           YENİ  (Word template)
2. NcmDispositionType kaydını DB'ye ekle:
   POST /api/ncm-disposition-types
   { "code": "YENİ", "label": "...", "template_file_name": "YeniSablon.docx" }
   -- Admin panelinden de yapılabilir.
```

### 4.4 OlcuParser'a yeni ölçüm formatı eklemek

```
1. Core/Services/OlcuParser.cs     DEĞİŞİR  (yeni OlcuFormati alt sınıfı ekle,
                                             OlcuYakalayici constructor'ına doğru sıraya ekle)
2. Core/Services/LimitCatcher.cs   DEĞİŞİR  (gerekirse yeni format case'i)
3. IRSGenerator.Tests/...          DEĞİŞİR  (yeni test senaryosu ekle)
```

### 4.5 Disposition state machine'i güncellemek

```
DB üzerinden veya API üzerinden:
  POST /api/disposition-transitions/bulk-set   [AdminOnly]
  -- Kod değişikliği GEREKMİYOR, geçişler tamamen DB tabanlıdır.

Yeni bir DispositionType kodu eklenirse:
  DispositionType kaydını POST /api/disposition-types ile ekle [AdminOnly]
  Ardından geçiş kurallarını bulk-set ile tanımla.
```

### 4.6 Yetkilendirme politikası değiştirmek

```
1. API/Extensions/ServiceExtensions.cs    DEĞİŞİR  (AddAuthorization içindeki politika)
2. İlgili Controller(lar)                DEĞİŞİR  ([Authorize(Policy="...")] attribute)
```

### 4.7 Yeni frontend sayfası eklemek

```
1. wwwroot/static/js/pages/{yeni-sayfa}.js    YENİ  (export async function render(params, root))
2. wwwroot/static/js/app.js                   DEĞİŞİR  (route tanımı ekle)
3. wwwroot/index.html                         DEĞİŞİR  (sidebar link ekle, gerekirse)
4. wwwroot/static/js/api.js                   DEĞİŞİR  (yeni API çağrıları gerekirse)
```

### 4.8 NuGet paketi güncellemek

```
1. Paketin ait olduğu proje .csproj dosyası    DEĞİŞİR
   - IRSGenerator.API.csproj        (Swashbuckle, Microsoft.AspNetCore.*, EF Design)
   - IRSGenerator.Core.csproj       (Microsoft.EntityFrameworkCore, DocumentFormat.OpenXml)
   - IRSGenerator.Data.csproj       (EF Core, Npgsql.EntityFrameworkCore.PostgreSQL)
   - IRSGenerator.Shared.csproj     (bağımlılık yok, değişmesi gerekmez)
   - IRSGenerator.Tests.csproj      (xUnit, test adaptörleri)
```

---

## 5. NuGet Paket Bağımlılıkları

### IRSGenerator.API.csproj

| Paket | Sürüm |
|---|---|
| `Microsoft.AspNetCore.Authentication.Negotiate` | 8.0.* |
| `Microsoft.AspNetCore.OpenApi` | 8.0.25 |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.* |
| `Swashbuckle.AspNetCore` | 10.1.7 |

### IRSGenerator.Core.csproj

| Paket | Sürüm |
|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.* |
| `DocumentFormat.OpenXml` | 3.* |

### IRSGenerator.Data.csproj

| Paket | Sürüm |
|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.* |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.* |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 8.0.* |
| `Microsoft.AspNetCore.Http.Abstractions` | 2.2.0 |

### IRSGenerator.Tests.csproj

| Paket | Sürüm |
|---|---|
| `xunit` | — |
| `xunit.runner.visualstudio` | — |
| `Microsoft.NET.Test.Sdk` | — |

---

## 6. Veritabanı Migration Zinciri

Tüm migration'lar `IRSGenerator.Data/Migrations/` altındadır.
Uygulanma sırası önemlidir — atlanmamalıdır.

| # | Migration Adı | Ne Ekler |
|---|---|---|
| 1 | `20260328112545_InitialCreate` | Tüm temel tablolar: User, IRSProject, Character, Inspection, Defect, DefectType, DefectField, Disposition, DispositionType, DispositionTransition, Photo, VisualProject, NumericPartResult, CategoricalPartResult, CategoricalZoneResult, VisualSystemConfig, Auth tabloları |
| 2 | `20260329000000_AddInspectionIdToCharacterAndOpSheetPath` | Character.InspectionId, Inspection.OpSheetPath |
| 3 | `20260329100000_AddNoteToCharacterAndPartLabelToNumericResult` | Character.Note, NumericPartResult.PartLabel |
| 4 | `20260329200000_AddCharacterIdToDisposition` | Disposition.CharacterId (dimensional karar desteği) |
| 5 | `20260403230333_AddNcmEntities` | CauseCode tablosu, NcmDispositionType tablosu |
| 6 | `20260404191854_AddUpdateReasonToNumericPartResult` | NumericPartResult.UpdateReason |
| 7 | `20260404211136_AddUpdateNoteToNumericPartResult` | NumericPartResult.UpdateNote |
| 8 | `20260404213554_AddUpdateReasonToCategoricalPartResult` | CategoricalPartResult.UpdateReason |

**Veritabanını sıfırdan kurmak:**
```bash
dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API
```

**Yeni migration eklemek:**
```bash
dotnet ef migrations add {AciklamaciIsim} \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API
```

---

## 7. Frontend Modül Bağımlılıkları

```
index.html
  └── app.js          (router — tüm sayfaları lazy-import eder)
       ├── api.js      (merkezi HTTP katmanı — tüm sayfalar kullanır)
       └── pages/
            ├── inspection-detail.js  ── api.js
            ├── ncm-detail.js         ── api.js
            ├── character-detail.js   ── api.js
            └── ...diğer sayfalar     ── api.js

admin.html
  └── admin.js        (admin SPA router)
```

**api.js içinde tanımlı nesne grupları (güncelleme referansı):**

```javascript
api.auth          → /api/auth
api.projects      → /api/projects
api.inspections   → /api/inspections
api.characters    → /api/characters
api.numericResults        → /api/numeric-part-results
api.categoricalResults    → /api/categorical-part-results
api.categoricalZoneResults→ /api/categorical-zone-results
api.defects       → /api/defects
api.defectTypes   → /api/defect-types
api.defectFields  → /api/defect-fields
api.dispositions  → /api/dispositions
api.dispositionTypes      → /api/disposition-types
api.dispositionTransitions→ /api/disposition-transitions
api.photos        → /api/photos
api.systemConfig  → /api/system-config
api.users         → /api/users
api.ncm           → /api/ncm
api.causeCodes    → /api/cause-codes
api.ncmDispositionTypes   → /api/ncm-disposition-types
```

Yeni bir backend endpoint eklediğinizde mutlaka `api.js`'e de karşılık gelen fonksiyonu ekleyin.
