using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;

namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Enum değerlerinin ve ekipman türlerinin kullanıcıya gösterilen Türkçe adları.
/// Kod içindeki adlar (ör. HizmetDisi) yalnızca burada okunabilir metne (Hizmet Dışı) çevrilir.
/// </summary>
public static class Metinler
{
    public static string Ad(Enum deger) => deger switch
    {
        EkipmanDurumu.Musait => "Müsait",
        EkipmanDurumu.Kirada => "Kirada",
        EkipmanDurumu.Bakimda => "Bakımda",
        EkipmanDurumu.HizmetDisi => "Hizmet Dışı",
        DeneyimSeviyesi.Baslangic => "Başlangıç",
        DeneyimSeviyesi.Orta => "Orta",
        DeneyimSeviyesi.Ileri => "İleri",
        Rol.Admin => "Yönetici",
        Rol.Personel => "Personel",
        SupBoardTipi.Sisme => "Şişme",
        SupBoardTipi.Sert => "Sert",
        _ => deger.ToString() // ör. beden: S, M, L, XL
    };

    public static string TurAdi(Ekipman ekipman) => ekipman switch
    {
        SupBoard => "SUP Board",
        Kano => "Kano",
        CanYelegi => "Can Yeleği",
        _ => ekipman.GetType().Name
    };

    /// <summary>Türe özel bilgiler tek satırda, ör. "320 cm · 120 kg · Şişme".</summary>
    public static string Ozellikler(Ekipman ekipman) => ekipman switch
    {
        SupBoard s => $"{s.UzunlukCm} cm · {s.MaxTasimaKg} kg · {Ad(s.Tip)}",
        Kano k => $"{k.KisiKapasitesi} kişilik",
        CanYelegi y => $"Beden {y.Beden}",
        _ => ""
    };

    /// <summary>Açılır listeyi enum değerleriyle doldurur: görünen metin Türkçe ad, değer enum'un kendisi.</summary>
    public static void Doldur<T>(ImageComboBoxEdit liste, params T[] haric) where T : struct, Enum
    {
        foreach (var deger in Enum.GetValues<T>())
            if (Array.IndexOf(haric, deger) < 0)
                liste.Properties.Items.Add(new ImageComboBoxItem(Ad(deger), deger));
    }
}
