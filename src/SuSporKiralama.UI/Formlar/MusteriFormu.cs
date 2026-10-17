using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Formlar;

/// <summary>
/// Müşteri ekleme ve düzenleme (aynı form; başlık ve davranış Yukle'ye verilen Id'ye göre değişir).
/// Form kendi DI kapsamında açılır, kaydı Id ile kendi DbContext'inden okur; İptal'de hiçbir şey yazılmaz.
/// İş kuralı hatasında form açık kalır, girilen veriler editörlerde durur.
/// </summary>
public partial class MusteriFormu : XtraForm
{
    private readonly IMusteriServisi _servis;
    private Musteri? _musteri; // null: yeni kayıt

    /// <summary>Kaydedilen müşterinin Id'si; liste bu kaydı seçili getirir.</summary>
    public int KayitId { get; private set; }

    public MusteriFormu(IMusteriServisi servis)
    {
        _servis = servis;
        InitializeComponent();
        Metinler.Doldur<DeneyimSeviyesi>(cmbDeneyim);
    }

    /// <summary>id null ise yeni müşteri, değilse o müşteri düzenlenir.</summary>
    public void Yukle(int? id)
    {
        if (id is null)
        {
            Text = "Yeni Müşteri";
            cmbDeneyim.EditValue = DeneyimSeviyesi.Baslangic;
            lciAktif.Visibility = LayoutVisibility.Never; // yeni müşteri her zaman aktif
            return;
        }

        _musteri = _servis.IdIleGetir(id.Value);
        Text = $"Müşteri Düzenle: {_musteri.AdSoyad}";
        txtAd.Text = _musteri.Ad;
        txtSoyad.Text = _musteri.Soyad;
        txtTelefon.Text = MaskeliTelefon(_musteri.Telefon);
        txtEposta.Text = _musteri.Eposta;
        cmbDeneyim.EditValue = _musteri.DeneyimSeviyesi;
        memoNotlar.Text = _musteri.Notlar;
        chkAktif.Checked = _musteri.AktifMi;
    }

    // Kayıtlı 05321234567 biçimi, maskedeki gibi "0532 123 45 67" gösterilir.
    private static string MaskeliTelefon(string t) =>
        t.Length == 11 ? $"{t[..4]} {t[4..7]} {t[7..9]} {t[9..]}" : t;

    private void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!dxValidationProvider.Validate())
            return;
        if (EkranYardimcisi.Dene(Kaydet))
            DialogResult = DialogResult.OK;
    }

    // Editörlerdeki değerler entity'ye yalnızca burada kopyalanır; Business telefonu 05XXXXXXXXX biçimine çevirir.
    private void Kaydet()
    {
        var deneyim = (DeneyimSeviyesi)cmbDeneyim.EditValue;
        var notlar = string.IsNullOrWhiteSpace(memoNotlar.Text) ? null : memoNotlar.Text.Trim();

        if (_musteri is null)
        {
            var yeni = new Musteri(txtAd.Text, txtSoyad.Text, txtTelefon.Text, deneyim)
            {
                Eposta = txtEposta.Text,
                Notlar = notlar
            };
            _servis.Ekle(yeni);
            KayitId = yeni.Id;
            return;
        }

        _musteri.Ad = txtAd.Text;
        _musteri.Soyad = txtSoyad.Text;
        _musteri.Telefon = txtTelefon.Text;
        _musteri.Eposta = txtEposta.Text;
        _musteri.DeneyimSeviyesi = deneyim;
        _musteri.Notlar = notlar;
        _musteri.AktifMi = chkAktif.Checked;
        _servis.Guncelle(_musteri);
        KayitId = _musteri.Id;
    }
}
