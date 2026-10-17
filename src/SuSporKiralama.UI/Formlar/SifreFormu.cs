using DevExpress.XtraEditors;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Formlar;

/// <summary>Seçili personelin şifresini değiştirir; yeni şifre iki kez girilir, Business hash'leyip kaydeder.</summary>
public partial class SifreFormu : XtraForm
{
    private readonly IPersonelServisi _servis;
    private int _personelId;

    public SifreFormu(IPersonelServisi servis)
    {
        _servis = servis;
        InitializeComponent();
    }

    public void Yukle(int personelId, string adSoyad)
    {
        _personelId = personelId;
        Text = $"Şifre Değiştir: {adSoyad}";
    }

    private void btnKaydet_Click(object sender, EventArgs e)
    {
        if (!dxValidationProvider.Validate())
            return;
        if (EkranYardimcisi.Dene(() => _servis.SifreDegistir(_personelId, txtSifre.Text)))
            DialogResult = DialogResult.OK;
    }
}
