namespace SuSporKiralama.UI.Formlar
{
    partial class PersonelFormu
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
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralAdSoyad = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralKullaniciAdi = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralRol = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralSifre = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule kuralSifreTekrar = new DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule();
            layoutControl = new DevExpress.XtraLayout.LayoutControl();
            txtAdSoyad = new DevExpress.XtraEditors.TextEdit();
            txtKullaniciAdi = new DevExpress.XtraEditors.TextEdit();
            cmbRol = new DevExpress.XtraEditors.ImageComboBoxEdit();
            txtSifre = new DevExpress.XtraEditors.TextEdit();
            txtSifreTekrar = new DevExpress.XtraEditors.TextEdit();
            btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            btnIptal = new DevExpress.XtraEditors.SimpleButton();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            lciAdSoyad = new DevExpress.XtraLayout.LayoutControlItem();
            lciKullaniciAdi = new DevExpress.XtraLayout.LayoutControlItem();
            lciRol = new DevExpress.XtraLayout.LayoutControlItem();
            grpSifre = new DevExpress.XtraLayout.LayoutControlGroup();
            lciSifre = new DevExpress.XtraLayout.LayoutControlItem();
            lciSifreTekrar = new DevExpress.XtraLayout.LayoutControlItem();
            esiBosluk = new DevExpress.XtraLayout.EmptySpaceItem();
            esiButonSol = new DevExpress.XtraLayout.EmptySpaceItem();
            lciKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            lciIptal = new DevExpress.XtraLayout.LayoutControlItem();
            dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(components);
            ((System.ComponentModel.ISupportInitialize)layoutControl).BeginInit();
            layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtAdSoyad.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtKullaniciAdi.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbRol.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciAdSoyad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciKullaniciAdi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciRol).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grpSifre).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciSifre).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciSifreTekrar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)esiBosluk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)esiButonSol).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciKaydet).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lciIptal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dxValidationProvider).BeginInit();
            SuspendLayout();
            //
            // layoutControl
            //
            layoutControl.Controls.Add(txtAdSoyad);
            layoutControl.Controls.Add(txtKullaniciAdi);
            layoutControl.Controls.Add(cmbRol);
            layoutControl.Controls.Add(txtSifre);
            layoutControl.Controls.Add(txtSifreTekrar);
            layoutControl.Controls.Add(btnKaydet);
            layoutControl.Controls.Add(btnIptal);
            layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl.Location = new System.Drawing.Point(0, 0);
            layoutControl.Name = "layoutControl";
            layoutControl.Root = Root;
            layoutControl.Size = new System.Drawing.Size(420, 250);
            layoutControl.TabIndex = 0;
            //
            // txtAdSoyad
            //
            txtAdSoyad.Name = "txtAdSoyad";
            txtAdSoyad.StyleController = layoutControl;
            txtAdSoyad.TabIndex = 0;
            kuralAdSoyad.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralAdSoyad.ErrorText = "Ad soyad zorunludur.";
            dxValidationProvider.SetValidationRule(txtAdSoyad, kuralAdSoyad);
            //
            // txtKullaniciAdi
            //
            txtKullaniciAdi.Name = "txtKullaniciAdi";
            txtKullaniciAdi.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            txtKullaniciAdi.StyleController = layoutControl;
            txtKullaniciAdi.TabIndex = 1;
            kuralKullaniciAdi.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralKullaniciAdi.ErrorText = "Kullanıcı adı zorunludur.";
            dxValidationProvider.SetValidationRule(txtKullaniciAdi, kuralKullaniciAdi);
            //
            // cmbRol
            //
            cmbRol.Name = "cmbRol";
            cmbRol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbRol.StyleController = layoutControl;
            cmbRol.TabIndex = 2;
            kuralRol.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralRol.ErrorText = "Rol seçilmelidir.";
            dxValidationProvider.SetValidationRule(cmbRol, kuralRol);
            //
            // txtSifre
            //
            txtSifre.Name = "txtSifre";
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifre.Properties.NullValuePrompt = "En az 8 karakter";
            txtSifre.StyleController = layoutControl;
            txtSifre.TabIndex = 3;
            kuralSifre.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralSifre.ErrorText = "Şifre zorunludur.";
            dxValidationProvider.SetValidationRule(txtSifre, kuralSifre);
            //
            // txtSifreTekrar
            //
            txtSifreTekrar.Name = "txtSifreTekrar";
            txtSifreTekrar.Properties.UseSystemPasswordChar = true;
            txtSifreTekrar.StyleController = layoutControl;
            txtSifreTekrar.TabIndex = 4;
            kuralSifreTekrar.CompareControlOperator = DevExpress.XtraEditors.DXErrorProvider.CompareControlOperator.Equals;
            kuralSifreTekrar.Control = txtSifre;
            kuralSifreTekrar.ErrorText = "Şifreler eşleşmiyor.";
            dxValidationProvider.SetValidationRule(txtSifreTekrar, kuralSifreTekrar);
            //
            // btnKaydet
            //
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Text = "Kaydet";
            btnKaydet.Click += btnKaydet_Click;
            btnKaydet.StyleController = layoutControl;
            btnKaydet.TabIndex = 5;
            //
            // btnIptal
            //
            btnIptal.Name = "btnIptal";
            btnIptal.CausesValidation = false;
            btnIptal.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnIptal.Text = "İptal";
            btnIptal.StyleController = layoutControl;
            btnIptal.TabIndex = 6;
            //
            // Root
            //
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciAdSoyad, lciKullaniciAdi, lciRol, grpSifre, esiBosluk, esiButonSol, lciKaydet, lciIptal });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(420, 250);
            Root.TextVisible = false;
            //
            // lciAdSoyad
            //
            lciAdSoyad.Control = txtAdSoyad;
            lciAdSoyad.Location = new System.Drawing.Point(0, 0);
            lciAdSoyad.Name = "lciAdSoyad";
            lciAdSoyad.Size = new System.Drawing.Size(400, 24);
            lciAdSoyad.Text = "Ad Soyad *";
            lciAdSoyad.TextSize = new System.Drawing.Size(110, 13);
            //
            // lciKullaniciAdi
            //
            lciKullaniciAdi.Control = txtKullaniciAdi;
            lciKullaniciAdi.Location = new System.Drawing.Point(0, 24);
            lciKullaniciAdi.Name = "lciKullaniciAdi";
            lciKullaniciAdi.Size = new System.Drawing.Size(400, 24);
            lciKullaniciAdi.Text = "Kullanıcı adı *";
            lciKullaniciAdi.TextSize = new System.Drawing.Size(110, 13);
            //
            // lciRol
            //
            lciRol.Control = cmbRol;
            lciRol.Location = new System.Drawing.Point(0, 48);
            lciRol.Name = "lciRol";
            lciRol.Size = new System.Drawing.Size(400, 24);
            lciRol.Text = "Rol *";
            lciRol.TextSize = new System.Drawing.Size(110, 13);
            //
            // grpSifre
            //
            grpSifre.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciSifre, lciSifreTekrar });
            grpSifre.Location = new System.Drawing.Point(0, 72);
            grpSifre.Name = "grpSifre";
            grpSifre.Size = new System.Drawing.Size(400, 90);
            grpSifre.Text = "Şifre";
            //
            // lciSifre
            //
            lciSifre.Control = txtSifre;
            lciSifre.Location = new System.Drawing.Point(0, 0);
            lciSifre.Name = "lciSifre";
            lciSifre.Size = new System.Drawing.Size(376, 24);
            lciSifre.Text = "Şifre *";
            lciSifre.TextSize = new System.Drawing.Size(110, 13);
            //
            // lciSifreTekrar
            //
            lciSifreTekrar.Control = txtSifreTekrar;
            lciSifreTekrar.Location = new System.Drawing.Point(0, 24);
            lciSifreTekrar.Name = "lciSifreTekrar";
            lciSifreTekrar.Size = new System.Drawing.Size(376, 24);
            lciSifreTekrar.Text = "Şifre (tekrar) *";
            lciSifreTekrar.TextSize = new System.Drawing.Size(110, 13);
            //
            // esiBosluk
            //
            esiBosluk.Location = new System.Drawing.Point(0, 162);
            esiBosluk.Name = "esiBosluk";
            esiBosluk.Size = new System.Drawing.Size(400, 10);
            //
            // esiButonSol
            //
            esiButonSol.Location = new System.Drawing.Point(0, 172);
            esiButonSol.Name = "esiButonSol";
            esiButonSol.Size = new System.Drawing.Size(200, 26);
            //
            // lciKaydet
            //
            lciKaydet.Control = btnKaydet;
            lciKaydet.Location = new System.Drawing.Point(200, 172);
            lciKaydet.Name = "lciKaydet";
            lciKaydet.Size = new System.Drawing.Size(100, 26);
            lciKaydet.TextVisible = false;
            //
            // lciIptal
            //
            lciIptal.Control = btnIptal;
            lciIptal.Location = new System.Drawing.Point(300, 172);
            lciIptal.Name = "lciIptal";
            lciIptal.Size = new System.Drawing.Size(100, 26);
            lciIptal.TextVisible = false;
            //
            // dxValidationProvider
            //
            dxValidationProvider.ValidateHiddenControls = false;
            dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            //
            // PersonelFormu
            //
            AcceptButton = btnKaydet;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnIptal;
            ClientSize = new System.Drawing.Size(420, 250);
            Controls.Add(layoutControl);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PersonelFormu";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Personel";
            ((System.ComponentModel.ISupportInitialize)layoutControl).EndInit();
            layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtAdSoyad.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtKullaniciAdi.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbRol.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciAdSoyad).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciKullaniciAdi).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciRol).EndInit();
            ((System.ComponentModel.ISupportInitialize)grpSifre).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciSifre).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciSifreTekrar).EndInit();
            ((System.ComponentModel.ISupportInitialize)esiBosluk).EndInit();
            ((System.ComponentModel.ISupportInitialize)esiButonSol).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciKaydet).EndInit();
            ((System.ComponentModel.ISupportInitialize)lciIptal).EndInit();
            ((System.ComponentModel.ISupportInitialize)dxValidationProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraEditors.TextEdit txtAdSoyad;
        private DevExpress.XtraEditors.TextEdit txtKullaniciAdi;
        private DevExpress.XtraEditors.ImageComboBoxEdit cmbRol;
        private DevExpress.XtraEditors.TextEdit txtSifre;
        private DevExpress.XtraEditors.TextEdit txtSifreTekrar;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.SimpleButton btnIptal;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciAdSoyad;
        private DevExpress.XtraLayout.LayoutControlItem lciKullaniciAdi;
        private DevExpress.XtraLayout.LayoutControlItem lciRol;
        private DevExpress.XtraLayout.LayoutControlGroup grpSifre;
        private DevExpress.XtraLayout.LayoutControlItem lciSifre;
        private DevExpress.XtraLayout.LayoutControlItem lciSifreTekrar;
        private DevExpress.XtraLayout.EmptySpaceItem esiBosluk;
        private DevExpress.XtraLayout.EmptySpaceItem esiButonSol;
        private DevExpress.XtraLayout.LayoutControlItem lciKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciIptal;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}
