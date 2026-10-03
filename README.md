# SuSporKiralama

Su sporları ekipmanı (SUP board, kano, can yeleği) kiralama otomasyonu.
Nesne Tabanlı Programlama dersi dönem projesi; katmanlı mimaride C# ile geliştirilmektedir.

## Kullanılan teknolojiler

| Alan | Teknoloji |
|---|---|
| Dil / platform | C#, .NET 10 |
| Veri erişimi | Entity Framework Core 10 (Code First, Fluent API, migration) |
| Veritabanı | SQL Server LocalDB |
| Arayüz | Windows Forms + DevExpress — *geliştirilecek* |
| Yapay zeka | Claude API (Messages API, HttpClient) ile ekipman önerisi; internet yoksa kural tabanlı yedek |

## Kurulum

Gerekenler: .NET 10 SDK, SQL Server Express LocalDB (Visual Studio Installer → Tek tek bileşenler → "SQL Server Express LocalDB").

```powershell
# 1. LocalDB'nin kurulu olduğunu kontrol edin (MSSQLLocalDB görünmeli)
sqllocaldb info

# 2. Proje araçlarını (dotnet-ef) geri yükleyin
dotnet tool restore

# 3. Bağlantı ayarını oluşturun (appsettings.json repoda yoktur)
Copy-Item src/SuSporKiralama.DataAccess/appsettings.example.json src/SuSporKiralama.DataAccess/appsettings.json

# 4. Derleyin, veritabanını oluşturun ve örnek veriyi yükleyin
dotnet build
dotnet ef database update -p src/SuSporKiralama.DataAccess
```

`database update` örnek veriyi de yükler; tekrar çalıştırılırsa veri ikinci kez eklenmez.

### Örnek kullanıcılar (geliştirme amaçlı)

> Bu şifreler yalnızca geliştirme ve sunum içindir. Veritabanında düz hâlleri değil, PBKDF2 özetleri saklanır.

| Kullanıcı adı | Şifre | Rol |
|---|---|---|
| admin | Admin123! | Admin |
| personel | Personel123! | Personel |

## Klasör yapısı

```
SuSporKiralama.slnx
dotnet-tools.json               dotnet-ef yerel araç sürümü
docs/PLAN.md                    10 aşamalı yol haritası
src/
├─ SuSporKiralama.Entities/     Entity sınıfları, enum'lar, soyut sınıflar
│  └─ Soyut/                    BaseEntity, Ekipman
├─ SuSporKiralama.DataAccess/   EF Core katmanı
│  ├─ Konfigurasyonlar/         Her entity için Fluent API ayarları
│  ├─ Repository/               IRepository<T>, EfRepository<T>
│  ├─ Guvenlik/                 PBKDF2 şifre özeti
│  ├─ Seed/                     Örnek veri
│  └─ Migrations/
├─ SuSporKiralama.Business/     İş kuralları, servisler, yetki, raporlar
│  └─ Oneri/                    Öneri servisi, sağlayıcı arayüzü (IAiOneriSaglayici), DTO'lar
└─ SuSporKiralama.YapayZeka/    Claude, kural tabanlı ve yedekli öneri sağlayıcıları
tests/SuSporKiralama.Tests/     xUnit testleri (bellekte SQLite, sahte HTTP)
```

Referans yönü: `DataAccess → Entities`, `Business → DataAccess + Entities`, `YapayZeka → Business`. Business, YapayZeka'yı bilmez. Arayüz projesi ileride eklenecek.

## Yapay zeka özelliği

Personel, müşterinin ihtiyacını serbest metinle (ör. "2 kişiyiz, ilk kez deneyeceğiz, sakin bir şey istiyoruz") ve kiralama aralığını girer; isteğe bağlı olarak kayıtlı bir müşteri seçer (deneyim seviyesi dikkate alınır). Sistem o aralıkta müsait ekipmanlardan bir paket, kısa bir açıklama ve güvenlik notu önerir. Öneri otomatik olarak kiralamaya dönüşmez; personel onaylar.

**Nasıl çalışır**

