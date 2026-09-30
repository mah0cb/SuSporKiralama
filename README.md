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
| Yapay zeka | Claude API ile ekipman önerisi — *geliştirilecek* |

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
└─ SuSporKiralama.Business/     İş kuralları (sonraki aşamada)
```

Referans yönü: `DataAccess → Entities`, `Business → DataAccess + Entities`. Arayüz projesi ileride eklenecek.
