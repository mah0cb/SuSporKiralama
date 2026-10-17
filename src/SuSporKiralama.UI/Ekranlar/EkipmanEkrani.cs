using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using SuSporKiralama.Business.Guvenlik;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.UI.Altyapi;
using SuSporKiralama.UI.Formlar;

namespace SuSporKiralama.UI.Ekranlar;

/// <summary>
/// Ekipman listesi. Personel rolü de açar (durum değiştirebilir); ekle/düzenle/sil ve fiyat
/// yalnızca yetkisi olana görünür. Durum sütunu temaya uyumlu renklerle biçimlenir.
/// </summary>
public partial class EkipmanEkrani : XtraUserControl, IKayitEkrani
{
    private readonly FormAcici _formAcici;
    private readonly IYetkiServisi _yetki;

    public EkipmanEkrani(FormAcici formAcici, IYetkiServisi yetki)
    {
        _formAcici = formAcici;
        _yetki = yetki;
        InitializeComponent();
        Text = "Ekipmanlar";
        EkranYardimcisi.ListeHazirla(gridView, Duzenle);
        DurumRenkleriniEkle();
    }

    public bool IslemGorunur(KayitIslemi islem) => islem switch
    {
        KayitIslemi.Ekle or KayitIslemi.Duzenle or KayitIslemi.Sil => _yetki.YetkisiVarMi(Islem.EkipmanYonetimi),
        KayitIslemi.FiyatGuncelle => _yetki.YetkisiVarMi(Islem.FiyatGuncelleme),
        KayitIslemi.DurumDegistir => _yetki.YetkisiVarMi(Islem.EkipmanDurumDegistirme),
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
            case KayitIslemi.ExcelAktar: EkranYardimcisi.ExcelAktar(gridControl, "Ekipmanlar"); break;
            case KayitIslemi.FiyatGuncelle: FiyatGuncelle(); break;
            case KayitIslemi.DurumDegistir: DurumDegistir(); break;
        }
    }

    private void EkipmanEkrani_Load(object sender, EventArgs e) => Yenile();

    /// <summary>
    /// Durum hücresinin rengi: DXSkinColors renkleri seçili temanın paletinden gelir, tema değişince
    /// renkler de uyum sağlar. Renkler koddan verilir; Designer bu özel renkleri saklayamaz.
    /// </summary>
    private void DurumRenkleriniEkle()
    {
        (EkipmanDurumu Durum, Color Renk)[] renkler =
        [
            (EkipmanDurumu.Musait, DXSkinColors.FillColors.Success),
            (EkipmanDurumu.Kirada, DXSkinColors.FillColors.Primary),
            (EkipmanDurumu.Bakimda, DXSkinColors.FillColors.Warning),
            (EkipmanDurumu.HizmetDisi, DXSkinColors.FillColors.Danger)
        ];

        foreach (var (durum, renk) in renkler)
        {
            var kural = new FormatConditionRuleValue { Condition = FormatCondition.Equal, Value1 = Metinler.Ad(durum) };
            kural.Appearance.BackColor = renk;
            kural.Appearance.ForeColor = Color.White;
            kural.Appearance.Options.UseBackColor = true;
            kural.Appearance.Options.UseForeColor = true;
            gridView.FormatRules.Add(new GridFormatRule(kural) { Column = colDurum });
        }
    }

    private void Yenile(int? seciliId = null) =>
        EkranYardimcisi.Yukle(gridView,
            _formAcici.Getir((IEkipmanServisi s) => s.TumunuGetir().ConvertAll(EkipmanSatiri.Olustur)), seciliId);

    // Tür ve durum kutuları ilgili sütunun filtresini ayarlar ("Tümü" filtreyi kaldırır).
    private void HizliFiltre_SelectedIndexChanged(object sender, EventArgs e)
    {
        SutunFiltresi(colTur, cmbTur);
        SutunFiltresi(colDurum, cmbDurum);
    }

    private static void SutunFiltresi(GridColumn sutun, ComboBoxEdit kutu) =>
        sutun.FilterInfo = kutu.SelectedIndex > 0
            ? new ColumnFilterInfo(new BinaryOperator(sutun.FieldName, kutu.Text))
            : new ColumnFilterInfo();

    private EkipmanSatiri? Secili => gridView.GetFocusedRow() as EkipmanSatiri;

    private void Duzenle()
    {
        if (IslemGorunur(KayitIslemi.Duzenle) && Secili is { } satir)
            FormuAc(satir.Id);
    }

    private void FormuAc(int? id)
    {
        using var form = _formAcici.Olustur<EkipmanFormu>();
        if (EkranYardimcisi.Dene(() => form.Yukle(id)) && form.ShowDialog(this) == DialogResult.OK)
            Yenile(form.KayitId);
    }

    private void Sil()
    {
        if (Secili is not { } satir || !EkranYardimcisi.Onayla($"{satir.Kod} kodlu ekipman silinsin mi?"))
            return;

        // Kiralama geçmişi olan ekipmanda Business "Hizmet Dışı yapın" mesajını verir.
        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IEkipmanServisi s) => s.Sil(satir.Id))))
            Yenile();
    }

    private void FiyatGuncelle()
    {
        if (Secili is not { } satir)
            return;

        var editor = new SpinEdit();
        editor.Properties.MaxValue = 100000;
        editor.Properties.DisplayFormat.FormatString = "n2";
        var cevap = XtraInputBox.Show(new XtraInputBoxArgs(FindForm(),
            $"{satir.Kod} için yeni birim ücret:", "Fiyat Güncelle", satir.BirimUcret) { Editor = editor });
        if (cevap is null)
            return; // vazgeçildi

        var ucret = Convert.ToDecimal(cevap);
        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IEkipmanServisi s) => s.FiyatGuncelle(satir.Id, ucret))))
            Yenile();
    }

    private void DurumDegistir()
    {
        if (Secili is not { } satir)
            return;

        // Kirada durumu yalnızca kiralama/iade akışıyla verilir, listede yer almaz.
        var editor = new ImageComboBoxEdit();
        Metinler.Doldur(editor, EkipmanDurumu.Kirada);
        var cevap = XtraInputBox.Show(new XtraInputBoxArgs(FindForm(),
            $"{satir.Kod} için yeni durum:", "Durum Değiştir", satir.DurumDegeri) { Editor = editor });
        if (cevap is not EkipmanDurumu durum || durum == satir.DurumDegeri)
            return;

        if (EkranYardimcisi.Dene(() => _formAcici.Calistir((IEkipmanServisi s) => s.DurumDegistir(satir.Id, durum))))
            Yenile();
    }
}
