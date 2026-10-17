using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Formlar;

/// <summary>
/// Personel ekleme ve düzenleme. Eklemede şifre ve tekrarı istenir (maskeli, eşleşmesi DXValidationProvider
/// ile denetlenir); şifre Business'ta hash'lenir. Düzenlemede şifre grubu gizlenir: şifre yalnızca
/// "Şifre Değiştir" işlemiyle değişir. Aktif/pasif de ayrı işlemdir.
/// </summary>
public partial class PersonelFormu : XtraForm
{
    private readonly IPersonelServisi _servis;
    private Personel? _personel; // null: yeni kayıt

    public int KayitId { get; private set; }

    public PersonelFormu(IPersonelServisi servis)
    {
        _servis = servis;
        InitializeComponent();
        Metinler.Doldur<Rol>(cmbRol);
        cmbRol.EditValue = Rol.Personel;
    }

    /// <summary>id null ise yeni personel, değilse o personel düzenlenir.</summary>
    public void Yukle(int? id)
    {
        if (id is null)
        {
            Text = "Yeni Personel";
            return;
        }

        _personel = _servis.IdIleGetir(id.Value);
        Text = $"Personel Düzenle: {_personel.AdSoyad}";
        txtAdSoyad.Text = _personel.AdSoyad;
        txtKullaniciAdi.Text = _personel.KullaniciAdi;
        cmbRol.EditValue = _personel.Rol;
        grpSifre.Visibility = LayoutVisibility.Never; // gizli alanlar doğrulanmaz
    }

    private void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!dxValidationProvider.Validate())
            return;
        if (EkranYardimcisi.Dene(Kaydet))
            DialogResult = DialogResult.OK;
    }

    private void Kaydet()
    {
        var rol = (Rol)cmbRol.EditValue;
        if (_personel is null)
        {
            KayitId = _servis.Ekle(txtAdSoyad.Text, txtKullaniciAdi.Text, txtSifre.Text, rol).Id;
            return;
        }

        // Son aktif yöneticinin rolü düşürülürse Business reddeder ve mesajı gösterilir.
        _personel.AdSoyad = txtAdSoyad.Text;
        _personel.KullaniciAdi = txtKullaniciAdi.Text;
        _personel.Rol = rol;
        _servis.Guncelle(_personel);
        KayitId = _personel.Id;
    }
}
