# Yol Haritası

Proje dönem boyunca 10 aşamada geliştirilir. Her aşama bitince burada "tamamlandı" olarak işaretlenir.

| # | Aşama | Durum |
|---|---|---|
| 1 | İskelet + veritabanı | ✅ Tamamlandı |
| 2 | Business CRUD + test projesi | ✅ Tamamlandı |
| 3 | Kiralama akışı + ücret/kiralama testleri | Bekliyor |
| 4 | Giriş ve raporlama | Bekliyor |
| 5 | Yapay zeka servisi | Bekliyor |
| 6 | DevExpress kurulumu | Bekliyor |
| 7 | CRUD formları | Bekliyor |
| 8 | Kiralama/iade ekranları, dashboard | Bekliyor |
| 9 | Kiralama fişi (XtraReports), yapay zeka öneri ekranı | Bekliyor |
| 10 | Son düzenlemeler | Bekliyor |

## 1. İskelet + veritabanı — ✅ Tamamlandı
Solution, entity'ler, EF Core, migration, örnek veri.

**Not (30.09.2026):** Entities / DataAccess / Business projeleri (net10.0) oluşturuldu. Ekipman hiyerarşisi (SupBoard, Kano, CanYelegi) TPH ile tek tabloda; `UcretHesapla(sure, birimUcret)` polimorfik, iade anında kiralama sırasındaki fiyat (`UygulananBirimUcret`) kullanılabilsin diye birim ücret parametre olarak alınıyor. Geçmişi olan müşteri/personel/ekipman silinmez (Restrict), pasife alınır (`AktifMi` / `HizmetDisi`). InitialCreate uygulandı; idempotent seed yükleniyor. Sonraki aşamalar için açık noktalar: depozito alındı/iade takibi (Odeme'de tür alanı) ve eşzamanlı kiralamaya karşı RowVersion 3. aşamada değerlendirilecek.

**Karar (2. aşama):** RowVersion eklenmeyecek; uygulama tek bilgisayarda çalışan bir masaüstü uygulaması, eşzamanlı güncelleme riski yok. Depozito takibi 3. aşamada ele alınacak.

## 2. Business CRUD + test projesi — ✅ Tamamlandı
Musteri, Ekipman ve Personel için servis sınıfları; servisler bağımlılıklarını constructor üzerinden alır (DI container 6. aşamada UI'da). İş kuralı doğrulamaları (ör. kiralama geçmişi olan müşteri/ekipman/personel silinemez, ekipman kodu benzersiz). Ortak bir temel exception'dan türeyen özel exception hiyerarşisi. xUnit test projesi (SQLite in-memory) bu aşamaya çekildi; servis kuralları test edilir.

**Not (30.09.2026):** `ICrudServisi<T>` + soyut `CrudServisiTemel<T>` (Template Method): ortak akış sabit, alt servisler `EklemeOncesiKontrol` / `GuncellemeOncesiKontrol` / `SilmeOncesiKontrol` kancalarını override eder. Exception'lar: `IsKuraliException` (soyut) → `DogrulamaException`, `KayitBulunamadiException`, `BenzersizlikIhlaliException`, `IliskiliKayitVarException`, `IslemYapilamazException`. Beklenmeyen veritabanı hataları sarmalanmaz (10. aşama). Kural ihlaliyle reddedilen `Guncelle`, nesneyi `IRepository.Reload` ile veritabanındaki haline döndürür; paylaşılan DbContext'te geçersiz değişiklik sonraki bir `SaveChanges` ile yazılamaz. Telefon `05XXXXXXXXX` biçimine normalize edilir. Ekipman Kirada durumuna elle alınamaz / Kirada'dan elle çıkarılamaz. Şifre en az 8 karakter; genel `Guncelle` şifre hash'ini değiştiremez; en az bir aktif Admin kalır. Testler bellekte SQLite kullanır; şema `CreateTables` ile oluşturulduğu için örnek veri yüklenmez. 62 test başarılı.

## 3. Kiralama akışı
Müsaitlik kontrolü (zaman aralığı çakışması + ekipman durumu), kiralama başlatma, iade (polimorfik UcretHesapla ile ücret, gecikme, hasar bedeli, ekipman durumunun geri alınması), ödeme kaydı, depozito takibi. Mevcut test projesine ücret hesabı, çakışma ve kiralama/iade senaryolarının testleri eklenir.

## 4. Giriş ve raporlama
PBKDF2 ile şifre doğrulama, oturumdaki personel bilgisi, rol bazlı yetki (Admin personel ve fiyat yönetebilir). Dashboard için rapor sorguları (günlük/aylık gelir, en çok kiralanan ekipman, doluluk oranı) DTO'lar ile.

## 5. Yapay zeka servisi
IAiOneriServisi arayüzü; Claude API'yi HttpClient ile çağıran gerçek implementasyon (API anahtarı repoya girmez) ve internet yokken çalışan sahte implementasyon. Senaryo: müşterinin serbest metin tarifi + müsait ekipman listesi → önerilen ekipman paketi (JSON yanıt).

## 6. DevExpress kurulumu
Windows Forms UI projesi (net10.0-windows) DevExpress şablonuyla; giriş formu, RibbonForm ana form, tema.

## 7. CRUD formları
GridControl listeleri, LayoutControl ile ekle/düzenle formları, DXValidationProvider ile doğrulama.

## 8. Kiralama başlatma ve iade ekranları
ChartControl ile dashboard.

## 9. Raporlar ve yapay zeka ekranı
XtraReports ile yazdırılabilir kiralama fişi (PDF dışa aktarma), yapay zeka öneri ekranı.

## 10. Son düzenlemeler
Genel hata yönetimi, EF Core'un ürettiği SQL'i loglayabilme, README'nin son hali, yazılı rapor için ER şeması ve sınıf açıklamaları.
