# Yol Haritası

Proje dönem boyunca 10 aşamada geliştirilir. Her aşama bitince burada "tamamlandı" olarak işaretlenir.

**Sıra değişikliği (16.10.2026):** 5. aşamadan sonra planlanan ara aşama (demo verisi, SQL loglama, diyagramlar) ertelendi. Yeni sıra: 6 → 7 → ara aşama → 8 → 9 → 10.

| # | Aşama | Durum |
|---|---|---|
| 1 | İskelet + veritabanı | ✅ Tamamlandı |
| 2 | Business CRUD + test projesi | ✅ Tamamlandı |
| 3 | Kiralama akışı + ücret/kiralama testleri | ✅ Tamamlandı |
| 4 | Giriş ve raporlama | ✅ Tamamlandı |
| 5 | Yapay zeka servisi | ✅ Tamamlandı |
| 6 | DevExpress kurulumu | ✅ Tamamlandı |
| 7 | CRUD formları | ✅ Tamamlandı |
| Ara | Demo verisi, SQL loglama, diyagramlar | Ertelendi (7. aşamadan sonra) |
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

## 3. Kiralama akışı — ✅ Tamamlandı
Müsaitlik kontrolü (zaman aralığı çakışması + ekipman durumu), kiralama başlatma, iade (polimorfik UcretHesapla ile ücret, gecikme, hasar bedeli, ekipman durumunun geri alınması), ödeme kaydı, depozito takibi. Mevcut test projesine ücret hesabı, çakışma ve kiralama/iade senaryolarının testleri eklenir.

**Not (30.09.2026):** Durum makinesi `Kiralama` entity'sinde: Rezerve →`TeslimEt`→ Aktif →`Tamamla`→ Tamamlandi, Rezerve →`IptalEt`→ IptalEdildi; `Durum` ve ücret/zaman alanları private set, geçersiz geçişte `InvalidOperationException` (servis `IslemYapilamazException`'a çevirir). `IMusaitlikServisi` ayrı servis (5. aşamadaki yapay zeka önerisi kullanacak): yarı açık [başlangıç, bitiş) çakışması Rezerve/Aktif kiralamalara karşı; iade edilmemiş gecikmiş kiralamada ekipman şimdiye kadar dolu sayılır; Bakımda/Hizmet Dışı hiç müsait değil. `KiralamaServisi` `CrudServisiTemel`'den türemez (genel Sil/Guncelle yok); her işlem önce tüm kontroller, sonra tek `SaveChanges`. Servisler "şimdi"yi `TimeProvider`'dan alır, testlerde `FakeTimeProvider`. Depozito teslimde ekipmanların o anki depozito toplamı olarak alınır; iadede hasar önce depozitodan mahsup edilir (`Kiralama.DepozitoMahsupHesapla`), aşan kısım borca eklenir; `KalanBorc = ToplamUcret - mahsup - ödemeler`. Ödeme yalnızca tamamlanmış kiralamaya ve kalan borcu aşmadan alınır. Migration `KiralamaDepozitoTakibi` (DepozitoTutari, DepozitoDurumu, DepozitoMahsupTutari); mevcut tamamlanmış kayıtlarda depozito iade edilmiş sayılır. Ayrıca 2. aşamadan kalan iki düzeltme: genel `Ekle(Personel)` hash'lenmemiş şifreyi reddeder; `ArgumentException` parametre adsız fırlatılır. 153 test başarılı.

## 4. Giriş ve raporlama — ✅ Tamamlandı
PBKDF2 ile şifre doğrulama, oturumdaki personel bilgisi, rol bazlı yetki (Admin personel ve fiyat yönetebilir). Dashboard için rapor sorguları (günlük/aylık gelir, en çok kiralanan ekipman, doluluk oranı) DTO'lar ile.

