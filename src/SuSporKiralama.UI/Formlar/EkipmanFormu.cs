using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using SuSporKiralama.Business.Servisler;
using SuSporKiralama.Entities;
using SuSporKiralama.Entities.Soyut;
using SuSporKiralama.UI.Altyapi;

namespace SuSporKiralama.UI.Formlar;

/// <summary>
/// Ekipman ekleme ve düzenleme. Eklemede önce tür seçilir; formda yalnızca o türün alan grubu görünür
/// (gizli gruplardaki alanlar doğrulanmaz). Düzenlemede tür değiştirilemez. Durum burada değil,
/// ekrandaki "Durum Değiştir" işlemiyle değişir. Form kendi DI kapsamında çalışır (bkz. MusteriFormu).
/// </summary>
public partial class EkipmanFormu : XtraForm
{
    // rgTur'daki seçenek sırası
    private const int SupBoardTuru = 0, KanoTuru = 1, CanYelegiTuru = 2;

    private readonly IEkipmanServisi _servis;
    private Ekipman? _ekipman; // null: yeni kayıt

    public int KayitId { get; private set; }

    public EkipmanFormu(IEkipmanServisi servis)
    {
        _servis = servis;
        InitializeComponent();
        Metinler.Doldur<SupBoardTipi>(cmbTip);
        Metinler.Doldur<Beden>(cmbBeden);
        cmbTip.EditValue = SupBoardTipi.Sisme;
        cmbBeden.EditValue = Beden.M;
        TuruGoster();
    }

    /// <summary>id null ise yeni ekipman, değilse o ekipman düzenlenir.</summary>
    public void Yukle(int? id)
    {
        if (id is null)
        {
            Text = "Yeni Ekipman";
            return;
        }

        _ekipman = _servis.IdIleGetir(id.Value);
        Text = $"Ekipman Düzenle: {_ekipman.Kod}";
        rgTur.SelectedIndex = _ekipman switch { SupBoard => SupBoardTuru, Kano => KanoTuru, _ => CanYelegiTuru };
        rgTur.Enabled = false; // tür sonradan değiştirilemez

        txtKod.Text = _ekipman.Kod;
        txtMarka.Text = _ekipman.Marka;
        txtModel.Text = _ekipman.Model;
        spnBirimUcret.Value = _ekipman.BirimUcret;
        spnDepozito.Value = _ekipman.DepozitoTutari;
        switch (_ekipman)
        {
            case SupBoard sup:
                spnUzunluk.Value = sup.UzunlukCm;
                spnMaxTasima.Value = sup.MaxTasimaKg;
                cmbTip.EditValue = sup.Tip;
                break;
            case Kano kano:
                spnKapasite.Value = kano.KisiKapasitesi;
                break;
            case CanYelegi yelek:
                cmbBeden.EditValue = yelek.Beden;
                break;
        }
    }

    private void rgTur_SelectedIndexChanged(object sender, EventArgs e) => TuruGoster();

    // Seçili türün grubu görünür, diğerleri gizlenir. Birim ücretin anlamı türe göre değişir.
    private void TuruGoster()
    {
        var tur = rgTur.SelectedIndex;
        grpSupBoard.Visibility = tur == SupBoardTuru ? LayoutVisibility.Always : LayoutVisibility.Never;
        grpKano.Visibility = tur == KanoTuru ? LayoutVisibility.Always : LayoutVisibility.Never;
        grpCanYelegi.Visibility = tur == CanYelegiTuru ? LayoutVisibility.Always : LayoutVisibility.Never;
        lciBirimUcret.Text = tur switch
        {
            SupBoardTuru => "Saatlik ücret *",
            KanoTuru => "4 saatlik blok ücreti *",
            CanYelegiTuru => "Kiralama başı ücret *",
            _ => "Birim ücret *"
        };
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
        if (_ekipman is null)
        {
            Ekipman yeni = rgTur.SelectedIndex switch
            {
                SupBoardTuru => new SupBoard(txtKod.Text, txtMarka.Text, txtModel.Text, spnBirimUcret.Value, spnDepozito.Value,
                    (int)spnUzunluk.Value, (int)spnMaxTasima.Value, (SupBoardTipi)cmbTip.EditValue),
                KanoTuru => new Kano(txtKod.Text, txtMarka.Text, txtModel.Text, spnBirimUcret.Value, spnDepozito.Value,
                    (int)spnKapasite.Value),
                _ => new CanYelegi(txtKod.Text, txtMarka.Text, txtModel.Text, spnBirimUcret.Value, spnDepozito.Value,
                    (Beden)cmbBeden.EditValue)
            };
            _servis.Ekle(yeni);
            KayitId = yeni.Id;
            return;
        }

        _ekipman.Kod = txtKod.Text;
        _ekipman.Marka = txtMarka.Text;
        _ekipman.Model = txtModel.Text;
        _ekipman.BirimUcret = spnBirimUcret.Value;
        _ekipman.DepozitoTutari = spnDepozito.Value;
        switch (_ekipman)
        {
            case SupBoard sup:
                sup.UzunlukCm = (int)spnUzunluk.Value;
                sup.MaxTasimaKg = (int)spnMaxTasima.Value;
                sup.Tip = (SupBoardTipi)cmbTip.EditValue;
                break;
            case Kano kano:
                kano.KisiKapasitesi = (int)spnKapasite.Value;
                break;
            case CanYelegi yelek:
                yelek.Beden = (Beden)cmbBeden.EditValue;
                break;
        }
        _servis.Guncelle(_ekipman);
        KayitId = _ekipman.Id;
    }
}
