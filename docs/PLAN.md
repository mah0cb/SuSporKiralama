# Yol Haritası

Proje dönem boyunca 10 aşamada geliştirilir. Her aşama bitince burada "tamamlandı" olarak işaretlenir.

| # | Aşama | Durum |
|---|---|---|
| 1 | İskelet + veritabanı | ✅ Tamamlandı |
| 2 | Business CRUD | Bekliyor |
| 3 | Kiralama akışı + xUnit testleri | Bekliyor |
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

## 2. Business CRUD
Musteri, Ekipman ve Personel için servis sınıfları; servisler bağımlılıklarını constructor üzerinden alır (ileride UI'da dependency injection). İş kuralı doğrulamaları (ör. aktif kiralaması olan müşteri/ekipman silinemez, ekipman kodu benzersiz). Ortak bir temel exception'dan türeyen özel exception hiyerarşisi (ör. IsKuraliException → KayitBulunamadiException, DogrulamaException).

## 3. Kiralama akışı
Müsaitlik kontrolü (zaman aralığı çakışması + ekipman durumu), kiralama başlatma, iade (polimorfik UcretHesapla ile ücret, gecikme, hasar bedeli, ekipman durumunun geri alınması), ödeme kaydı. xUnit test projesi: ücret hesapları ve çakışma senaryoları.

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
