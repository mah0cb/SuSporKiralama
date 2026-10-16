namespace SuSporKiralama.UI.Ekranlar
{
    partial class BaslangicEkrani
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
            lblHosgeldiniz = new DevExpress.XtraEditors.LabelControl();
            lblRol = new DevExpress.XtraEditors.LabelControl();
            lblAciklama = new DevExpress.XtraEditors.LabelControl();
            SuspendLayout();
            //
            // lblHosgeldiniz
            //
            lblHosgeldiniz.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            lblHosgeldiniz.Appearance.Options.UseFont = true;
            lblHosgeldiniz.Location = new System.Drawing.Point(32, 32);
            lblHosgeldiniz.Name = "lblHosgeldiniz";
            lblHosgeldiniz.Size = new System.Drawing.Size(158, 32);
            lblHosgeldiniz.TabIndex = 0;
            lblHosgeldiniz.Text = "Hoş geldiniz";
            //
            // lblRol
            //
            lblRol.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F);
            lblRol.Appearance.Options.UseFont = true;
            lblRol.Location = new System.Drawing.Point(34, 78);
            lblRol.Name = "lblRol";
            lblRol.Size = new System.Drawing.Size(55, 20);
            lblRol.TabIndex = 1;
            lblRol.Text = "Rolünüz:";
            //
            // lblAciklama
            //
            lblAciklama.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblAciklama.Appearance.Options.UseFont = true;
            lblAciklama.Location = new System.Drawing.Point(34, 116);
            lblAciklama.Name = "lblAciklama";
            lblAciklama.Size = new System.Drawing.Size(453, 17);
            lblAciklama.TabIndex = 2;
            lblAciklama.Text = "Yapmak istediğiniz işlemi yukarıdaki menüden seçebilirsiniz. İyi çalışmalar!";
            //
            // BaslangicEkrani
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(lblAciklama);
            Controls.Add(lblRol);
            Controls.Add(lblHosgeldiniz);
            Name = "BaslangicEkrani";
            Size = new System.Drawing.Size(640, 400);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblHosgeldiniz;
        private DevExpress.XtraEditors.LabelControl lblRol;
        private DevExpress.XtraEditors.LabelControl lblAciklama;
    }
}
