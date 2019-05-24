using C1.Win.C1Ribbon;
using DataHub;
using StockData;
using StockPolicies;
using StockPolicyContorl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PolicyClient
{
    public partial class frm_Monitor : C1RibbonForm
    {
        List<Thread> SimulatThread;
        //定义一个变量等于0
        int stockCount = 0;
        Dictionary<Guid, uc_StockMonitor> stockMonitors;
        //创建一个布尔值

        //定义一个线程
        DataMonitor da;
        CheckBox cb_RaiseMessage;
        public frm_Monitor()
        {
            stockMonitors = new Dictionary<Guid, uc_StockMonitor>();
            InitializeComponent();
            panel_stockmonitor.HorizontalScroll.Maximum = 0;
            panel_stockmonitor.AutoScroll = false;
            panel_stockmonitor.VerticalScroll.Visible = false;
            //注意启用滚动的顺序，应是完成设置的最后一条语句
            panel_stockmonitor.AutoScroll = true;
            da = new DataMonitor();
            SimulatThread = new List<Thread>();
            GUITools.DoubleBuffer(this.grid_result, true);
            InitializeGridX();
            cb_RaiseMessage = new CheckBox();
            cb_RaiseMessage.Text = string.Empty;
            cb_RaiseMessage.FlatStyle = FlatStyle.Popup;
            cb_RaiseMessage.Width = 100;
            //cb_RaiseMessage.AutoSize = true;
            cb_RaiseMessage.BackColor = Color.Transparent;
            ToolStripControlHost host = new ToolStripControlHost(cb_RaiseMessage);
            this.ts_main.Items.Insert(this.ts_main.Items.Count -1,host);

        }

        private void InitializeGridX()
        {
            this.grid_resultX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[2] { "策略", 150 });
            titles.Add(new object[2] { "代码", 80 });
            titles.Add(new object[2] { "类型", 40 });
            titles.Add(new object[2] { "盈利%", 60 });
            titles.Add(new object[2] { "盈利金额", 120 });
            titles.Add(new object[2] { "入场价格", 80 });
            titles.Add(new object[2] { "入场时间", 120 });
            titles.Add(new object[2] { "入场方向", 80 });
            titles.Add(new object[2] { "入场数量", 80 });
            titles.Add(new object[2] { "出场价格", 80 });
            titles.Add(new object[2] { "出场时间", 120 });
            titles.Add(new object[2] { "出场方向", 80 });
            titles.Add(new object[2] { "入场描述", 300 });
            titles.Add(new object[2] { "出场描述", 300 });
            this.grid_resultX.Cols.Count = titles.Count;
            this.grid_resultX.Cols.Fixed = 0;
            this.grid_resultX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_resultX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_resultX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.None;
            this.grid_resultX.AllowEditing = false;
            for(int i=0;i<titles.Count;i++)
            {
                this.grid_resultX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_resultX.Cols[i].Width = (int)titles[i][1];
            }
        }
        private void panel_stockmonitor_Resize(object sender, EventArgs e)
        {
            //base.OnResize(e);
            foreach (var x in stockMonitors)
            {
                x.Value.Width = panel_stockmonitor.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * System.Convert.ToInt32(panel_stockmonitor.VerticalScroll.Visible);
            }
            //this.panel_stockmonitor.HorizontalScroll.Visible = false;
            base.OnResize(e);
        }
        private void tsb_add_Click(object sender, EventArgs e)
        {
            frm_SelectMonitor frm = new frm_SelectMonitor();
            frm.MainForm = this;
            frm.ShowDialog(this);
        }

        internal void AddStock(List<StockPolicies.RunningPolicy> policies)
        {
            for (int i = 0; i < policies.Count; i++)
            {
                //if(stockMonitors.ContainsKey(policies[i].StockCode))
                //{
                //    //结束本次循环，进行下次循环
                //   continue;
                //}
                //使用uc_StockMonitor控件接收数据
                RunningPolicy rp = policies[i];
                rp.CanStart = true;
                uc_StockMonitor stockMonitor = new uc_StockMonitor();
                stockMonitor.InitialStock(rp);
                stockMonitor.Width = panel_stockmonitor.Width;
                //设置控件的高度
                stockMonitor.Height = 30;
                //设置控件上边缘与其容器的工作区上边缘之间的距离
                stockMonitor.Top = stockCount * 30 - this.panel_stockmonitor.VerticalScroll.Value;
                stockMonitor.Width = panel_stockmonitor.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * System.Convert.ToInt32(panel_stockmonitor.VerticalScroll.Visible);
                //设置控件左边缘与其容器的工作区左边缘之间的距离
                stockMonitor.Left = 0;
                //添加
                stockMonitors.Add(rp.PolicyGuid, stockMonitor);
                //数据填充到panel_stockmonitor中
                panel_stockmonitor.Controls.Add(stockMonitor);
                stockMonitor.StockMonitor_ResultArrival += stockMonitor_StockMonitor_ResultArrival;
                stockMonitor.Policy_Remove += stockMonitor_Policy_Remove;
                stockCount = stockMonitors.Count;

                if (!rp.IsReal)
                {
                    Simulator(rp.SecInfo, rp.StartDate, rp.EndDate, rp.PolicyDataReceiver, rp.Inteval);
                }
                else
                {
                }
            }
        }
        private void Simulator(SecurityInfo si, DateTime StartDate, DateTime EndDate, DataReceiver dataReceiver, int inteval)
        {
            StockSimulator ss = new StockSimulator(si, StartDate, EndDate, da, dataReceiver, inteval,true);
            Thread th = new Thread(new ThreadStart(ss.Start));
            SimulatThread.Add(th);
            th.Start();
        }
        void stockMonitor_Policy_Remove(uc_StockMonitor ucs)
        {
            ucs.StockMonitor_ResultArrival -= stockMonitor_StockMonitor_ResultArrival;
            stockMonitors.Remove(ucs.Policy.PolicyGuid);
            this.panel_stockmonitor.Controls.Remove(ucs);
            ReDisplayControls();
            stockCount = stockMonitors.Count;
        }
        private void ReDisplayControls()
        {
            int index = 0;
            foreach (var x in stockMonitors)
            {
                x.Value.Top = index * 30 - this.panel_stockmonitor.VerticalScroll.Value;
                x.Value.Width = panel_stockmonitor.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth * System.Convert.ToInt32(panel_stockmonitor.VerticalScroll.Visible);
                index++;
            }

        }
        void stockMonitor_StockMonitor_ResultArrival(RunningPolicy policy,StockPolicies.PolicyResultEventArgs args)
        {
            if (!args.PairePoint.Closed)
            {
                policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Opened, args.PairePoint.EnterPoint.OpenQty);
            }
            else
            {
                policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Close, args.PairePoint.OutPoint.OpenQty, 0, "1234");
                policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Closed, args.PairePoint.OutPoint.OpenQty, args.PairePoint.OutPoint.OpenPrice, "1234");
            }
            updateResult(args);
            Write(args);
            //updateResult(args);
            //Write(args);
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
        private void frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (uc_StockMonitor us in stockMonitors.Values)
            {
                us.Policy.Stop();
            }
            try
            {
                foreach (Thread th in SimulatThread)
                {
                    //终止线程
                    th.Abort();
                }
            }
            catch { }
            
        }
        private delegate void updateResultDelegate(PolicyResultEventArgs args);
        void updateResult(PolicyResultEventArgs args)
        {
            if (this.grid_result.InvokeRequired)
            {
                //通过调用委托，来改变args的值
                this.Invoke(new updateResultDelegate(updateResult), new object[] { args });
            }
            else
            {
                //在grid_result中显示历史数据
                if (args.PairePoint.Closed)
                {

                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    this.grid_resultX.Rows.Add();
                    int rowindex = grid_resultX.Rows.Count - 1;
                    grid_resultX.Rows[rowindex][1] = args.SecInfo.Code;
                    grid_resultX.Rows[rowindex][0] = args.PolicyName1;

                    grid_resultX.Rows[rowindex][6] = args.PairePoint.EnterPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss");
                    grid_resultX.Rows[rowindex][7] = Enum.GetName(typeof(OpenType), args.PairePoint.EnterPoint.OpenType);
                    grid_resultX.Rows[rowindex][5] = args.PairePoint.EnterPoint.OpenPrice.ToString("0.000");

                    grid_resultX.Rows[rowindex][8] = args.PairePoint.EnterPoint.OpenQty;
                    grid_resultX.Rows[rowindex][2] = "出场";
                    grid_resultX.Rows[rowindex][9] = args.PairePoint.OutPoint.OpenPrice.ToString("0.000");
                    grid_resultX.Rows[rowindex][10] = args.PairePoint.OutPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss");
                    grid_resultX.Rows[rowindex][11] = Enum.GetName(typeof(OpenType), args.PairePoint.OutPoint.OpenType);

                   // grid_resultX.Rows[rowindex][12] = args.SecInfo.Market1;
                    double startprice, endprice, baseprice;
                    if (args.PairePoint.EnterPoint.OpenType == OpenType.KaiDuo)
                    {

                        startprice = args.PairePoint.EnterPoint.OpenPrice;
                        endprice = args.PairePoint.OutPoint.OpenPrice;
                        baseprice = startprice;
                    }
                    else if (args.PairePoint.EnterPoint.OpenType == OpenType.KaiKong)
                    {
                        endprice = args.PairePoint.EnterPoint.OpenPrice;
                        startprice = args.PairePoint.OutPoint.OpenPrice;
                        baseprice = endprice;
                    }
                    else if (args.PairePoint.EnterPoint.OpenType == OpenType.Buy)
                    {
                        startprice = args.PairePoint.EnterPoint.OpenPrice;
                        endprice = args.PairePoint.OutPoint.OpenPrice;
                        baseprice = startprice;
                    }
                    else
                    {
                        endprice = args.PairePoint.EnterPoint.OpenPrice;
                        startprice = args.PairePoint.OutPoint.OpenPrice;
                        baseprice = endprice;
                    }
                    double profit = 0;
                    double profitamt = 0;
                    if (args.PairePoint.EnterPoint.OpenType == OpenType.KaiDuo || args.PairePoint.EnterPoint.OpenType == OpenType.KaiKong)
                    {
                        profitamt = Math.Round((1 / startprice - 1 / endprice) * args.PairePoint.EnterPoint.OpenQty * System.Convert.ToInt32(args.SecInfo.ContractVal) - (1 / startprice + 1 / endprice) * args.PairePoint.EnterPoint.OpenQty * args.PairePoint.Fee * System.Convert.ToInt32(args.SecInfo.ContractVal), 8, MidpointRounding.AwayFromZero);
                        profit = Math.Round(profitamt / args.PairePoint.EnterPoint.OpenQty / baseprice * 100, 8, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        profitamt = Math.Round((endprice - startprice) * args.PairePoint.EnterPoint.OpenQty * System.Convert.ToInt16(args.SecInfo.ContractVal) - 28, 8, MidpointRounding.AwayFromZero);
                        profit = Math.Round(profitamt / args.PairePoint.EnterPoint.OpenQty / baseprice * 100, 8, MidpointRounding.AwayFromZero);
                    }
                    
                    grid_resultX.Rows[rowindex][4] = profitamt;
                    grid_resultX.Rows[rowindex][3] = string.Format("{0}%", profit);// +"%";
                    grid_resultX.Rows[rowindex][12] = args.OpenRmks;
                    grid_resultX.Rows[rowindex][13] = args.CloseRmks;



                }
                else
                {
                    this.grid_resultX.Rows.Add();
                    int rowindex = grid_resultX.Rows.Count - 1;
                    grid_resultX.Rows[rowindex][1] = args.SecInfo.Code;
                    grid_resultX.Rows[rowindex][0] = args.PolicyName1;

                    grid_resultX.Rows[rowindex][6] = args.PairePoint.EnterPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss");
                    grid_resultX.Rows[rowindex][7] = Enum.GetName(typeof(OpenType), args.PairePoint.EnterPoint.OpenType);
                    grid_resultX.Rows[rowindex][5] = args.PairePoint.EnterPoint.OpenPrice.ToString("0.000");

                    grid_resultX.Rows[rowindex][8] = args.PairePoint.EnterPoint.OpenQty;
                    grid_resultX.Rows[rowindex][2] = "入场";
                    grid_resultX.Rows[rowindex][12] = args.OpenRmks;
                    //grid_resultX.Rows[rowindex][9] = args.PairePoint.OutPoint.OpenPrice.ToString("0.00");
                    //grid_resultX.Rows[rowindex][10] = args.PairePoint.OutPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss");
                    //grid_resultX.Rows[rowindex][11] = Enum.GetName(typeof(OpenType), args.PairePoint.OutPoint.OpenType);
                }
                //grid_result.Rows[0].Cells[10].Value = "View";
                //grid_result.Rows[0].Tag = args.Tickdata;
            }
        }

        private void tsb_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsb_export_Click(object sender, EventArgs e)
        {
            Keep();
        }

        public void Keep()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl工作表(*.xls)|*.xls";
            saveFileDialog.FileName = string.Format("策略结果-{0}", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "另存信息";
            saveFileDialog.ShowDialog();
            string filename = saveFileDialog.FileName;
            if (filename != string.Empty)
            {
                try
                {
                    this.grid_resultX.SaveExcel(filename, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                }
                catch
                {
                    MessageBox.Show("保存文件错误");
                }
            }
        }

        private void tsb_clear_Click(object sender, EventArgs e)
        {
            //grid_result.Rows.Clear();
            this.grid_resultX.Rows.RemoveRange(grid_resultX.Rows.Fixed, grid_resultX.Rows.Count - grid_resultX.Rows.Fixed);
        }

        private void grid_result_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                if(e.ColumnIndex == 10)
                {
                    frm_PolicyParameter frm = new frm_PolicyParameter(this.grid_result.Rows[e.RowIndex].Tag);
                    frm.ShowDialog(this);
                    
                }
            }
        }

    }
}
