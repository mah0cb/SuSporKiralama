using System.Globalization;
using System.IO;
using System.Net.Http;
using DevExpress.XtraEditors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Oneri;
using SuSporKiralama.Business.Raporlar;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.DataAccess;
using SuSporKiralama.DataAccess.Repository;
using SuSporKiralama.UI.Altyapi;
using SuSporKiralama.UI.Ekranlar;
using SuSporKiralama.UI.Formlar;
using SuSporKiralama.YapayZeka;

namespace SuSporKiralama.UI;

/// <summary>
/// Composition root: uygulamanın tüm nesneleri yalnızca burada birbirine bağlanır.
/// Formlar ve servisler bağımlılıklarını constructor'dan alır, kimin oluşturduğunu bilmez.
/// </summary>
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        HataYoneticisi.Baglan();

        // Açılıştaki hatalar (bozuk appsettings.json, hatalı DI kaydı) henüz ThreadException'a
        // gelmez; burada yakalanıp aynı şekilde işlenir.
        try
        {
            Baslat();
        }
        catch (Exception hata)
        {
            HataYoneticisi.Isle(hata);
        }
    }

    private static void Baslat()
    {
        // Tema ilk iş uygulanır; açılıştaki hata mesajları da aynı temayla görünür.
        TemaAyari.Uygula();

        var klasor = AppContext.BaseDirectory;
        if (!File.Exists(Path.Combine(klasor, "appsettings.json")))
        {
            AcilisHatasi("appsettings.json bulunamadı.\n\nsrc/SuSporKiralama.UI klasöründeki appsettings.example.json " +
                  "dosyasını appsettings.json adıyla kopyalayıp projeyi yeniden derleyin.");
            return;
        }

        var ayarlar = new ConfigurationBuilder()
            .SetBasePath(klasor)
            .AddJsonFile("appsettings.json")
            .Build();

        var baglantiDizesi = ayarlar.GetConnectionString("SuSporKiralama");
        if (string.IsNullOrWhiteSpace(baglantiDizesi))
        {
            AcilisHatasi("appsettings.json içinde 'ConnectionStrings:SuSporKiralama' tanımlı değil.");
            return;
        }

        // ValidateOnBuild: eksik/hatalı kayıt uygulama açılırken yakalanır, ilgili ekran açılınca değil.
        // ValidateScopes: singleton bir nesnenin scoped bir nesneyi (ör. DbContext) tutması engellenir.
        using var servisler = ServisleriKaydet(ayarlar, baglantiDizesi, klasor)
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        if (!VeritabaniErisilebilir(servisler))
        {
            AcilisHatasi("Veritabanına bağlanılamadı.\n\nappsettings.json'daki bağlantı dizesini ve LocalDB'nin kurulu " +
                  "olduğunu kontrol edin (komut satırında: sqllocaldb info). Veritabanı henüz oluşturulmadıysa: " +
                  "dotnet ef database update -p src/SuSporKiralama.DataAccess");
            return;
        }

        var formAcici = servisler.GetRequiredService<FormAcici>();

        // Giriş → ana form döngüsü. "Çıkış Yap" ile kapanan ana formdan sonra giriş formu yeniden açılır;
        // ana form X ile kapatılırsa ya da giriş vazgeçilirse uygulama kapanır.
        while (true)
        {
            // Giriş formu her seferinde yeni oluşturulur: alanlar boş gelir, kendi kapsamı olur.
            using (var girisFormu = formAcici.Olustur<GirisFormu>())
            {
                if (girisFormu.ShowDialog() != DialogResult.OK)
                    return;
            }

            using var anaForm = formAcici.Olustur<AnaForm>();
            Application.Run(anaForm);
            if (!anaForm.CikisYapildi)
                return;
        }
    }

    private static ServiceCollection ServisleriKaydet(IConfiguration ayarlar, string baglantiDizesi, string klasor)
    {
        var servisler = new ServiceCollection();

        // --- Uygulama boyunca tek (singleton) ---
        // Oturum tek nesnedir; GirisServisi somut Oturum'u (Ac/Kapat), diğerleri IOturum'u alır.
        servisler.AddSingleton<Oturum>();
        servisler.AddSingleton<IOturum>(sp => sp.GetRequiredService<Oturum>());
        servisler.AddSingleton<IYetkiServisi, YetkiServisi>();
        servisler.AddSingleton<GirisDenemeSayaci>();
        servisler.AddSingleton(TimeProvider.System);
        servisler.AddSingleton(IsletmeAyarlariniOku(ayarlar));
        servisler.AddSingleton(new UygulamaBilgisi(new SqlConnectionStringBuilder(baglantiDizesi).InitialCatalog));
        servisler.AddSingleton<FormAcici>();

        // Yapay zeka: Claude denenir, olmazsa kural tabanlı öneriye geçilir (Decorator).
        // Sağlayıcılar durumsuz, HttpClient uygulama boyunca tek olduğu için singleton.
        var claudeAyarlari = ClaudeAyarlari.Yukle(klasor);
        HataYoneticisi.GizliDeger = claudeAyarlari.ApiAnahtari; // anahtar hiçbir koşulda loga girmesin
        servisler.AddSingleton(claudeAyarlari);
        servisler.AddSingleton<IAiOneriSaglayici>(sp => new YedekliOneriSaglayici(
            new ClaudeAiOneriSaglayici(new HttpClient(), claudeAyarlari),
            new KuralTabanliOneriSaglayici(),
            claudeAyarlari.ZamanAsimi,
            sp.GetRequiredService<TimeProvider>()));

        // --- Her form/ekran kapsamında (scoped): kendi DbContext'i ve ona bağlı nesneler ---
        servisler.AddDbContext<SuSporKiralamaDbContext>(o => o.UseSqlServer(baglantiDizesi));
        servisler.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        servisler.AddScoped<IGirisServisi, GirisServisi>();
        servisler.AddScoped<IMusteriServisi, MusteriServisi>();
        servisler.AddScoped<IEkipmanServisi, EkipmanServisi>();
        servisler.AddScoped<IPersonelServisi, PersonelServisi>();
        servisler.AddScoped<IMusaitlikServisi, MusaitlikServisi>();
        servisler.AddScoped<IKiralamaServisi, KiralamaServisi>();
        servisler.AddScoped<IRaporServisi, RaporServisi>();
        servisler.AddScoped<IOneriServisi, OneriServisi>();

        // --- Formlar: her açılışta yeni nesne, FormAcici'nin açtığı kapsamın içinden ---
        servisler.AddTransient<GirisFormu>();
        servisler.AddTransient<AnaForm>();
        servisler.AddTransient<BaslangicEkrani>();
        servisler.AddTransient<EkipmanEkrani>();
        servisler.AddTransient<EkipmanFormu>();
        servisler.AddTransient<MusteriEkrani>();
        servisler.AddTransient<MusteriFormu>();
        servisler.AddTransient<PersonelEkrani>();
        servisler.AddTransient<PersonelFormu>();
        servisler.AddTransient<SifreFormu>();

        return servisler;
    }

    // Bölüm yoksa varsayılan çalışma saatleri (09:00–19:00) kullanılır.
    private static IsletmeAyarlari IsletmeAyarlariniOku(IConfiguration ayarlar)
    {
        var bolum = ayarlar.GetSection("IsletmeAyarlari");
        var varsayilan = IsletmeAyarlari.Varsayilan;
        return new IsletmeAyarlari(
            bolum["AcilisSaati"] is { } acilis ? TimeOnly.Parse(acilis, CultureInfo.InvariantCulture) : varsayilan.AcilisSaati,
            bolum["KapanisSaati"] is { } kapanis ? TimeOnly.Parse(kapanis, CultureInfo.InvariantCulture) : varsayilan.KapanisSaati);
    }

    private static bool VeritabaniErisilebilir(IServiceProvider servisler)
    {
        using var scope = servisler.CreateScope();
        try
        {
            return scope.ServiceProvider.GetRequiredService<SuSporKiralamaDbContext>().Database.CanConnect();
        }
        catch (Exception)
        {
            return false; // ör. bağlantı dizesi biçimi bozuk
        }
    }

    private static void AcilisHatasi(string mesaj) =>
        XtraMessageBox.Show(mesaj, "Su Sporları Kiralama", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
