namespace SuSporKiralama.UI.Formlar
{
    partial class AnaForm
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
            ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
            btnYeniKiralama = new DevExpress.XtraBars.BarButtonItem();
            btnRezervasyon = new DevExpress.XtraBars.BarButtonItem();
            btnTeslimEt = new DevExpress.XtraBars.BarButtonItem();
            btnIadeAl = new DevExpress.XtraBars.BarButtonItem();
            btnAktifKiralamalar = new DevExpress.XtraBars.BarButtonItem();
            btnEkipmanlar = new DevExpress.XtraBars.BarButtonItem();
            btnMusteriler = new DevExpress.XtraBars.BarButtonItem();
            btnPersonel = new DevExpress.XtraBars.BarButtonItem();
            btnDashboard = new DevExpress.XtraBars.BarButtonItem();
            btnEkipmanOnerisi = new DevExpress.XtraBars.BarButtonItem();
            btnCikisYap = new DevExpress.XtraBars.BarButtonItem();
            galeriTema = new DevExpress.XtraBars.SkinRibbonGalleryBarItem();
            lblPersonel = new DevExpress.XtraBars.BarStaticItem();
            lblVeritabani = new DevExpress.XtraBars.BarStaticItem();
            lblSaat = new DevExpress.XtraBars.BarStaticItem();
            rpKiralama = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgKiralama = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpKayitlar = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgKayitlar = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpRaporlar = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgRaporlar = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpYapayZeka = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgYapayZeka = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpSistem = new DevExpress.XtraBars.Ribbon.RibbonPage();
            rpgTema = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            rpgOturum = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            documentManager = new DevExpress.XtraBars.Docking2010.DocumentManager(components);
            tabbedView = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(components);
            timerSaat = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)ribbonControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)documentManager).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabbedView).BeginInit();
            SuspendLayout();
            //
            // ribbonControl
            //
            ribbonControl.ExpandCollapseItem.Id = 0;
            ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[] { ribbonControl.ExpandCollapseItem, btnYeniKiralama, btnRezervasyon, btnTeslimEt, btnIadeAl, btnAktifKiralamalar, btnEkipmanlar, btnMusteriler, btnPersonel, btnDashboard, btnEkipmanOnerisi, btnCikisYap, galeriTema, lblPersonel, lblVeritabani, lblSaat });
            ribbonControl.Location = new System.Drawing.Point(0, 0);
            ribbonControl.MaxItemId = 16;
            ribbonControl.Name = "ribbonControl";
            ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] { rpKiralama, rpKayitlar, rpRaporlar, rpYapayZeka, rpSistem });
            ribbonControl.Size = new System.Drawing.Size(1024, 158);
            ribbonControl.StatusBar = ribbonStatusBar;
            //
            // btnYeniKiralama
            //
            btnYeniKiralama.Caption = "Yeni Kiralama";
            btnYeniKiralama.Id = 1;
            btnYeniKiralama.Name = "btnYeniKiralama";
            btnYeniKiralama.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnYeniKiralama.ItemClick += YerTutucu_ItemClick;
            //
            // btnRezervasyon
            //
            btnRezervasyon.Caption = "Rezervasyon";
            btnRezervasyon.Id = 2;
            btnRezervasyon.Name = "btnRezervasyon";
            btnRezervasyon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnRezervasyon.ItemClick += YerTutucu_ItemClick;
            //
            // btnTeslimEt
            //
            btnTeslimEt.Caption = "Teslim Et";
            btnTeslimEt.Id = 3;
            btnTeslimEt.Name = "btnTeslimEt";
            btnTeslimEt.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnTeslimEt.ItemClick += YerTutucu_ItemClick;
            //
            // btnIadeAl
            //
            btnIadeAl.Caption = "İade Al";
            btnIadeAl.Id = 4;
            btnIadeAl.Name = "btnIadeAl";
            btnIadeAl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnIadeAl.ItemClick += YerTutucu_ItemClick;
            //
            // btnAktifKiralamalar
            //
            btnAktifKiralamalar.Caption = "Aktif Kiralamalar";
            btnAktifKiralamalar.Id = 5;
            btnAktifKiralamalar.Name = "btnAktifKiralamalar";
            btnAktifKiralamalar.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnAktifKiralamalar.ItemClick += YerTutucu_ItemClick;
            //
            // btnEkipmanlar
            //
            btnEkipmanlar.Caption = "Ekipmanlar";
            btnEkipmanlar.Id = 6;
            btnEkipmanlar.Name = "btnEkipmanlar";
            btnEkipmanlar.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnEkipmanlar.ItemClick += YerTutucu_ItemClick;
            //
            // btnMusteriler
            //
            btnMusteriler.Caption = "Müşteriler";
            btnMusteriler.Id = 7;
            btnMusteriler.Name = "btnMusteriler";
            btnMusteriler.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnMusteriler.ItemClick += YerTutucu_ItemClick;
            //
            // btnPersonel
            //
            btnPersonel.Caption = "Personel";
            btnPersonel.Id = 8;
            btnPersonel.Name = "btnPersonel";
            btnPersonel.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnPersonel.ItemClick += YerTutucu_ItemClick;
            //
            // btnDashboard
            //
            btnDashboard.Caption = "Dashboard";
            btnDashboard.Id = 9;
            btnDashboard.Name = "btnDashboard";
            btnDashboard.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnDashboard.ItemClick += YerTutucu_ItemClick;
            //
            // btnEkipmanOnerisi
            //
            btnEkipmanOnerisi.Caption = "Ekipman Önerisi";
            btnEkipmanOnerisi.Id = 10;
            btnEkipmanOnerisi.Name = "btnEkipmanOnerisi";
            btnEkipmanOnerisi.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnEkipmanOnerisi.ItemClick += YerTutucu_ItemClick;
            //
            // btnCikisYap
            //
            btnCikisYap.Caption = "Çıkış Yap";
            btnCikisYap.Id = 11;
            btnCikisYap.Name = "btnCikisYap";
            btnCikisYap.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            btnCikisYap.ItemClick += btnCikisYap_ItemClick;
            //
            // galeriTema
            //
            galeriTema.Caption = "Tema";
            galeriTema.Id = 12;
            galeriTema.Name = "galeriTema";
            //
            // lblPersonel
            //
            lblPersonel.Caption = "Personel";
            lblPersonel.Id = 13;
            lblPersonel.Name = "lblPersonel";
            //
            // lblVeritabani
            //
            lblVeritabani.Caption = "Veritabanı";
            lblVeritabani.Id = 14;
            lblVeritabani.Name = "lblVeritabani";
            //
            // lblSaat
            //
            lblSaat.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            lblSaat.Caption = "Saat";
            lblSaat.Id = 15;
            lblSaat.Name = "lblSaat";
            //
            // rpKiralama
            //
            rpKiralama.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgKiralama });
            rpKiralama.Name = "rpKiralama";
            rpKiralama.Text = "Kiralama";
            //
            // rpgKiralama
            //
            rpgKiralama.ItemLinks.Add(btnYeniKiralama);
            rpgKiralama.ItemLinks.Add(btnRezervasyon);
            rpgKiralama.ItemLinks.Add(btnTeslimEt);
            rpgKiralama.ItemLinks.Add(btnIadeAl);
            rpgKiralama.ItemLinks.Add(btnAktifKiralamalar);
            rpgKiralama.Name = "rpgKiralama";
            rpgKiralama.Text = "Kiralama İşlemleri";
            //
            // rpKayitlar
            //
            rpKayitlar.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgKayitlar });
            rpKayitlar.Name = "rpKayitlar";
            rpKayitlar.Text = "Kayıtlar";
            //
            // rpgKayitlar
            //
            rpgKayitlar.ItemLinks.Add(btnEkipmanlar);
            rpgKayitlar.ItemLinks.Add(btnMusteriler);
            rpgKayitlar.ItemLinks.Add(btnPersonel);
            rpgKayitlar.Name = "rpgKayitlar";
            rpgKayitlar.Text = "Kayıtlar";
            //
            // rpRaporlar
            //
            rpRaporlar.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgRaporlar });
            rpRaporlar.Name = "rpRaporlar";
            rpRaporlar.Text = "Raporlar";
            //
            // rpgRaporlar
            //
            rpgRaporlar.ItemLinks.Add(btnDashboard);
            rpgRaporlar.Name = "rpgRaporlar";
            rpgRaporlar.Text = "Raporlar";
            //
            // rpYapayZeka
            //
            rpYapayZeka.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgYapayZeka });
            rpYapayZeka.Name = "rpYapayZeka";
            rpYapayZeka.Text = "Yapay Zeka";
            //
            // rpgYapayZeka
            //
            rpgYapayZeka.ItemLinks.Add(btnEkipmanOnerisi);
            rpgYapayZeka.Name = "rpgYapayZeka";
            rpgYapayZeka.Text = "Öneri";
            //
            // rpSistem
            //
            rpSistem.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] { rpgTema, rpgOturum });
            rpSistem.Name = "rpSistem";
            rpSistem.Text = "Sistem";
            //
            // rpgTema
            //
            rpgTema.ItemLinks.Add(galeriTema);
            rpgTema.Name = "rpgTema";
            rpgTema.Text = "Tema";
            //
            // rpgOturum
            //
            rpgOturum.ItemLinks.Add(btnCikisYap);
            rpgOturum.Name = "rpgOturum";
            rpgOturum.Text = "Oturum";
            //
            // ribbonStatusBar
            //
            ribbonStatusBar.ItemLinks.Add(lblPersonel);
            ribbonStatusBar.ItemLinks.Add(lblVeritabani);
            ribbonStatusBar.ItemLinks.Add(lblSaat);
            ribbonStatusBar.Location = new System.Drawing.Point(0, 676);
            ribbonStatusBar.Name = "ribbonStatusBar";
            ribbonStatusBar.Ribbon = ribbonControl;
            ribbonStatusBar.Size = new System.Drawing.Size(1024, 24);
            //
            // documentManager
            //
            documentManager.MdiParent = this;
            documentManager.MenuManager = ribbonControl;
            documentManager.View = tabbedView;
            documentManager.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] { tabbedView });
            //
            // timerSaat
            //
            timerSaat.Enabled = true;
            timerSaat.Interval = 1000;
            timerSaat.Tick += timerSaat_Tick;
            //
            // AnaForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1024, 700);
            Controls.Add(ribbonStatusBar);
            Controls.Add(ribbonControl);
            IsMdiContainer = true;
            Name = "AnaForm";
            Ribbon = ribbonControl;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            StatusBar = ribbonStatusBar;
            Text = "Su Sporları Kiralama";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Load += AnaForm_Load;
            ((System.ComponentModel.ISupportInitialize)ribbonControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)documentManager).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabbedView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl;
        private DevExpress.XtraBars.BarButtonItem btnYeniKiralama;
        private DevExpress.XtraBars.BarButtonItem btnRezervasyon;
        private DevExpress.XtraBars.BarButtonItem btnTeslimEt;
        private DevExpress.XtraBars.BarButtonItem btnIadeAl;
        private DevExpress.XtraBars.BarButtonItem btnAktifKiralamalar;
        private DevExpress.XtraBars.BarButtonItem btnEkipmanlar;
        private DevExpress.XtraBars.BarButtonItem btnMusteriler;
        private DevExpress.XtraBars.BarButtonItem btnPersonel;
        private DevExpress.XtraBars.BarButtonItem btnDashboard;
        private DevExpress.XtraBars.BarButtonItem btnEkipmanOnerisi;
        private DevExpress.XtraBars.BarButtonItem btnCikisYap;
        private DevExpress.XtraBars.SkinRibbonGalleryBarItem galeriTema;
        private DevExpress.XtraBars.BarStaticItem lblPersonel;
        private DevExpress.XtraBars.BarStaticItem lblVeritabani;
        private DevExpress.XtraBars.BarStaticItem lblSaat;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpKiralama;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgKiralama;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpKayitlar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgKayitlar;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpRaporlar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgRaporlar;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpYapayZeka;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgYapayZeka;
        private DevExpress.XtraBars.Ribbon.RibbonPage rpSistem;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgTema;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup rpgOturum;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView;
        private System.Windows.Forms.Timer timerSaat;
    }
}
