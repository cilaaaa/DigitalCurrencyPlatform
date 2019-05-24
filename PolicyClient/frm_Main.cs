using C1.Win.C1Ribbon;
using DataBase;
using GeneralForm;
using StockData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PolicyClient
{
    public partial class frm_Main : C1RibbonForm
    {
        IniFile inifile;
        public frm_Main()
        {
            InitializeComponent();
            GlobalValue.Initialize();
            PolicyProgram.Load();
            CheckDirectory();
            inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
        }

        private void CheckDirectory()
        {
            if (!Directory.Exists(ConfigFileName.TradeLogDriectory))
            {
                Directory.CreateDirectory(ConfigFileName.TradeLogDriectory);
            }
        }


        private void frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void ribbonButton2_Click(object sender, EventArgs e)
        {
            frm_Monitor frm = new frm_Monitor();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton3_Click(object sender, EventArgs e)
        {
            frm_TickReviewA frm = new frm_TickReviewA();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            frm_backtest frm = new frm_backtest();
            //frm.MdiParent = this;
            frm.Show();
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            
        }

        private void ribbonButton4_Click(object sender, EventArgs e)
        {
            frm_PolicyUpdate frm = new frm_PolicyUpdate();
            frm.ShowDialog();
        }

        private void ribbonButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void bt_bidamt_Click(object sender, EventArgs e)
        {
            //frm_calculateLargeVolumn frm = new frm_calculateLargeVolumn();
            //frm.ShowDialog();
            //return;
        }

        private void frm_Main_Shown(object sender, EventArgs e)
        {
            this.Height = c1Ribbon1.Height;
        }

        private void ribbonButton7_Click(object sender, EventArgs e)
        {
            frm_kReview frm = new frm_kReview();
            //frm.MdiParent = this;
            frm.Show();
        }
    }
}
