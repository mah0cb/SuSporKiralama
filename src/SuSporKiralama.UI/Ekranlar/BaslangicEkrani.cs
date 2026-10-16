using DevExpress.XtraEditors;
using SuSporKiralama.Business.Guvenlik;

namespace SuSporKiralama.UI.Ekranlar;

/// <summary>Ana formda ilk açılan sekme: oturumdaki personelin adı, rolü ve kısa bir karşılama.</summary>
public partial class BaslangicEkrani : XtraUserControl
{
    public BaslangicEkrani(IOturum oturum)
    {
        InitializeComponent();
        Text = "Başlangıç";
        lblHosgeldiniz.Text = $"Hoş geldiniz, {oturum.AdSoyad}";
        lblRol.Text = $"Rolünüz: {oturum.Rol}";
    }
}
