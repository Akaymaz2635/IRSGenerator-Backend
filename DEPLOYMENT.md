# IRSGenerator — IT Entegrasyon ve Kurulum Rehberi

Bu belge, IRSGenerator uygulamasını şirket ağına veya sunucuya kurulum ve entegrasyon adımlarını açıklamaktadır.

---

## 1. Sistem Gereksinimleri

| Bileşen | Versiyon | Notlar |
|---|---|---|
| .NET SDK | 8.0 veya üzeri | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| PostgreSQL | 14 veya üzeri | Mevcut PostgreSQL sunucusu kullanılabilir |
| İşletim Sistemi | Windows Server 2019+, Ubuntu 20.04+ | IIS veya Kestrel ile çalışır |
| RAM | Min. 2 GB | Üretimde 4 GB önerilir |
| Disk | Min. 5 GB | Op-sheet ve fotoğraf dosyaları için ek alan gerekebilir |

---

## 2. Veritabanı Kurulumu

### 2.1 PostgreSQL'de Veritabanı Oluşturma

```sql
CREATE DATABASE "IRSGeneratorDb";
CREATE USER irsapp WITH ENCRYPTED PASSWORD 'güçlü_şifre_buraya';
GRANT ALL PRIVILEGES ON DATABASE "IRSGeneratorDb" TO irsapp;
```

### 2.2 Connection String Yapılandırması

`IRSGenerator.API/appsettings.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "PosgresConnection": "Host=<SUNUCU_IP>;Port=5432;Database=IRSGeneratorDb;Username=irsapp;Password=güçlü_şifre_buraya"
  }
}
```

> **Not:** Production ortamında şifre bilgisini environment variable olarak tanımlamak daha güvenlidir:
> ```bash
> export ConnectionStrings__PosgresConnection="Host=...;Password=..."
> ```

---

## 3. Uygulama Kurulumu

### 3.1 Kaynak Kodunu İndirin

```bash
git clone https://github.com/Akaymaz2635/IRSGenerator-Backend.git
cd IRSGenerator-Backend
```

### 3.2 Migration'ları Çalıştırın (Veritabanı Tablolarını Oluştur)

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project IRSGenerator.Data --startup-project IRSGenerator.API
```

### 3.3 Uygulamayı Derleyin

```bash
dotnet publish IRSGenerator.API -c Release -o ./publish
```

### 3.4 Uygulamayı Başlatın

```bash
cd publish
dotnet IRSGenerator.API.dll --urls "http://0.0.0.0:5297"
```

Tarayıcıdan `http://<SUNUCU_IP>:5297` adresine giderek uygulamanın çalıştığını doğrulayın.

---

## 4. Çok Kullanıcılı Ağ Kurulumu (İntranet)

Birden fazla müfettişin aynı anda sistemi kullanabilmesi için uygulamanın şirket ağındaki bir sunucuya kurulması yeterlidir.

### 4.1 Mimari

```
[Müfettiş 1 - Tablet/PC]  ─┐
[Müfettiş 2 - Tablet/PC]  ─┤──► [Uygulama Sunucusu :5297] ──► [PostgreSQL Sunucusu :5432]
[Müfettiş 3 - Tablet/PC]  ─┘
```

Kullanıcılar tarayıcıdan `http://<SUNUCU_IP>:5297` adresine bağlanır. Ek bir kurulum veya uygulama indirmesi gerekmez.

### 4.2 Güvenlik Duvarı (Firewall)

Sunucunuzun güvenlik duvarında aşağıdaki portları açın:

```
TCP 5297  →  İntranet kullanıcıları için uygulama portu
TCP 5432  →  Yalnızca uygulama sunucusundan PostgreSQL'e erişim (dışarıya kapalı)
```

### 4.3 Ters Proxy (Önerilir — IIS veya Nginx)

Doğrudan port yerine standart HTTP(S) üzerinden erişim için:

**IIS (Windows Server):**
1. IIS'e "URL Rewrite" ve "Application Request Routing" modüllerini yükleyin.
2. Yeni bir site oluşturun → Binding: `http://*:80` veya `https://*:443`.
3. web.config içinde reverse proxy kuralını tanımlayın:

