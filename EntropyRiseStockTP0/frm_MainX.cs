using C1.Win.C1Ribbon;
using DataAPI;
using DataBase;
using DataHub;
using StockData;
using StockTradeAPI;
using StockTrade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Collections.Concurrent;
using GeneralForm;
using System.Reflection;
using StockPolicies;

namespace EntropyRiseStockTP0
{
    
    public partial class frm_MainX : C1RibbonForm
    {
        List<SecurityInfo> SiArray;
        Dictionary<string, string> LastStockTime;
        DataMonitor da;
        bool isConnecting = false;
        ConcurrentQueue<TickData> _dataQueue;
        Dictionary<string,StockTradeAPI.BaseTradeAPI> RunningST;
        bool SaveData = false;
        bool showTradeView = false;
        bool StartWarn = false;
        frm_ReceiveLog logform;
        Dictionary<string, Trader> traders;
        Thread _writeHistoryThread;
        frm_TradeLog tradelog;
        int ReceiveTickCount = 0;
        int warningTimes = 0;
        public frm_MainX()
        {
            InitializeComponent();
            SiArray = new List<SecurityInfo>();
            da = new DataMonitor();
            this.rbt_stop.Enabled = false;
            GlobalValue.Initialize();
            PolicyProgram.Load();
            TradeSDK.Load();
            CheckDirectory();
            _dataQueue = new ConcurrentQueue<TickData>();
            RunningST = new Dictionary<string, BaseTradeAPI>();
            traders = new Dictionary<string, Trader>();
        }
        #region 日志

        private void CheckDirectory()
        {
            if (!Directory.Exists(ConfigFileName.TickDataDirectory))
            {
                Directory.CreateDirectory(ConfigFileName.TickDataDirectory);
            }
            if (!Directory.Exists(ConfigFileName.TradeLogDriectory))
            {
                Directory.CreateDirectory(ConfigFileName.TradeLogDriectory);
            }
        //    if (!Directory.Exists(ConfigFileName.SysLogDirectory))
        //    {
        //        Directory.CreateDirectory(ConfigFileName.SysLogDirectory);
            //}
        }

        //delegate void LogThreadDelegate();
        //private void LogThread()
        //{
        //    string message;
        //    while (true)
        //    {
        //        string date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        try
        //        {
        //            while (_receiveMessage.TryDequeue(out message))
        //            {
        //                MessageLog.SystemLog(message);
        //                StreamWriter sw = new StreamWriter(ConfigFileName.ReceiveLogFileName, true, Encoding.UTF8);
        //                sw.WriteLine(string.Format("{0}>{1}", date, message));
        //                sw.Close();
        //            }
        //        }
        //        catch
        //        {

        //        }
        //        try
        //        {

        //            while (_tradeMessage.TryDequeue(out message))
        //            {
        //                MessageLog.TradeLog(message);
        //                StreamWriter sw = new StreamWriter(ConfigFileName.TradeLogFileName, true, Encoding.UTF8);
        //                sw.WriteLine(string.Format("{0}>{1}", date, message));
        //                sw.Close();
        //            }
        //        }
        //        catch
        //        {

        //        }

        //        try
        //        {

        //            while (_errorMessage.TryDequeue(out message))
        //            {
        //                MessageLog.ErrorLog(message);
        //                StreamWriter sw = new StreamWriter(ConfigFileName.ErrorLogFileName, true, Encoding.UTF8);
        //                sw.WriteLine(string.Format("{0}>{1}", date, message));
        //                sw.Close();
        //            }
        //        }
        //        catch
        //        {

        //        }

        //        try
        //        {
        //            while (_policyMessage.TryDequeue(out message))
        //            {
        //                MessageLog.PolicyLog(message);
        //                StreamWriter sw = new StreamWriter(ConfigFileName.PolicyLogFileName, true, Encoding.UTF8);
        //                sw.WriteLine(string.Format("{0}>{1}", date, message));
        //                sw.Close();
        //            }
        //        }
        //        catch { }

