namespace SuSporKiralama.UI.Formlar
{
    partial class GirisFormu
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
            lblBaslik = new DevExpress.XtraEditors.LabelControl();
            lblKullaniciAdi = new DevExpress.XtraEditors.LabelControl();
            txtKullaniciAdi = new DevExpress.XtraEditors.TextEdit();
            lblSifre = new DevExpress.XtraEditors.LabelControl();
            txtSifre = new DevExpress.XtraEditors.TextEdit();
            btnGiris = new DevExpress.XtraEditors.SimpleButton();
            btnKapat = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)txtKullaniciAdi.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).BeginInit();
            SuspendLayout();
            //
            // lblBaslik
            //
            lblBaslik.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblBaslik.Appearance.Options.UseFont = true;
            lblBaslik.Location = new System.Drawing.Point(24, 20);
            lblBaslik.Name = "lblBaslik";
            lblBaslik.Size = new System.Drawing.Size(206, 25);
            lblBaslik.TabIndex = 0;
            lblBaslik.Text = "Su Sporları Kiralama";
            //
            // lblKullaniciAdi
            //
            lblKullaniciAdi.Location = new System.Drawing.Point(24, 66);
            lblKullaniciAdi.Name = "lblKullaniciAdi";
            lblKullaniciAdi.Size = new System.Drawing.Size(62, 13);
            lblKullaniciAdi.TabIndex = 1;
            lblKullaniciAdi.Text = "Kullanıcı adı";
            //
            // txtKullaniciAdi
            //
            txtKullaniciAdi.Location = new System.Drawing.Point(24, 85);
            txtKullaniciAdi.Name = "txtKullaniciAdi";
            txtKullaniciAdi.Size = new System.Drawing.Size(280, 20);
            txtKullaniciAdi.TabIndex = 2;
            //
            // lblSifre
            //
            lblSifre.Location = new System.Drawing.Point(24, 117);
            lblSifre.Name = "lblSifre";
            lblSifre.Size = new System.Drawing.Size(24, 13);
            lblSifre.TabIndex = 3;
            lblSifre.Text = "Şifre";
            //
            // txtSifre
            //
            txtSifre.Location = new System.Drawing.Point(24, 136);
            txtSifre.Name = "txtSifre";
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifre.Size = new System.Drawing.Size(280, 20);
            txtSifre.TabIndex = 4;
            //
            // btnGiris
            //
            btnGiris.Location = new System.Drawing.Point(124, 178);
            btnGiris.Name = "btnGiris";
            btnGiris.Size = new System.Drawing.Size(88, 28);
            btnGiris.TabIndex = 5;
            btnGiris.Text = "Giriş";
            btnGiris.Click += btnGiris_Click;
            //
            // btnKapat
            //
            btnKapat.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnKapat.Location = new System.Drawing.Point(216, 178);
            btnKapat.Name = "btnKapat";
            btnKapat.Size = new System.Drawing.Size(88, 28);
            btnKapat.TabIndex = 6;
            btnKapat.Text = "Kapat";
            //
            // GirisFormu
            //
            AcceptButton = btnGiris;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnKapat;
            ClientSize = new System.Drawing.Size(328, 228);
            Controls.Add(btnKapat);
            Controls.Add(btnGiris);
            Controls.Add(txtSifre);
            Controls.Add(lblSifre);
            Controls.Add(txtKullaniciAdi);
            Controls.Add(lblKullaniciAdi);
            Controls.Add(lblBaslik);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GirisFormu";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Giriş";
            ((System.ComponentModel.ISupportInitialize)txtKullaniciAdi.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblBaslik;
        private DevExpress.XtraEditors.LabelControl lblKullaniciAdi;
        private DevExpress.XtraEditors.TextEdit txtKullaniciAdi;
        private DevExpress.XtraEditors.LabelControl lblSifre;
        private DevExpress.XtraEditors.TextEdit txtSifre;
        private DevExpress.XtraEditors.SimpleButton btnGiris;
        private DevExpress.XtraEditors.SimpleButton btnKapat;
    }
}
