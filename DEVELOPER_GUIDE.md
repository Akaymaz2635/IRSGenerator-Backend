# IRSGenerator — Geliştirici Kılavuzu

Bu belge, IRSGenerator projesini geliştiren veya bakımını yapan yazılımcılar içindir.

---

## İçindekiler

1. [Proje Mimarisi](#1-proje-mimarisi)
2. [Çözüm Yapısı](#2-çözüm-yapısı)
3. [Geliştirme Ortamı Kurulumu](#3-geliştirme-ortamı-kurulumu)
4. [Veritabanı Şeması](#4-veritabanı-şeması)
5. [Temel Servisler](#5-temel-servisler)
6. [API Endpoint Referansı](#6-api-endpoint-referansı)
7. [Frontend Mimarisi](#7-frontend-mimarisi)
8. [Yeni Özellik Ekleme](#8-yeni-özellik-ekleme)
9. [Migration Yönetimi](#9-migration-yönetimi)
10. [Test Etme](#10-test-etme)
11. [Yaygın Bakım Görevleri](#11-yaygın-bakım-görevleri)
12. [Bilinen Kısıtlamalar ve Gelecek Geliştirmeler](#12-bilinen-kısıtlamalar-ve-gelecek-geliştirmeler)

---

## 1. Proje Mimarisi

Proje klasik **N-Katmanlı (N-Tier) Clean Architecture** yaklaşımıyla tasarlanmıştır:

```
┌─────────────────────────────────────────────────────┐
│  IRSGenerator.API  (Presentation Layer)             │
│  ├── Controllers/   ASP.NET Core Web API            │
│  ├── wwwroot/       Static HTML/JS/CSS frontend     │
│  └── Program.cs     DI container, middleware        │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Core  (Domain Layer)                  │
│  ├── Entities/      Domain modelleri                │
│  ├── Repositories/  Interface tanımları (sözleşme)  │
│  └── Services/      İş mantığı servisleri           │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Data  (Infrastructure Layer)          │
│  ├── Repositories/  EF Core implementasyonları      │
│  ├── Configurations/ Fluent API entity config       │
│  ├── Migrations/    DB migration dosyaları          │
│  └── IRSGeneratorDbContext.cs                       │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Shared  (DTO Layer)                   │
│  └── Dtos/          Request/Response nesneleri      │
└─────────────────────────────────────────────────────┘
```

**Teknoloji Yığını:**

| Katman | Teknoloji |
|---|---|
| Backend framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Veritabanı | PostgreSQL 14+ |
| Word işleme | DocumentFormat.OpenXml (Open XML SDK) |
| JSON | System.Text.Json (snake_case policy) |
| Frontend | Vanilla JS (ES Modules), HTML5, CSS3 |
| Auth | JWT (BCrypt şifre hash) |

---

## 2. Çözüm Yapısı

```
IRSGenerator-Backend/
│
├── IRSGenerator.API/
│   ├── Controllers/
│   │   ├── AuthController.cs              # Giriş / JWT üretme
│   │   ├── CharactersController.cs        # Muayene kalemleri + disposition
│   │   ├── InspectionsController.cs       # Muayene CRUD + rapor
│   │   ├── DispositionsController.cs      # Disposition (defect bazlı)
│   │   ├── DispositionTypesController.cs  # Disposition type yönetimi
│   │   ├── DispositionTransitionsController.cs  # State machine geçişleri
│   │   ├── DefectsController.cs           # Görsel kusurlar
│   │   ├── DefectTypesController.cs       # Kusur tipi yönetimi
│   │   ├── NumericPartResultsController.cs
│   │   ├── CategoricalPartResultsController.cs
│   │   ├── CategoricalZoneResultsController.cs
│   │   ├── IRSProjectsController.cs       # Proje şablonları
│   │   ├── ProjectsController.cs          # Görsel proje yönetimi
│   │   ├── UsersController.cs
│   │   └── PhotosController.cs
│   │
│   ├── wwwroot/
│   │   ├── index.html                     # SPA giriş noktası
│   │   ├── op-sheets/                     # Yüklenen .docx dosyaları (gitignore)
│   │   ├── photos/                        # Yüklenen fotoğraflar (gitignore)
│   │   └── static/
│   │       ├── css/style.css
│   │       └── js/
│   │           ├── api.js                 # API istek katmanı
│   │           ├── app.js                 # Router, layout
│   │           ├── session.js             # JWT session yönetimi
│   │           └── pages/
│   │               ├── inspection-detail.js   # Ana muayene sayfası
│   │               ├── inspections.js
│   │               ├── inspection-form.js
│   │               ├── character-detail.js
│   │               ├── irs-projects.js
│   │               └── irs-project-detail.js
│   │
│   ├── appsettings.json                   # DB bağlantısı (üretimde env var kullanın)
│   └── Program.cs                         # Uygulama başlangıcı, DI, middleware
│
├── IRSGenerator.Core/
│   ├── Entities/
│   │   ├── BaseEntity.cs                  # Id, CreatedAt, UpdatedAt
│   │   ├── Inspection.cs
│   │   ├── Character.cs                   # Muayene kalemi (dimensional/LOT)
│   │   ├── NumericPartResult.cs           # Ölçüm sonuçları
│   │   ├── CategoricalPartResult.cs       # LOT sonuçları
│   │   ├── CategoricalZoneResult.cs       # Bölge bazlı LOT sonuçları
│   │   ├── Defect.cs                      # Görsel kusur
│   │   ├── Disposition.cs                 # Karar kaydı
│   │   ├── DispositionType.cs             # Karar tipi (USE_AS_IS, REWORK vb.)
│   │   ├── DispositionTransition.cs       # State machine geçişleri
│   │   └── ...
│   │
│   ├── Repositories/                      # Interface tanımları
│   │   ├── IBaseRepository.cs
│   │   ├── ICharacterRepository.cs
│   │   ├── IInspectionRepository.cs
│   │   ├── IDispositionRepository.cs
│   │   └── ...
│   │
│   └── Services/
│       ├── OlcuParser.cs                  # Dimension string → limit çözümleme
│       ├── LimitCatcher.cs                # OlcuParser'ı sarmalayan yardımcı
│       ├── WordOpSheetParser.cs           # .docx → Character[] dönüşümü
│       └── WordReportWriter.cs            # Inspection → Word raporu
│
├── IRSGenerator.Data/
│   ├── IRSGeneratorDbContext.cs
│   ├── Configurations/                    # Fluent API entity konfigürasyonları
│   ├── Migrations/                        # EF Core migration dosyaları
│   └── Repositories/                      # EF Core implementasyonları
│
└── IRSGenerator.Shared/
    └── Dtos/                              # CreateDto, ReadDto, UpdateDto
```

---

## 3. Geliştirme Ortamı Kurulumu

### 3.1 Gereksinimler

- .NET SDK 8.0+
- PostgreSQL 14+
- Visual Studio 2022 / Rider / VS Code (C# DevKit eklentisiyle)
- Git

### 3.2 Repo'yu Klonla

```bash
git clone https://github.com/Akaymaz2635/IRSGenerator-Backend.git
cd IRSGenerator-Backend
```

### 3.3 Veritabanı Hazırlığı

```sql
-- psql ile bağlan
CREATE DATABASE "IRSGeneratorDb";
```

### 3.4 Connection String Ayarla

`IRSGenerator.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PosgresConnection": "Host=localhost;Port=5432;Database=IRSGeneratorDb;Username=postgres;Password=postgres"
  }
}
```

### 3.5 Migration Uygula

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API
```

### 3.6 Uygulamayı Çalıştır

```bash
dotnet run --project IRSGenerator.API
# → http://localhost:5297
# → http://localhost:5297/swagger  (API dökümantasyonu)
```

### 3.7 Hot Reload (Geliştirme)

```bash
dotnet watch run --project IRSGenerator.API
```

---

## 4. Veritabanı Şeması

### Temel Entity İlişkileri

```
IRSProject (şablon)
  └── Character[] ─── irs_project_id

Inspection (muayene örneği)
  ├── Character[]    ─── inspection_id
  │     ├── NumericPartResult[]
  │     ├── CategoricalPartResult[]
  │     ├── CategoricalZoneResult[]
  │     └── Disposition[]          ─── character_id (dimensional karar)
  └── Defect[]
        └── Disposition[]          ─── defect_id (görsel karar)

DispositionType   ←── Disposition.Decision (code referansı, FK değil)
DispositionTransition  (from_code → to_code state machine)
```

### Önemli Alanlar

**Character:**
- `badge` — `null` = dimensional, `"LOT"` = kategorik
- `inspection_result` — `"Unidentified"` başlangıç, ölçüm sonrası sayısal değer veya `"Conform"/"Not Conform"`
- `lower_limit`, `upper_limit` — `OlcuParser` tarafından `dimension` stringinden çıkarılır
- `inspection_id` — bağlı olduğu muayene (nullable; şablon karakterler `null`)
- `irs_project_id` — bağlı olduğu proje şablonu (nullable)

**NumericPartResult:**
- `actual` — ham giriş ("17.65" veya "254.005/254.003/253.998")
- `part_label` — çok parçalı ölçümde `"P1"`, `"P2"` vb.; rework tarihçesi `"PRE_REWORK"`, `"POST_REWORK"`

**Disposition:**
- `decision` — `DispositionType.code` değerlerinden biri (string FK, enum değil)
- `defect_id` veya `character_id` — ikisinden biri `null` olur (görsel vs. dimensional)

---

## 5. Temel Servisler

### 5.1 OlcuParser (`IRSGenerator.Core/Services/OlcuParser.cs`)

Dimension stringlerini parse ederek alt/üst limit değerlerini çıkarır.

**Mimari:**
```
OlcuYakalayici.Isle(string) → OlcuSonucu?
  │
  ├── IplikToleransi      "1/8 NPT", "M8", "UNC" → Format="diş", limitler null
  ├── FormToleransi        "FLATNESS 0.02"
  ├── OryantasyonToleransi "PERPENDICULARITY 0.01 | A"
  ├── LokasyonToleransi    "TRUE POSITION ∅0.05 | A | B"
  ├── ProfilToleransi      "SP | 0.15"
  ├── RunoutToleransi      "RUNOUT 0.03 | A-B"
  ├── SembolTolerans       "⊥ 0.01"
  ├── EsitToleransliOlcu   "25.4 ± 0.1"
  ├── ArtiEksiOlcu         "17.75 +0 / -0.15"
  ├── ArtiEksiOlcu2        "17.75 +0.1 -0.15"
  ├── MaxMinOlcu           "MIN 5 / MAX 10"
  ├── LimitToleransliOlcu  "17.60 / 17.75"  ← SONA KONULDU (öncelik sorunu önlenir)
  ├── MinOlcu              "0.5 MIN"
  ├── MaxOlcu              "6.3 MAX"
  └── PuruzlulukOlcu       "Ra 1.6", "6.3 RA"
```

> **Önemli:** Format listesindeki sıra kritiktir. `EsitToleransliOlcu` ve `ArtiEksiOlcu`, `LimitToleransliOlcu`'dan önce gelmelidir. Aksi hâlde `"17.75 +0/-0.15"` gibi değerler yanlış parse edilir.

**Yeni format eklemek:**

```csharp
public class YeniFormat : OlcuFormati
{
    public override bool Eslestir(string olcu) { /* regex match */ }
    public override OlcuSonucu Degerler()  { /* değerleri döndür */ }
}

// OlcuYakalayici constructor'ına uygun pozisyona ekle:
_formatTipleri = new List<OlcuFormati> { ..., new YeniFormat(), ... };
```

### 5.2 LimitCatcherService (`IRSGenerator.Core/Services/LimitCatcher.cs`)

`OlcuParser` çıktısını `double[2] = [lowerLimit, upperLimit]` formatına normalize eder.

| Format | Lower | Upper |
|---|---|---|
| geometrik, yüzey | `0` | tolerans değeri |
| minimum | `altLimit` | `double.MaxValue` |
| maksimum | `0` | üst limit |
| her ikisi | `altLimit` | `üstLimit` |
| diş (`"diş"`) | — | `[]` boş döner |

> `double.MaxValue` frontend'de `"≥ X"` olarak gösterilir (`formatLimits()` fonksiyonu).

### 5.3 WordOpSheetParser (`IRSGenerator.Core/Services/WordOpSheetParser.cs`)

Yüklenen `.docx` op-sheet dosyasından `Character[]` üretir.

**Algoritma:**
1. Tüm tablolarda `"ITEM NO"` + `"DIMENSION"` içeren header satırını bul
2. Header'dan kolon indekslerini tespit et (ITEM NO, DIMENSION, BADGE, TOOLING vb.)
3. Her veri satırı için:
   - Skip kontrolü: `"ITEM NO KC"`, `"INSPECTION"`, `"PAGE NO"` vb. atla
   - `Regex.Replace(itemNo, @"\s+", "")` — NBSP dahil tüm whitespace'i temizle
   - `IsLotDimension()` — "VISUAL", "CHECK", "MARKING" içeriyorsa `badge="LOT"` yap
   - `LimitCatcherService.CatchMeasurement(dimension)` çağır

**Yeni skip keyword eklemek:**
```csharp
private static readonly string[] SkipItemSuffixes =
    ["ITEM NO KC", "ITEM NO", "RECORD", "INSPECTION", "INSPECTOR", "/0", "**", "PAGE NO",
     "YENİ_KEYWORD"];  // buraya ekle
```

**Yeni LOT keyword eklemek:**
```csharp
private static readonly string[] LotKeywords =
    ["VISUAL", "CHECK", "MARKING", "SURFACE", "COATING",
     "YENİ_LOT_KEYWORD"];  // buraya ekle
```

### 5.4 WordReportWriter (`IRSGenerator.Core/Services/WordReportWriter.cs`)

Muayene verilerinden Word raporu üretir.

**Rapor yapısı:**
1. **Op-sheet klonu** — orijinal op-sheet'e ACTUAL değerleri yazılır
2. **Dimensional Results tablosu** — tüm karakterler + son ölçüm
3. **Non-Conformances bölümü** — yalnızca `"Not Conform"` karakterler + aktif disposition

**Rework tarihçesi yönetimi:**
Aynı `part_label` için birden fazla `NumericPartResult` varsa (örn. rework öncesi/sonrası), raporda en son kayıt gösterilir:
```csharp
.GroupBy(r => r.PartLabel)
.Select(g => g.OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue).First().Actual)
```

---

## 6. API Endpoint Referansı

Tüm endpoint'ler `http://localhost:5297/swagger` adresinde interaktif olarak test edilebilir. Aşağıda önemli endpoint'lerin özeti verilmiştir.

### JSON Formatı

Tüm request ve response'lar **snake_case** kullanır (`PropertyNamingPolicy = SnakeCaseLower`).

```json
// ✅ Doğru
{ "part_number": "ABC123", "serial_number": "SN001" }

// ❌ Yanlış
{ "partNumber": "ABC123", "serialNumber": "SN001" }
```

### Muayene (Inspection)

| Method | URL | Açıklama |
|---|---|---|
| GET | `/api/inspections` | Liste; `?status=open`, `?search=` filtreleri |
| POST | `/api/inspections` | Yeni muayene oluştur |
| GET | `/api/inspections/{id}` | Detay (defects + dispositions dahil) |
| PUT | `/api/inspections/{id}` | Güncelle |
| DELETE | `/api/inspections/{id}` | Sil |
| POST | `/api/inspections/{id}/parse-op-sheet` | `multipart/form-data`, `file` alanı |
| POST | `/api/inspections/{id}/complete` | Kapat (tüm NC'ler disposition gerektiriyor) |
| GET | `/api/inspections/{id}/report` | Word raporu indir (.docx) |
| GET | `/api/inspections/{id}/report-data` | Rapor verisi JSON olarak |

### Karakter (Character)

| Method | URL | Açıklama |
|---|---|---|
| GET | `/api/characters?inspection_id={id}` | Muayeneye ait karakterler |
| GET | `/api/characters?irs_project_id={id}` | Proje şablonuna ait karakterler |
| PUT | `/api/characters/{id}` | Güncelle (inspection_result, note vb.) |
| GET | `/api/characters/{id}/dispositions` | Karakterin karar geçmişi |
| POST | `/api/characters/{id}/dispositions` | Yeni karar ekle |

**inspection_result validasyonu:** Sayısal string (`"17.65"`, `"254.005/254.003"`) veya `"Unidentified"`, `"Conform"`, `"Not Conform"`, `"Pass"`, `"Fail"`, `"Passed"`, `"Failed"` kabul edilir.

### Disposition

| Method | URL | Açıklama |
|---|---|---|
| POST | `/api/dispositions` | Defect'e disposition ekle |
| GET | `/api/dispositions?defect_id={id}` | Defect'in karar geçmişi |
| DELETE | `/api/dispositions/{id}` | Kaydı sil |
| GET | `/api/disposition-types` | Tüm karar tipleri |
| GET | `/api/disposition-transitions/allowed?current_code=REWORK` | İzin verilen sonraki kararlar |
| POST | `/api/disposition-transitions/bulk-set` | Geçiş kurallarını toplu güncelle |

### Hata Yanıtları

| HTTP Kodu | Durum |
|---|---|
| 400 | Validasyon hatası — `{ "detail": "açıklama" }` |
| 404 | Kaynak bulunamadı |
| 500 | Beklenmedik sunucu hatası (log'a bakın) |

---

## 7. Frontend Mimarisi

Frontend, herhangi bir build aracı gerektirmeyen **Vanilla JS ES Modules** tabanlıdır.

### Dosya Organizasyonu

```
static/js/
├── api.js          # Tüm HTTP çağrıları — merkezi API katmanı
├── app.js          # Hash-based router (#/inspections, #/inspection/5 vb.)
├── session.js      # JWT token saklama/okuma (localStorage)
├── camera.js       # Kamera erişimi (fotoğraf çekme)
├── annotator.js    # Fotoğraf üzerine çizim aracı
└── pages/
    ├── inspection-detail.js   # En karmaşık sayfa — dimensional panel
    ├── inspections.js
    ├── inspection-form.js
    ├── character-detail.js
    ├── irs-projects.js
    └── irs-project-detail.js
```

### api.js Yapısı

Her API grubu nesne olarak tanımlanmıştır:
```javascript
export const api = {
  inspections: { list, getById, create, update, delete, parseOpSheet, complete, getReport },
  characters:  { list, getById, update, listDispositions, addDisposition },
  dispositions: { create, list, delete },
  dispositionTypes: { list },
  dispositionTransitions: { list, getAllowed, bulkSet },
  numericResults: { list, create, delete },
  categoricalResults: { list, create, delete },
  // ...
};
```

### inspection-detail.js Önemli Fonksiyonlar

```
render(id, root)                   # Sayfayı yükle
loadDimensional()                  # Karakterleri yükle, sol panel + sağ doc viewer
_activateChar(index)               # Bir karakteri seç
_renderEntryPanel(c, index)        # Ölçüm giriş panelini çiz (LOT veya numeric)
_saveNumeric(c, rawVal)            # Ölçümü kaydet → API
_saveLot(c, resultStr)             # LOT sonucunu kaydet → API
_openDetailModal(c)                # Detay modal → disposition geçmişi
_openQuickDecisionPanel(c, el)     # 🎯 Karar butonu → inline disposition panel
_loadDispTypes()                   # DispositionType[] cache
_loadDispTrans()                   # Transition matrix cache
allowedNextDecisions(currentCode)  # Mevcut koddan geçiş izni sorgusu
```

### Yeni Sayfa Eklemek

1. `static/js/pages/yeni-sayfa.js` oluştur, `export async function render(params, root)` tanımla
2. `app.js` router'ına route ekle:
   ```javascript
   '#/yeni-sayfa': () => import('./pages/yeni-sayfa.js'),
   ```
3. `index.html` içinde gerekirse sidebar link ekle

---

## 8. Yeni Özellik Ekleme

### 8.1 Yeni Entity Ekleme

**Örnek: `WorkOrder` entity eklemek**

1. **Core/Entities/WorkOrder.cs:**
   ```csharp
   public class WorkOrder : BaseEntity
   {
       public string OrderNo { get; set; } = "";
       public long InspectionId { get; set; }
       public Inspection? Inspection { get; set; }
   }
   ```

2. **Core/Repositories/IWorkOrderRepository.cs** — interface tanımla

3. **Data/Configurations/WorkOrderConfiguration.cs** — Fluent API config

4. **IRSGeneratorDbContext.cs** — `DbSet<WorkOrder> WorkOrders` ekle

5. **Migration oluştur:**
   ```bash
   dotnet ef migrations add AddWorkOrder --project IRSGenerator.Data --startup-project IRSGenerator.API
   dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API
   ```

6. **Data/Repositories/WorkOrderRepository.cs** — implementasyon

7. **Shared/Dtos/WorkOrder/** — CreateDto, ReadDto

8. **API/Controllers/WorkOrdersController.cs** — controller

9. **Program.cs** — DI kaydı:
   ```csharp
   builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
   ```

### 8.2 OlcuParser'a Yeni Format Eklemek

1. `OlcuParser.cs` içinde yeni sınıf yaz (`OlcuFormati`'yi miras al)
2. `OlcuYakalayici` constructor'ında doğru pozisyona ekle (öncelik sırasına dikkat et)
3. `LimitCatcherService.CatchMeasurement()` içinde gerekirse özel case ekle

### 8.3 Disposition State Machine'e Yeni Geçiş Eklemek

Swagger veya doğrudan DB üzerinden:
```sql
INSERT INTO "DispositionTransitions" ("FromCode", "ToCode", "CreatedAt", "UpdatedAt")
VALUES ('REWORK', 'YENI_KARAR', NOW(), NOW());
```

Ya da `/api/disposition-transitions/bulk-set` endpoint'ini kullan.

---

## 9. Migration Yönetimi

```bash
# Yeni migration ekle
dotnet ef migrations add AciklamaciIsim \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Veritabanına uygula
dotnet ef database update \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Son migration'ı geri al (geliştirme sırasında)
dotnet ef migrations remove \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Belirli bir migration'a geri dön
dotnet ef database update HedefMigrationIsmi \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# SQL script üret (prod deployment için)
dotnet ef migrations script \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API \
  --output ./migration.sql
```

> **Prod için:** Doğrudan `dotnet ef database update` yerine üretilen SQL scriptini DBA'ya verin.

---

## 10. Test Etme

### Manuel API Testi — Swagger

```
http://localhost:5297/swagger/index.html
```

### Manuel API Testi — curl

```bash
# Muayene oluştur
curl -X POST http://localhost:5297/api/inspections \
  -H "Content-Type: application/json" \
  -d '{"part_number":"TEST001","serial_number":"SN001","operation_number":"OP-10","inspector":"Admin","status":"open"}'

# Op-sheet parse et
curl -X POST http://localhost:5297/api/inspections/1/parse-op-sheet \
  -F "file=@/path/to/opsheet.docx"

# Karakterleri listele
curl http://localhost:5297/api/characters?inspection_id=1

# Ölçüm gir
curl -X POST http://localhost:5297/api/numeric-part-results \
  -H "Content-Type: application/json" \
  -d '{"character_id":1,"actual":"17.65","part_label":"P1"}'
```

### OlcuParser Test Senaryoları

```csharp
var parser = new OlcuYakalayici();

// Eşit tolerans
parser.Isle("254 ± 0.01")          // lower=253.99, upper=254.01

// Artı-eksi (unilateral)
parser.Isle("17.75 +0 / -0.15")   // lower=17.60, upper=17.75

// Diş (limit olmamalı)
parser.Isle("1/8 NPT")            // Format="diş", AltLimit=null, UstLimit=null
parser.Isle("M8 x 1.25")         // Format="diş"

// Surface profile
parser.Isle("[SP | 0.15]")        // lower=0, upper=0.15

// Minimum
parser.Isle("0.5 MIN")            // lower=0.5, upper=double.MaxValue
```

---

## 11. Yaygın Bakım Görevleri

### 11.1 Yeni Disposition Tipi Eklemek

```sql
INSERT INTO "DispositionTypes"
  ("Code", "Label", "CssClass", "IsNeutralizing", "IsInitial", "SortOrder", "Active", "CreatedAt", "UpdatedAt")
VALUES
  ('YENİ_KOD', 'Türkçe Etiket', 'disp-custom', false, true, 99, true, NOW(), NOW());

-- İzin verilen geçişleri ekle
INSERT INTO "DispositionTransitions" ("FromCode", "ToCode", "CreatedAt", "UpdatedAt")
VALUES ('REWORK', 'YENİ_KOD', NOW(), NOW());
```

### 11.2 Seeded Verileri Güncellemek

`Program.cs` içinde `SeedDataAsync()` metodu bulunur. Seed verisini değiştirip migration yerine doğrudan çalıştırabilirsiniz:
```bash
dotnet run --project IRSGenerator.API -- --seed-only
```
*(Bu flag mevcut değilse, SeedDataAsync içinde idempotent kontrol ekleyerek her başlatmada çalışır.)*

### 11.3 Log İnceleme

Varsayılan log seviyesi `Information`. EF Core sorgu loglarını görmek için `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 11.4 Veritabanı Bakımı

```sql
-- Tamamlanmış eski inspection'ları arşivle (örnek)
SELECT id, part_number, serial_number, created_at
FROM "Inspections"
WHERE status = 'completed' AND created_at < NOW() - INTERVAL '1 year';

-- NumericPartResult boyutu izle
SELECT COUNT(*), pg_size_pretty(pg_total_relation_size('"NumericPartResults"'))
FROM "NumericPartResults";
```

---

## 12. Bilinen Kısıtlamalar ve Gelecek Geliştirmeler

### Mevcut Kısıtlamalar

| Alan | Kısıtlama |
|---|---|
| Auth | JWT token refresh yok — her 1 saatte (varsayılan) yeniden giriş gerekebilir |
| Op-sheet parser | Sadece `.docx` desteklenir, `.doc` veya `.xls` desteklenmez |
| Fotoğraf | Boyut sınırı ayarlanmamış, büyük yüklemeler disk doldurabilir |
| Real-time | WebSocket yok — aynı muayeneyi düzenleyen iki kişi çakışabilir |
| Raporlama | Yalnızca Word formatı — Excel veya PDF formatı yok |
| `appsettings.json` | DB şifresi plain text — production'da env variable kullanılmalı |

### Önerilen Geliştirmeler

- [ ] Excel raporu export (`ClosedXML` veya `EPPlus` ile)
- [ ] Gerçek zamanlı bildirim (SignalR) — aynı inspection'da çalışan kullanıcılar için
- [ ] Fotoğraf boyutu limiti ve otomatik küçültme
- [ ] JWT refresh token mekanizması
- [ ] Role tabanlı yetkilendirme (`[Authorize(Roles = "inspector")]`)
- [ ] Audit log — kimin ne zaman ne değiştirdiği
- [ ] `appsettings.Example.json` şablonu + `appsettings.json` gitignore'a alma
- [ ] Integration testleri (EF Core InMemory veya Testcontainers ile)
- [ ] Op-sheet önizleme için PDF dönüşümü
