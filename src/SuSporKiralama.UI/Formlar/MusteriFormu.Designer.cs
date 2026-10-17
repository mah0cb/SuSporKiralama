namespace SuSporKiralama.UI.Formlar
{
    partial class MusteriFormu
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralAd = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralSoyad = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralTelefon = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralDeneyim = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            layoutControl = new DevExpress.XtraLayout.LayoutControl();
            txtAd = new DevExpress.XtraEditors.TextEdit();
            txtSoyad = new DevExpress.XtraEditors.TextEdit();
            txtTelefon = new DevExpress.XtraEditors.TextEdit();
            txtEposta = new DevExpress.XtraEditors.TextEdit();
            cmbDeneyim = new DevExpress.XtraEditors.ImageComboBoxEdit();
            memoNotlar = new DevExpress.XtraEditors.MemoEdit();
            chkAktif = new DevExpress.XtraEditors.CheckEdit();
            btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            btnIptal = new DevExpress.XtraEditors.SimpleButton();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            lciAd = new DevExpress.XtraLayout.LayoutControlItem();
            lciSoyad = new DevExpress.XtraLayout.LayoutControlItem();
            lciTelefon = new DevExpress.XtraLayout.LayoutControlItem();
            lciEposta = new DevExpress.XtraLayout.LayoutControlItem();
            lciDeneyim = new DevExpress.XtraLayout.LayoutControlItem();
            lciNotlar = new DevExpress.XtraLayout.LayoutControlItem();
            lciAktif = new DevExpress.XtraLayout.LayoutControlItem();
            esiBosluk = new DevExpress.XtraLayout.EmptySpaceItem();
            esiButonSol = new DevExpress.XtraLayout.EmptySpaceItem();
            lciKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            lciIptal = new DevExpress.XtraLayout.LayoutControlItem();
            dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(components);
            ((System.ComponentModel.ISupportInitialize)layoutControl).BeginInit();
            layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtAd.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSoyad.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtTelefon.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtEposta.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbDeneyim.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoNotlar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chkAktif.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciAd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciSoyad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciTelefon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciEposta).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciDeneyim).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciNotlar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciAktif).BeginInit();
            ((System.ComponentModel.ISupportInitialize)esiBosluk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)esiButonSol).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciKaydet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciIptal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dxValidationProvider).BeginInit();
            SuspendLayout();
            //
            // layoutControl
            //
            layoutControl.Controls.Add(txtAd);
            layoutControl.Controls.Add(txtSoyad);
            layoutControl.Controls.Add(txtTelefon);
            layoutControl.Controls.Add(txtEposta);
            layoutControl.Controls.Add(cmbDeneyim);
            layoutControl.Controls.Add(memoNotlar);
            layoutControl.Controls.Add(chkAktif);
            layoutControl.Controls.Add(btnKaydet);
            layoutControl.Controls.Add(btnIptal);
            layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl.Location = new System.Drawing.Point(0, 0);
            layoutControl.Name = "layoutControl";
            layoutControl.Root = Root;
            layoutControl.Size = new System.Drawing.Size(420, 330);
            layoutControl.TabIndex = 0;
            //
            // txtAd
            //
            txtAd.Name = "txtAd";
            txtAd.StyleController = layoutControl;
            txtAd.TabIndex = 0;
            kuralAd.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralAd.ErrorText = "Ad zorunludur.";
            dxValidationProvider.SetValidationRule(txtAd, kuralAd);
            //
            // txtSoyad
            //
            txtSoyad.Name = "txtSoyad";
            txtSoyad.StyleController = layoutControl;
            txtSoyad.TabIndex = 1;
            kuralSoyad.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralSoyad.ErrorText = "Soyad zorunludur.";
            dxValidationProvider.SetValidationRule(txtSoyad, kuralSoyad);
            //
            // txtTelefon
            //
            txtTelefon.Name = "txtTelefon";
            txtTelefon.Properties.MaskSettings.MaskManagerType = typeof(DevExpress.Data.Mask.SimpleMaskManager);
            txtTelefon.Properties.MaskSettings.MaskExpression = "0000 000 00 00";
            txtTelefon.Properties.NullValuePrompt = "0532 123 45 67";
            txtTelefon.StyleController = layoutControl;
            txtTelefon.TabIndex = 2;
            kuralTelefon.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralTelefon.ErrorText = "Telefon zorunludur.";
            dxValidationProvider.SetValidationRule(txtTelefon, kuralTelefon);
            //
            // txtEposta
            //
            txtEposta.Name = "txtEposta";
            txtEposta.Properties.NullValuePrompt = "İsteğe bağlı";
            txtEposta.StyleController = layoutControl;
            txtEposta.TabIndex = 3;
            //
            // cmbDeneyim
            //
            cmbDeneyim.Name = "cmbDeneyim";
            cmbDeneyim.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbDeneyim.StyleController = layoutControl;
            cmbDeneyim.TabIndex = 4;
            kuralDeneyim.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralDeneyim.ErrorText = "Deneyim seviyesi seçilmelidir.";
            dxValidationProvider.SetValidationRule(cmbDeneyim, kuralDeneyim);
            //
            // memoNotlar
            //
            memoNotlar.Name = "memoNotlar";
            memoNotlar.StyleController = layoutControl;
            memoNotlar.TabIndex = 5;
            //
            // chkAktif
            //
            chkAktif.Name = "chkAktif";
            chkAktif.Properties.Caption = "Aktif";
            chkAktif.StyleController = layoutControl;
            chkAktif.TabIndex = 6;
            //
            // btnKaydet
            //
            btnKaydet.Name = "btnKaydet";
            btnKaydet.StyleController = layoutControl;
            btnKaydet.TabIndex = 7;
            btnKaydet.Text = "Kaydet";
            btnKaydet.Click += btnKaydet_Click;
            //
            // btnIptal
            //
            btnIptal.CausesValidation = false;
            btnIptal.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnIptal.Name = "btnIptal";
            btnIptal.StyleController = layoutControl;
            btnIptal.TabIndex = 8;
            btnIptal.Text = "İptal";
            //
            // Root
            //
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciAd, lciSoyad, lciTelefon, lciEposta, lciDeneyim, lciNotlar, lciAktif, esiBosluk, esiButonSol, lciKaydet, lciIptal });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(420, 330);
            Root.TextVisible = false;
            //
            // lciAd
            //
            lciAd.Control = txtAd;
            lciAd.Location = new System.Drawing.Point(0, 0);
            lciAd.Name = "lciAd";
            lciAd.Size = new System.Drawing.Size(400, 24);
            lciAd.Text = "Ad *";
            lciAd.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciSoyad
            //
            lciSoyad.Control = txtSoyad;
            lciSoyad.Location = new System.Drawing.Point(0, 24);
            lciSoyad.Name = "lciSoyad";
            lciSoyad.Size = new System.Drawing.Size(400, 24);
            lciSoyad.Text = "Soyad *";
            lciSoyad.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciTelefon
            //
            lciTelefon.Control = txtTelefon;
            lciTelefon.Location = new System.Drawing.Point(0, 48);
            lciTelefon.Name = "lciTelefon";
            lciTelefon.Size = new System.Drawing.Size(400, 24);
            lciTelefon.Text = "Telefon *";
            lciTelefon.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciEposta
            //
            lciEposta.Control = txtEposta;
            lciEposta.Location = new System.Drawing.Point(0, 72);
            lciEposta.Name = "lciEposta";
            lciEposta.Size = new System.Drawing.Size(400, 24);
            lciEposta.Text = "E-posta";
            lciEposta.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciDeneyim
            //
            lciDeneyim.Control = cmbDeneyim;
            lciDeneyim.Location = new System.Drawing.Point(0, 96);
            lciDeneyim.Name = "lciDeneyim";
            lciDeneyim.Size = new System.Drawing.Size(400, 24);
            lciDeneyim.Text = "Deneyim seviyesi *";
            lciDeneyim.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciNotlar
            //
            lciNotlar.Control = memoNotlar;
            lciNotlar.Location = new System.Drawing.Point(0, 120);
            lciNotlar.Name = "lciNotlar";
            lciNotlar.Size = new System.Drawing.Size(400, 64);
            lciNotlar.Text = "Notlar";
            lciNotlar.TextSize = new System.Drawing.Size(90, 13);
            //
            // lciAktif
            //
            lciAktif.Control = chkAktif;
            lciAktif.Location = new System.Drawing.Point(0, 184);
            lciAktif.Name = "lciAktif";
            lciAktif.Size = new System.Drawing.Size(400, 24);
            lciAktif.Text = " ";
            lciAktif.TextSize = new System.Drawing.Size(90, 13);
            //
            // esiBosluk
            //
            esiBosluk.Location = new System.Drawing.Point(0, 208);
            esiBosluk.Name = "esiBosluk";
            esiBosluk.Size = new System.Drawing.Size(400, 76);
            //
            // esiButonSol
            //
            esiButonSol.Location = new System.Drawing.Point(0, 284);
            esiButonSol.Name = "esiButonSol";
            esiButonSol.Size = new System.Drawing.Size(200, 26);
            //
            // lciKaydet
            //
            lciKaydet.Control = btnKaydet;
            lciKaydet.Location = new System.Drawing.Point(200, 284);
            lciKaydet.Name = "lciKaydet";
            lciKaydet.Size = new System.Drawing.Size(100, 26);
            lciKaydet.TextVisible = false;
            //
            // lciIptal
            //
            lciIptal.Control = btnIptal;
            lciIptal.Location = new System.Drawing.Point(300, 284);
            lciIptal.Name = "lciIptal";
            lciIptal.Size = new System.Drawing.Size(100, 26);
            lciIptal.TextVisible = false;
            //
            // dxValidationProvider
            //
            dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            //
            // MusteriFormu
            //
            AcceptButton = btnKaydet;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnIptal;
            ClientSize = new System.Drawing.Size(420, 330);
            Controls.Add(layoutControl);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MusteriFormu";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Müşteri";
            ((System.ComponentModel.ISupportInitialize)layoutControl).EndInit();
            layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtAd.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSoyad.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtTelefon.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtEposta.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbDeneyim.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoNotlar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)chkAktif.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciAd).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciSoyad).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciTelefon).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciEposta).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciDeneyim).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciNotlar).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciAktif).EndInit();
            ((System.ComponentModel.ISupportInitialize)esiBosluk).EndInit();
            ((System.ComponentModel.ISupportInitialize)esiButonSol).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciKaydet).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciIptal).EndInit();
            ((System.ComponentModel.ISupportInitialize)dxValidationProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraEditors.TextEdit txtAd;
        private DevExpress.XtraEditors.TextEdit txtSoyad;
        private DevExpress.XtraEditors.TextEdit txtTelefon;
        private DevExpress.XtraEditors.TextEdit txtEposta;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbDeneyim;
        private DevExpress.XtraEditors.MemoEdit memoNotlar;
        private DevExpress.XtraEditors.CheckEdit chkAktif;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.SimpleButton btnIptal;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciAd;
        private DevExpress.XtraLayout.LayoutControlItem lciSoyad;
        private DevExpress.XtraLayout.LayoutControlItem lciTelefon;
        private DevExpress.XtraLayout.LayoutControlItem lciEposta;
        private DevExpress.XtraLayout.LayoutControlItem lciDeneyim;
        private DevExpress.XtraLayout.LayoutControlItem lciNotlar;
        private DevExpress.XtraLayout.LayoutControlItem lciAktif;
        private DevExpress.XtraLayout.EmptySpaceItem esiBosluk;
        private DevExpress.XtraLayout.EmptySpaceItem esiButonSol;
        private DevExpress.XtraLayout.LayoutControlItem lciKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciIptal;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}
