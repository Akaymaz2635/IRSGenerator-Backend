# IRSGenerator — Kullanıcı Kılavuzu

Bu kılavuz, IRSGenerator sistemini kullanan **müfettişler** ve **kalite kontrolörler** için hazırlanmıştır. Teknik altyapı bilgisi gerektirmez.

---

## İçindekiler

1. [Sisteme Giriş](#1-sisteme-giriş)
2. [Ana Sayfa ve Navigasyon](#2-ana-sayfa-ve-navigasyon)
3. [Yeni Muayene Oluşturma](#3-yeni-muayene-oluşturma)
4. [Operasyon Sayfası Yükleme](#4-operasyon-sayfası-yükleme)
5. [Lot Karakterleri](#5-lot-karakterleri)
6. [Ölçüsel Karakterler](#6-ölçüsel-karakterler)
7. [Karar (Disposition) Mekanizması](#7-karar-disposition-mekanizması)
8. [Rapor Oluşturma](#8-rapor-oluşturma)
9. [Muayeneyi Tamamlama](#9-muayeneyi-tamamlama)
10. [Sık Sorulan Sorular](#10-sık-sorulan-sorular)

---

## 1. Sisteme Giriş

Tarayıcınızı açın ve sistem yöneticinizin verdiği adrese gidin (örnek: `http://192.168.1.10:5297`).

**Varsayılan giriş bilgileri:**

| Alan   | Değer   |
|--------|---------|
| Sicil  | `admin` |
| Şifre  | `admin` |

> **Önemli:** İlk girişten sonra şifrenizi değiştirin.

---

## 2. Ana Sayfa ve Navigasyon

Giriş yaptıktan sonra **Muayeneler** listesi karşınıza gelir.

| Düğme / Bağlantı | Ne Yapar? |
|---|---|
| **+ Yeni Muayene** | Yeni bir muayene kaydı oluşturur |
| Muayene satırına tıkla | Muayene detayını açar |
| **Karakterler** | Parçanın ölçüm ve lot listesini gösterir |

---

## 3. Yeni Muayene Oluşturma

1. Ana sayfada **"+ Yeni Muayene"** düğmesine tıklayın.
2. Formdaki alanları doldurun:

| Alan | Açıklama |
|---|---|
| **Proje** | Hangi IRS projesine ait olduğu |
| **Parça Numarası** | İncelenen parçanın teknik resim numarası |
| **Seri Numarası** | Parçanın seri numarası |
| **Operasyon No** | Hangi operasyonun muayene edildiği |
| **Müfettiş** | Muayeneyi yapan kişi |

3. **Kaydet** düğmesine tıklayın.

---

## 4. Operasyon Sayfası Yükleme

Operasyon sayfası (op-sheet), parçanın ölçüm özelliklerini tanımlayan Word dosyasıdır.

1. Muayene detay sayfasını açın.
2. **"Op-Sheet Yükle"** bölümüne gidin.
3. `.docx` uzantılı operasyon sayfasını seçin ve **Yükle** düğmesine tıklayın.
4. Sistem dosyayı otomatik olarak analiz eder ve karakterleri oluşturur.

> **Not:** Op-sheet yalnızca muayene **tamamlanmadan önce** yüklenebilir. Tamamlanan muayenelere tekrar yüklenemez.

---

## 5. Lot Karakterleri

Lot karakterleri, sayısal ölçüm gerektirmeyen (görsel, fonksiyonel) kontrol maddelerini içerir.

### Sonuç Girme

1. Karakterler listesinde bir **LOT** karakterine tıklayın.
2. Sağ panelde giriş alanı açılır.
3. Açılır listeden sonucu seçin:

| Sonuç | Anlamı |
|---|---|
| **Conform** | Uygun |
| **Not Conform** | Uygun değil |
| **Pass** | Geçti |
| **Fail** | Başarısız |

4. Gerekirse not ekleyin.
5. **Kaydet** düğmesine tıklayın.

---

## 6. Ölçüsel Karakterler

Ölçüsel karakterler, belirli tolerans sınırları olan sayısal ölçümler içerir.

### Ölçüm Değeri Girme

1. Karakterler listesinde bir **DIM** karakterine tıklayın.
2. Sağ panelde ölçüm giriş formu açılır.

**Formda görünen bilgiler:**

| Alan | Açıklama |
|---|---|
| **Özellik** | Ölçülen boyutun tanımı (teknik resimden gelen) |
| **Nominal** | Teorik hedef değer |
| **Alt Limit** | Minimum kabul edilebilir değer |
| **Üst Limit** | Maksimum kabul edilebilir değer |
| **Aktüel** | Sizin ölçtüğünüz değer |

3. **Aktüel** alanına ölçtüğünüz değeri girin.
4. **Part Label** alanında ölçümün hangi parça bölümüne ait olduğunu belirtin.
5. **Kaydet** düğmesine tıklayın.

Sistem otomatik olarak değerin tolerans içinde olup olmadığını gösterir:
- **Yeşil** → Tolerans içi (uygun)
- **Kırmızı** → Tolerans dışı (uygunsuz)

### Sınır Değerleri Anlama

| Gösterim | Anlamı |
|---|---|
| `25.4 ±0.1` | 25.3 ile 25.5 arasında olmalı |
| `≥ 2.5` | En az 2.5 olmalı |
| `≤ 10.0` | En fazla 10.0 olmalı |
| `Diş (thread)` | Sayısal ölçüm beklenmez |

---

## 7. Karar (Disposition) Mekanizması

Ölçüm sonucu tolerans dışı çıktığında veya özel bir karar gerektiğinde **Disposition** ekleyebilirsiniz.

### Disposition Ekleme

1. Ölçüsel karakter panelinde **"🎯 Karar"** düğmesine tıklayın.
2. Açılan panelde karar tipini seçin.
3. Gerekirse not ekleyin.
4. **Kaydet** düğmesine tıklayın.

### Disposition Tipleri

| Kod | Anlamı | Ne Zaman Kullanılır? |
|---|---|---|
| **ACCEPT** | Kabul | Ölçüm sınırlar içinde, resmi onay |
| **MRB ACCEPT** | MRB Kabulü | Tolerans dışı ama MRB onayıyla kabul |
| **RE-INSPECT** | Yeniden Muayene | Ölçüm şüpheli, tekrar yapılacak |
| **REWORK** | Tamir/Revizyon | Parça işleme geri gönderilecek |
| **SCRAP** | Hurda | Parça kullanılamaz |
| **USE-AS-IS** | Olduğu Gibi Kullan | Sapma mevcut ama kabul edildi |
| **CONCESSION** | Devir/Müsamaha | Müşteri onayıyla kabul |
| **VOID** | İptal | Önceki kararı iptal eder |

### REWORK Sonrası Yeniden Ölçüm

Bir karaktere **REWORK** kararı verilirse:

1. Parça işleme geri gönderilir ve düzeltme yapılır.
2. Karakteri yeniden seçip yeni ölçüm değeri girin.
3. **Part Label** alanında yeni bir etiket kullanın (örn. `POST_REWORK`).
4. Raporda yalnızca en son ölçüm görünür; eski ölçümler sistemde saklanmaya devam eder.

---

## 8. Rapor Oluşturma

Muayene tamamlandıktan sonra Word formatında rapor oluşturabilirsiniz.

1. Muayene detay sayfasında **"Rapor Oluştur"** düğmesine tıklayın.
2. Sistem `.docx` dosyasını oluşturur ve indirir.

**Raporda yer alan bilgiler:**
- Proje ve parça bilgileri
- Tüm karakterlerin listesi
- Her karakter için nominal değer, alt/üst limitler ve aktüel ölçüm
- Disposition kararları
- Özet: toplam / uygun / uygunsuz karakter sayıları

---

## 9. Muayeneyi Tamamlama

Tüm karakterlere sonuç girildiğinde muayeneyi tamamlayabilirsiniz.

1. Muayene detay sayfasında **"Tamamla"** düğmesine tıklayın.
2. Onay iletişim kutusunda **Evet** seçin.
3. Muayene durumu **"Tamamlandı"** olarak işaretlenir.

> **Uyarı:** Tamamlanan muayenelere yeni op-sheet yüklenemez.

---

## 10. Sık Sorulan Sorular

**S: Op-sheet yükledim ama karakterler gelmedi?**
C: Dosyanın `.docx` formatında olduğundan emin olun. Dosya bozuk veya farklı bir şablon yapısında olabilir. Sistem yöneticinize veya teknik ekibe başvurun.

---

**S: Yanlış ölçüm girdim, nasıl düzeltirim?**
C: Mevcut ölçüm kaydını düzenleyebilirsiniz. Karakter üzerindeki düzenleme ikonuna tıklayın ve değeri güncelleyin.

---

**S: "Tolerans dışı" çıkan bir karakteri kabul etmem gerekiyor?**
C: Bu durumda **MRB ACCEPT**, **USE-AS-IS** veya **CONCESSION** disposition tiplerinden uygun olanı seçin ve açıklama notu girin.

---

**S: Aynı karaktere birden fazla ölçüm girebilir miyim?**
C: Evet, farklı **Part Label** değerleriyle birden fazla ölçüm girebilirsiniz. Raporda her etiket için en son ölçüm gösterilir.

---

**S: Karanlık/aydınlık tema seçeneği var mı?**
C: Evet, sayfanın sağ üst köşesindeki tema düğmesiyle karanlık ve aydınlık mod arasında geçiş yapabilirsiniz.

---

**S: Sisteme erişemiyorum?**
C: Sisteme erişim için cihazınızın şirket ağına bağlı olması gerekir. Tarayıcı adres çubuğunda sistem yöneticinizin verdiği adresi doğru yazdığınızdan emin olun.

---

**S: Op-sheet'teki KN numaraları listede farklı görünüyor?**
C: Sistem KN numaralarını otomatik normalleştirir; boşluklar kaldırılır (örn. "KN 026" → "KN026"). Bu beklenen davranıştır.

---

## Destek

Teknik sorunlar için sistem yöneticinize veya aşağıdaki proje sayfasına başvurun:

**GitHub:** https://github.com/Akaymaz2635/IRSGenerator-Backend

**Geliştirici:** Ali Kaymaz
