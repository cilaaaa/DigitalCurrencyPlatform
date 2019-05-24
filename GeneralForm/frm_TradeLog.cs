using C1.Win.C1Ribbon;
using System;
using System.IO;
using System.Windows.Forms;

namespace GeneralForm
{
    public partial class frm_TradeLog : C1RibbonForm
    {
        public frm_TradeLog()
        {
            InitializeComponent();
        }
        protected override void WndProc(ref Message m)
        {

            const int WM_SYSCOMMAND = 0x0112;


            const int SC_CLOSE = 0xF060;


            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                // 屏蔽传入的消息事件
                //this.WindowState = FormWindowState.Minimized;
                return;
            }
            base.WndProc(ref m);
        }
        delegate void LogDelegate(string logtext);
        public void Log(string logtext)
        {
            if (this.list_log.InvokeRequired)
            {
                this.Invoke(new LogDelegate(Log), new object[] { logtext});
            }
            else
            {
                
                try
                {
                    string logstring = string.Format("{0}>{1}", System.DateTime.Now.ToString("HH:mm:ss"), logtext);
                    string writestring = string.Format("{0}>{1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logtext);
                    try
                    {
                        if (this.list_log.Items.Count > 1000)
                        {
                            this.list_log.Items.Clear();
                        }
                        
                        this.list_log.Items.Insert(0, logstring);
                    }
                    catch { }
                    using (StreamWriter file  = new StreamWriter(string.Format("{0}\\trade.log", Application.StartupPath),true)) 
                    {
                        file.WriteLine(writestring);
                        file.Close();
                    }
                }
                catch { }
            }
        }

        private void frm_TradeLog_Shown(object sender, EventArgs e)
        {
            Log("程序启动");
        }

        private void frm_TradeLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log("程序退出");
        }
    }
}
