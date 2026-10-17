namespace SuSporKiralama.UI.Ekranlar
{
    partial class EkipmanEkrani
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
            lblTur = new DevExpress.XtraEditors.LabelControl();
            cmbTur = new DevExpress.XtraEditors.ComboBoxEdit();
            lblDurum = new DevExpress.XtraEditors.LabelControl();
            cmbDurum = new DevExpress.XtraEditors.ComboBoxEdit();
            gridControl = new DevExpress.XtraGrid.GridControl();
            gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            colId = new DevExpress.XtraGrid.Columns.GridColumn();
            colKod = new DevExpress.XtraGrid.Columns.GridColumn();
            colTur = new DevExpress.XtraGrid.Columns.GridColumn();
            colMarka = new DevExpress.XtraGrid.Columns.GridColumn();
            colModel = new DevExpress.XtraGrid.Columns.GridColumn();
            colDurum = new DevExpress.XtraGrid.Columns.GridColumn();
            colBirimUcret = new DevExpress.XtraGrid.Columns.GridColumn();
            colDepozito = new DevExpress.XtraGrid.Columns.GridColumn();
            colOzellikler = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)pnlUst).BeginInit();
            pnlUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbTur.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbDurum.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            //
            // pnlUst
            //
            pnlUst.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlUst.Controls.Add(lblTur);
            pnlUst.Controls.Add(cmbTur);
            pnlUst.Controls.Add(lblDurum);
            pnlUst.Controls.Add(cmbDurum);
            pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            pnlUst.Location = new System.Drawing.Point(0, 0);
            pnlUst.Name = "pnlUst";
            pnlUst.Size = new System.Drawing.Size(900, 40);
            pnlUst.TabIndex = 0;
            //
            // lblTur
            //
            lblTur.Location = new System.Drawing.Point(12, 13);
            lblTur.Name = "lblTur";
            lblTur.Size = new System.Drawing.Size(19, 13);
            lblTur.TabIndex = 0;
            lblTur.Text = "Tür:";
            //
            // cmbTur
            //
            cmbTur.EditValue = "Tümü";
            cmbTur.Location = new System.Drawing.Point(50, 10);
            cmbTur.Name = "cmbTur";
            cmbTur.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbTur.Properties.Items.AddRange(new object[] { "Tümü", "SUP Board", "Kano", "Can Yeleği" });
            cmbTur.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbTur.Size = new System.Drawing.Size(150, 20);
            cmbTur.TabIndex = 1;
            cmbTur.SelectedIndexChanged += HizliFiltre_SelectedIndexChanged;
            //
            // lblDurum
            //
            lblDurum.Location = new System.Drawing.Point(230, 13);
            lblDurum.Name = "lblDurum";
            lblDurum.Size = new System.Drawing.Size(34, 13);
            lblDurum.TabIndex = 2;
            lblDurum.Text = "Durum:";
            //
            // cmbDurum
            //
            cmbDurum.EditValue = "Tümü";
            cmbDurum.Location = new System.Drawing.Point(280, 10);
            cmbDurum.Name = "cmbDurum";
            cmbDurum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbDurum.Properties.Items.AddRange(new object[] { "Tümü", "Müsait", "Kirada", "Bakımda", "Hizmet Dışı" });
            cmbDurum.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbDurum.Size = new System.Drawing.Size(150, 20);
            cmbDurum.TabIndex = 3;
            cmbDurum.SelectedIndexChanged += HizliFiltre_SelectedIndexChanged;
            //
            // gridControl
            //
            gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl.Location = new System.Drawing.Point(0, 40);
            gridControl.MainView = gridView;
            gridControl.Name = "gridControl";
            gridControl.Size = new System.Drawing.Size(900, 460);
            gridControl.TabIndex = 1;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            //
            // gridView
            //
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colId, colKod, colTur, colMarka, colModel, colDurum, colBirimUcret, colDepozito, colOzellikler });
            gridView.GridControl = gridControl;
            gridView.Name = "gridView";
            //
            // colId
            //
            colId.Caption = "Id";
            colId.FieldName = "Id";
            colId.Name = "colId";
            //
            // colKod
            //
            colKod.Caption = "Kod";
            colKod.FieldName = "Kod";
            colKod.Name = "colKod";
            colKod.Visible = true;
            colKod.VisibleIndex = 0;
            //
            // colTur
            //
            colTur.Caption = "Tür";
            colTur.FieldName = "Tur";
            colTur.Name = "colTur";
            colTur.Visible = true;
            colTur.VisibleIndex = 1;
            //
            // colMarka
            //
            colMarka.Caption = "Marka";
            colMarka.FieldName = "Marka";
            colMarka.Name = "colMarka";
            colMarka.Visible = true;
            colMarka.VisibleIndex = 2;
            //
            // colModel
            //
            colModel.Caption = "Model";
            colModel.FieldName = "Model";
            colModel.Name = "colModel";
            colModel.Visible = true;
            colModel.VisibleIndex = 3;
            //
            // colDurum
            //
            colDurum.Caption = "Durum";
            colDurum.FieldName = "Durum";
            colDurum.Name = "colDurum";
            colDurum.Visible = true;
            colDurum.VisibleIndex = 4;
            //
            // colBirimUcret
            //
            colBirimUcret.Caption = "Birim Ücret";
            colBirimUcret.DisplayFormat.FormatString = "c2";
            colBirimUcret.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colBirimUcret.FieldName = "BirimUcret";
            colBirimUcret.Name = "colBirimUcret";
            colBirimUcret.Visible = true;
            colBirimUcret.VisibleIndex = 5;
            //
            // colDepozito
            //
            colDepozito.Caption = "Depozito";
            colDepozito.DisplayFormat.FormatString = "c2";
            colDepozito.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colDepozito.FieldName = "Depozito";
            colDepozito.Name = "colDepozito";
            colDepozito.Visible = true;
            colDepozito.VisibleIndex = 6;
            //
            // colOzellikler
            //
            colOzellikler.Caption = "Özellikler";
            colOzellikler.FieldName = "Ozellikler";
            colOzellikler.Name = "colOzellikler";
            colOzellikler.Visible = true;
            colOzellikler.VisibleIndex = 7;
            //
            // EkipmanEkrani
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gridControl);
            Controls.Add(pnlUst);
            Name = "EkipmanEkrani";
            Size = new System.Drawing.Size(900, 500);
            Load += EkipmanEkrani_Load;
            ((System.ComponentModel.ISupportInitialize)pnlUst).EndInit();
            pnlUst.ResumeLayout(false);
            pnlUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbTur.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbDurum.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlUst;
        private DevExpress.XtraEditors.LabelControl lblTur;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTur;
        private DevExpress.XtraEditors.LabelControl lblDurum;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDurum;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colKod;
        private DevExpress.XtraGrid.Columns.GridColumn colTur;
        private DevExpress.XtraGrid.Columns.GridColumn colMarka;
        private DevExpress.XtraGrid.Columns.GridColumn colModel;
        private DevExpress.XtraGrid.Columns.GridColumn colDurum;
        private DevExpress.XtraGrid.Columns.GridColumn colBirimUcret;
        private DevExpress.XtraGrid.Columns.GridColumn colDepozito;
        private DevExpress.XtraGrid.Columns.GridColumn colOzellikler;
    }
}
