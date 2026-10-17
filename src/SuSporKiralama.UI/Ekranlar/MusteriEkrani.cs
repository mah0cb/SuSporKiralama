using DevExpress.XtraEditors;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.UI.Altyapi;
using SuSporKiralama.UI.Formlar;

namespace SuSporKiralama.UI.Ekranlar;

/// <summary>
/// Müşteri listesi. Arama kutusu Business'taki Ara metodunu kullanır (ad, soyad, telefon);
/// grid'in Ctrl+F arama paneli de ayrıca açıktır. Her işlem kendi kısa kapsamında çalışır (FormAcici).
/// </summary>
public partial class MusteriEkrani : XtraUserControl, IKayitEkrani
{
    private readonly FormAcici _formAcici;
    private readonly IYetkiServisi _yetki;

    public MusteriEkrani(FormAcici formAcici, IYetkiServisi yetki)
    {
        _formAcici = formAcici;
        _yetki = yetki;
        InitializeComponent();
        Text = "Müşteriler";
        EkranYardimcisi.ListeHazirla(gridView, Duzenle);
    }

    public bool IslemGorunur(KayitIslemi islem) => islem switch
    {
        KayitIslemi.Ekle or KayitIslemi.Duzenle or KayitIslemi.Sil => _yetki.YetkisiVarMi(Islem.MusteriIslemleri),
        KayitIslemi.Yenile or KayitIslemi.ExcelAktar => true,
        _ => false
    };

    public void Calistir(KayitIslemi islem)
    {
        switch (islem)
        {
            case KayitIslemi.Ekle: FormuAc(null); break;
            case KayitIslemi.Duzenle: Duzenle(); break;
            case KayitIslemi.Sil: Sil(); break;
            case KayitIslemi.Yenile: Yenile(); break;
            case KayitIslemi.ExcelAktar: EkranYardimcisi.ExcelAktar(gridControl, "Musteriler"); break;
        }
    }

    private void MusteriEkrani_Load(object sender, EventArgs e) => Yenile();

    private void Yenile(int? seciliId = null)
    {
        var metin = txtAra.Text;
        var satirlar = _formAcici.Getir((IMusteriServisi s) => s.Ara(metin).ConvertAll(MusteriSatiri.Olustur));
        EkranYardimcisi.Yukle(gridView, satirlar, seciliId);
    }

    private void Duzenle()
    {
        if (IslemGorunur(KayitIslemi.Duzenle) && EkranYardimcisi.SeciliId(gridView) is { } id)
            FormuAc(id);
    }

    private void FormuAc(int? id)
    {
        using var form = _formAcici.Olustur<MusteriFormu>();
        if (EkranYardimcisi.Dene(() => form.Yukle(id)) && form.ShowDialog(this) == DialogResult.OK)
            Yenile(form.KayitId);
    }

    private void Sil()
    {
        if (gridView.GetFocusedRow() is not MusteriSatiri satir
            || !EkranYardimcisi.Onayla($"{satir.AdSoyad} adlı müşteri silinsin mi?"))
            return;

        // Kiralama geçmişi olan müşteride Business "pasif yapabilirsiniz" mesajını verir.
        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IMusteriServisi s) => s.Sil(satir.Id))))
            Yenile();
    }

    // Her tuş vuruşunda veritabanına gidilmesin: yazma 300 ms durunca aranır.
    private void txtAra_EditValueChanged(object sender, EventArgs e)
    {
        timerArama.Stop();
        timerArama.Start();
    }

    private void timerArama_Tick(object sender, EventArgs e)
    {
        timerArama.Stop();
        Yenile();
    }
}
