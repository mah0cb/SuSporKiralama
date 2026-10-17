namespace SuSporKiralama.UI.Ekranlar
{
    partial class MusteriEkrani
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
            components = new System.ComponentModel.Container();
            pnlUst = new DevExpress.XtraEditors.PanelControl();
            lblAra = new DevExpress.XtraEditors.LabelControl();
            txtAra = new DevExpress.XtraEditors.TextEdit();
            gridControl = new DevExpress.XtraGrid.GridControl();
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colId = new DevExpress.XtraGrid.Columns.GridColumn();
            colAdSoyad = new DevExpress.XtraGrid.Columns.GridColumn();
            colTelefon = new DevExpress.XtraGrid.Columns.GridColumn();
            colEposta = new DevExpress.XtraGrid.Columns.GridColumn();
            colDeneyim = new DevExpress.XtraGrid.Columns.GridColumn();
            colAktif = new DevExpress.XtraGrid.Columns.GridColumn();
            colKayitTarihi = new DevExpress.XtraGrid.Columns.GridColumn();
            timerArama = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pnlUst).BeginInit();
            pnlUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtAra.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            //
            // pnlUst
            //
            pnlUst.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlUst.Controls.Add(lblAra);
            pnlUst.Controls.Add(txtAra);
            pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            pnlUst.Location = new System.Drawing.Point(0, 0);
            pnlUst.Name = "pnlUst";
            pnlUst.Size = new System.Drawing.Size(800, 40);
            pnlUst.TabIndex = 0;
            //
            // lblAra
            //
            lblAra.Location = new System.Drawing.Point(12, 13);
            lblAra.Name = "lblAra";
            lblAra.Size = new System.Drawing.Size(73, 13);
            lblAra.TabIndex = 0;
            lblAra.Text = "Müşteri ara:";
            //
            // txtAra
            //
            txtAra.Location = new System.Drawing.Point(100, 10);
            txtAra.Name = "txtAra";
            txtAra.Properties.NullValuePrompt = "Ad, soyad veya telefon";
            txtAra.Size = new System.Drawing.Size(300, 20);
            txtAra.TabIndex = 1;
            txtAra.EditValueChanged += txtAra_EditValueChanged;
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
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colId, colAdSoyad, colTelefon, colEposta, colDeneyim, colAktif, colKayitTarihi });
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
            // colTelefon
            //
            colTelefon.Caption = "Telefon";
            colTelefon.FieldName = "Telefon";
            colTelefon.Name = "colTelefon";
            colTelefon.Visible = true;
            colTelefon.VisibleIndex = 1;
            //
            // colEposta
            //
            colEposta.Caption = "E-posta";
            colEposta.FieldName = "Eposta";
            colEposta.Name = "colEposta";
            colEposta.Visible = true;
            colEposta.VisibleIndex = 2;
            //
            // colDeneyim
            //
            colDeneyim.Caption = "Deneyim Seviyesi";
            colDeneyim.FieldName = "Deneyim";
            colDeneyim.Name = "colDeneyim";
            colDeneyim.Visible = true;
            colDeneyim.VisibleIndex = 3;
            //
            // colAktif
            //
            colAktif.Caption = "Aktif";
            colAktif.FieldName = "Aktif";
            colAktif.Name = "colAktif";
            colAktif.Visible = true;
            colAktif.VisibleIndex = 4;
            //
            // colKayitTarihi
            //
            colKayitTarihi.Caption = "Kayıt Tarihi";
            colKayitTarihi.DisplayFormat.FormatString = "dd.MM.yyyy";
            colKayitTarihi.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colKayitTarihi.FieldName = "KayitTarihi";
            colKayitTarihi.Name = "colKayitTarihi";
            colKayitTarihi.Visible = true;
            colKayitTarihi.VisibleIndex = 5;
            //
            // timerArama
            //
            timerArama.Interval = 300;
            timerArama.Tick += timerArama_Tick;
            //
            // MusteriEkrani
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gridControl);
            Controls.Add(pnlUst);
            Name = "MusteriEkrani";
            Size = new System.Drawing.Size(800, 500);
            Load += MusteriEkrani_Load;
            ((System.ComponentModel.ISupportInitialize)pnlUst).EndInit();
            pnlUst.ResumeLayout(false);
            pnlUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtAra.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlUst;
        private DevExpress.XtraEditors.LabelControl lblAra;
        private DevExpress.XtraEditors.TextEdit txtAra;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colAdSoyad;
        private DevExpress.XtraGrid.Columns.GridColumn colTelefon;
        private DevExpress.XtraGrid.Columns.GridColumn colEposta;
        private DevExpress.XtraGrid.Columns.GridColumn colDeneyim;
        private DevExpress.XtraGrid.Columns.GridColumn colAktif;
        private DevExpress.XtraGrid.Columns.GridColumn colKayitTarihi;
        private System.Windows.Forms.Timer timerArama;
    }
}