        //        Thread.Sleep(500);
        //    }
        //}
        #endregion

        #region 默认方法
        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ServerLog.LogServer(e.Exception.Message);
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

        private void rbt_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_MainX_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
        #endregion


        private void frm_MainX_Shown(object sender, EventArgs e)
        {
            this.Height = c1Ribbon1.Height;
            logform = new frm_ReceiveLog();
            ServerLog.frm = logform;
            logform.Show();

            IniFile inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
            if (inifile.GetString("System", "startWarn", "False") == "True")
            {
                StartWarn = true;
            }
            if (inifile.GetString("System", "saveData", "False") == "True")
            {
                SaveData = true;
            }
            if (inifile.GetString("System", "showTradeView", "False") == "True")
            {
                showTradeView = true;
                tradelog = new frm_TradeLog();
                TradeLog.frm = tradelog;
                tradelog.Show();
            }
            Thread UpdateReceiveDataLog = new Thread(new ThreadStart(UpdateReceiveData));
            UpdateReceiveDataLog.Start();
        }

        private void rbt_start_Click(object sender, EventArgs e)
        {
            ChangeButtonFace();
            SiArray = new List<SecurityInfo>();
            LastStockTime = new Dictionary<string, string>();
            StartConnectTDX();
            return;
        }


        delegate void ChangeButtonFaceDelegate();
        private void ChangeButtonFace()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ChangeButtonFaceDelegate(ChangeButtonFace));
            }
            else
            {
                this.rbt_stop.Enabled = true;
                this.rbt_start.Enabled = false;
            }
        }

        delegate void ResetButtonFaceDelegate();
        private void ResetButtonFace()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ResetButtonFaceDelegate(ChangeButtonFace));
            }
            else
            {
                this.rbt_stop.Enabled = false;
                this.rbt_start.Enabled = true;
            }
        }
        private void StartConnectTDX()
        {
            if (!isConnecting)
            {
                isConnecting = true;
                ConnectTdx();
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void ConnectTdx()
        {
            getMonitorList();
            StartGetData();
            StartWriteHistoryThread();
        }
        private void StartGetData()
        {
            ServerLog.ClearList();
            for (int i = 0; i < SiArray.Count; i++)
            {
                if (!RunningST.ContainsKey(SiArray[i].Market))
                {
                    string market = SiArray[i].Market;
                    string dllname = TradeSDK.getDllName(market);
                    Assembly assembly = Assembly.Load(TradeSDK.getSDK(dllname));
                    Type ClassTradeAPI = assembly.GetType(string.Format("{0}.TradeAPI", dllname.Substring(0, dllname.Length - 4)));
                    Object ObjectTradeAPI = assembly.CreateInstance(ClassTradeAPI.FullName, true, BindingFlags.CreateInstance, null, new object[] { }, null, null);
                    BaseTradeAPI bta = ((BaseTradeAPI)ObjectTradeAPI);
                    RunningST.Add(market, bta);
                    TraderAccount sa = new TraderAccount(bta);
                    bta.queryDataArrival += queryArrival;
                    if (showTradeView)
                    {
                        CurrentBalances.Insert(market);
                        Trader trader = Trader.CreateTrader(sa);
                        traders.Add(market, trader);
                        frm_TradeMonitor tradeform = new frm_TradeMonitor(trader);
                        tradeform.tradePolicyResult_Arrival += PolicyResultArraival;
                        tradeform.tradeFunPolicyResult_Arrival += PolicyFunArraival;
                        tradeform.Left = 0;
                        tradeform.Top = 3 * logform.Height;
                        tradeform.Show();
                        tradeform.ConnectTrade();
                    }
                }
                foreach(var st in RunningST)
                {
                    BaseTradeAPI bta = ((BaseTradeAPI)st.Value);
                    if (bta.AbortedWs)
                    {
                        bta.InitWebSocket();
                    }
                    if (bta.market == SiArray[i].Market)
                    {
                        if (!bta.DicSi.ContainsKey(SiArray[i].Code))
                        {
                            bta.DicSi.Add(SiArray[i].Code, SiArray[i]);
                            CurrentTicker.Insert(SiArray[i].Market + SiArray[i].Code);
                        }
                        bta.Subscribe(SiArray[i].Code, "marketDepth", SiArray[i].Type);
                        bta.Subscribe(SiArray[i].Code, "trade", SiArray[i].Type);
                        if (showTradeView)
                        {
                            bta.Subscribe(SiArray[i].Code, "order", SiArray[i].Type);
                            bta.Subscribe(SiArray[i].Code, "account", SiArray[i].Type);
                            if (SiArray[i].Type != "spot")
                            {
                                bta.Subscribe(SiArray[i].Code, "position", SiArray[i].Type);
                            }
                        }
                    }
                }
            }
        }

        //策略开仓
        void PolicyResultArraival(object sender, PolicyResultEventArgs args)
        {
            RunningPolicy policy = (RunningPolicy)sender;
            if (args.IsSim)
            {
                if (!args.PairePoint.Closed)
                {
                    policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Opened);
                }
                else
                {
                    policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Closed);
                }
            }
            else
            {
                ////////////add/////////////
                if (!args.IsReal)
                {
                    if (!args.PairePoint.Closed)
                    {
                        policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Opened);
                    }
                    else
                    {
                        policy.Notify(args.PairePoint.TradeGuid, OpenStatus.Closed);
                    }
                }
                //////////add end////////////
                //if (args.IsReal)
                else
                {
                    Trader trader = traders[args.SecInfo.Market];
                    if (!args.PairePoint.Closed) //开仓
                    {
                        trader.policyTrade(args.SecInfo, args.PolicyName1, args.PairePoint, policy, PolicyTradeType.Open);
                    }
                    else //平仓
                    {
                        trader.policyTrade(args.SecInfo, args.PolicyName1, args.PairePoint, policy, PolicyTradeType.Close);
                    }
                }
            }
        }

        //策略需要拉取信息
        void PolicyFunArraival(object sender, PolicyFunCGetEventArgs args)
        {
            RunningPolicy policy = (RunningPolicy)sender;
            BaseTradeAPI bta = RunningST[args.SecInfo.Market];
            var result = bta.GetType().GetMethod(args.FunName).Invoke(bta, args.Parameters);
            ((RunningPolicy)args.PolicyObject).FunGetResult(args.FunName,result);
        }

        void queryArrival(object sender, TickData args)
        {
            try
            {
                ProcessSaveData(args);
            }
            catch (Exception ex)
            {
                ServerLog.LogServer(string.Format("保存数据失败:{0}", ex.Message));
            }
        }

        public delegate void voidNonParameterDelegate();

        private void StartWriteHistoryThread()
        {
            if (_writeHistoryThread != null)
            {
                _writeHistoryThread.Abort();
            }
            ThreadStart ts = new ThreadStart(new voidNonParameterDelegate(StartWrite));
            _writeHistoryThread = new Thread(ts);
            _writeHistoryThread.Start();
        }

        private void StartWrite()
        {
            TickData td;
            while (true)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, List<TickData>>>> data = new Dictionary<string, Dictionary<string, Dictionary<string, List<TickData>>>>();
                while (!_dataQueue.IsEmpty)
                {
                    if (_dataQueue.TryDequeue(out td))
                    {
                        string market = td.SecInfo.Market;
                        string code = td.SecInfo.Code;
                        string date = td.Time.Date.ToString("yyyyMMdd");
                        if (!data.ContainsKey(market))
                        {
                            data[market] = new Dictionary<string, Dictionary<string, List<TickData>>>();
                        }
                        if (!data[market].ContainsKey(date))
                        {
                            data[market][date] = new Dictionary<string, List<TickData>>();
                        }
                        if (!data[market][date].ContainsKey(code))
                        {
                            data[market][date][code] = new List<TickData>();
                        }
                        data[market][date][code].Add(td);
                    }
                }
                foreach (var marketItem in data)
                {
                    string market = marketItem.Key;
                    foreach (var dateItem in data[market])
                    {
                        string date = dateItem.Key;
                        foreach (var symbolItem in data[market][date])
                        {
                            try
                            {
                                string code = symbolItem.Key;
                                string filename = string.Format("{0}%{1}%{2}.csv", market, code, date);
                                if (File.Exists(string.Format("{0}\\{1}", ConfigFileName.TickDataDirectory, filename)))
                                {
                                    StreamWriter sw = new StreamWriter(string.Format("{0}\\{1}", ConfigFileName.TickDataDirectory, filename), true, Encoding.UTF8);
                                    string writeStr = "";
                                    foreach (var tickData in data[market][date][code])
                                    {
                                        writeStr += string.Format("{0},{1},{2},{3},{4},{5},{6}\n", tickData.Time.ToString("yyyy-MM-ddDHH:mm:ss.fff"), code, tickData.Bidsizes[0], tickData.Bid, tickData.Ask, tickData.Asksizes[0], tickData.Last);
                                    }
                                    sw.Write(writeStr);
                                    sw.Close();
                                }
                                else
                                {
                                    StreamWriter sw = new StreamWriter(string.Format("{0}\\{1}", ConfigFileName.TickDataDirectory, filename), true, Encoding.UTF8);
                                    string writeStr = "timestamp,symbol,bidSize,bidPrice,askPrice,askSize,lastPrice\n";
                                    foreach (var tickData in data[market][date][code])
                                    {
                                        writeStr += string.Format("{0},{1},{2},{3},{4},{5},{6}\n", tickData.Time.ToString("yyyy-MM-ddDHH:mm:ss.fff"), code, tickData.Bidsizes[0], tickData.Bid, tickData.Ask, tickData.Asksizes[0], tickData.Last);
                                    }
                                    sw.Write(writeStr);
                                    sw.Close();
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                Thread.Sleep(20000);
            }
        }

        private void ProcessSaveData(TickData td)
        {
            CurrentStockData.Update(td.SecInfo, td);
            da.SendTick(td);
            if (LastStockTime.ContainsKey(td.SecInfo.Key))
            {
                LastStockTime[td.SecInfo.Key] = td.Time.TimeOfDay.ToString();
            }
            else
            {
                LastStockTime.Add(td.SecInfo.Key, td.Time.TimeOfDay.ToString());
            }
            if (SaveData)
            {
                _dataQueue.Enqueue(td);
            }
            ReceiveTickCount += 1;
            
        }

        private void UpdateReceiveData()
        {
            while (true)
            {
                if (ReceiveTickCount < SiArray.Count)
                {
                    warningTimes += 1;
                }
                else
                {
                    warningTimes = 0;
                }
                if (warningTimes > 30 && StartWarn)
                {
                    EmailParameterSet model = new EmailParameterSet();
                    model.SendEmail = "472185361@qq.com";
                    model.SendPwd = "libisrsmpjhqbhgh";//密码
                    model.SendSetSmtp = "smtp.qq.com";//发送的SMTP服务地址 ，每个邮箱的是不一样的。。根据发件人的邮箱来定
                    model.ConsigneeAddress = "472185361@qq.com";
                    model.ConsigneeTheme = "！！数字平台行情接收器报警！！";
                    model.ConsigneeHand = "数据行情接收异常";
                    model.ConsigneeName = "dongnan_chen";
                    model.SendContent = "数据行情接受器接受数据少于订阅数据数量长达30秒";
                    string errinfo;
                    if (model.MailSend(model, out errinfo) == true)
                    {
                        Console.WriteLine("邮件发送成功！");
                    }
                    else
                    {
                        Console.WriteLine(errinfo);
                    }
                }
                ServerLog.Log(string.Format("√{0},", ReceiveTickCount), false);
                ReceiveTickCount = 0;
                Thread.Sleep(1000);
            }
        }

        private void getMonitorList()
        {
            SiArray = GlobalValue.SecurityList;
        }

        

        private void rbt_stop_Click(object sender, EventArgs e)
        {
            foreach (var bta in RunningST)
            {
                ((BaseTradeAPI)bta.Value).DisposeWebSocket();
            }
            StopGetData();
        }

        private void StopGetData()
        {
            isConnecting = false;
            this.rbt_start.Enabled = true;
            this.rbt_stop.Enabled = false;
            _writeHistoryThread.Abort();
        }

        private void rbt_list_Click(object sender, EventArgs e)
        {
#if !READONLY
            if (frm_MonitorList.isRunning)
            {
                MessageBox.Show("该功能已经在运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                frm_MonitorList.isRunning = true;
                frm_MonitorList.isMdiStyle = false;
                frm_MonitorList frm = new frm_MonitorList();
                frm.onRemoveStock += frm_onRemoveStock;
                frm.onAddStock += frm_onAddStock;
                frm.Show();
            }
#endif
        }

        void frm_onAddStock(object sender, MonitorStockEventArgs args)
        {
            AddQueryStock(args.SI);
        }

        void frm_onRemoveStock(object sender, MonitorStockEventArgs args)
        {
            RemoveQueryStock(args.SI);
        }

        private void rbt_policy_Click(object sender, EventArgs e)
        {
#if !READONLY
            if (frm_PolicyUpdate.isRunning)
            {
                MessageBox.Show("该功能已经在运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                frm_PolicyUpdate.isRunning = true;
                frm_PolicyUpdate frm = new frm_PolicyUpdate();
                frm.Show();
            }
#endif
        }

        private void rbt_sdk_Click(object sender, EventArgs e)
        {
#if !READONLY
            if (frm_SDKUpdate.isRunning)
            {
                MessageBox.Show("该功能已经在运行", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                frm_SDKUpdate.isRunning = true;
                frm_SDKUpdate frm = new frm_SDKUpdate();
                frm.Show();
            }
#endif
        }


        internal void AddQueryStock(SecurityInfo si)
        {
            foreach (var st in RunningST)
            {
                BaseTradeAPI bta = ((BaseTradeAPI)st.Value);
                if (bta.market == si.Market)
                {
                    if (bta.isOpened)
                    {
                        if (CurrentTicker.getCurrentTickerPrice(si.Market + si.Code) == 0)
                        {
                            CurrentTicker.Insert(si.Market + si.Code);
                        }
                        if (!bta.DicSi.ContainsKey(si.Code))
                        {
                            bta.DicSi.Add(si.Code, si);
                        }
                        bta.Subscribe(si.Code, "marketDepth", si.Type);
                        bta.Subscribe(si.Code, "trade", si.Type);
                        if (showTradeView)
                        {
                            bta.Subscribe(si.Code, "order", si.Type);
                            bta.Subscribe(si.Code, "account", si.Type);
                            if (si.Type != "spot")
                            {
                                bta.Subscribe(si.Code, "position", si.Type);
                            }
                        }
                    }
                }
            }
        }


        internal void RemoveQueryStock(SecurityInfo si)
        {
            for (int i = 0; i < SiArray.Count; i++)
            {
                if (SiArray[i].Code == si.Code && SiArray[i].Market == si.Market)
                {
                    SiArray.RemoveAt(i);
                    break;
                }
            }
        }

        private void rbt_refreshmarket_Click(object sender, EventArgs e)
        {
            GlobalValue.Initialize();
        }

        private void rbt_account_Click(object sender, EventArgs e)
        {
            frm_Account frm = new frm_Account();
            frm.ShowDialog(this);
        }
    }
}
