using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.UI.Altyapi;
using SuSporKiralama.UI.Ekranlar;

namespace SuSporKiralama.UI.Formlar;

/// <summary>
/// Ribbon menülü ana form. Ekranlar formun içinde sekme olarak açılır (DocumentManager + TabbedView).
/// Butonların görünürlüğü oturumdaki personelin yetkisine göre belirlenir.
/// </summary>
public partial class AnaForm : RibbonForm
{
    private readonly IOturum _oturum;
    private readonly IYetkiServisi _yetki;
    private readonly IGirisServisi _girisServisi;
    private readonly FormAcici _formAcici;
    private readonly TimeProvider _zaman;

    /// <summary>Kullanıcı "Çıkış Yap" ile kapattıysa true; Program giriş formuna döner.</summary>
    public bool CikisYapildi { get; private set; }

    public AnaForm(IOturum oturum, IYetkiServisi yetki, IGirisServisi girisServisi,
        FormAcici formAcici, UygulamaBilgisi bilgi, TimeProvider zaman)
    {
        _oturum = oturum;
        _yetki = yetki;
        _girisServisi = girisServisi;
        _formAcici = formAcici;
        _zaman = zaman;
        InitializeComponent();

        ButonlariHazirla();
        KayitButonlariniHazirla();
        lblPersonel.Caption = $"{_oturum.AdSoyad} ({_oturum.Rol})";
        lblVeritabani.Caption = $"Veritabanı: {bilgi.VeritabaniAdi}";
        SaatiGuncelle();
    }

    /// <summary>
    /// Buton → gerekli yetki ve ikon eşlemesi; yalnızca burada tanımlanır. Yetkisi olmayan buton
    /// gizlenir, hiç görünür butonu kalmayan sayfa da gizlenir (ör. Personel rolünde Raporlar).
    /// Yetki null ise buton herkese açıktır. İkonlar DevExpress'in hazır SVG ikonlarıdır.
    /// </summary>
    private void ButonlariHazirla()
    {
        (BarButtonItem Buton, Islem? Yetki, string Ikon)[] butonlar =
        [
            (btnYeniKiralama, Islem.KiralamaIslemleri, "svgimages/actions/new.svg"),
            (btnRezervasyon, Islem.KiralamaIslemleri, "svgimages/scheduling/newappointment.svg"),
            (btnTeslimEt, Islem.KiralamaIslemleri, "svgimages/xaf/state_task_inprogress.svg"),
            (btnIadeAl, Islem.KiralamaIslemleri, "svgimages/xaf/state_task_completed.svg"),
            (btnAktifKiralamalar, Islem.KiralamaIslemleri, "svgimages/scheduling/listview.svg"),
            // Personel de ekipman durumunu değiştirebildiği için ekranı görür; ekle/sil yetkisi ekranda denetlenir.
            (btnEkipmanlar, Islem.EkipmanDurumDegistirme, "svgimages/hybriddemoicons/tiles/hybriddemo_products.svg"),
            (btnMusteriler, Islem.MusteriIslemleri, "svgimages/hybriddemoicons/tiles/hybriddemo_customers.svg"),
            (btnPersonel, Islem.PersonelYonetimi, "svgimages/hybriddemoicons/tiles/hybriddemo_employees.svg"),
            (btnDashboard, Islem.RaporGoruntuleme, "svgimages/hybriddemoicons/tiles/hybriddemo_dashboard.svg"),
            (btnEkipmanOnerisi, Islem.KiralamaIslemleri, "svgimages/miscellaneous/ai.svg"),
            (btnCikisYap, null, "svgimages/xaf/action_logoff.svg")
        ];

        foreach (var (buton, yetki, ikon) in butonlar)
        {
            buton.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(ikon);
            buton.Visibility = yetki is null || _yetki.YetkisiVarMi(yetki.Value)
                ? BarItemVisibility.Always
                : BarItemVisibility.Never;
        }

        foreach (RibbonPage sayfa in ribbonControl.Pages)
            sayfa.Visible = sayfa.Groups
                .SelectMany(grup => grup.ItemLinks.Cast<BarItemLink>())
                .Any(link => link.Item.Visibility != BarItemVisibility.Never);
    }

