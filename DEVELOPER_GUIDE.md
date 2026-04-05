# QualiSight — Geliştirici Kılavuzu

Bu belge, QualiSight projesini geliştiren veya bakımını yapan yazılımcılar içindir.

---

## İçindekiler

1. [Proje Mimarisi](#1-proje-mimarisi)
2. [Çözüm Yapısı](#2-çözüm-yapısı)
3. [Geliştirme Ortamı Kurulumu](#3-geliştirme-ortamı-kurulumu)
4. [Kimlik Doğrulama & Yetkilendirme](#4-kimlik-doğrulama--yetkilendirme)
5. [Veritabanı Şeması](#5-veritabanı-şeması)
6. [Temel Servisler](#6-temel-servisler)
7. [API Endpoint Referansı](#7-api-endpoint-referansı)
8. [Frontend Mimarisi](#8-frontend-mimarisi)
9. [Yeni Özellik Ekleme](#9-yeni-özellik-ekleme)
10. [Migration Yönetimi](#10-migration-yönetimi)
11. [Test Etme](#11-test-etme)
12. [Yaygın Bakım Görevleri](#12-yaygın-bakım-görevleri)
13. [Bilinen Kısıtlamalar ve Önerilen Geliştirmeler](#13-bilinen-kısıtlamalar-ve-önerilen-geliştirmeler)

---

## 1. Proje Mimarisi

Proje klasik **N-Katmanlı (N-Tier) Clean Architecture** yaklaşımıyla tasarlanmıştır:

```
┌─────────────────────────────────────────────────────┐
│  IRSGenerator.API  (Presentation Layer)             │
│  ├── Controllers/   ASP.NET Core Web API (20 ctrl)  │
│  ├── Extensions/    ServiceExtensions.cs (DI + Auth)│
│  ├── DispositionTemplates/  Word şablonları (.docx) │
│  ├── wwwroot/       Statik SPA (HTML/JS/CSS)        │
│  └── Program.cs     Uygulama başlangıcı, middleware  │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Core  (Domain Layer)                  │
│  ├── Entities/      24 domain modeli                │
│  ├── Repositories/  21 interface tanımı             │
│  └── Services/      5 iş mantığı servisi            │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Data  (Infrastructure Layer)          │
│  ├── Repositories/  21 EF Core implementasyonu      │
│  ├── Migrations/    8 migration dosyası             │
│  └── IRSGeneratorDbContext.cs                       │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Shared  (DTO Layer)                   │
│  └── Dtos/          Request/Response nesneleri      │
├─────────────────────────────────────────────────────┤
│  IRSGenerator.Tests  (Test Layer)                   │
│  └── Services/      xUnit servis testleri           │
└─────────────────────────────────────────────────────┘
```

**Teknoloji Yığını:**

| Katman | Teknoloji |
|---|---|
| Backend framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Veritabanı | PostgreSQL 14+ |
| Word işleme | DocumentFormat.OpenXml 3.* (Open XML SDK) |
| JSON serileştirme | System.Text.Json — `SnakeCaseLower` policy |
| Frontend | Vanilla JS ES Modules, HTML5, CSS3 (build aracı yok) |
| Kimlik doğrulama | Cookie tabanlı (`qc_auth` HttpOnly cookie) |
| Şifre hash | SHA256 hex (salt yok) |
| Swagger | Swashbuckle.AspNetCore 10.1.7 |

---

## 2. Çözüm Yapısı

```
QualiSight/
├── IRSGenerator.sln
│
├── IRSGenerator.API/
│   ├── Controllers/
│   │   ├── AuthController.cs                   # Giriş/çıkış, oturum bilgisi
│   │   ├── ProjectsController.cs               # VisualProject CRUD
│   │   ├── InspectionsController.cs            # Muayene CRUD + NCM verisi
│   │   ├── CharactersController.cs             # Muayene kalemi + disposition
│   │   ├── NumericPartResultsController.cs     # Sayısal ölçüm sonuçları
│   │   ├── CategoricalPartResultsController.cs # Kategorik sonuçlar
│   │   ├── CategoricalZoneResultsController.cs # Bölge kategorik sonuçları
│   │   ├── DefectsController.cs                # Görsel kusurlar
│   │   ├── DefectTypesController.cs            # Kusur tipi yönetimi
│   │   ├── DefectFieldsController.cs           # Kusur özel alanları
│   │   ├── DispositionsController.cs           # Karar kayıtları
│   │   ├── DispositionTypesController.cs       # Karar tipi yönetimi
│   │   ├── DispositionTransitionsController.cs # State machine geçişleri
│   │   ├── PhotosController.cs                 # Fotoğraf yükleme/listeleme
│   │   ├── VisualSystemConfigController.cs     # Sistem konfigürasyonu
│   │   ├── UsersController.cs                  # Kullanıcı yönetimi [AdminOnly]
│   │   ├── NcmController.cs                    # NCM disposition sheet üretme
│   │   ├── CauseCodesController.cs             # Kök neden kodları
│   │   └── NcmDispositionTypesController.cs    # NCM disposition tipleri
│   │
│   ├── Extensions/
│   │   └── ServiceExtensions.cs                # DI kayıtları + Auth politikaları
│   │
│   ├── DispositionTemplates/                   # NCM Word şablonları (.docx)
│   │   ├── ACCEPT.docx
│   │   ├── CTP&MRB.docx
│   │   ├── CTP&R-I.docx
│   │   ├── CTP&R-W.docx
│   │   ├── DEBURR R-W.docx
│   │   ├── EMPTY.docx
│   │   ├── MRB.docx
│   │   ├── RETURN-TO-VENDOR.docx
│   │   ├── SCRAP-IND.docx
│   │   ├── SCRAP-LOT.docx
│   │   ├── STD-OP-R-W.docx
│   │   └── WELD R-W.docx
│   │
│   ├── wwwroot/
│   │   ├── index.html                          # SPA giriş noktası
│   │   ├── admin.html                          # Admin SPA
│   │   └── static/
│   │       ├── css/style.css                   # Karanlık/aydınlık tema
│   │       └── js/
│   │           ├── api.js                      # Merkezi API istek katmanı
│   │           ├── app.js                      # Hash router + layout
│   │           ├── admin.js                    # Admin SPA router
│   │           └── pages/                      # 22 sayfa modülü
│   │
│   ├── appsettings.json
│   ├── Program.cs
│   └── IRSGenerator.API.csproj
│
├── IRSGenerator.Core/
│   ├── Entities/                               # 24 entity dosyası
│   ├── Repositories/                           # 21 interface
│   ├── Services/
│   │   ├── OlcuParser.cs                       # Dimension string parser
│   │   ├── LimitCatcher.cs                     # Parser wrapper → double[2]
│   │   ├── WordOpSheetParser.cs                # .docx → Character[]
│   │   ├── WordReportWriter.cs                 # Inspection → Word raporu
│   │   └── NcmSheetGenerator.cs               # NCM Word şablon doldurma
│   └── IRSGenerator.Core.csproj
│
├── IRSGenerator.Data/
│   ├── IRSGeneratorDbContext.cs                # EF Core DbContext + seed data
│   ├── Repositories/                           # 21 EF Core implementasyonu
│   ├── Migrations/                             # 8 migration
│   └── IRSGenerator.Data.csproj
│
├── IRSGenerator.Shared/
│   ├── Dtos/                                   # 19 klasör, 50+ DTO sınıfı
│   └── IRSGenerator.Shared.csproj
│
└── IRSGenerator.Tests/
    ├── Services/                               # OlcuParser vb. testleri
    └── IRSGenerator.Tests.csproj
```

---

## 3. Geliştirme Ortamı Kurulumu

### 3.1 Gereksinimler

- .NET SDK 8.0+
- PostgreSQL 14+
- Visual Studio 2022 / JetBrains Rider / VS Code (C# DevKit eklentisiyle)
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
CREATE USER irsapp WITH ENCRYPTED PASSWORD 'sifreniz';
GRANT ALL PRIVILEGES ON DATABASE "IRSGeneratorDb" TO irsapp;
```

### 3.4 Connection String Ayarla

`IRSGenerator.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PosgresConnection": "Host=localhost;Port=5432;Database=IRSGeneratorDb;Username=postgres;Password=postgres"
  },
  "NcmSettings": {
    "TemplatesPath": ""
  }
}
```

> `NcmSettings.TemplatesPath` boş bırakılırsa `DispositionTemplates/` klasörü otomatik kullanılır.

### 3.5 Migration Uygula

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API
```

### 3.6 Uygulamayı Çalıştır

```bash
dotnet run --project IRSGenerator.API
# → http://localhost:5297          (QualiSight UI)
# → http://localhost:5297/admin.html  (Admin paneli)
# → http://localhost:5297/swagger  (API dökümantasyonu)
```

### 3.7 Hot Reload (Geliştirme)

```bash
dotnet watch run --project IRSGenerator.API
```

---

## 4. Kimlik Doğrulama & Yetkilendirme

### 4.1 Cookie Tabanlı Auth

Uygulama **JWT kullanmaz**. Kimlik doğrulama `HttpOnly` cookie ile yönetilir.

| Özellik | Değer |
|---|---|
| Cookie adı | `qc_auth` |
| Cookie tipi | HttpOnly, SameSite=Strict |
| Şema adı | `AppCookie` |
| Şifre hash | SHA256 hex (salt yok) |
| Oturum süresi | Tarayıcı kapanmasına kadar (kalıcı değil) |

**Login akışı:**
```
POST /api/auth/login  { "sicil": "6518", "password": "****" }
  → Cookie set edilir
  → 200 { "id": ..., "display_name": "...", "role": "admin", "is_admin": true }
```

**Çıkış:**
```
POST /api/auth/logout  → Cookie silinir
```

**Mevcut oturum:**
```
GET /api/auth/me  → { "id": ..., "display_name": "...", "role": "..." }
```

### 4.2 Yetkilendirme Politikaları

`IRSGenerator.API/Extensions/ServiceExtensions.cs` içinde tanımlıdır:

| Politika | Tanım | Hangi roller |
|---|---|---|
| `AdminOnly` | Yalnızca admin | `admin` |
| `CanWrite` | Veri yazabilir | `engineer`, `admin` (inspector hariç) |
| `CanWriteNcm` | NCM sheet üretebilir | `engineer`, `admin` |

### 4.3 Kullanıcı Rolleri

| Rol | Erişim Seviyesi |
|---|---|
| `inspector` | Sadece okuma (GET endpoint'leri) |
| `engineer` | Veri yazma + NCM sheet üretme |
| `admin` | Tam erişim + kullanıcı/sistem yönetimi |

---

## 5. Veritabanı Şeması

### Temel Entity İlişkileri

```
IRSProject (şablon)
  └── Character[] ─── irs_project_id

Inspection (muayene örneği)
  ├── IrsProjectId (nullable — şablon bağlantısı)
  ├── VisualProjectId (nullable — görsel proje bağlantısı)
  ├── Character[]
  │     ├── NumericPartResult[]    (sayısal ölçümler)
  │     ├── CategoricalPartResult[] (kategorik sonuçlar)
  │     ├── CategoricalZoneResult[] (bölge sonuçları)
  │     └── Disposition[]          character_id üzerinden (dimensional karar)
  ├── Defect[]
  │     ├── Disposition[]          defect_id üzerinden (görsel karar)
  │     └── PhotoDefect[] ─── Photo[]
  └── Photo[]

DispositionType (code, label, cssClass, isNeutralizing, isInitial)
DispositionTransition (from_code → to_code) — state machine

CauseCode (NCM kök neden)
NcmDispositionType (NCM karar tipi + Word şablon dosyası)
```

### Kritik Alan Notları

**Character:**
- `badge` — `null` = boyutsal (DIM), `"LOT"` = kategorik
- `inspection_result` — başlangıç: `"Unidentified"`; ölçüm sonrası: sayısal string veya `"Conform"/"Not Conform"`
- `lower_limit`, `upper_limit` — `OlcuParser` tarafından `dimension` stringinden otomatik çıkarılır
- `inspection_id` — nullable; şablon karakterleri `null`

**NumericPartResult:**
- `actual` — ham giriş: `"17.65"` veya çoklu `"254.005/254.003/253.998"`
- `part_label` — çok parçalı ölçümde `"P1"`, `"P2"`; rework tarihçesi `"PRE_REWORK"`, `"POST_REWORK"`
- `update_reason` — değer güncellendiğinde zorunlu açıklama
- `update_note` — ek not

**CategoricalPartResult:**
- `update_reason` — değer güncellendiğinde açıklama

**Disposition:**
- `decision` — `DispositionType.code` değerlerinden biri (string referans, FK değil)
- `defect_id` veya `character_id` — ikisinden biri `null` (görsel vs. dimensional karar)

**VisualSystemConfig (key-value):**
- `PhotoRootFolder` — fotoğrafların saklandığı kök klasör
- `ReportRootFolder` — rapor çıktı klasörü
- `BackupRootFolder` — yedek klasörü

### Seed Data

`IRSGeneratorDbContext.OnModelCreating()` içinde:

- **Kullanıcılar:** Employee 6518 (Erdem.Demirtaş), Employee 5956 (Uras.Erken)
- **13 DispositionType:** USE_AS_IS, KABUL_RESIM, CONFORMS, REWORK, RE_INSPECT, CTP_RE_INSPECT, MRB_SUBMITTED, MRB_CTP, MRB_ACCEPTED, MRB_REJECTED, VOID, REPAIR, SCRAP
- **DispositionTransition'lar:** State machine geçiş kuralları (REWORK/RE_INSPECT/MRB akışları)

---

## 6. Temel Servisler

### 6.1 OlcuParser (`IRSGenerator.Core/Services/OlcuParser.cs`)

Dimension stringlerini parse ederek alt/üst limit değerlerini çıkarır.

**Desteklenen 15+ format (öncelik sırasıyla):**

```
OlcuYakalayici.Isle(string) → OlcuSonucu?
  ├── IplikToleransi       "1/8 NPT", "M8", "M10x1.5", "UNC" → Format="diş"
  ├── FormToleransi         "FLATNESS 0.02", "CYLINDRICITY 0.01"
  ├── OryantasyonToleransi  "PERPENDICULARITY 0.01 | A"
  ├── LokasyonToleransi     "TRUE POSITION ∅0.05 | A | B"
  ├── ProfilToleransi       "SP | 0.15"
  ├── RunoutToleransi       "RUNOUT 0.03 | A-B"
  ├── SembolTolerans        "⊥ 0.01", "○ 0.02"
  ├── EsitToleransliOlcu    "25.4 ± 0.1"  → lower=25.3, upper=25.5
  ├── ArtiEksiOlcu          "17.75 +0 / -0.15" → lower=17.60, upper=17.75
  ├── ArtiEksiOlcu2         "17.75 +0.1 -0.15"
  ├── MaxMinOlcu            "MIN 5 / MAX 10" → lower=5, upper=10
  ├── MinOlcu               "0.5 MIN" → lower=0.5, upper=MaxValue
  ├── MaxOlcu               "6.3 MAX" → lower=0, upper=6.3
  ├── PuruzlulukOlcu        "Ra 1.6", "6.3 RA"
  └── LimitToleransliOlcu   "17.60 / 17.75"  ← EN SONA konuldu (öncelik sorunu önlenir)
```

> **Kritik:** Format listesindeki sıra önemlidir. `EsitToleransliOlcu` ve `ArtiEksiOlcu`,
> `LimitToleransliOlcu`'dan önce gelmek zorundadır.

**Yeni format eklemek:**

```csharp
public class YeniFormat : OlcuFormati
{
    public override bool Eslestir(string olcu) { /* regex match */ return false; }
    public override OlcuSonucu Degerler() { /* return new OlcuSonucu(...) */ }
}

// OlcuYakalayici constructor'ında doğru pozisyona ekle:
_formatTipleri = new List<OlcuFormati> { ..., new YeniFormat(), ... };
```

### 6.2 LimitCatcher (`IRSGenerator.Core/Services/LimitCatcher.cs`)

`OlcuParser` çıktısını `double[2] = [lowerLimit, upperLimit]` formatına normalize eder.

| Format türü | Lower | Upper |
|---|---|---|
| Geometrik, yüzey kalitesi | `0` | tolerans değeri |
| Minimum | `altLimit` | `double.MaxValue` |
| Maksimum | `0` | üst limit |
| Her ikisi mevcut | `altLimit` | `üstLimit` |
| Diş (`"diş"`) | — | `[]` boş array döner |

> `double.MaxValue`, frontend `formatLimits()` fonksiyonunda `"≥ X"` olarak gösterilir.

`CharactersController` bu servisi çağırarak `dimension` stringinden `LowerLimit` ve `UpperLimit`'i otomatik doldurur.

### 6.3 WordOpSheetParser (`IRSGenerator.Core/Services/WordOpSheetParser.cs`)

Yüklenen `.docx` op-sheet dosyasından `Character[]` üretir.

**Algoritma:**
1. Tüm tablolarda `"ITEM NO"` + `"DIMENSION"` içeren header satırını bul
2. Header'dan kolon indekslerini tespit et
3. Her veri satırı için:
   - Skip: `"ITEM NO KC"`, `"INSPECTION"`, `"PAGE NO"` vb. keyword içeriyorsa atla
   - `Regex.Replace(itemNo, @"\s+", "")` — NBSP dahil tüm whitespace temizlenir
   - `IsLotDimension()` — `"VISUAL"`, `"CHECK"`, `"MARKING"` içeriyorsa `badge="LOT"` atar
   - `LimitCatcher.CatchMeasurement(dimension)` çağırarak limitleri doldurur

**Skip keyword listesi** (`SkipItemSuffixes` dizisi):
```csharp
["ITEM NO KC", "ITEM NO", "RECORD", "INSPECTION", "INSPECTOR", "/0", "**", "PAGE NO"]
```

**LOT keyword listesi** (`LotKeywords` dizisi):
```csharp
["VISUAL", "CHECK", "MARKING", "SURFACE", "COATING"]
```

### 6.4 WordReportWriter (`IRSGenerator.Core/Services/WordReportWriter.cs`)

Muayene verilerinden Word raporu üretir.

**Rapor yapısı:**
1. Op-sheet klonu — orijinal op-sheet'e ACTUAL değerleri yazılır
2. Dimensional Results tablosu — tüm karakterler + son ölçüm
3. Non-Conformances bölümü — yalnızca `"Not Conform"` karakterler + aktif disposition

**Rework tarihçesi:** Aynı `part_label` için birden fazla sonuç varsa en son kayıt gösterilir:
```csharp
.GroupBy(r => r.PartLabel)
.Select(g => g.OrderByDescending(r => r.CreatedAt ?? DateTime.MinValue).First().Actual)
```

### 6.5 NcmSheetGenerator (`IRSGenerator.Core/Services/NcmSheetGenerator.cs`)

NCM disposition sheet'leri için Word şablonlarını doldurur.

**Yöntemi:**
- `DispositionTemplates/` klasöründen ilgili `.docx` şablonu yükler (in-memory)
- Şablon içindeki placeholder'ları değiştirir:

| Placeholder | Karşılık |
|---|---|
| `[SERIAL NUMBER]` | Parça seri numarası |
| `[OPER]` | Operasyon numarası |
| `[C-OP]` | Kök neden operasyonu |
| `[QTY]` | Etkilenen adet |
| `[C.CODE]` | Kök neden kodu |
| `[NONCONFROMANCE PLACE HOLDER]` | NC açıklaması (21 slot: 3 kolon × 7 satır) |

- Tek sheet: `.docx` döner
- Birden fazla sheet: `.zip` içinde birden fazla `.docx` döner
- Tamamen in-memory — diske yazılmaz

**Servis kaydı:** `Program.cs` içinde **singleton** olarak kayıtlıdır.

---

## 7. API Endpoint Referansı

Tüm endpoint'ler `http://localhost:5297/swagger` adresinden interaktif test edilebilir.

### JSON Formatı

Tüm istek ve yanıtlar **snake_case** kullanır:

```json
{ "part_number": "ABC123", "serial_number": "SN001" }  // ✅ Doğru
{ "partNumber": "ABC123", "serialNumber": "SN001" }     // ❌ Yanlış
```

### Auth (`/api/auth`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| POST | `/login` | Giriş — cookie set edilir | Herkese açık |
| POST | `/logout` | Çıkış — cookie silinir | Herkes |
| GET | `/me` | Aktif oturum bilgisi | Herkes |

### Projeler (`/api/projects`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/` | Liste (`?all=true` ile pasifler dahil) | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST | `/` | Oluştur | AdminOnly |
| PUT | `/{id}` | Güncelle | AdminOnly |
| DELETE | `/{id}` | Soft delete (active=false) | AdminOnly |

### Muayeneler (`/api/inspections`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/` | Liste | Herkes |
| GET | `/{id}` | Detay (defects + dispositions) | Herkes |
| POST | `/` | Oluştur | CanWrite |
| PUT | `/{id}` | Güncelle | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |
| GET | `/{id}/ncm` | Bu muayeneye ait NCM verisi | Herkes |

### Karakterler (`/api/characters`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?inspection_id={id}` | Muayene karakterleri | Herkes |
| GET | `/?irs_project_id={id}` | Proje şablon karakterleri | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST | `/` | Oluştur | CanWrite |
| PUT | `/{id}` | Güncelle | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |
| GET | `/{id}/dispositions` | Karar geçmişi | Herkes |
| POST | `/{id}/dispositions` | Yeni karar ekle | CanWrite |

**`inspection_result` geçerli değerleri:**
Sayısal string (`"17.65"`, `"254.005/254.003"`), `"Unidentified"`, `"Conform"`, `"Not Conform"`, `"Pass"`, `"Fail"`, `"Passed"`, `"Failed"`

### Sayısal Sonuçlar (`/api/numeric-part-results`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?character_id={id}` | Liste | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST | `/` | Oluştur | CanWrite |
| PUT | `/{id}` | Güncelle (`update_reason` zorunlu) | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |
| DELETE | `/?character_id={id}` | Toplu sil | CanWrite |

### Kategorik & Bölge Sonuçları

`/api/categorical-part-results` ve `/api/categorical-zone-results` — aynı pattern.

### Kusurlar (`/api/defects`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?inspection_id={id}` | Liste | Herkes |
| GET | `/{id}` | Detay (child defects + dispositions) | Herkes |
| POST | `/` | Oluştur | CanWrite |
| PUT/PATCH | `/{id}` | Güncelle | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |

### Dispositionlar (`/api/dispositions`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?defect_id={id}` | Defect kararları | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST | `/` | Oluştur | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |

### Disposition Tipleri (`/api/disposition-types`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/` | Tüm tipler | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST | `/` | Oluştur | AdminOnly |
| PUT | `/{id}` | Güncelle | AdminOnly |
| DELETE | `/{id}` | Sil | AdminOnly |

### Disposition Geçişleri (`/api/disposition-transitions`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/` | Tüm geçişler (`?from_code=` ile filtre) | Herkes |
| GET | `/allowed?current_code={kod}` | İzin verilen sonraki kodlar | Herkes |
| POST | `/bulk-set` | Bir kod için tüm geçişleri toplu güncelle | AdminOnly |

### NCM (`/api/ncm`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| POST | `/generate` | Disposition sheet üret (.docx veya .zip) | CanWriteNcm |
| GET | `/templates` | Mevcut şablon dosyaları | AdminOnly |
| POST | `/templates/{fileName}` | Yeni şablon yükle | AdminOnly |

**`/generate` istek body (`GenerateDispositionSheetDto`):**
```json
{
  "inspection_id": 1,
  "oper": "OP-10",
  "cause_oper": "OP-05",
  "qty": 1,
  "cause_code_id": 3,
  "disposition_type_id": 2,
  "items": [
    { "description": "Dim. 7 — Ölçü: 17.80 (Max: 17.75)", "is_dimensional": true },
    { "description": "Yüzey çizigi — Photo #3", "is_dimensional": false }
  ]
}
```

### Kök Neden Kodları (`/api/cause-codes`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?active_only=true` | Liste | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST, PUT, DELETE | `/` | Yönetim | AdminOnly |

### NCM Disposition Tipleri (`/api/ncm-disposition-types`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?active_only=true` | Liste | Herkes |
| GET | `/{id}` | Detay | Herkes |
| POST, PUT, DELETE | `/` | Yönetim | AdminOnly |

### Fotoğraflar (`/api/photos`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/?inspection_id={id}` | Muayene fotoğrafları | Herkes |
| GET | `/?defect_id={id}` | Kusur fotoğrafları | Herkes |
| GET | `/{id}/file` | Dosyaya yönlendirme | Herkes |
| POST | `/` | Yükle (multipart/form-data) | CanWrite |
| PUT | `/{id}/defects` | Kusur eşlemesi güncelle | CanWrite |
| DELETE | `/{id}` | Sil | CanWrite |

### Kullanıcılar (`/api/users`) — AdminOnly

GET `/`, GET `/{id}`, POST `/`, PUT `/{id}`, DELETE `/{id}` (soft delete — active=false)

### Sistem Konfigürasyonu (`/api/system-config`)

| Method | URL | Açıklama | Yetki |
|---|---|---|---|
| GET | `/` | Tüm konfigürasyon | Herkes |
| GET | `/{key}` | Tek değer | Herkes |
| PUT | `/` | Güncelle | AdminOnly |

---

## 8. Frontend Mimarisi

Herhangi bir build aracı gerektirmeyen **Vanilla JS ES Modules** tabanlıdır.

### Dosya Organizasyonu

```
static/js/
├── api.js             # Tüm HTTP çağrıları — merkezi API katmanı
├── app.js             # Hash-based router (#/inspections, #/inspection/5)
├── admin.js           # Admin SPA router
└── pages/
    ├── dashboard.js
    ├── inspections.js
    ├── inspection-form.js
    ├── inspection-detail.js   # En karmaşık — dimensional + defekt + disposition panelleri
    ├── settings.js
    ├── irs-projects.js
    ├── irs-project-detail.js
    ├── character-detail.js
    ├── ncm.js
    ├── ncm-detail.js          # NCM disposition sheet üretme UI
    ├── nonconformance-descriptions.js
    ├── analytics.js
    ├── admin-dashboard.js
    ├── admin-users.js
    ├── admin-projects.js
    ├── admin-defect-types.js
    ├── admin-defect-fields.js
    ├── admin-disposition-types.js
    ├── admin-ncm-disposition-types.js
    ├── admin-cause-codes.js
    ├── admin-config.js
    └── admin-backup.js
```

### Kimlik Doğrulama (Frontend)

JWT veya localStorage token **yoktur**. Tarayıcı `qc_auth` cookie'sini otomatik gönderir.
`api.js` içindeki tüm fetch çağrıları `credentials: 'include'` kullanır.

### api.js Yapısı

```javascript
export const api = {
  auth:                    { login, logout, me },
  projects:                { list, getById, create, update, delete },
  inspections:             { list, getById, create, update, delete, getNcm },
  characters:              { list, getById, create, update, delete, listDispositions, addDisposition },
  numericResults:          { list, getById, create, update, delete, deleteByCharacterId },
  categoricalResults:      { list, getById, create, update, delete, deleteByCharacterId },
  categoricalZoneResults:  { list, getById, create, update, delete, deleteByCharacterId },
  defects:                 { list, getById, create, update, delete },
  defectTypes:             { list, getById, create, update, delete },
  defectFields:            { list, getById, create, update, delete },
  dispositions:            { list, getById, create, delete },
  dispositionTypes:        { list, getById, create, update, delete },
  dispositionTransitions:  { list, getAllowed, bulkSet },
  photos:                  { list, getById, upload, updateDefects, delete },
  systemConfig:            { list, getByKey, update },
  users:                   { list, getById, create, update, delete },
  ncm:                     { generate, getTemplates, uploadTemplate },
  causeCodes:              { list, getById, create, update, delete },
  ncmDispositionTypes:     { list, getById, create, update, delete },
};
```

### inspection-detail.js Önemli Fonksiyonlar

```
render(id, root)                   # Sayfayı yükle
loadDimensional()                  # Sol panel karakter listesi + sağ giriş paneli
_activateChar(index)               # Karakter seç
_renderEntryPanel(c, index)        # LOT veya numeric giriş paneli
_saveNumeric(c, rawVal)            # Ölçümü kaydet → API
_saveLot(c, resultStr)             # LOT sonucunu kaydet → API
_openDetailModal(c)                # Disposition geçmişi modal
_openQuickDecisionPanel(c, el)     # Inline disposition panel
_loadDispTypes()                   # DispositionType[] cache
_loadDispTrans()                   # Transition matrix cache
allowedNextDecisions(currentCode)  # Geçiş izni sorgusu
```

### Yeni Sayfa Eklemek

1. `static/js/pages/yeni-sayfa.js` oluştur — `export async function render(params, root)` tanımla
2. `app.js` veya `admin.js` router'ına route ekle:
   ```javascript
   '#/yeni-sayfa': () => import('./pages/yeni-sayfa.js'),
   ```
3. Gerekirse `index.html` veya `admin.html` içine sidebar link ekle

---

## 9. Yeni Özellik Ekleme

### 9.1 Yeni Entity Ekleme — Adım Adım

**Örnek: `WorkOrder` entity eklemek**

```
1. Core/Entities/WorkOrder.cs               → entity sınıfı (BaseEntity miras al)
2. Core/Repositories/IWorkOrderRepository.cs → interface
3. Data/Repositories/WorkOrderRepository.cs  → EF Core implementasyon
4. Data/IRSGeneratorDbContext.cs             → DbSet<WorkOrder> WorkOrders ekle
5. Migration: dotnet ef migrations add AddWorkOrder --project ... --startup-project ...
6. dotnet ef database update ...
7. Shared/Dtos/WorkOrder/WorkOrderCreateDto.cs, WorkOrderReadDto.cs
8. API/Controllers/WorkOrdersController.cs
9. API/Extensions/ServiceExtensions.cs       → DI kaydı ekle
10. wwwroot/static/js/api.js                 → yeni API grubu
11. wwwroot/static/js/pages/work-orders.js  → sayfa modülü
12. wwwroot/static/js/app.js                → route ekle
```

### 9.2 OlcuParser'a Yeni Format Eklemek

```csharp
// Core/Services/OlcuParser.cs
public class YeniFormat : OlcuFormati
{
    private static readonly Regex _pattern = new Regex(@"...", RegexOptions.IgnoreCase);

    public override bool Eslestir(string olcu) => _pattern.IsMatch(olcu);

    public override OlcuSonucu Degerler()
    {
        var m = _pattern.Match(/* son eşleşen string */);
        return new OlcuSonucu
        {
            AltLimit = double.Parse(m.Groups[1].Value),
            UstLimit = double.Parse(m.Groups[2].Value),
            Format = "yeni"
        };
    }
}

// OlcuYakalayici constructor'ında uygun pozisyona ekle (öncelik sırasına dikkat!)
_formatTipleri = new List<OlcuFormati>
{
    new IplikToleransi(),
    // ...
    new YeniFormat(),      // ← doğru pozisyon
    // ...
    new LimitToleransliOlcu()  // ← her zaman en sonda
};
```

### 9.3 Yeni NCM Disposition Şablonu Eklemek

1. Word şablonunu `DispositionTemplates/` klasörüne kopyala
2. Şablon içinde placeholder'ları yerleştir: `[SERIAL NUMBER]`, `[OPER]`, `[C-OP]`, `[QTY]`, `[C.CODE]`, `[NONCONFROMANCE PLACE HOLDER]`
3. Admin panelden veya API üzerinden yeni `NcmDispositionType` kaydı ekle:
   ```http
   POST /api/ncm-disposition-types
   { "code": "YENİ_KOD", "label": "Etiket", "template_file_name": "YeniSablon.docx" }
   ```

### 9.4 Disposition State Machine'e Yeni Geçiş Eklemek

Kod değişikliği gerekmez — DB tabanlıdır:

```http
POST /api/disposition-transitions/bulk-set  [AdminOnly]
{
  "from_code": "REWORK",
  "to_codes": ["CONFORMS", "YENİ_KARAR", "MRB_SUBMITTED"]
}
```

### 9.5 Yeni Kullanıcı Rolü veya Politika Eklemek

```csharp
// API/Extensions/ServiceExtensions.cs
options.AddPolicy("YeniPolitika", policy =>
    policy.RequireClaim("role", "yeni_rol"));

// Controller'da kullan:
[Authorize(Policy = "YeniPolitika")]
```

---

## 10. Migration Yönetimi

```bash
# Yeni migration ekle
dotnet ef migrations add AciklamaciIsim \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Veritabanına uygula
dotnet ef database update \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Son migration'ı geri al (yalnızca geliştirme ortamında)
dotnet ef migrations remove \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Belirli bir migration'a geri dön
dotnet ef database update HedefMigrationIsmi \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API

# Prod için SQL script üret
dotnet ef migrations script \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API \
  --output ./migration.sql
```

> **Prod için:** Doğrudan `database update` yerine üretilen SQL scriptini DBA'ya verin.

---

## 11. Test Etme

### Manuel API Testi — Swagger

```
http://localhost:5297/swagger/index.html
```

### Manuel API Testi — curl

```bash
# Giriş (cookie alınır — -c ile kaydet)
curl -c cookies.txt -X POST http://localhost:5297/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"sicil":"6518","password":"sifre"}'

# Muayene oluştur
curl -b cookies.txt -X POST http://localhost:5297/api/inspections \
  -H "Content-Type: application/json" \
  -d '{"part_number":"TEST001","serial_number":"SN001","operation_number":"OP-10"}'

# Karakterleri listele
curl -b cookies.txt http://localhost:5297/api/characters?inspection_id=1

# NCM sheet üret
curl -b cookies.txt -X POST http://localhost:5297/api/ncm/generate \
  -H "Content-Type: application/json" \
  -d '{"inspection_id":1,"oper":"OP-10","cause_oper":"OP-05","qty":1,"cause_code_id":1,"disposition_type_id":1,"items":[{"description":"Test NC","is_dimensional":true}]}' \
  --output sheet.docx
```

### OlcuParser Test Senaryoları

```csharp
var parser = new OlcuYakalayici();

// Eşit tolerans
Assert.Equal(253.99, parser.Isle("254 ± 0.01").AltLimit);

// Artı-eksi
Assert.Equal(17.60, parser.Isle("17.75 +0 / -0.15").AltLimit);

// Diş — limit beklenmez
Assert.Equal("diş", parser.Isle("1/8 NPT").Format);

// Minimum
Assert.Equal(double.MaxValue, parser.Isle("0.5 MIN").UstLimit);
```

---

## 12. Yaygın Bakım Görevleri

### 12.1 Yeni Kullanıcı Eklemek

Admin paneli: `/admin.html` → Kullanıcılar → Yeni Kullanıcı

veya API:
```http
POST /api/users  [AdminOnly]
{ "employee_id": "12345", "display_name": "Ad Soyad", "role": "inspector", "password": "sifre" }
```

### 12.2 Yeni Disposition Tipi Eklemek

Admin paneli: `/admin.html` → Disposition Tipleri → Yeni

veya SQL:
```sql
INSERT INTO "DispositionTypes"
  ("Code", "Label", "CssClass", "IsNeutralizing", "IsInitial", "SortOrder", "Active", "CreatedAt", "UpdatedAt")
VALUES
  ('YENİ_KOD', 'Etiket', 'disp-custom', false, true, 99, true, NOW(), NOW());
```

Ardından geçiş kurallarını `/api/disposition-transitions/bulk-set` ile tanımlayın.

### 12.3 Log İnceleme

```json
// appsettings.json — EF Core sorgu loglarını görmek için:
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### 12.4 Veritabanı Bakımı

```sql
-- Tamamlanmış eski muayeneler
SELECT id, part_number, serial_number, created_at
FROM "Inspections"
WHERE status = 'completed' AND created_at < NOW() - INTERVAL '1 year';

-- NumericPartResult boyutu
SELECT COUNT(*), pg_size_pretty(pg_total_relation_size('"NumericPartResults"'))
FROM "NumericPartResults";
```

---

## 13. Bilinen Kısıtlamalar ve Önerilen Geliştirmeler

### Mevcut Kısıtlamalar

| Alan | Kısıtlama |
|---|---|
| Şifre hash | SHA256 hex, salt yok — production'da bcrypt/Argon2 kullanılmalı |
| Op-sheet parser | Yalnızca `.docx` desteklenir; `.doc` veya `.xls` desteklenmez |
| Fotoğraf boyutu | Sınır ayarlanmamış — büyük yüklemeler diski doldurabilir |
| Real-time | WebSocket yok — aynı muayenede çakışma riski |
| Raporlama | Yalnızca Word formatı — Excel veya PDF desteği yok |
| Connection string | `appsettings.json` içinde — production'da environment variable kullanılmalı |
| DTO mapping | AutoMapper yok — controller'larda manuel mapping |

### Önerilen Geliştirmeler

- [ ] Şifre hash'ini bcrypt veya Argon2 ile değiştir
- [ ] Excel raporu export (`ClosedXML` veya `EPPlus` ile)
- [ ] Fotoğraf boyutu limiti ve otomatik küçültme (önizleme thumbnail)
- [ ] Gerçek zamanlı bildirim (SignalR) — aynı inspection'da çalışan kullanıcılar için
- [ ] Op-sheet önizleme — PDF dönüşümü
- [ ] `appsettings.Example.json` şablonu oluştur + gerçek `appsettings.json`'ı `.gitignore`'a al
- [ ] Integration testleri (EF Core Testcontainers ile)
- [ ] Audit log — kimin ne zaman ne değiştirdiği (ayrı tablo)
- [ ] Fotoğraf depolama NAS/S3 entegrasyonu
