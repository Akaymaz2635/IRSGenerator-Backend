# QualiSight — IT Entegrasyon ve Kurulum Rehberi

Bu belge, QualiSight uygulamasını şirket ağına veya sunucuya kurulum ve yapılandırma adımlarını açıklamaktadır.

---

## İçindekiler

1. [Sistem Gereksinimleri](#1-sistem-gereksinimleri)
2. [Veritabanı Kurulumu](#2-veritabanı-kurulumu)
3. [Uygulama Kurulumu](#3-uygulama-kurulumu)
4. [Yapılandırma (appsettings.json)](#4-yapılandırma-appsettingsjson)
5. [NCM Disposition Şablonları](#5-ncm-disposition-şablonları)
6. [Çok Kullanıcılı Ağ Kurulumu](#6-çok-kullanıcılı-ağ-kurulumu)
7. [Servis Olarak Çalıştırma](#7-servis-olarak-çalıştırma)
8. [Dosya Depolama Yapılandırması](#8-dosya-depolama-yapılandırması)
9. [Yedekleme](#9-yedekleme)
10. [İlk Kullanıcı ve Rol Tanımları](#10-i̇lk-kullanıcı-ve-rol-tanımları)
11. [Sistem Konfigürasyonu (Admin Panel)](#11-sistem-konfigürasyonu-admin-panel)
12. [Güncelleme Prosedürü](#12-güncelleme-prosedürü)
13. [Sorun Giderme](#13-sorun-giderme)

---

## 1. Sistem Gereksinimleri

| Bileşen | Versiyon | Notlar |
|---|---|---|
| .NET Runtime | 8.0 veya üzeri | Production'da SDK değil Runtime yeterli |
| PostgreSQL | 14 veya üzeri | Mevcut PostgreSQL sunucusu kullanılabilir |
| İşletim Sistemi | Windows Server 2019+, Ubuntu 20.04+ | IIS veya Kestrel ile çalışır |
| RAM | Min. 2 GB | Üretimde 4 GB önerilir |
| Disk | Min. 10 GB | Op-sheet, fotoğraf ve NCM şablon dosyaları için |
| Tarayıcı (istemci) | Chrome, Firefox, Edge (güncel sürüm) | Mobil tarayıcı desteklenir |

---

## 2. Veritabanı Kurulumu

### 2.1 PostgreSQL'de Veritabanı Oluşturma

```sql
-- psql ile bağlan (postgres kullanıcısıyla)
CREATE DATABASE "IRSGeneratorDb";
CREATE USER irsapp WITH ENCRYPTED PASSWORD 'güçlü_şifre_buraya';
GRANT ALL PRIVILEGES ON DATABASE "IRSGeneratorDb" TO irsapp;

-- PostgreSQL 15+ için ek izin gerekebilir:
\c IRSGeneratorDb
GRANT ALL ON SCHEMA public TO irsapp;
```

### 2.2 İlk Migration (Tablo Oluşturma)

Kurulum sırasında aşağıdaki komut tüm 8 migration'ı sırasıyla uygular:

```bash
dotnet ef database update \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API
```

Bu komut şunları oluşturur:
- Tüm ana tablolar (Inspection, Character, Defect, Disposition, vb.)
- NCM tabloları (CauseCode, NcmDispositionType)
- Disposition state machine tabloları + seed data (13 disposition tipi)
- Başlangıç kullanıcı kayıtları

---

## 3. Uygulama Kurulumu

### 3.1 Kaynak Kodunu İndirin

```bash
git clone https://github.com/Akaymaz2635/IRSGenerator-Backend.git
cd IRSGenerator-Backend
```

### 3.2 Uygulamayı Derleyin

```bash
dotnet publish IRSGenerator.API -c Release -o ./publish
```

> **Not:** Derleme çıktısında `wwwroot/` klasörü ve `DispositionTemplates/` klasörü de bulunur.
> Bunların publish klasörüne kopyalandığından emin olun.

### 3.3 DispositionTemplates Klasörünü Kontrol Edin

```bash
ls ./publish/DispositionTemplates/
# Şu dosyalar bulunmalıdır:
# ACCEPT.docx, CTP&MRB.docx, CTP&R-I.docx, CTP&R-W.docx,
# DEBURR R-W.docx, EMPTY.docx, MRB.docx, RETURN-TO-VENDOR.docx,
# SCRAP-IND.docx, SCRAP-LOT.docx, STD-OP-R-W.docx, WELD R-W.docx
```

### 3.4 Uygulamayı Test Başlatın

```bash
cd publish
dotnet IRSGenerator.API.dll --urls "http://0.0.0.0:5297"
```

Tarayıcıdan şu adresleri kontrol edin:
- `http://localhost:5297` → QualiSight ana arayüzü (giriş ekranı)
- `http://localhost:5297/admin.html` → Admin paneli
- `http://localhost:5297/swagger` → API dökümantasyonu

---

## 4. Yapılandırma (appsettings.json)

`IRSGenerator.API/appsettings.json` dosyasını production değerleriyle güncelleyin:

```json
{
  "ConnectionStrings": {
    "PosgresConnection": "Host=<SUNUCU_IP>;Port=5432;Database=IRSGeneratorDb;Username=irsapp;Password=güçlü_şifre_buraya"
  },
  "NcmSettings": {
    "TemplatesPath": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Parametre Açıklamaları:**

| Parametre | Açıklama |
|---|---|
| `ConnectionStrings.PosgresConnection` | PostgreSQL bağlantı dizesi |
| `NcmSettings.TemplatesPath` | NCM Word şablon klasörü — boş bırakılırsa `DispositionTemplates/` otomatik kullanılır |

> **Güvenlik:** Production'da bağlantı dizesini environment variable olarak tanımlayın:
> ```bash
> export ConnectionStrings__PosgresConnection="Host=...;Password=..."
> ```
> Windows'ta: Sistem Özellikleri → Gelişmiş → Ortam Değişkenleri

---

## 5. NCM Disposition Şablonları

NCM modülü, disposition sheet üretmek için Word şablonlarına ihtiyaç duyar.

### Şablonların Konumu

Varsayılan konum: `IRSGenerator.API/DispositionTemplates/`

Publish sonrası: `publish/DispositionTemplates/`

### Mevcut 12 Şablon

| Dosya | Kullanım Durumu |
|---|---|
| `ACCEPT.docx` | Kabul kararı |
| `CTP&MRB.docx` | CTP ve MRB birleşik |
| `CTP&R-I.docx` | CTP ve Re-Inspect |
| `CTP&R-W.docx` | CTP ve Rework |
| `DEBURR R-W.docx` | Çapak alma + Rework |
| `EMPTY.docx` | Boş şablon |
| `MRB.docx` | Material Review Board |
| `RETURN-TO-VENDOR.docx` | Tedarikçiye iade |
| `SCRAP-IND.docx` | Bireysel hurdaya çıkarma |
| `SCRAP-LOT.docx` | Lot hurdaya çıkarma |
| `STD-OP-R-W.docx` | Standart operasyon + Rework |
| `WELD R-W.docx` | Kaynak + Rework |

### Yeni Şablon Ekleme (IT)

1. Word şablonunu hazırlayın (içinde placeholder'lar olmalı: `[SERIAL NUMBER]`, `[OPER]`, `[C-OP]`, `[QTY]`, `[C.CODE]`, `[NONCONFROMANCE PLACE HOLDER]`)
2. Dosyayı `DispositionTemplates/` klasörüne kopyalayın
3. Admin panelinden `NcmDispositionType` kaydı ekleyin — **Template File Name** alanına dosya adını yazın

> Alternatif: `/api/ncm/templates` endpoint'i üzerinden admin arayüzünden doğrudan yükleyebilirsiniz.

---

## 6. Çok Kullanıcılı Ağ Kurulumu

### Mimari

```
[Müfettiş 1 — Tablet/PC]  ─┐
[Müfettiş 2 — Tablet/PC]  ─┼──► [Uygulama Sunucusu :5297] ──► [PostgreSQL :5432]
[Mühendis   — Masaüstü]   ─┘
[Admin      — Masaüstü]   ─┘
```

Kullanıcılar **tarayıcıdan** `http://<SUNUCU_IP>:5297` adresine bağlanır. Ekstra kurulum gerekmez.

### Güvenlik Duvarı (Firewall)

```
TCP 5297  →  İntranet kullanıcılarına aç (uygulama portu)
TCP 5432  →  YALNIZCA uygulama sunucusundan PostgreSQL'e (dışarıya kapalı)
```

### Ters Proxy — IIS (Windows Server) — Önerilir

1. IIS'e "URL Rewrite" ve "Application Request Routing" modüllerini yükleyin.
2. Yeni site oluşturun → Binding: `http://*:80`.
3. `web.config` ters proxy kuralı:

```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="QualiSight Proxy" stopProcessing="true">
        <match url="(.*)" />
        <action type="Rewrite" url="http://localhost:5297/{R:1}" />
      </rule>
    </rules>
  </rewrite>
  <security>
    <requestFiltering>
      <requestLimits maxAllowedContentLength="52428800" /> <!-- 50 MB -->
    </requestFiltering>
  </security>
</system.webServer>
```

### Ters Proxy — Nginx (Linux)

```nginx
server {
    listen 80;
    server_name qualisight.sirket.local;

    location / {
        proxy_pass         http://localhost:5297;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_cache_bypass $http_upgrade;
    }

    client_max_body_size 50M;   # Op-sheet ve fotoğraf yükleme için
}
```

---

## 7. Servis Olarak Çalıştırma

### Windows — NSSM ile Windows Servisi

```bash
# NSSM: https://nssm.cc/
nssm install QualiSight "C:\publish\IRSGenerator.API.exe"
nssm set QualiSight AppParameters "--urls http://0.0.0.0:5297"
nssm set QualiSight AppDirectory "C:\publish"
nssm set QualiSight AppEnvironmentExtra "ASPNETCORE_ENVIRONMENT=Production"
nssm set QualiSight AppEnvironmentExtra "ConnectionStrings__PosgresConnection=Host=...;Password=..."
nssm start QualiSight
```

Durum kontrolü:
```bash
nssm status QualiSight
```

### Linux — systemd

`/etc/systemd/system/qualisight.service`:

```ini
[Unit]
Description=QualiSight API
After=network.target postgresql.service

[Service]
WorkingDirectory=/opt/qualisight
ExecStart=/usr/bin/dotnet /opt/qualisight/IRSGenerator.API.dll --urls http://0.0.0.0:5297
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__PosgresConnection=Host=localhost;Database=IRSGeneratorDb;Username=irsapp;Password=güçlü_şifre

[Install]
WantedBy=multi-user.target
```

```bash
systemctl daemon-reload
systemctl enable qualisight
systemctl start qualisight
systemctl status qualisight
```

---

## 8. Dosya Depolama Yapılandırması

Uygulama üç tür dosya kullanır:

| Tür | Varsayılan Konum | Yapılandırma |
|---|---|---|
| NCM Word şablonları | `DispositionTemplates/` (uygulama yanında) | `NcmSettings.TemplatesPath` (appsettings.json) |
| Muayene fotoğrafları | `VisualSystemConfig.PhotoRootFolder` | Admin paneli → Sistem Konfigürasyonu |
| IRS raporları | `VisualSystemConfig.ReportRootFolder` | Admin paneli → Sistem Konfigürasyonu |
| Yedekler | `VisualSystemConfig.BackupRootFolder` | Admin paneli → Sistem Konfigürasyonu |

**Tavsiye edilen disk ayrımı:**

| Klasör | Boyut Tahmini |
|---|---|
| `DispositionTemplates/` | Sabit ~5 MB (12 şablon) |
| Fotoğraflar | Muayene başına ~10-50 MB |
| Raporlar | Muayene başına ~500 KB |
| Yedekler | Haftalık artış, monitor edilmeli |

Bu klasörleri harici bir NAS veya ağ sürücüsüne yönlendirmek için sembolik link kullanabilirsiniz:

```bash
# Linux
ln -s /mnt/nas/qualisight-photos /opt/qualisight/photos

# Windows (yönetici olarak)
mklink /D "C:\publish\photos" "\\NAS\qualisight-photos"
```

---

## 9. Yedekleme

### 9.1 Veritabanı Yedeklemesi

```bash
# Günlük yedek (cron job olarak eklenebilir)
pg_dump -U irsapp -h localhost IRSGeneratorDb > /backup/qualisight_$(date +%Y%m%d_%H%M).sql

# Sıkıştırılmış format (daha küçük dosya)
pg_dump -U irsapp -h localhost -Fc IRSGeneratorDb > /backup/qualisight_$(date +%Y%m%d).dump
```

### 9.2 Dosya Yedeklemesi

Aşağıdaki klasörleri düzenli olarak yedekleyin:

```
DispositionTemplates/    ← NCM Word şablonları
photos/                  ← Muayene fotoğrafları
reports/                 ← Oluşturulan raporlar (opsiyonel)
appsettings.json         ← Yapılandırma
```

### 9.3 Geri Yükleme

```bash
# Veritabanı geri yükleme
psql -U irsapp -h localhost IRSGeneratorDb < /backup/qualisight_YYYYMMDD.sql

# Veya dump formatından:
pg_restore -U irsapp -h localhost -d IRSGeneratorDb /backup/qualisight_YYYYMMDD.dump
```

---

## 10. İlk Kullanıcı ve Rol Tanımları

Uygulama migration sonrası seed data ile iki kullanıcı oluşturur. İlk girişten sonra:

**Admin erişimi için seed kullanıcıları:**

| Sicil | Ad Soyad | Rol |
|---|---|---|
| `6518` | Erdem.Demirtaş | Seed kullanıcı |
| `5956` | Uras.Erken | Seed kullanıcı |

> **Not:** Seed kullanıcıların şifreleri `appsettings.json` veya `IRSGeneratorDbContext.cs` içinde tanımlıdır.
> İlk erişim sonrası admin şifresini değiştirin.

**İlk yapılacaklar:**

1. Admin hesabıyla giriş yapın
2. Şifreyi değiştirin (Kullanıcı yönetimi → Düzenle)
3. Müfettişler için kullanıcı hesapları oluşturun:
   - Rol: `inspector` (yalnızca okuma)
   - Rol: `engineer` (veri yazma + NCM)
   - Rol: `admin` (tam yetki)
4. Defect type tanımlarını şirket standartlarına göre güncelleyin
5. Disposition type ve transition kurallarını gözden geçirin
6. Kök neden kodlarını (cause codes) tanımlayın
7. NCM disposition tiplerini ve şablonlarını yapılandırın

---

## 11. Sistem Konfigürasyonu (Admin Panel)

`http://<SUNUCU_IP>:5297/admin.html` → **Sistem Konfigürasyonu**

| Anahtar | Açıklama | Örnek Değer |
|---|---|---|
| `PhotoRootFolder` | Muayene fotoğraflarının kaydedileceği klasör | `C:\QualiSight\Photos` veya `/data/photos` |
| `ReportRootFolder` | Rapor çıktılarının kaydedileceği klasör | `C:\QualiSight\Reports` |
| `BackupRootFolder` | Otomatik yedek klasörü | `C:\QualiSight\Backups` |

> Bu değerler `VisualSystemConfig` tablosunda saklanır. Uygulama yeniden başlatılmadan değişiklik etkili olur.

---

## 12. Güncelleme Prosedürü

### 12.1 Uygulama Güncellemesi

```bash
# 1. Uygulamayı durdur
nssm stop QualiSight          # Windows
systemctl stop qualisight     # Linux

# 2. Yeni sürümü derle
git pull
dotnet publish IRSGenerator.API -c Release -o ./publish-new

# 3. Mevcut publish'i yedekle
cp -r ./publish ./publish-backup

# 4. Yeni dosyaları kopyala
cp -r ./publish-new/* ./publish/

# 5. DispositionTemplates klasörünü kontrol et (şablonlar silinmediyse)
ls ./publish/DispositionTemplates/

# 6. Migration varsa uygula
dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API

# 7. Uygulamayı başlat
nssm start QualiSight          # Windows
systemctl start qualisight     # Linux
```

### 12.2 Sadece Veritabanı Güncellemesi (Yeni migration eklendiğinde)

```bash
dotnet ef database update \
  --project IRSGenerator.Data \
  --startup-project IRSGenerator.API
```

### 12.3 Sadece NCM Şablon Güncellemesi

Uygulama durdurulmadan yapılabilir:

```bash
# Yeni şablonu klasöre kopyala
cp YeniSablon.docx ./publish/DispositionTemplates/

# Admin panelinden NcmDispositionType kaydı ekle veya güncelle
```

---

## 13. Sorun Giderme

| Belirti | Olası Neden | Çözüm |
|---|---|---|
| Uygulama başlamıyor | Yanlış connection string | `appsettings.json` veya environment variable kontrol et |
| `FATAL: password authentication failed` | DB şifresi hatalı | PostgreSQL şifresi doğrula |
| Migration hatası | DB kullanıcısı yetersiz izin | `GRANT ALL ON SCHEMA public TO irsapp;` çalıştır |
| `413 Request Too Large` | Büyük fotoğraf/op-sheet yükleme | Nginx `client_max_body_size 50M` veya IIS `maxAllowedContentLength` ayarla |
| `404` tüm sayfalarda | `wwwroot/` klasörü eksik | Publish çıktısında `wwwroot/` var mı kontrol et |
| Op-sheet parse hatası | Dosya formatı uyumsuz | Yalnızca `.docx` desteklenir; `.doc`, `.xls` desteklenmez |
| NCM sheet üretilmiyor | Şablon dosyası eksik | `DispositionTemplates/` klasöründe ilgili `.docx` dosyasının varlığını doğrula |
| NCM sheet üretilmiyor | `NcmDispositionType.TemplateFileName` hatalı | Admin panelinden şablon dosya adının tam olarak eşleştiğini doğrula |
| Cookie çalışmıyor (giriş kalıcı değil) | Farklı origin/port | SameSite=Strict cookie — aynı domain üzerinden erişin |
| Yavaş sorgu | İndeks eksik | `pg_stat_user_tables` ile sorgula; `EXPLAIN ANALYZE` kullan |
| Uygulama çöküyor | Bellek yetersiz | En az 2 GB RAM tahsis edin; log'ları inceleyin |

**Log konumu (varsayılan — konsol çıktısı):**

```bash
# Linux — systemd
journalctl -u qualisight -f

# Windows — NSSM
# Uygulama Event Log'a yazar; NSSM ayrıca AppStdout/AppStderr dosyası oluşturabilir:
nssm set QualiSight AppStdout "C:\publish\logs\stdout.log"
nssm set QualiSight AppStderr "C:\publish\logs\stderr.log"
```

---

## Teknik Destek

Proje kaynağı: **https://github.com/Akaymaz2635/IRSGenerator-Backend**

Geliştirici: **Ali Kaymaz**
