using DevExpress.XtraEditors;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Istisnalar;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Formlar;

/// <summary>
/// Kullanıcı adı ve şifreyle giriş. Enter = Giriş (AcceptButton), Esc = Kapat (CancelButton).
/// Başarılı girişte DialogResult.OK ile kapanır; ana formu Program açar.
/// Hata mesajları (hatalı giriş, pasif hesap, kilit) servisten geldiği gibi gösterilir.
/// </summary>
public partial class GirisFormu : XtraForm
{
    private readonly IGirisServisi _girisServisi;

    public GirisFormu(IGirisServisi girisServisi)
    {
        _girisServisi = girisServisi;
        InitializeComponent();
    }

    private void btnGiris_Click(object sender, EventArgs e)
    {
        try
        {
            _girisServisi.GirisYap(txtKullaniciAdi.Text, txtSifre.Text);
            DialogResult = DialogResult.OK;
        }
        catch (IsKuraliException hata)
        {
            HataYoneticisi.Isle(hata);
            txtSifre.Text = "";
            txtSifre.Focus();
        }
    }
}
