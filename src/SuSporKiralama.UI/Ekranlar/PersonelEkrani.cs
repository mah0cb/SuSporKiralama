using DevExpress.XtraEditors;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.UI.Altyapi;
using SuSporKiralama.UI.Formlar;

namespace SuSporKiralama.UI.Ekranlar;

/// <summary>
/// Personel listesi (yalnızca PersonelYonetimi yetkisi). Şifre hash'i satır modelinde hiç yoktur.
/// Son aktif yönetici ve oturumdaki kişinin kendi hesabı kuralları Business'tan gelen mesajla gösterilir.
/// </summary>
public partial class PersonelEkrani : XtraUserControl, IKayitEkrani
{
    private readonly FormAcici _formAcici;
    private readonly IYetkiServisi _yetki;

    public PersonelEkrani(FormAcici formAcici, IYetkiServisi yetki)
    {
        _formAcici = formAcici;
        _yetki = yetki;
        InitializeComponent();
        Text = "Personel";
        EkranYardimcisi.ListeHazirla(gridView, Duzenle);
    }

    public bool IslemGorunur(KayitIslemi islem) => islem switch
    {
        KayitIslemi.Ekle or KayitIslemi.Duzenle or KayitIslemi.Sil
            or KayitIslemi.SifreDegistir or KayitIslemi.AktiflikDegistir => _yetki.YetkisiVarMi(Islem.PersonelYonetimi),
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
            case KayitIslemi.ExcelAktar: EkranYardimcisi.ExcelAktar(gridControl, "Personel"); break;
            case KayitIslemi.SifreDegistir: SifreDegistir(); break;
            case KayitIslemi.AktiflikDegistir: AktiflikDegistir(); break;
        }
    }

    private void PersonelEkrani_Load(object sender, EventArgs e) => Yenile();

    private void Yenile(int? seciliId = null) =>
        EkranYardimcisi.Yukle(gridView,
            _formAcici.Getir((IPersonelServisi s) => s.TumunuGetir().ConvertAll(PersonelSatiri.Olustur)), seciliId);

    private PersonelSatiri? Secili => gridView.GetFocusedRow() as PersonelSatiri;

    private void Duzenle()
    {
        if (IslemGorunur(KayitIslemi.Duzenle) && Secili is { } satir)
            FormuAc(satir.Id);
    }

    private void FormuAc(int? id)
    {
        using var form = _formAcici.Olustur<PersonelFormu>();
        if (EkranYardimcisi.Dene(() => form.Yukle(id)) && form.ShowDialog(this) == DialogResult.OK)
            Yenile(form.KayitId);
    }

    private void Sil()
    {
        if (Secili is not { } satir || !EkranYardimcisi.Onayla($"{satir.AdSoyad} adlı personel silinsin mi?"))
            return;

        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IPersonelServisi s) => s.Sil(satir.Id))))
            Yenile();
    }

    private void SifreDegistir()
    {
        if (Secili is not { } satir)
            return;

        using var form = _formAcici.Olustur<SifreFormu>();
        form.Yukle(satir.Id, satir.AdSoyad);
        if (form.ShowDialog(this) == DialogResult.OK)
            XtraMessageBox.Show($"{satir.AdSoyad} adlı personelin şifresi değiştirildi.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void AktiflikDegistir()
    {
        if (Secili is not { } satir)
            return;

        var yeniDurum = !satir.Aktif;
        var soru = yeniDurum
            ? $"{satir.AdSoyad} adlı personel aktif yapılsın mı?"
            : $"{satir.AdSoyad} adlı personel pasif yapılsın mı? Pasif personel giriş yapamaz.";
        if (!EkranYardimcisi.Onayla(soru))
            return;

        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IPersonelServisi s) => s.AktiflikDegistir(satir.Id, yeniDurum))))
            Yenile();
    }
}
