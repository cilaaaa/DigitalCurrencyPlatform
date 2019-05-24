using C1.Win.C1Ribbon;
using System;
using System.IO;
using System.Windows.Forms;

namespace GeneralForm
{
    public partial class frm_ReceiveLog : C1RibbonForm
    {
        bool lastWriteToFile;
        public frm_ReceiveLog()
        {
            InitializeComponent();
            lastWriteToFile = true;
            GUITools.DoubleBuffer(this.grid_list, true);
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

        delegate void LogServerDelegate(string logtext);
        public void LogServer(string logtext)
        {
            if(this.tx_serverLog.InvokeRequired)
            {
                this.Invoke(new LogServerDelegate(LogServer), new object[] { logtext });
            }
            else
            {
                try
                {
                    if (this.tx_serverLog.Lines.Length >= 100)
                        this.tx_serverLog.Clear();
                    string logstring = string.Format("{0}>{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logtext,Environment.NewLine);
                    this.tx_serverLog.AppendText(logstring);
                }
                catch { }
            }
        }




        delegate void LogDelegate(string logtext, bool writeFile);
        public void Log(string logtext, bool writeFile)
        {
            if (this.tx_log.InvokeRequired)
            {
                this.Invoke(new LogDelegate(Log), new object[] { logtext,writeFile });
            }
            else
            {
                try
                {
                    if (this.tx_log.Lines.Length >= 200)
                        this.tx_log.Clear();
                    if (this.tx_log.Text.Length >= 1000)
                        this.tx_log.Clear();
                    string logstring = string.Format("{0}>{1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logtext);
                    
                    //this.tx_log.AppendText(string.Format("{0}{1}", logstring, System.Environment.NewLine));
                    if (writeFile)
                    {
                        if(!lastWriteToFile)
                        {
                            this.tx_log.AppendText(System.Environment.NewLine);
                        }
                        this.tx_log.AppendText(string.Format("{0}{1}", logstring, System.Environment.NewLine));

                        StreamWriter sw = new StreamWriter(string.Format("{0}\\log.txt", Application.StartupPath), true);
                        sw.WriteLine(logstring);
                        sw.Close();
                    }
                    else
                    {
                        if (lastWriteToFile)
                        {
                            this.tx_log.AppendText(string.Format("{0}>{1}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logtext));
                        }
                        else
                        {
                            this.tx_log.AppendText(logtext);
                        }
                    }
                    lastWriteToFile = writeFile;
                }
                catch { }
            }
        }

        delegate void ClearListDelegate();
        public void ClearList()
        {
            if(this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new ClearListDelegate(ClearList));
                }
                catch { }
            }
            else
            {
                try
                {
                    grid_list.Rows.RemoveRange(1, grid_list.Rows.Count - 1);
                }
                catch { }
            }
        }
        delegate void AddListDelegate(string name, string status, string server);
        public void AddList(string name, string status, string server)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddListDelegate(AddList), new object[] { name, status, server });
            }
            else
            {
                try
                {
                    this.grid_list.Rows.Add();
                    int index = grid_list.Rows.Count - 1;
                    this.grid_list.Rows[index][1] = name;
                    this.grid_list.Rows[index][2] = status;
                    this.grid_list.Rows[index][3] = server;
                }
                catch { }
            }
        }


        delegate void UpdateConnectingDelegate(string name);
        public void UpdateConnecting(string name)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateConnectingDelegate(UpdateConnecting), new object[] { name });
            }
            else
            {
                for (int i = 0; i < this.grid_list.Rows.Count; i++)
                {
                    if (grid_list.Rows[i][1].ToString() == name)
                    {
                        this.grid_list.Rows[i][2] = "正在连接";
                        return;
                    }
                }
            }
        }

        delegate void ErrorConnectDelegate(string name);
        public void ErrorConnect(string name)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ErrorConnectDelegate(ErrorConnect), new object[] { name });
            }
            else
            {
                for (int i = 0; i < this.grid_list.Rows.Count; i++)
                {
                    if (grid_list.Rows[i][1].ToString() == name)
                    {
                        this.grid_list.Rows[i][2] = "连接错误";
                        this.grid_list.Rows[i][3] = string.Empty;
                        return;
                    }
                }
            }
        }

        delegate void ConnectedDelegate(string name,string server);
        public void Connected(string name, string server)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ConnectedDelegate(Connected), new object[] { name,server });
            }
            else
            {
                for (int i = 0; i < this.grid_list.Rows.Count; i++)
                {
                    if (grid_list.Rows[i][1].ToString() == name)
                    {
                        this.grid_list.Rows[i][2]= "已连接";
                        this.grid_list.Rows[i][3] = server;
                        return;
                    }
                }
            }
        }
    }
}