```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="IRSGenerator Proxy" stopProcessing="true">
        <match url="(.*)" />
        <action type="Rewrite" url="http://localhost:5297/{R:1}" />
      </rule>
    </rules>
  </rewrite>
</system.webServer>
```

**Nginx (Linux):**
```nginx
server {
    listen 80;
    server_name irsgenerator.sirket.local;

    location / {
        proxy_pass         http://localhost:5297;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
    }

    client_max_body_size 50M;   # Op-sheet yükleme için
}
```

---

## 5. Servis Olarak Çalıştırma (Otomatik Başlatma)

### Windows — NSSM ile Windows Servisi

```bash
# NSSM indir: https://nssm.cc/
nssm install IRSGenerator "C:\publish\IRSGenerator.API.exe"
nssm set IRSGenerator AppParameters "--urls http://0.0.0.0:5297"
nssm set IRSGenerator AppDirectory "C:\publish"
nssm start IRSGenerator
```

### Linux — systemd

`/etc/systemd/system/irsgenerator.service`:

```ini
[Unit]
Description=IRSGenerator API
After=network.target postgresql.service

[Service]
WorkingDirectory=/opt/irsgenerator
ExecStart=/usr/bin/dotnet /opt/irsgenerator/IRSGenerator.API.dll --urls http://0.0.0.0:5297
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
systemctl enable irsgenerator
systemctl start irsgenerator
```

---

## 6. Dosya Depolama Yapılandırması

Uygulama iki klasöre dosya yazar:

| Klasör | İçerik | Boyut Tahmini |
|---|---|---|
| `wwwroot/op-sheets/` | Yüklenen Word op-sheet dosyaları | Muayene başına ~500 KB |
| `wwwroot/photos/` | Kusur fotoğrafları | Fotoğraf başına ~2-5 MB |

Bu klasörlerin bulunduğu disk bölümünde yeterli alan olduğundan emin olun. Gerekirse bu klasörleri harici bir network sürücüsüne veya NAS'a sembolik link ile yönlendirebilirsiniz.

---

## 7. Yedekleme

Aşağıdakileri düzenli olarak yedekleyin:

1. **PostgreSQL Veritabanı:**
   ```bash
   pg_dump -U irsapp IRSGeneratorDb > yedek_$(date +%Y%m%d).sql
   ```

2. **Yüklenen Dosyalar:**
   `wwwroot/op-sheets/` ve `wwwroot/photos/` klasörlerini yedek sisteminize kopyalayın.

---

## 8. İlk Kullanıcı ve Rol Tanımları

Uygulama başlatıldıktan sonra varsayılan admin kullanıcısıyla giriş yapın:

| Alan | Değer |
|---|---|
| Sicil | `admin` |
| Şifre | `admin` |

**İlk yapılacaklar:**
1. Admin şifresini değiştirin.
2. Müfettişler için kullanıcı hesapları oluşturun (`/api/users`).
3. Defect type tanımlarını şirket standartlarına göre güncelleyin.
4. Disposition type ve transition kurallarını gözden geçirin.

---

## 9. API Dokümantasyonu

Uygulama çalışırken Swagger arayüzüne şu adresten ulaşabilirsiniz:

```
http://<SUNUCU_IP>:5297/swagger/index.html
```

Tüm endpoint'lerin detaylı açıklaması, istek/yanıt örnekleri ve test arayüzü burada mevcuttur.

---

## 10. Sorun Giderme

| Belirti | Olası Neden | Çözüm |
|---|---|---|
| Uygulama başlamıyor | Yanlış connection string | `appsettings.json` kontrol et |
| Migration hatası | DB kullanıcısı yetersiz izin | `GRANT` komutlarını tekrar çalıştır |
| 413 Request Too Large | Büyük op-sheet yükleme | Nginx `client_max_body_size 50M` ekle |
| Yavaş sorgu | İndeks eksik | `pg_stat_user_tables` ile incele |
| Op-sheet parse hatası | Dosya formatı uyumsuz | Yalnızca `.docx` desteklenir |

---

## 11. Teknik Destek

Proje kaynağı: **https://github.com/Akaymaz2635/IRSGenerator-Backend**

Geliştirici: **Ali Kaymaz**