**Not (01.10.2026):** Ön kontrolde `IadeAl`'daki `Tamamla` ve `KiralamaBaslat`'taki `TeslimEt` çağrılarının sarılmadığı görüldü; saat geri alınınca (yaz saati) çıplak `InvalidOperationException` kaçıyordu, düzeltildi. `SifreHasher.Dogrula` sabit süreli karşılaştırma yapar. `GirisServisi`: hatalı kullanıcı adı ve şifre aynı mesajı alır (olmayan kullanıcıda da sahte hash doğrulanır), pasif hesap ayrı mesaj alır (yalnızca şifre doğruysa), aynı kullanıcı adıyla art arda 5 hatada 5 dakika kilit (sayaç bellekte, zaman `TimeProvider` UTC). `Oturum` uygulama boyunca tek; `Ac`/`Kapat` internal olduğu için oturum yalnızca giriş servisiyle açılır. Yetki matrisi yalnızca `YetkiServisi`'nde (`Dictionary<Rol, HashSet<Islem>>`):

| Islem | Admin | Personel |
|---|:-:|:-:|
| PersonelYonetimi | ✅ | ❌ |
| MusteriIslemleri | ✅ | ✅ |
| EkipmanYonetimi (ekle, sil, genel güncelle) | ✅ | ❌ |
| FiyatGuncelleme | ✅ | ❌ |
| EkipmanDurumDegistirme | ✅ | ✅ |
| KiralamaIslemleri | ✅ | ✅ |
| OdemeAlma | ✅ | ✅ |
| RaporGoruntuleme (dashboard dahil) | ✅ | ❌ |

