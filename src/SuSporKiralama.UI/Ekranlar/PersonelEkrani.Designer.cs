namespace SuSporKiralama.UI.Ekranlar
{
    partial class PersonelEkrani
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlUst = new DevExpress.XtraEditors.PanelControl();
            lblBilgi = new DevExpress.XtraEditors.LabelControl();
            gridControl = new DevExpress.XtraGrid.GridControl();
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colId = new DevExpress.XtraGrid.Columns.GridColumn();
            colAdSoyad = new DevExpress.XtraGrid.Columns.GridColumn();
            colKullaniciAdi = new DevExpress.XtraGrid.Columns.GridColumn();
            colRol = new DevExpress.XtraGrid.Columns.GridColumn();
            colAktif = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)pnlUst).BeginInit();
            pnlUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            //
            // pnlUst
            //
            pnlUst.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlUst.Controls.Add(lblBilgi);
            pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            pnlUst.Location = new System.Drawing.Point(0, 0);
            pnlUst.Name = "pnlUst";
            pnlUst.Size = new System.Drawing.Size(800, 40);
            pnlUst.TabIndex = 0;
            //
            // lblBilgi
            //
            lblBilgi.Location = new System.Drawing.Point(12, 13);
            lblBilgi.Name = "lblBilgi";
            lblBilgi.Size = new System.Drawing.Size(500, 13);
            lblBilgi.TabIndex = 0;
            lblBilgi.Text = "Şifreler veritabanında yalnızca özet (hash) olarak saklanır; hiçbir ekranda gösterilmez.";
            //
            // gridControl
            //
            gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl.Location = new System.Drawing.Point(0, 40);
            gridControl.MainView = gridView;
            gridControl.Name = "gridControl";
            gridControl.Size = new System.Drawing.Size(800, 460);
            gridControl.TabIndex = 1;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            //
            // gridView
            //
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colId, colAdSoyad, colKullaniciAdi, colRol, colAktif });
            gridView.GridControl = gridControl;
            gridView.Name = "gridView";
            //
            // colId
            //
            colId.Caption = "Id";
            colId.FieldName = "Id";
            colId.Name = "colId";
            //
            // colAdSoyad
            //
            colAdSoyad.Caption = "Ad Soyad";
            colAdSoyad.FieldName = "AdSoyad";
            colAdSoyad.Name = "colAdSoyad";
            colAdSoyad.Visible = true;
            colAdSoyad.VisibleIndex = 0;
            //
            // colKullaniciAdi
            //
            colKullaniciAdi.Caption = "Kullanıcı Adı";
            colKullaniciAdi.FieldName = "KullaniciAdi";
            colKullaniciAdi.Name = "colKullaniciAdi";
            colKullaniciAdi.Visible = true;
            colKullaniciAdi.VisibleIndex = 1;
            //
            // colRol
            //
            colRol.Caption = "Rol";
            colRol.FieldName = "Rol";
            colRol.Name = "colRol";
            colRol.Visible = true;
            colRol.VisibleIndex = 2;
            //
            // colAktif
            //
            colAktif.Caption = "Aktif";
            colAktif.FieldName = "Aktif";
            colAktif.Name = "colAktif";
            colAktif.Visible = true;
            colAktif.VisibleIndex = 3;
            //
            // PersonelEkrani
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gridControl);
            Controls.Add(pnlUst);
            Name = "PersonelEkrani";
            Size = new System.Drawing.Size(800, 500);
            Load += PersonelEkrani_Load;
            ((System.ComponentModel.ISupportInitialize)pnlUst).EndInit();
            pnlUst.ResumeLayout(false);
            pnlUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlUst;
        private DevExpress.XtraEditors.LabelControl lblBilgi;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colAdSoyad;
        private DevExpress.XtraGrid.Columns.GridColumn colKullaniciAdi;
        private DevExpress.XtraGrid.Columns.GridColumn colRol;
        private DevExpress.XtraGrid.Columns.GridColumn colAktif;
    }
}
