using System.IO;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using SuSporKiralama.Business.Istisnalar;

namespace SuSporKiralama.UI.Altyapi;

/// <summary>
/// Liste ekranlarının ve düzenleme formlarının ortak davranışı. Ortak bir taban sınıf yerine bu
/// yardımcı kullanılır (kompozisyon): her ekran kendi GridControl'ünü ve sütunlarını kendi
/// Designer'ında tutar, ortak davranışı tek satırla bağlar. Designer generic/soyut taban sınıftan
/// türeyen ekranları açamadığı için bu yol seçildi.
/// </summary>
public static class EkranYardimcisi
{
    /// <summary>
    /// Grid'i liste ekranı olarak hazırlar: hücreler düzenlenemez, satıra çift tıklayınca düzenleme açılır,
    /// liste boşken "Kayıt yok" yazar. Arama paneli (Ctrl+F), sütun filtresi, sıralama ve gruplama
    /// GridView'da varsayılan olarak açıktır.
    /// </summary>
    public static void ListeHazirla(GridView gorunum, Action duzenle)
    {
        gorunum.OptionsBehavior.Editable = false;

        // Yalnızca veri satırına çift tıklanınca (başlık, grup satırı, boş alan hariç).
        gorunum.DoubleClick += (_, e) =>
        {
            var fare = DXMouseEventArgs.GetMouseArgs(gorunum.GridControl, e);
            if (gorunum.CalcHitInfo(fare.Location).InDataRow)
                duzenle();
        };

        gorunum.CustomDrawEmptyForeground += (_, e) =>
        {
            e.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            e.Appearance.DrawString(e.Cache, "Kayıt yok", e.Bounds);
        };
    }

    /// <summary>Odaktaki satırın Id'si; satır seçili değilse null.</summary>
    public static int? SeciliId(GridView gorunum) => gorunum.GetFocusedRowCellValue("Id") as int?;

    /// <summary>
    /// Listeyi grid'e bağlar; verilen Id'li (verilmezse o an seçili olan) kayıt yeniden seçilir.
    /// Böylece kaydet/sil/yenile sonrası kullanıcı listedeki yerini kaybetmez.
    /// </summary>
    public static void Yukle(GridView gorunum, object satirlar, int? seciliId = null)
    {
        seciliId ??= SeciliId(gorunum);
        gorunum.GridControl.DataSource = satirlar;

        if (seciliId is null)
            return;
        var satir = gorunum.LocateByValue("Id", seciliId.Value);
        if (satir != GridControl.InvalidRowHandle)
            gorunum.FocusedRowHandle = satir;
    }

    /// <summary>
    /// Grid'in o anki görünümünü (filtre, sıralama ve gruplama dahil) .xlsx olarak kaydeder.
    /// Kaydetme yeri kullanıcıya sorulur.
    /// </summary>
    public static void ExcelAktar(GridControl grid, string dosyaAdi)
    {
        using var dialog = new XtraSaveFileDialog
        {
            Title = "Excel'e Aktar",
            Filter = "Excel dosyası (*.xlsx)|*.xlsx",
            FileName = $"{dosyaAdi}-{DateTime.Now:yyyy-MM-dd}.xlsx"
        };
        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            grid.ExportToXlsx(dialog.FileName);
        }
        catch (IOException)
        {
            // En sık sebep: aynı dosya Excel'de açık.
            XtraMessageBox.Show("Dosya kaydedilemedi. Dosya başka bir programda açık olabilir.", "Uyarı",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        XtraMessageBox.Show($"Liste kaydedildi:\n{dialog.FileName}", "Excel'e Aktar",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static bool Onayla(string soru) =>
        XtraMessageBox.Show(soru, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    /// <summary>
    /// Business işlemini çalıştırır. İş kuralı ihlalinde mesajı gösterir ve false döner; çağıran (ör. açık
    /// düzenleme formu) olduğu gibi kalır. Entity'nin kendi doğrulaması (ör. geçersiz e-posta) ArgumentException
    /// fırlattığı için o da aynı şekilde gösterilir. Beklenmeyen hatalar HataYoneticisi'ne bırakılır.
    /// </summary>
    public static bool Dene(Action islem)
    {
        try
        {
            islem();
            return true;
        }
        catch (IsKuraliException hata)
        {
            HataYoneticisi.Isle(hata);
        }
        catch (ArgumentException hata)
        {
            HataYoneticisi.Isle(new DogrulamaException(hata));
        }
        return false;
    }
}
