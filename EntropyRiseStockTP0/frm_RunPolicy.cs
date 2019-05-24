using C1.Win.C1FlexGrid;
using C1.Win.C1Ribbon;
using DataAPI;
using DataHub;
using GeneralForm;
using StockData;
using StockPolicies;
using StockPolicyContorl;
using StockTradeAPI;
using StockTrade;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EntropyRiseStockTP0
{
    public partial class frm_RunPolicy : C1RibbonForm
    {
        List<Thread> SimulatThread;
        Dictionary<Guid, uc_StockMonitor> stockMonitors;
        DataMonitor da;
        bool isSim = false;
        Trader _trader;
        CellStyle cs_up;
        CellStyle cs_down;
        CellStyle cs_even;
        CellStyle cs_change;
        //RunningPolicy rptest;
        private bool moving = false;
        private Point oldMousePosition;
        List<StockPolicies.RunningPolicy> _policies;
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
        private frm_RunPolicy(bool issim)
        {
            stockMonitors = new Dictionary<Guid, uc_StockMonitor>();
            InitializeComponent();
            //panel_stockmonitor.HorizontalScroll.Maximum = 0;
            //panel_stockmonitor.AutoScroll = false;
            //panel_stockmonitor.VerticalScroll.Visible = false;
            //panel_stockmonitor.AutoScroll = true;
            da = new DataMonitor();
            SimulatThread = new List<Thread>();
            this.isSim = issim;
            cs_up = grid_realpolicy.Styles.Add("up");
            cs_up.ForeColor = Color.Red;
            
            cs_down = grid_realpolicy.Styles.Add("down");
            cs_down.ForeColor = Color.Green;
            cs_even = grid_realpolicy.Styles.Add("even");
            cs_even.ForeColor = Color.Black;
            cs_change = grid_realpolicy.Styles.Add("change");
            cs_change.ForeColor = Color.White;
            cs_change.BackColor = Color.Blue;
        }

        public frm_RunPolicy(Trader trader, bool issim):this(issim)
        {
            this._trader = trader;
            
            if (issim)
            {
                this.Text = string.Format("模拟策略监控-{0}",_trader.AccountID);
            }
            else
            {
                this.Text = string.Format("实盘策略监控-{0}", _trader.AccountID);
            }
            
            _trader.Policy_Notify += sa_Policy_Notify;
        }

        void sa_Policy_Notify(object sender, PolicyNotifyEventArgs args)
        {
            ((RunningPolicy)args.Policy).Notify(args.TradeGuid, args.OpenStatus);
        }


        private void frm_Main_Load(object sender, EventArgs e)
        {
            this.grid_realpolicy.MouseClick+=grid_realpolicy_MouseClick;
        }
        internal void AddStock(List<StockPolicies.RunningPolicy> policies)
        {
            this._policies = policies;
            for (int i = 0; i < policies.Count; i++)
            {
                RunningPolicy rp = policies[i];
                grid_realpolicy.Rows.Add();
                int row = grid_realpolicy.Rows.Count -1;

                grid_realpolicy.SetCellImage(row, 0, global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Ext_Net_icons_chart_line);
                grid_realpolicy.SetCellImage(row, 8, global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Ext_Net_icons_cancel);
                grid_realpolicy.SetCellImage(row, 9, global::EntropyRiseStockTP0.Properties.Resources.Pause_Normal_Red_16px_560489_easyicon_net);
                grid_realpolicy.Rows[row][1] = rp.SecInfo.Code;
                grid_realpolicy.Rows[row][2] = rp.SecInfo.Name;
                grid_realpolicy.Rows[row][3] = "-";
                grid_realpolicy.Rows[row][4] = "-";
                grid_realpolicy.Rows[row][5] = "-";
                grid_realpolicy.Rows[row][6] = "-";
                grid_realpolicy.Rows[row][7] = rp.PolicyName;
                grid_realpolicy.Rows[row].UserData = rp;
                rp.PolicyMessage_Arrival += rp_PolicyMessage_Arrival;
                rp.PolicyResult_Arrival += rp_PolicyResult_Arrival;
                rp.PolicyFunCGet_Arrival += rp_PolicyFunCGet_Arrival;
                rp_PolicyKeCheArgs_Arrival(rp.kca);
                rp.LiveData_Arrival += rp_LiveData_Arrival;
                rp.PolicyParam_Arrival += rp_PolicyParam_Arrival;
                rp.InitArgs();
                Simulator(rp.SecInfo, System.DateTime.Now.Date, System.DateTime.Now.Date, rp.PolicyDataReceiver, 0);
            }
        }

        void rp_LiveData_Arrival(object sender, LiveDataEventArgs args)
        {
            try
            {
                if (args.IsReal)
                {
                    RunningPolicy rp = (RunningPolicy)sender;
                    for (int i = 1; i < grid_realpolicy.Rows.Count; i++)
                    {
                        if (((RunningPolicy)grid_realpolicy.Rows[i].UserData).PolicyGuid == rp.PolicyGuid)
                        {
                            double lastprice;
                            double lastbid = 0;
                            double lastask = 0;
                            try
                            {
                                lastprice = System.Convert.ToDouble(grid_realpolicy.Rows[i][3]);
                            }
                            catch
                            {
                                lastprice = 0;
                            }
                            try
                            {
                                lastbid = System.Convert.ToDouble(grid_realpolicy.Rows[i][5]);
                            }
                            catch { }
                            try
                            {
                                lastask = System.Convert.ToDouble(grid_realpolicy.Rows[i][6]);
                            }
                            catch { }
                            if (args.CurrentPrice != lastprice || args.Bid != lastbid || args.Ask != lastask)
                            {
                                CellStyle ncs;
                                if (args.PreClose < args.CurrentPrice)
                                {
                                    grid_realpolicy.SetCellStyle(i, 3, cs_change);
                                    ncs = cs_up;
                                    grid_realpolicy.SetCellStyle(i, 4, cs_up);
                                }
                                else if (args.PreClose > args.CurrentPrice)
                                {
                                    grid_realpolicy.SetCellStyle(i, 3, cs_change);
                                    ncs = cs_down;
                                    grid_realpolicy.SetCellStyle(i, 4, cs_down);
                                }
                                else
                                {
                                    grid_realpolicy.SetCellStyle(i, 3, cs_change);
                                    ncs = cs_even;
                                    grid_realpolicy.SetCellStyle(i, 4, cs_even);
                                }
                                if (args.Bid > args.PreClose)
                                {
                                    grid_realpolicy.SetCellStyle(i, 5, cs_up);
                                }
                                else if (args.Bid < args.PreClose)
                                {
                                    grid_realpolicy.SetCellStyle(i, 5, cs_down);
                                }
                                else
                                {
                                    grid_realpolicy.SetCellStyle(i, 5, cs_even);
                                }
                                if (args.Ask > args.PreClose)
                                {
                                    grid_realpolicy.SetCellStyle(i, 6, cs_up);
                                }
                                else if (args.Ask < args.PreClose)
                                {
                                    grid_realpolicy.SetCellStyle(i, 6, cs_down);
                                }
                                else
                                {
                                    grid_realpolicy.SetCellStyle(i, 6, cs_even);
                                }
                                //grid_realpolicy.SetCellStyle(i,3,new C1.Win.C1FlexGrid.CellStyle()
                                grid_realpolicy.Rows[i][3] = Math.Round(args.CurrentPrice, 8, MidpointRounding.AwayFromZero);
                                grid_realpolicy.Rows[i][5] = Math.Round(args.Bid, 8, MidpointRounding.AwayFromZero);
                                grid_realpolicy.Rows[i][6] = Math.Round(args.Ask, 8, MidpointRounding.AwayFromZero);
                                grid_realpolicy.Rows[i][4] = string.Format("{0}%", Math.Round((args.CurrentPrice - args.PreClose) / args.PreClose * 100, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                ParameterizedThreadStart pts = new ParameterizedThreadStart(reset_grid);
                                Thread th = new Thread(pts);
                                th.Start(new ResetObject(i, ncs));
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        delegate void resetgridDelegate(object o);
        void reset_grid(object o)
        {
            Thread.Sleep(1000);
            SwitchStyle(o);
        }

        private void SwitchStyle(object o)
        {
            ResetObject ro = (ResetObject)o;
            if (this.InvokeRequired)
            {
                this.Invoke(new resetgridDelegate(SwitchStyle), new object[] { ro });
            }
            else
            {
                try
                {
                    grid_realpolicy.SetCellStyle(ro.line, 3, ro.cellstyle);
                }
                catch { }
            }
        }



        //void rp_PolicyResult_Arrival(object sender, PolicyResultEventArgs args)
        //{
        //    //throw new NotImplementedException();
        //}

        void rp_PolicyMessage_Arrival(object sender, PolicyMessageEventArgs args)
        {
            TradeLog.Log(args.Message);
        }

        //void stockMonitor_StockMonitor_MessageArrival(RunningPolicy policy,PolicyMessageEventArgs args)
        //{
        //    TradeLog.Log(args.Message);
        //}

        //void stockMonitor_Policy_Remove(uc_StockMonitor ucs)
        //{
        //    //ucs.StockMonitor_ResultArrival -= stockMonitor_StockMonitor_ResultArrival;
        //    //stockMonitors.Remove(ucs.Policy.PolicyGuid);
        //    //this.panel_stockmonitor.Controls.Remove(ucs);
        //    //ReDisplayControls();
        //    //stockCount = stockMonitors.Count;
        //}

        //private void ReDisplayControls()
        //{
        //    int index = 0;
        //    foreach (var x in stockMonitors)
        //    {
        //        x.Value.Top = index * 30 - this.panel_stockmonitor.VerticalScroll.Value;
        //        x.Value.Width = panel_stockmonitor.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * System.Convert.ToInt32(panel_stockmonitor.VerticalScroll.Visible);
        //        index++;
        //    }

        //}

        private void Simulator(SecurityInfo si, DateTime StartDate, DateTime EndDate, DataReceiver dataReceiver, int inteval)
        {
            StockSimulator ss = new StockSimulator(si, StartDate, EndDate, da, dataReceiver, inteval);
            Thread th = new Thread(new ThreadStart(ss.Start));
            SimulatThread.Add(th);
            th.Start();
        }
        void rp_PolicyKeCheArgs_Arrival(KeCheArgs kca)
        {
            _trader.AddKeCheArgs(kca);
        }

        public delegate void FrmFunPolicyResultArrival(object sender, PolicyFunCGetEventArgs args);
        public event FrmFunPolicyResultArrival frmFunPolicyResult_Arrival;
        public void frmFunRaiseResult(object sender, PolicyFunCGetEventArgs args)
        {
            if (frmFunPolicyResult_Arrival != null)
            {
                frmFunPolicyResult_Arrival(sender, args);
            }
        }

        void rp_PolicyFunCGet_Arrival(object sender, PolicyFunCGetEventArgs args)
        {
            frmFunRaiseResult(sender, args);
        }

        public delegate void FrmPolicyResultArrival(object sender, PolicyResultEventArgs args);
        public event FrmPolicyResultArrival frmPolicyResult_Arrival;
        public void frmRaiseResult(object sender, PolicyResultEventArgs args)
        {
            if (frmPolicyResult_Arrival != null)
            {
                frmPolicyResult_Arrival(sender, args);
            }
        }

        void rp_PolicyResult_Arrival(object sender, PolicyResultEventArgs args)
        //void stockMonitor_StockMonitor_ResultArrival(RunningPolicy policy,StockPolicies.PolicyResultEventArgs args)
        {
            args.IsSim = isSim;
            frmRaiseResult(sender,args);
        }

        void rp_PolicyParam_Arrival(object sender, PolicyParamEventArgs args)
        //void stockMonitor_StockMonitor_ResultArrival(RunningPolicy policy,StockPolicies.PolicyResultEventArgs args)
        {
            RunningPolicy policy = (RunningPolicy)sender;
            _trader.policyParamChange(policy, args.ParamName, args.ParamValue);
        }

        private void Write(PolicyResultEventArgs args)
        {
            try
            {
                StreamWriter sw = new StreamWriter(string.Format("{0}\\result.txt", Application.StartupPath), true, Encoding.UTF8);
                if (!args.PairePoint.Closed)
                {
                    sw.WriteLine(string.Format("{0},=\"{1}\",{2},{3},入场,{4},{5},{6}", args.PolicyName1, args.SecInfo.Code, args.SecInfo.Name, args.SecInfo.Market, Enum.GetName(typeof(OpenType), args.PairePoint.EnterPoint.OpenType), args.PairePoint.EnterPoint.OpenPrice, args.PairePoint.EnterPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    sw.WriteLine(string.Format("{0},=\"{1}\",{2},{3},入场,{4},{5},{6},出场,{7},{8},{9}", args.PolicyName1, args.SecInfo.Code, args.SecInfo.Name, args.SecInfo.Market, Enum.GetName(typeof(OpenType), args.PairePoint.EnterPoint.OpenType), args.PairePoint.EnterPoint.OpenPrice, args.PairePoint.EnterPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss"), Enum.GetName(typeof(OpenType), args.PairePoint.OutPoint.OpenType), args.PairePoint.OutPoint.OpenPrice, args.PairePoint.OutPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss")));

                }
                sw.Close();
            }
            catch { }
        }

        void frm_onAddPolicies(object sender, SelectMonitorEventArgs args)
        {
            AddStock(args.Policies);
        }

        private void tsm_exit_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this._policies.Count; i++)
            {
                try
                {
                    _policies[i].Stop();
                }
                catch { }
            }
            this.Close();
        }

        //private void panel_stockmonitor_Resize(object sender, EventArgs e)
        //{
        //    foreach (var x in stockMonitors)
        //    {
        //        x.Value.Width = panel_stockmonitor.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * System.Convert.ToInt32(panel_stockmonitor.VerticalScroll.Visible);
        //    }
        //    base.OnResize(e);
        //}

        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (Thread th in SimulatThread)
                {
                    th.Abort();
                }
            }
            catch { }
            
        }


        private void frm_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
        }


        private void frm_RunPolicy_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach(uc_StockMonitor us in stockMonitors.Values)
            {
                us.Policy.Stop();
            }
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //SecurityInfo si = new SecurityInfo("603189","网达软件",1);
            //OpenPoint op = new OpenPoint();
            //op.CancelLimitTime = 20;
            //op.OpenPrice = 28.00;
            //op.OpenPriceType = TradeSendOrderPriceType.XianJiaWeiTuo;
            //op.OpenQty = 100;
            //op.OpenTime = System.DateTime.Now;
            //op.OpenType = OpenType.Buy;
            //op.ReEnterPecentage = 0.005;
            //op.Remark = string.Empty;
            //op.SecInfo = si;
            
            //TradePoints tps = new TradePoints(op,27.80);
            ////tps.Closed = false;
            //tps.TradeGuid = Guid.NewGuid();
            //PolicyResultEventArgs prea = new PolicyResultEventArgs();
            //prea.IsReal = true;
            //prea.PolicyName1 = "测试策略";
            //prea.PolicyObject = prea;
            //prea.SecInfo = si;
            //prea.Tickdata = new TickData();
            //prea.PairePoint = tps;
            //stockMonitor_StockMonitor_ResultArrival(rptest, prea);
        }

        private void 平仓ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void c1Command2_Click(object sender, C1.Win.C1Command.ClickEventArgs e)
        {
            this.Close();
        }

        private void grid_realpolicy_MouseClick(object sender, MouseEventArgs e)
        {
            
            int col = grid_realpolicy.Col;
            if(col == 0)
            {
                ((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).showMonitor(Screen.FromControl(this));
                this.grid_realpolicy.Select(grid_realpolicy.Row, 1);
            }
            else if(col == 8)
            {
                if (MessageBox.Show("是否要删除该策略", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).Stop();
                    this.grid_realpolicy.Rows.Remove(grid_realpolicy.Row);
                }
                this.grid_realpolicy.Select(grid_realpolicy.Row, 1);
            }
            else if (col == 9)
            {
                if (((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).start)
                {
                    ((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).Stop();
                    grid_realpolicy.SetCellImage(grid_realpolicy.Row, 9, global::EntropyRiseStockTP0.Properties.Resources.Start_16px_1186321_easyicon_net);
                }
                else
                {
                    ((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).Start();
                    grid_realpolicy.SetCellImage(grid_realpolicy.Row, 9, global::EntropyRiseStockTP0.Properties.Resources.Pause_Normal_Red_16px_560489_easyicon_net);
                }
                this.grid_realpolicy.Select(grid_realpolicy.Row, 1);
            }
            else if(col == 7)
            {
#if !READONLY
                Screen screen = Screen.FromControl(this);
                ((RunningPolicy)grid_realpolicy.Rows[grid_realpolicy.Row].UserData).showParameter(this.ParentForm, screen);
                this.grid_realpolicy.Select(grid_realpolicy.Row, 1);
#endif
            }

        }

        private void command_AddPolicy_Click(object sender, C1.Win.C1Command.ClickEventArgs e)
        {
            frm_SelectMonitor frm = new frm_SelectMonitor(isSim, _trader.TA.Bta);
            frm.onAddPolicies += frm_onAddPolicies;
            frm.ShowDialog();
        }

        private void command_quit_Click(object sender, C1.Win.C1Command.ClickEventArgs e)
        {
            try
            {
                for (int i = 0; i < this._policies.Count; i++)
                {
                    try
                    {
                        _policies[i].Stop();
                    }
                    catch { }
                }
            }
            catch { }
            this.Close();
        }

        private void lb_title_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void lb_title_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                return;
            }
            //Titlepanel.Cursor = Cursors.NoMove2D;
            oldMousePosition = e.Location;
            moving = true;
        }

        private void lb_title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && moving)
            {
                Point newPosition = new Point(e.Location.X - oldMousePosition.X, e.Location.Y - oldMousePosition.Y);
                this.Location += new Size(newPosition);
            }
        }

        private void pb_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

       
    }

    public class ResetObject
    {
        public int line;
        public CellStyle cellstyle;
        public ResetObject(int line,CellStyle cs)
        {
            this.line = line;
            this.cellstyle = cs;
        }
    }
}