1. `OneriServisi` müsait ekipmanları `MusaitlikServisi`'nden alır ve sağlayıcıya müşteri metni, deneyim seviyesi ve ekipman özetlerini (kod, tür, SUP uzunluk/taşıma/tip, kano kapasitesi, yelek bedeni, birim ücret) verir.
2. Sağlayıcı (`IAiOneriSaglayici`) yalnızca ham öneri döndürür: ekipman kodları, kişi sayısı, açıklama, güvenlik notu. Claude'a Türkçe bir sistem promptu ve JSON şeması gönderilir (`output_config.format`); yanıt şemaya uymazsa hata sayılır.
3. `OneriServisi` sağlayıcıya güvenmez: müsait olmayan / listede olmayan / tekrar eden kodları atar, her kişiye bir can yeleği olacak şekilde eksikleri tamamlar, ekipman yetersizse bunu açıkça yazar. Ücret ve depozito modelden alınmaz, `UcretHesapla` ile hesaplanır. Yapılan her düzeltme sonuçta listelenir.
4. `YedekliOneriSaglayici` önce Claude'u dener; anahtar yoksa, HTTP hatasında, geçersiz yanıtta ya da zaman aşımında (varsayılan 20 sn) `KuralTabanliOneriSaglayici`'ya geçer ve sonuçta "Yapay zeka şu an kullanılamıyor, kural tabanlı öneri gösteriliyor." yazar.

Bileşenler DI container olmadan şöyle bağlanır (arayüz katmanında yapılacak):

```csharp
var ayarlar = ClaudeAyarlari.Yukle(AppContext.BaseDirectory);
IAiOneriSaglayici saglayici = new YedekliOneriSaglayici(
    new ClaudeAiOneriSaglayici(new HttpClient(), ayarlar),
    new KuralTabanliOneriSaglayici(),
    ayarlar.ZamanAsimi);
var oneriServisi = new OneriServisi(musaitlikServisi, musteriRepo, ekipmanRepo, saglayici, yetkiServisi);
```

**API anahtarı nasıl ayarlanır**

Anahtar repoya, loga ya da hata mesajına yazılmaz. İki yol vardır (ilk bulunan kullanılır):

```powershell
# 1. Ortam değişkeni (önerilen)
setx ANTHROPIC_API_KEY "sk-ant-..."

# 2. gitignore'daki appsettings.json içindeki "Claude" bölümü
#    (appsettings.example.json'ı kopyalayıp ApiAnahtari alanını doldurun)
```

| Ayar | Varsayılan | Açıklama |
|---|---|---|
| `Claude:ApiAnahtari` | boş | `ANTHROPIC_API_KEY` tanımlıysa o kullanılır |
| `Claude:Model` | `claude-sonnet-5-5` | Hızlı ve görece ucuz. Daha ucuz `claude-haiku-4-5` 15.10.2026'dan sonra emekliye ayrılabilir |
| `Claude:Efor` | `low` | Haiku 4.5 gibi effort desteklemeyen modellerde boş bırakın |
| `Claude:ZamanAsimiSaniye` | `20` | Bu süre dolarsa kural tabanlı öneriye geçilir |

**Anahtar yoksa ne olur?** Uygulama çalışmaya devam eder: Claude'a istek gönderilmez, öneri kural tabanlı sağlayıcıdan gelir (metindeki sayılar, "ilk kez", "sakin", "kano", "çocuk" gibi ipuçları ve deneyim seviyesiyle) ve sonuçta yapay zekanın kullanılamadığı belirtilir. Doğrulama, can yeleği kuralı ve ücret hesabı her iki durumda da aynıdır.

## Geliştirme süreci

Proje, dönem boyunca 10 aşamada geliştirilmektedir.

| # | Aşama | Durum |
|---|---|---|
| 1 | İskelet + veritabanı | ✅ Tamamlandı |
| 2 | Business CRUD + test projesi | ✅ Tamamlandı |
| 3 | Kiralama akışı + ücret/kiralama testleri | ✅ Tamamlandı |
| 4 | Giriş ve raporlama | ✅ Tamamlandı |
| 5 | Yapay zeka servisi | ✅ Tamamlandı |
| 6 | DevExpress kurulumu | ⏳ Planlandı |
| 7 | CRUD formları | ⏳ Planlandı |
| 8 | Kiralama/iade ekranları, dashboard | ⏳ Planlandı |
| 9 | Kiralama fişi (XtraReports), yapay zeka öneri ekranı | ⏳ Planlandı |
| 10 | Son düzenlemeler | ⏳ Planlandı |

Her aşamanın kapsamı ve tamamlanma notları için: [docs/PLAN.md](docs/PLAN.md)
