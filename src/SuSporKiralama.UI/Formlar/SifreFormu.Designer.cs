namespace SuSporKiralama.UI.Formlar
{
    partial class SifreFormu
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
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule kuralSifre = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule kuralSifreTekrar = new DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule();
            layoutControl = new DevExpress.XtraLayout.LayoutControl();
            txtSifre = new DevExpress.XtraEditors.TextEdit();
            txtSifreTekrar = new DevExpress.XtraEditors.TextEdit();
            btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            btnIptal = new DevExpress.XtraEditors.SimpleButton();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            lciSifre = new DevExpress.XtraLayout.LayoutControlItem();
            lciSifreTekrar = new DevExpress.XtraLayout.LayoutControlItem();
            esiBosluk = new DevExpress.XtraLayout.EmptySpaceItem();
            esiButonSol = new DevExpress.XtraLayout.EmptySpaceItem();
            lciKaydet = new DevExpress.XtraLayout.LayoutControlItem();
            lciIptal = new DevExpress.XtraLayout.LayoutControlItem();
            dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(components);
            ((System.ComponentModel.ISupportInitialize)layoutControl).BeginInit();
            layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrar.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
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
            layoutControl.Controls.Add(txtSifre);
            layoutControl.Controls.Add(txtSifreTekrar);
            layoutControl.Controls.Add(btnKaydet);
            layoutControl.Controls.Add(btnIptal);
            layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl.Location = new System.Drawing.Point(0, 0);
            layoutControl.Name = "layoutControl";
            layoutControl.Root = Root;
            layoutControl.Size = new System.Drawing.Size(400, 130);
            layoutControl.TabIndex = 0;
            //
            // txtSifre
            //
            txtSifre.Name = "txtSifre";
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifre.Properties.NullValuePrompt = "En az 8 karakter";
            txtSifre.StyleController = layoutControl;
            txtSifre.TabIndex = 0;
            kuralSifre.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            kuralSifre.ErrorText = "Şifre zorunludur.";
            dxValidationProvider.SetValidationRule(txtSifre, kuralSifre);
            //
            // txtSifreTekrar
            //
            txtSifreTekrar.Name = "txtSifreTekrar";
            txtSifreTekrar.Properties.UseSystemPasswordChar = true;
            txtSifreTekrar.StyleController = layoutControl;
            txtSifreTekrar.TabIndex = 1;
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
            btnKaydet.TabIndex = 2;
            //
            // btnIptal
            //
            btnIptal.Name = "btnIptal";
            btnIptal.CausesValidation = false;
            btnIptal.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnIptal.Text = "İptal";
            btnIptal.StyleController = layoutControl;
            btnIptal.TabIndex = 3;
            //
            // Root
            //
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { lciSifre, lciSifreTekrar, esiBosluk, esiButonSol, lciKaydet, lciIptal });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(400, 130);
            Root.TextVisible = false;
            //
            // lciSifre
            //
            lciSifre.Control = txtSifre;
            lciSifre.Location = new System.Drawing.Point(0, 0);
            lciSifre.Name = "lciSifre";
            lciSifre.Size = new System.Drawing.Size(380, 24);
            lciSifre.Text = "Yeni şifre *";
            lciSifre.TextSize = new System.Drawing.Size(110, 13);
            //
            // lciSifreTekrar
            //
            lciSifreTekrar.Control = txtSifreTekrar;
            lciSifreTekrar.Location = new System.Drawing.Point(0, 24);
            lciSifreTekrar.Name = "lciSifreTekrar";
            lciSifreTekrar.Size = new System.Drawing.Size(380, 24);
            lciSifreTekrar.Text = "Yeni şifre (tekrar) *";
            lciSifreTekrar.TextSize = new System.Drawing.Size(110, 13);
            //
            // esiBosluk
            //
            esiBosluk.Location = new System.Drawing.Point(0, 48);
            esiBosluk.Name = "esiBosluk";
            esiBosluk.Size = new System.Drawing.Size(380, 36);
            //
            // esiButonSol
            //
            esiButonSol.Location = new System.Drawing.Point(0, 84);
            esiButonSol.Name = "esiButonSol";
            esiButonSol.Size = new System.Drawing.Size(180, 26);
            //
            // lciKaydet
            //
            lciKaydet.Control = btnKaydet;
            lciKaydet.Location = new System.Drawing.Point(180, 84);
            lciKaydet.Name = "lciKaydet";
            lciKaydet.Size = new System.Drawing.Size(100, 26);
            lciKaydet.TextVisible = false;
            //
            // lciIptal
            //
            lciIptal.Control = btnIptal;
            lciIptal.Location = new System.Drawing.Point(280, 84);
            lciIptal.Name = "lciIptal";
            lciIptal.Size = new System.Drawing.Size(100, 26);
            lciIptal.TextVisible = false;
            //
            // dxValidationProvider
            //
            dxValidationProvider.ValidateHiddenControls = false;
            dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            //
            // SifreFormu
            //
            AcceptButton = btnKaydet;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnIptal;
            ClientSize = new System.Drawing.Size(400, 130);
            Controls.Add(layoutControl);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SifreFormu";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Şifre Değiştir";
            ((System.ComponentModel.ISupportInitialize)layoutControl).EndInit();
            layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrar.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
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
        private DevExpress.XtraEditors.TextEdit txtSifre;
        private DevExpress.XtraEditors.TextEdit txtSifreTekrar;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.SimpleButton btnIptal;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciSifre;
        private DevExpress.XtraLayout.LayoutControlItem lciSifreTekrar;
        private DevExpress.XtraLayout.EmptySpaceItem esiBosluk;
        private DevExpress.XtraLayout.EmptySpaceItem esiButonSol;
        private DevExpress.XtraLayout.LayoutControlItem lciKaydet;
        private DevExpress.XtraLayout.LayoutControlItem lciIptal;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}