`CrudServisiTemel` Ekle/Guncelle/Sil'de servisin `YonetimIslemi` yetkisini kontrol eder; kendi yetkisi olan özel işlemler yetkisiz iç akış `GuncellemeAkisi`'nı kullanır. Okuma metotları yetkiye tabi değil. `RezervasyonOlustur`/`KiralamaBaslat` artık `personelId` almaz, personel oturumdan gelir. Raporlar `IRepository.Query()` (AsNoTracking) ile veritabanında çalışır; gelir = ödeme tarihine göre `Odemeler`. En çok kiralananlarda süre ve doluluk oranında çalışma saati kesişimi bellekte hesaplanır (tarih farkı SQLite'ta çevrilemiyor). Çalışma saatleri `IsletmeAyarlari` ile verilir (varsayılan 09:00–19:00). Sorguların SQL Server'da da SQL'e çevrildiği LocalDB'de doğrulandı. 216 test başarılı.

## 5. Yapay zeka servisi — ✅ Tamamlandı
IAiOneriServisi arayüzü; Claude API'yi HttpClient ile çağıran gerçek implementasyon (API anahtarı repoya girmez) ve internet yokken çalışan sahte implementasyon. Senaryo: müşterinin serbest metin tarifi + müsait ekipman listesi → önerilen ekipman paketi (JSON yanıt).

**Not (03.10.2026):** Önce iki temizlik: `ponytail:` yorum etiketleri `// NOT:` oldu; `.gitattributes` eklendi (depoda LF, çalışma kopyasında CRLF). Business'ta `Oneri/` klasörü: `IAiOneriSaglayici` (ham öneri üretir), `OneriServisi` (`IOneriServisi`, `KiralamaIslemleri` yetkisi) ve record DTO'lar. Yeni `SuSporKiralama.YapayZeka` projesi (YapayZeka → Business; Business onu bilmez): `ClaudeAiOneriSaglayici`, `KuralTabanliOneriSaglayici`, `YedekliOneriSaglayici`, `ClaudeAyarlari`. Desenler: **Strategy** (üç sağlayıcı aynı arayüzü uygular, OneriServisi hangisi olduğunu bilmez) ve **Decorator** (`YedekliOneriSaglayici` aynı arayüzü uygulayıp asıl sağlayıcıyı sarar, zaman aşımı + yedeğe geçiş davranışı ekler). `OneriServisi` sağlayıcıya güvenmez: listede olmayan kodu (kayıtlı değil / o aralıkta müsait değil ayrımıyla), tekrarı atar, kodları büyük harfe çevirir, kişi sayısını 1–20'ye çeker, her kişiye bir can yeleği olacak şekilde müsait yeleklerden tamamlar; yelek ya da taşıma kapasitesi (SUP 1, kano kapasitesi kadar) yetmiyorsa `EkipmanYeterli=false` ve sebebi yazar (tekne otomatik eklenmez). Ücret ve depozito `UcretHesapla` ile hesaplanır. Aralıkta hiç müsait ekipman yoksa sağlayıcı çağrılmaz. Claude: `POST /v1/messages`, `x-api-key` istek başına header'da, `anthropic-version: 2023-06-01`, Türkçe sistem promptu, yanıt `output_config.format` JSON şemasıyla zorlanır ve istemcide de katı ayrıştırılır. Varsayılan model `claude-sonnet-5-5` + `effort: low`: Haiku 4.5 daha ucuz ama 15.10.2026'dan sonra emekliye ayrılabiliyor; model/efor ayardan değişir. Anahtar önce `ANTHROPIC_API_KEY`, yoksa appsettings.json; `ClaudeAyarlari.ToString()` ve hata mesajları anahtarı yazmaz. Yedekli sağlayıcı anahtar yokken, HTTP/bağlantı hatasında, geçersiz yanıtta ve zaman aşımında (varsayılan 20 sn, `TimeProvider` ile) kural tabanlıya geçip uyarı yazar; çağıranın iptali ve beklenmeyen hatalar gizlenmez. Testler gerçek ağa çıkmaz (sahte `HttpMessageHandler`). Ortamda API anahtarı olmadığı için gerçek API denemesi yapılmadı. 286 test başarılı.

## 6. DevExpress kurulumu — ✅ Tamamlandı
Windows Forms UI projesi (net10.0-windows) DevExpress şablonuyla; giriş formu, RibbonForm ana form, tema.

**Not (16.10.2026):** `SuSporKiralama.UI` (net10.0-windows) eklendi; Business, DataAccess, YapayZeka ve Entities'e referans verir, Business UI'ı bilmez. Paketler (26.1.5, "DevExpress 26.1 Local"): `DevExpress.Win.Design` (resmî Designer/IDE entegrasyonu paketi; Ribbon, editörler, DocumentManager ve temaları içeren Win.Navigation'ı, ayrıca Grid, Charts ve Reporting'i geçişli getirir, bu yüzden 7–9. aşamalarda yeni DevExpress paketi gerekmez), `DevExpress.Images` (hazır SVG ikonlar), `Microsoft.Extensions.DependencyInjection` 10.0.12. DevExpress Documentation MCP bu oturumda yoktu, docs.devexpress.com erişime kapalıydı; API'ler paketlerdeki XML belge dosyalarından ve derlemeyle doğrulandı. DevExpress lisans dosyaları gitignore'a eklendi. Ön kontrolde EF Core SqlServer üzerinden gelen `System.Security.Cryptography.Pkcs` 9.0.11 için yeni NU1902/NU1903 güvenlik uyarıları görüldü; DataAccess'te 10.0.13'e sabitlendi.

Composition root `Program.cs`: `ValidateOnBuild` ve `ValidateScopes` açık; appsettings.json yoksa, bağlantı dizesi eksikse veya veritabanına ulaşılamazsa açıklayıcı mesajla kapanır. UI kendi `appsettings.json`'ını okur (ConnectionStrings, Claude, yeni `IsletmeAyarlari` bölümü); `ClaudeAyarlari.Yukle` aynen kullanılır.

| Kayıt | Ömür | Gerekçe |
|---|---|---|
| `Oturum` / `IOturum` (aynı nesne), `IYetkiServisi`, `GirisDenemeSayaci` | Singleton | Oturum, yetki ve kaba kuvvet sayacı uygulama boyunca tek |
| `TimeProvider`, `IsletmeAyarlari`, `ClaudeAyarlari`, `UygulamaBilgisi`, `FormAcici` | Singleton | Açılışta bir kez oluşturulur, durumsuz |
| `IAiOneriSaglayici` → `YedekliOneriSaglayici(Claude, KuralTabanli)` | Singleton | Sağlayıcılar durumsuz, tek HttpClient |
| `SuSporKiralamaDbContext`, `IRepository<>`, tüm Business servisleri | Scoped | Her form kendi DbContext'iyle çalışır; eski takip verisi formlar arasında karışmaz |
| `GirisFormu`, `AnaForm`, `BaslangicEkrani` | Transient | `FormAcici` her açılışta yeni kapsam açar, form dispose olunca kapsam da kapanır |

Kaba kuvvet sayacı `GirisServisi`'nden `GirisDenemeSayaci`'na taşındı (GirisServisi DbContext'e bağlı olduğu için scoped); paylaşılan sayaçla kilidin yeni serviste de sürdüğünü gösteren test eklendi. Giriş akışı: Program'da döngü; giriş formu her seferinde yeni açılır (alanlar boş), başarılı girişte kapanır, ana form `Application.Run` ile çalışır; "Çıkış Yap" oturumu kapatıp ana formu (sekmeleriyle) kapatır ve giriş formu yeniden açılır, X ile kapatma uygulamayı kapatır. Ana form: Ribbon sayfaları Kiralama / Kayıtlar / Raporlar / Yapay Zeka / Sistem; buton → yetki ve SVG ikon eşlemesi `AnaForm.ButonlariHazirla`'daki tek tabloda (Kiralama butonları ve Ekipman Önerisi: KiralamaIslemleri, Ekipmanlar: EkipmanDurumDegistirme, Müşteriler: MusteriIslemleri, Personel: PersonelYonetimi, Dashboard: RaporGoruntuleme, tema ve çıkış herkese). Yetkisiz buton ve görünür butonu kalmayan sayfa gizlenir (Personel rolünde Personel butonu ve Raporlar sayfası). Yapılmamış ekranlar ortak yer tutucu mesajı verir. `EkraniAc<T>()` ekranı DocumentManager + TabbedView ile sekmede açar, açıksa öne getirir (7. aşamada kullanılacak); başlangıç sekmesi kapatılamaz. Durum çubuğu: personel ve rol, veritabanı adı, saat. İkonlar kodda atanır (Designer'da SVG ikon seçimi resx'e ikili veri yazar, elle üretilemiyor). Hata yönetimi: `HataYoneticisi` üç olayı bağlar; `IsKuraliException` mesajı uyarı olarak aynen, diğerleri genel mesaj + `logs/hata-yyyy-MM-dd.log` (API anahtarı loga yazılmaz). Tema varsayılan WXI; galeriden seçilen tema ve palet `%AppData%\SuSporKiralama\tema.json`'da hatırlanır. Uygulama UI Automation/MSAA ile çalıştırılarak denendi: açılış ve DI doğrulaması, hatalı giriş, 5 hatada kilit, Admin/Personel buton görünürlüğü, yer tutucu, çıkış, tema hatırlama. 287 test başarılı.

## 7. CRUD formları — ✅ Tamamlandı
GridControl listeleri, LayoutControl ile ekle/düzenle formları, DXValidationProvider ile doğrulama.

**Not (17.10.2026):** Ekipman, Müşteri ve Personel ekranları ana formda sekme olarak açılır; üç buton için yer tutucu kalktı. **Ortak liste davranışı kompozisyonla:** Designer generic/soyut taban sınıftan türeyen ekranları açamadığı ve DevExpress grid'in görsel kalıtımı sütun düzenlemeyi kısıtladığı için taban sınıf yok; her ekran kendi GridControl'ünü ve sütunlarını kendi Designer'ında tutar, ortak davranış statik `EkranYardimcisi`'nden tek satırla bağlanır (salt okunur grid, veri satırına çift tıkla düzenle, boş listede "Kayıt yok", seçili kaydı Id ile koruyarak yükleme, Excel'e aktarma, silme onayı, iş kuralı mesajı). Arama paneli (Ctrl+F), filtre, sıralama ve gruplama GridView'da varsayılan olarak açık. Grid'e entity değil salt okunur satır record'ları (`Satirlar.cs`) bağlanır: enum'lar Türkçe adlarıyla (`Metinler`) gelir, personel satırında şifre hash'i hiç yoktur.

**DI kapsamı kararı:** liste ekranları uzun ömürlü DbContext tutmaz; her işlem (yenile, sil, fiyat, durum, aktiflik) `FormAcici.Getir/Calistir` ile kendi kısa kapsamında çalışır. Sebep: başka bir DbContext'te kaydedilen kaydı, listenin DbContext'i takip ediyorsa EF Core tekrar sorguda güncellemez (liste eski veriyi gösterir) ve o eski nesneyle yapılan sonraki `Update` diğer değişikliği ezer. Düzenleme formları `FormAcici.Olustur` ile kendi kapsamında açılır, kaydı Id ile kendi bağlamından okur; editörlerdeki değerler entity'ye yalnızca Kaydet'te kopyalanır. İş kuralı hatasında (veya entity'nin `ArgumentException`'ında) form açık kalır, veriler kaybolmaz; İptal'de bağlam atılır.

**Ribbon:** bağlamsal "Kayıt İşlemleri" kategorisi (Ekle, Düzenle, Sil, Yenile, Excel'e Aktar + İşlemler grubu: Fiyat Güncelle, Durum Değiştir, Şifre Değiştir, Aktif/Pasif Yap). Ekranlar `IKayitEkrani`'yı uygular (`IslemGorunur`, `Calistir`); aktif sekme değişince butonlar ekranın işlemlerine ve `IYetkiServisi.YetkisiVarMi`'ye göre gösterilir, boş grup gizlenir, tıklama aktif ekrana iletilir. Business yetkiyi ayrıca denetler.

Ekipman: Durum sütunu temaya uyumlu `DXSkinColors.FillColors` renkleriyle (Müsait/Kirada/Bakımda/Hizmet Dışı), grid üstünde tür ve durum hızlı filtreleri (sütun filtresi). Eklemede önce tür seçilir, yalnızca o türün LayoutControl grubu görünür (`ValidateHiddenControls = false`), birim ücret etiketi türe göre değişir; düzenlemede tür kilitli. Fiyat Güncelle ve Durum Değiştir (Kirada listede yok) `XtraInputBox` ile. Müşteri: arama kutusu Business'taki `Ara`'yı 300 ms gecikmeyle çağırır, telefon maskesi `0000 000 00 00`. Personel: eklemede maskeli şifre + tekrar (eşleşme `CompareAgainstControlValidationRule`), şifre değiştirme ayrı form, aktif/pasif onaylı. Business'a eklenen kural: oturumdaki personel kendi hesabını pasifleştiremez/silemez (`PersonelServisi` artık `IOturum` alır, 3 test). Uygulama UI Automation/MSAA ve pencereye gönderilen mesajlarla denendi (masaüstü görüntüsü alınmadı): üç ekranın ekle/düzenle/sil akışları, aynı kod/telefon, geçmişi olan kayıt, son yönetici, kendi hesabı, şifre eşleşmesi, filtreler, Excel çıktısı ve Personel rolünde buton görünürlüğü. Çift tıkla düzenleme sentetik mesajla tetiklenemediği için elle denenecek. DevExpress arayüz metinleri (ör. "Drag a column header...", Yes/No) İngilizce; Türkçe yerelleştirme bu aşamanın kapsamında değil. 290 test başarılı.

## 8. Kiralama başlatma ve iade ekranları
ChartControl ile dashboard.

## 9. Raporlar ve yapay zeka ekranı
XtraReports ile yazdırılabilir kiralama fişi (PDF dışa aktarma), yapay zeka öneri ekranı.

## 10. Son düzenlemeler
Genel hata yönetimi, EF Core'un ürettiği SQL'i loglayabilme, README'nin son hali, yazılı rapor için ER şeması ve sınıf açıklamaları.
