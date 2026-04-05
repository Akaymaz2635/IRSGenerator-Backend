# QualiSight — Kullanıcı Kılavuzu

Bu kılavuz, QualiSight sistemini kullanan **müfettişler**, **mühendisler** ve **yöneticiler** için hazırlanmıştır. Teknik altyapı bilgisi gerektirmez.

---

## İçindekiler

1. [Sisteme Giriş ve Çıkış](#1-sisteme-giriş-ve-çıkış)
2. [Ana Sayfa ve Navigasyon](#2-ana-sayfa-ve-navigasyon)
3. [Kullanıcı Rolleri ve Yetkiler](#3-kullanıcı-rolleri-ve-yetkiler)
4. [Muayene Oluşturma](#4-muayene-oluşturma)
5. [Operasyon Sayfası Yükleme](#5-operasyon-sayfası-yükleme)
6. [Boyutsal (DIM) Karakterler — Ölçüm Girişi](#6-boyutsal-dim-karakterler--ölçüm-girişi)
7. [LOT Karakterleri — Kategorik Sonuç Girişi](#7-lot-karakterleri--kategorik-sonuç-girişi)
8. [Karar (Disposition) Mekanizması](#8-karar-disposition-mekanizması)
9. [Fotoğraf Ekleme](#9-fotoğraf-ekleme)
10. [Görsel Kusur (Defect) Kaydetme](#10-görsel-kusur-defect-kaydetme)
11. [NCM — Uygunsuzluk Yönetimi](#11-ncm--uygunsuzluk-yönetimi)
12. [Rapor Oluşturma](#12-rapor-oluşturma)
13. [Muayeneyi Tamamlama](#13-muayeneyi-tamamlama)
14. [Admin Paneli](#14-admin-paneli)
15. [Sık Sorulan Sorular](#15-sık-sorulan-sorular)

---

## 1. Sisteme Giriş ve Çıkış

Tarayıcınızı açın ve sistem yöneticinizin verdiği adrese gidin (örnek: `http://192.168.1.10:5297`).

**Giriş Ekranı:**

| Alan | Açıklama |
|---|---|
| **Sicil** | Çalışan sicil numaranız |
| **Şifre** | Sistem yöneticinizin verdiği şifre |

Giriş başarılıysa ana sayfaya yönlendirilirsiniz.

**Çıkış:** Sağ üst köşedeki kullanıcı menüsünden **Çıkış Yap**'a tıklayın.

> **Not:** Oturum tarayıcı kapanana kadar açık kalır. Paylaşılan bilgisayarlarda mutlaka çıkış yapın.

---

## 2. Ana Sayfa ve Navigasyon

Giriş yaptıktan sonra sol kenar çubuğu (sidebar) üzerinden tüm modüllere erişebilirsiniz:

| Menü | Açıklama |
|---|---|
| **Muayeneler** | Tüm muayene kayıtlarını listeler |
| **IRS Projeleri** | Muayene şablonları (proje tanımları) |
| **NCM** | Uygunsuzluk yönetimi listesi |
| **Analitik** | İstatistik ve grafikler |
| **Admin** | (Yalnızca admin) Sistem yönetimi |

**Sağ üst köşe:**
- Tema butonu — karanlık/aydınlık mod geçişi
- Tablet modu butonu — dokunmatik ekran optimizasyonu
- Açık muayene göstergesi — aktif muayene varsa gösterilir

---

## 3. Kullanıcı Rolleri ve Yetkiler

| Rol | Ne Yapabilir? |
|---|---|
| **inspector** (Müfettiş) | Muayene, karakter, sonuç, fotoğraf **görüntüleme**. Veri girişi yapamaz. |
| **engineer** (Mühendis) | Tüm veri girişi: muayene oluşturma, ölçüm girme, disposition, NCM sheet üretme |
| **admin** (Yönetici) | Her şey + kullanıcı yönetimi + sistem konfigürasyonu + admin paneli |

---

## 4. Muayene Oluşturma

1. Sol menüden **Muayeneler**'e tıklayın.
2. **+ Yeni Muayene** düğmesine tıklayın.
3. Formdaki alanları doldurun:

| Alan | Açıklama | Zorunlu? |
|---|---|---|
| **Proje** | Hangi görsel projeye ait | Hayır |
| **Parça Numarası** | Teknik resim numarası | Evet |
| **Seri Numarası** | Parçanın seri numarası | Evet |
| **Operasyon No** | Hangi operasyon muayene ediliyor | Evet |
| **Notlar** | Serbest not alanı | Hayır |

4. **Kaydet** düğmesine tıklayın.

---

## 5. Operasyon Sayfası Yükleme

Operasyon sayfası (op-sheet), parçanın ölçüm özelliklerini tanımlayan Word dosyasıdır.
Yüklendiğinde sistem karakterleri (boyutlar ve LOT kalemleri) otomatik oluşturur.

1. Muayene detay sayfasını açın.
2. **"Op-Sheet Yükle"** bölümüne gidin.
3. `.docx` uzantılı operasyon sayfasını seçin ve **Yükle** düğmesine tıklayın.
4. Sistem dosyayı analiz eder:
   - Tüm ölçüm kalemlerini karakter olarak kaydeder
   - Her kalem için alt/üst limitler otomatik çıkarılır
   - "VISUAL", "CHECK", "MARKING" içeren kalemler LOT olarak işaretlenir

> **Desteklenen format:** Yalnızca `.docx` (Word 2007+). `.doc`, `.xls` desteklenmez.

> **Uyarı:** Op-sheet yalnızca muayene **tamamlanmadan önce** yüklenebilir.

---

## 6. Boyutsal (DIM) Karakterler — Ölçüm Girişi

Boyutsal karakterler, tolerans sınırı olan sayısal ölçümlerdir.

### Adımlar

1. Karakterler listesinde bir **DIM** karakterine tıklayın.
2. Sağ panelde ölçüm giriş formu açılır.

**Panelde görünen bilgiler:**

| Alan | Açıklama |
|---|---|
| **Özellik (Dimension)** | Teknik resimden gelen boyut tanımı |
| **Alt Limit** | Minimum kabul edilebilir değer |
| **Üst Limit** | Maksimum kabul edilebilir değer |
| **Aktüel (Actual)** | Ölçtüğünüz değer |
| **Part Label** | Ölçümün hangi parça/konuma ait olduğu |

3. **Aktüel** alanına değeri girin.
4. **Part Label** belirtin (tek parça için `P1`, birden fazla parça için `P1`, `P2`, vb.)
5. **Kaydet** düğmesine tıklayın.

**Renk göstergesi:**
- Yeşil → Tolerans içi (uygun)
- Kırmızı → Tolerans dışı (uygunsuz)

### Değer Güncelleme

Girilen bir ölçüm güncellendiğinde sistem **UpdateReason** (güncelleme sebebi) ister.
Bu bilgi denetim kaydı olarak saklanır.

### Sınır Değerleri Anlama

| Gösterim | Anlamı |
|---|---|
| `25.4 ±0.1` | 25.3 ile 25.5 arasında olmalı |
| `≥ 2.5` | En az 2.5 olmalı |
| `≤ 10.0` | En fazla 10.0 olmalı |
| `Diş (thread)` | Sayısal ölçüm beklenmez — geç/kaldır olarak işaretlenir |

### Rework Sonrası Yeniden Ölçüm

Bir karaktere **REWORK** kararı verilip parça işleme geri gönderildikten sonra:

1. Karakteri tekrar seçin.
2. Yeni ölçüm değerini girin.
3. **Part Label** alanına yeni bir etiket yazın (örn. `POST_REWORK`).
4. Raporda yalnızca en son ölçüm gösterilir; eski ölçümler sistemde saklanmaya devam eder.

---

## 7. LOT Karakterleri — Kategorik Sonuç Girişi

LOT karakterleri, sayısal ölçüm gerektirmeyen görsel/fonksiyonel kontrol kalemleridir.

### Adımlar

1. Karakterler listesinde bir **LOT** karakterine tıklayın.
2. Sağ panelde açılır liste belirir.
3. Sonucu seçin:

| Sonuç | Anlamı |
|---|---|
| **Conform** | Uygun |
| **Not Conform** | Uygun değil |
| **Pass** | Geçti |
| **Fail** | Başarısız |

4. Gerekirse not ekleyin.
5. **Kaydet** düğmesine tıklayın.

---

## 8. Karar (Disposition) Mekanizması

Ölçüm tolerans dışı çıktığında veya bir karakter için özel karar gerektiğinde **Disposition** ekleyebilirsiniz.

### Disposition Ekleme (Karakter Bazlı)

1. Karakter panelinde **"🎯 Karar"** düğmesine tıklayın.
2. İzin verilen karar tipleri listelenir (durum makinesi kurallarına göre filtrelenir).
3. Uygun kararı seçin.
4. Zorunlu alanları doldurun (mühendis adı, not, ref. numarası vb. kararın tipine göre değişir).
5. **Kaydet** düğmesine tıklayın.

### Disposition Tipleri

| Kod | Türkçe Etiket | Kullanım Zamanı |
|---|---|---|
| **USE_AS_IS** | Olduğu Gibi Kullan | Sapma var, kabul edildi |
| **KABUL_RESIM** | Accept/Image | Resme göre kabul |
| **CONFORMS** | Uygun | Parça gereksinimleri karşılıyor |
| **REWORK** | Tamir/Revizyon | Parça işleme geri gönderiliyor |
| **RE_INSPECT** | Yeniden Muayene | Ölçüm şüpheli, tekrar yapılacak |
| **CTP_RE_INSPECT** | CTP — Next Op Re-inspect | Sonraki operasyonda yeniden kontrol |
| **MRB_SUBMITTED** | MRB'ye Gönderildi | Maddi inceleme kuruluna iletildi |
| **MRB_CTP** | MRB — CTP ile Devam | MRB onayıyla sonraki operasyona |
| **MRB_ACCEPTED** | MRB Kabul | MRB tarafından kabul edildi |
| **MRB_REJECTED** | MRB Ret | MRB tarafından reddedildi |
| **VOID** | İptal | Önceki kararı geçersiz kılar |
| **REPAIR** | Onarım | Onarıma gönderildi |
| **SCRAP** | Hurda | Parça kullanılamaz |

### Durum Makinesi (State Machine)

Disposition geçişleri kurallara göre kısıtlanmıştır. Örneğin:
- `REWORK` → `CONFORMS` veya başka bir tam karar
- `MRB_SUBMITTED` → `MRB_CTP` veya `MRB_ACCEPTED` veya `MRB_REJECTED`
- Geçersiz geçişler listede **görünmez**.

### Disposition Geçmişi

Bir karakterin tüm karar geçmişini görmek için:
1. Karaktere tıklayın.
2. **"Detay"** veya **"Karar Geçmişi"** butonuna tıklayın.
3. Modal pencerede tüm kararlar zaman damgalı şekilde listelenir.

---

## 9. Fotoğraf Ekleme

Muayene sırasında kusur veya genel muayene fotoğrafı ekleyebilirsiniz.

1. Muayene detay sayfasında **Fotoğraflar** sekmesine gidin.
2. **"Fotoğraf Yükle"** düğmesiyle dosya seçin (veya tablet/kamera ile çekin).
3. Fotoğraf yüklendikten sonra ilgili kusur(lar)la eşleştirebilirsiniz:
   - Fotoğraf üzerine tıklayın → **"Kusurları Bağla"**
   - Listeden ilgili kusur(ları) seçin → Kaydet

---

## 10. Görsel Kusur (Defect) Kaydetme

Görsel muayenede parça yüzeyinde tespit edilen kusurlar **Defect** olarak kaydedilir.

1. Muayene detay sayfasında **Kusurlar** sekmesine gidin.
2. **"+ Yeni Kusur"** düğmesine tıklayın.
3. Formu doldurun:

| Alan | Açıklama |
|---|---|
| **Kusur Tipi** | Önceden tanımlanmış kusur kategorisi |
| **Boyutlar** | Derinlik, genişlik, uzunluk, yarıçap vb. (kusur tipine göre) |
| **Notlar** | Serbest açıklama |

4. Kusuru kaydettikten sonra fotoğraf ekleyebilir ve disposition verebilirsiniz.

### Kusur Üzerine Disposition

1. Kusur listesinde ilgili kusura tıklayın.
2. **"Karar Ekle"** düğmesine tıklayın.
3. Disposition tipini seçin ve gerekli bilgileri doldurun.

---

## 11. NCM — Uygunsuzluk Yönetimi

NCM modülü, muayene sürecinde tespit edilen uygunsuzluklar için resmi **Disposition Sheet** (karar belgesi) üretir.

> **Gerekli yetki:** `engineer` veya `admin` rolü.

### NCM Akışı

```
1. Muayene tamamlanır (ölçümler girilir, kusurlar kaydedilir)
2. NCM listesinden ilgili muayene seçilir
3. Uygunsuzluklar (dimensional + görsel) listelenir
4. Dahil edilecek NC kalemleri seçilir
5. Kök neden kodu ve disposition tipi seçilir
6. "Disposition Sheet Üret" tıklanır
7. Word belgesi indirilir
```

### Adım Adım NCM Sheet Üretme

1. Sol menüden **NCM**'ye tıklayın.
2. Listeden ilgili muayeneye tıklayın.
3. **NCM Detay** sayfasında uygunsuzlukları görürsünüz:
   - **Boyutsal uygunsuzluklar** — tolerans dışı karakterler
   - **Görsel uygunsuzluklar** — kayıtlı kusurlar
4. Sheet'e dahil edilecek kalemleri seçin (checkbox ile).
5. Üst bölümde şu alanları doldurun:

| Alan | Açıklama |
|---|---|
| **Operasyon (Oper)** | Uygunsuzluğun tespit edildiği operasyon |
| **Kök Neden Op. (C-OP)** | Uygunsuzluğun oluştuğu operasyon |
| **Adet (Qty)** | Etkilenen parça adedi |
| **Kök Neden Kodu** | Listeden seçin (admin tarafından tanımlanır) |
| **Disposition Tipi** | Hangi karar belgesi şablonu kullanılacak |

6. **"Disposition Sheet Üret"** düğmesine tıklayın.
7. Belge otomatik indirilir:
   - Tek sayfaya sığıyorsa → tek `.docx` dosyası
   - Çok sayıda NC kalemi varsa (21'den fazla) → `.zip` içinde birden fazla `.docx`

### Kök Neden Kodları

Kök neden kodları admin tarafından tanımlanır (Admin Panel → Kök Neden Kodları).
Örnek kodlar: `TOOL_WEAR`, `MATERIAL_DEFECT`, `OPERATOR_ERROR`, `FIXTURE_ISSUE`

### Disposition Tipleri (NCM)

Her NCM disposition tipi ayrı bir Word şablonuna bağlıdır:

| Tip | Şablon |
|---|---|
| ACCEPT | `ACCEPT.docx` |
| MRB | `MRB.docx` |
| CTP & MRB | `CTP&MRB.docx` |
| SCRAP (Bireysel) | `SCRAP-IND.docx` |
| SCRAP (Lot) | `SCRAP-LOT.docx` |
| Return to Vendor | `RETURN-TO-VENDOR.docx` |
| … | … |

---

## 12. Rapor Oluşturma

IRS muayenelerinde ölçüm verileri Word formatında rapor olarak indirilebilir.

1. Muayene detay sayfasında **"Rapor Oluştur"** düğmesine tıklayın.
2. Sistem `.docx` dosyasını oluşturur ve otomatik indirir.

**Rapor içeriği:**
- Proje ve parça bilgileri
- Tüm karakterlerin listesi (nominal, alt limit, üst limit, aktüel)
- Rework yapılmışsa en son ölçüm değeri gösterilir
- Disposition kararları
- Özet: toplam / uygun / uygunsuz karakter sayıları
- Non-conformance bölümü (yalnızca uygunsuz karakterler)

---

## 13. Muayeneyi Tamamlama

Tüm karakterlere sonuç girildiğinde ve gerekli disposition kararları verildikten sonra muayeneyi kapatabilirsiniz.

1. Muayene detay sayfasında **"Muayeneyi Tamamla"** düğmesine tıklayın.
2. Onay iletişim kutusunda **Evet** seçin.
3. Muayene durumu **"Tamamlandı"** olarak işaretlenir.

> **Uyarı:** Tamamlanan muayenelere yeni op-sheet yüklenemez. Ölçüm değerleri yine de güncellenebilir.

---

## 14. Admin Paneli

Admin paneline `http://<SUNUCU_IP>:5297/admin.html` adresinden erişilir.
Yalnızca **admin** rolüne sahip kullanıcılar erişebilir.

### 14.1 Kullanıcı Yönetimi

- Yeni kullanıcı ekle, düzenle, deaktive et
- Rol ata: `inspector`, `engineer`, `admin`
- Şifre sıfırla

### 14.2 Proje Yönetimi

Görsel muayene projelerini (VisualProject) oluşturun ve yönetin.

### 14.3 Kusur Tipi Yönetimi

**Defect Types:** Görsel kusur kategorilerini tanımlayın (örn. Çizik, Çökme, Çatlak).

**Defect Fields:** Her kusur tipine özel alanlar ekleyin (Derinlik, Genişlik, Renk vb.):
- Alan tipi: metin, sayı, evet/hayır
- Min/Max değer kısıtları
- Zorunluluk ayarı

### 14.4 Disposition Tipi Yönetimi

Mevcut 13 disposition tipini görüntüleyin ve yeni tipler ekleyin:
- Kod (benzersiz), Etiket, CSS sınıfı
- `Is Initial` — başlangıç kararı olarak gösterilip gösterilmeyeceği
- `Is Neutralizing` — önceki kararı geçersiz kılıp kılmadığı
- Sıralama

### 14.5 Disposition Geçiş Kuralları

State machine'i yapılandırın:
- Hangi disposition'dan hangisine geçilebileceğini belirleyin
- `REWORK` kararından sonra hangi kararlar verilebilir gibi

### 14.6 NCM Kök Neden Kodları

NCM sheet üretiminde kullanılacak kök neden kodlarını yönetin:
- Kod (kısa kod), Açıklama, Aktif/pasif, Sıralama

### 14.7 NCM Disposition Tipleri

NCM için kullanılacak disposition tiplerini ve Word şablon eşleşmelerini yönetin:
- Kod, Etiket, Açıklama
- **Template File Name** — `DispositionTemplates/` klasöründeki `.docx` dosyasının adı (tam eşleşmeli)
- Aktif/pasif, Sıralama

### 14.8 Sistem Konfigürasyonu

- `PhotoRootFolder` — fotoğraf depolama klasörü
- `ReportRootFolder` — rapor çıktı klasörü
- `BackupRootFolder` — yedek klasörü

### 14.9 Yedekleme

Veritabanı yedekleme işlemlerini başlatın ve geçmiş yedekleri görüntüleyin.

---

## 15. Sık Sorulan Sorular

**S: Op-sheet yükledim ama karakterler gelmedi?**
C: Dosyanın `.docx` formatında olduğundan emin olun. Dosya bozuk veya standart dışı bir tablo yapısında olabilir. Teknik ekibe bildirin.

---

**S: Yanlış ölçüm girdim, nasıl düzeltirim?**
C: Karakter panelinde düzenleme ikonuna tıklayın ve değeri güncelleyin. Sistem **güncelleme sebebi** (UpdateReason) girmenizi isteyecektir — bu bilgi denetim kaydı olarak saklanır.

---

**S: "Tolerans dışı" çıkan bir karakteri kabul etmem gerekiyor?**
C: Bu durumda uygun disposition tipini seçin: `USE_AS_IS`, `MRB_SUBMITTED`, `KABUL_RESIM` vb. Açıklama notu ve mühendis onayı gerekir.

---

**S: Aynı karaktere birden fazla ölçüm girebilir miyim?**
C: Evet. Farklı **Part Label** değerleriyle birden fazla ölçüm kaydedebilirsiniz (`P1`, `P2`, `POST_REWORK` vb.). Raporda her etiket için en son ölçüm gösterilir.

---

**S: NCM sheet üretiyorum ama dosya inmiyor?**
C: İlk olarak tarayıcı popup engelleyicisini kontrol edin. Sorun devam ederse Admin Panel → NCM Disposition Tipleri bölümünde seçilen tipin şablon dosya adının doğru yazıldığını kontrol edin.

---

**S: Disposition eklemeye çalışıyorum ama bazı kararlar görünmüyor?**
C: Disposition state machine devrededir. Mevcut karara göre izin verilen geçişler gösterilir. Admin Panel → Disposition Geçişleri bölümünden kuralları görüntüleyebilirsiniz.

---

**S: Karanlık/aydınlık tema seçeneği var mı?**
C: Evet. Sağ üst köşedeki tema butonuyla geçiş yapabilirsiniz.

---

**S: Tablet modunu nasıl açarım?**
C: Sağ üst köşedeki tablet butonu ile tablet modu açılır. Dokunmatik kullanımda büyük düğmeler ve geliştirilmiş dokunma desteği etkinleşir.

---

**S: Sisteme erişemiyorum?**
C: Cihazınızın şirket ağına bağlı olduğundan emin olun. Tarayıcı adres çubuğunda doğru sunucu adresini yazdığınızdan emin olun. Sorun devam ederse sistem yöneticinize başvurun.

---

**S: Op-sheet'teki kalem numaraları listede farklı görünüyor?**
C: Sistem kalem numaralarını otomatik normalleştirir — boşluklar kaldırılır (örn. `KN 026` → `KN026`). Bu beklenen davranıştır.

---

**S: NCM sheet'te 21'den fazla NC kalemi var, ne olur?**
C: Sistem otomatik olarak kalemleri sayfalara böler (sayfa başına 21 kalem: 3 kolon × 7 satır). Çok sayfalıysa tüm sayfalar `.zip` dosyası içinde sunulur.

---

## Destek

Teknik sorunlar için sistem yöneticinize başvurun.

**GitHub:** https://github.com/Akaymaz2635/IRSGenerator-Backend

**Geliştirici:** Ali Kaymaz