    /// <summary>
    /// "Kayıt İşlemleri" bağlamsal Ribbon sayfasındaki buton → işlem ve ikon eşlemesi. İşlem butonun
    /// Tag'ine yazılır; tek ortak tıklama işleyicisi işlemi aktif ekrana iletir.
    /// </summary>
    private void KayitButonlariniHazirla()
    {
        (BarButtonItem Buton, KayitIslemi Islem, string Ikon)[] butonlar =
        [
            (btnEkle, KayitIslemi.Ekle, "svgimages/actions/new.svg"),
            (btnDuzenle, KayitIslemi.Duzenle, "svgimages/xaf/action_edit.svg"),
            (btnSil, KayitIslemi.Sil, "svgimages/xaf/action_delete.svg"),
            (btnYenile, KayitIslemi.Yenile, "svgimages/xaf/action_refresh.svg"),
            (btnExcelAktar, KayitIslemi.ExcelAktar, "svgimages/export/exporttoxlsx.svg"),
            (btnFiyatGuncelle, KayitIslemi.FiyatGuncelle, "svgimages/dashboards/currency.svg"),
            (btnDurumDegistir, KayitIslemi.DurumDegistir, "svgimages/xaf/action_change_state.svg"),
            (btnSifreDegistir, KayitIslemi.SifreDegistir, "svgimages/xaf/action_security_changepassword.svg"),
            (btnAktiflikDegistir, KayitIslemi.AktiflikDegistir, "svgimages/xaf/state_itemvisibility_show.svg")
        ];

        foreach (var (buton, islem, ikon) in butonlar)
        {
            buton.Tag = islem;
            buton.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(ikon);
        }
    }

    private IKayitEkrani? AktifKayitEkrani => tabbedView.ActiveDocument?.Control as IKayitEkrani;

    /// <summary>
    /// Aktif sekme bir kayıt ekranıysa "Kayıt İşlemleri" sayfası görünür ve seçilir; butonlar o ekranın
    /// sunduğu ve personelin yetkisinin yettiği işlemlere göre gösterilir. Başka sekmede sayfa gizlenir.
    /// </summary>
    private void KayitSayfasiniGuncelle()
    {
        var ekran = AktifKayitEkrani;
        rpcKayitIslemleri.Visible = ekran is not null;
        if (ekran is null)
            return;

        foreach (var grup in new[] { rpgKayit, rpgEkranIslemleri })
        {
            var gorunurButonVar = false;
            foreach (BarItemLink link in grup.ItemLinks)
            {
                var gorunur = ekran.IslemGorunur((KayitIslemi)link.Item.Tag);
                link.Item.Visibility = gorunur ? BarItemVisibility.Always : BarItemVisibility.Never;
                gorunurButonVar |= gorunur;
            }
            grup.Visible = gorunurButonVar; // ör. Müşteri ekranında "İşlemler" grubu boştur
        }
        ribbonControl.SelectedPage = rpKayitIslemleri;
    }

    private void KayitIslemi_ItemClick(object sender, ItemClickEventArgs e) =>
        AktifKayitEkrani?.Calistir((KayitIslemi)e.Item.Tag);

    private void tabbedView_DocumentActivated(object sender, DocumentEventArgs e) => KayitSayfasiniGuncelle();

    private void tabbedView_DocumentClosed(object sender, DocumentEventArgs e) => KayitSayfasiniGuncelle();

    /// <summary>
    /// Ekranı ana formun içinde sekme olarak açar. Aynı ekran zaten açıksa yenisi açılmaz,
    /// mevcut sekme öne getirilir. Her ekran kendi kapsamında (kendi DbContext'iyle) oluşturulur.
    /// </summary>
    public BaseDocument EkraniAc<T>() where T : Control
    {
        var acik = tabbedView.Documents.FirstOrDefault(belge => belge.Control is T);
        if (acik is not null)
        {
            tabbedView.ActivateDocument(acik.Control);
            return acik;
        }

        var ekran = _formAcici.Olustur<T>();
        var yeni = tabbedView.AddDocument(ekran, ekran.Text);
        tabbedView.ActivateDocument(ekran);
        return yeni;
    }

    private void AnaForm_Load(object sender, EventArgs e)
    {
        // Başlangıç sekmesi kapatılamaz; diğer ekranlar kapatılabilir.
        EkraniAc<BaslangicEkrani>().Properties.AllowClose = DefaultBoolean.False;
    }

    // Henüz yapılmamış ekranların ortak davranışı; ekranlar eklendikçe ilgili buton kendi işleyicisine bağlanır.
    private void YerTutucu_ItemClick(object sender, ItemClickEventArgs e) =>
        XtraMessageBox.Show($"{e.Item.Caption}: Bu ekran sonraki aşamada eklenecek.", "Bilgi",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void btnCikisYap_ItemClick(object sender, ItemClickEventArgs e)
    {
        _girisServisi.CikisYap();
        CikisYapildi = true;
        Close(); // açık sekmeler formla birlikte kapanır
    }

    private void timerSaat_Tick(object sender, EventArgs e) => SaatiGuncelle();

    private void SaatiGuncelle() => lblSaat.Caption = _zaman.GetLocalNow().ToString("dd.MM.yyyy HH:mm:ss");
}
