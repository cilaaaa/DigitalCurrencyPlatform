using DataBase;
using DataHub;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolicyBtcFuture1225
{
    public class BackTest : RunningBackTest
    {
        SecurityInfo securityInfo;
        BackTestParameter btParameter;
        List<Policy> policys;
        Thread runningThread;
        //int totalCount;
        //DataMonitor da ;
        //DataTable tickdatas;
        bool isLianDong;
        List<DataTable> historyDatas = new List<DataTable>();
        List<int> LiveBarIn = new List<int>() { 120};
        List<int> LiveBarOut = new List<int>() { 30};
        public BackTest(SecurityInfo si, BackTestParameter btp,bool isLianDong)
        {
            this.isLianDong = isLianDong;
            this.btParameter = btp;
            this.securityInfo = si;
            //this.totalCount = 0;
            //da = new DataMonitor();
            try
            {
                IniFile ini = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "backtest.ini"));
                string section = "PolicyBtcFuture0315";
                ini.WriteString(section, "startdate", btp.StartDate.ToString("yyyy-MM-dd"));
                ini.WriteString(section, "enddate", btp.EndDate.ToString("yyyy-MM-dd"));
                ini.WriteString(section, "barIntevalFrom", btp.BarIntevalFrom.ToString());
                ini.WriteString(section, "barIntevalTo", btp.BarIntevalTo.ToString());
                ini.WriteString(section, "barIntevalStep", btp.BarIntevalStep.ToString());
                ini.WriteString(section, "barCountInFrom", btp.BarCountInFrom.ToString());
                ini.WriteString(section, "barCountInTo", btp.BarCountInTo.ToString());
                ini.WriteString(section, "barCountInStep", btp.BarCountInStep.ToString());
                ini.WriteString(section, "barCountOutFrom", btp.BarCountOutFrom.ToString());
                ini.WriteString(section, "barCountOutTo", btp.BarCountOutTo.ToString());
                ini.WriteString(section, "barCountOutStep", btp.BarCountOutStep.ToString());
                ini.WriteString(section, "ratioFrom", btp.RatioFrom.ToString());
                ini.WriteString(section, "ratioTo", btp.RatioTo.ToString());
                ini.WriteString(section, "ratioStep", btp.RatioStep.ToString());
                ini.WriteString(section, "qty", btp.Qty.ToString());
                ini.WriteString(section, "fee", btp.Fee.ToString());
            }
            catch { }
            policys = new List<Policy>();
            RunPolicies(si, true);
        }

        private void RunPolicies(SecurityInfo si,bool getCount)
        {
            
            for (int BarInteval = btParameter.BarIntevalFrom; BarInteval < btParameter.BarIntevalTo; BarInteval += btParameter.BarIntevalStep)
            {
                for (int BarCountIn = btParameter.BarCountInFrom; BarCountIn < btParameter.BarCountInTo; BarCountIn += btParameter.BarIntevalStep)
                {
                    for (int BarCountOut = btParameter.BarCountOutFrom; BarCountOut < btParameter.BarCountOutTo; BarCountOut+= btParameter.BarCountOutStep)
                    {
                        for (decimal Ratio = (decimal)btParameter.RatioFrom; Ratio < (decimal)btParameter.RatioTo; Ratio += (decimal)btParameter.RatioStep)
                        {
                            Parameter parameter = new Parameter();
                            parameter.StartTime = new TimeSpan(0, 0, 0);
                            parameter.EndTime = new TimeSpan(23, 59, 59);
                            parameter.BarInteval = 60;
                            parameter.Fee = btParameter.Fee;
                            parameter.Debug = true;
                            parameter.BarInteval = BarInteval;

                            parameter.BarCountIn = BarCountIn;
                            //parameter.BarCountOut = BarCountOut;
                            parameter.Ratio = (double)Ratio;

                            //parameter.BarInteval = BarInteval;
                            parameter.IsReal = false;
                            parameter.StartDate = btParameter.StartDate;
                            parameter.EndDate = btParameter.EndDate;
                            parameter.qty = btParameter.Qty;
                            PolicyProperties pp = new PolicyProperties();

                            pp.IsLianDong = false;
                            pp.IsSim = true;
                            Policy p = new Policy(si, parameter, pp);
                            policys.Add(p);
                        }
                    }
                }
            }
        }

        public override List<System.Windows.Forms.DataGridViewColumn> GridColumns()
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            DataGridViewColumn col = new DataGridViewColumn();
            //col.HeaderText = "序号";
            //col.Width = 50;
            //col.CellTemplate = new DataGridViewTextBoxCell();
            //col.SortMode = DataGridViewColumnSortMode.Automatic;
            //columns.Add(col);
            //col = new DataGridViewColumn();
            col.HeaderText = "开始金额";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);
            col = new DataGridViewColumn();
            col.HeaderText = "结束收益";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);
            col = new DataGridViewColumn();
            col.HeaderText = "入场BAR数目";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "出场BAR数目";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "时间周期";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "斜率";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "交易数量";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "交易次数";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);
            return columns;
        }

        public override int getPolicyCount()
        {

            return policys.Count;
        }

        public override void Start()
        {
            runningThread = new Thread(new ThreadStart(StartBackTest));
            runningThread.Start();
        }

        private void StartBackTest()
        {
            DateTime today = btParameter.StartDate.Date;
            MarketTimeRange marketRange = MarketTimeRange.getTimeRange(securityInfo.Market);
            int totalcount = this.policys.Count;
            
            DataMonitor da = new DataMonitor();
            Parallel.For(0, totalcount, (j, LoopState) =>
            {
                RunningPolicy policy = policys[j];
                Guid g = policy.PolicyGuid;
                string backtesttitle = policy.getBackTestTitle();
                PolicyStarted(new BackTestStartEventArgs(backtesttitle, g));
                policy.PolicyResult_Arrival += policy_PolicyResult_Arrival;
            });
            while (today <= btParameter.EndDate.Date)
            {
                string name = ConfigFileName.HistoryDataFileName + "\\" + today.ToString("yyyyMMdd") + ".csv";
                PolicyLoading(name);
                DataTable tickdatas = CSVFileHelper.OpenCSV(name);
                List<TickData> list_tickdata = new List<TickData>();
                
                for (int i = 0; i < tickdatas.Rows.Count; i++)
                {
                    DataRow dr = tickdatas.Rows[i];
                    if (dr["symbol"].ToString() != "XBTUSD")
                    {
                        continue;
                    }
                    //TickData tickdata = TickData.ConvertFromDataRow(dr);

                    TickData tickdata = new TickData();
                    tickdata.Code = dr["symbol"].ToString();
                    tickdata.SecInfo = this.securityInfo;
                    tickdata.Time = System.Convert.ToDateTime(dr["timestamp"].ToString().Replace("D", " ").Substring(0, 23));
                    tickdata.Preclose = 0;
                    tickdata.Open = 0;
                    tickdata.High = 0;
                    tickdata.Low = 0;
                    tickdata.Ask = System.Convert.ToDouble(dr["askPrice"]);
                    tickdata.Bid = System.Convert.ToDouble(dr["bidPrice"]);
                    tickdata.Last = Math.Round((tickdata.Ask + tickdata.Bid) / 2, 2, MidpointRounding.AwayFromZero);
                    tickdata.Volume = 0;
                    tickdata.Amt = 0;
                    tickdata.IsReal = false;
                    tickdata.Asks[0] = tickdata.Ask;
                    tickdata.Bids[0] = tickdata.Bid;
                    list_tickdata.Add(tickdata);
                }
                Parallel.For(0, totalcount, (j, LoopState) =>
                {
                    RunningPolicy policy = policys[j];
                    //policy.PolicyMessage_Arrival += policy_PolicyMessage_Arrival;
                    //da.Simulator(tickdatas, policy.PolicyDataReceiver, policy.Inteval);
                    da.BackSimulator(list_tickdata, policy.PolicyDataReceiver, 0);
                });

                list_tickdata.Clear();
                today = today.AddDays(1);
            }
            Parallel.For(0, totalcount, (j, LoopState) =>
            {
                Stopwatch sw = new Stopwatch();
                RunningPolicy policy = policys[j];
                Guid g = policy.PolicyGuid;
                PolicyFinished(g, sw.Elapsed);
                policy.PolicyResult_Arrival -= policy_PolicyResult_Arrival;
                sw.Stop();
            });
            
            
            RaiseFinished(new EventArgs());
        }

        void policy_PolicyResult_Arrival(object sender, PolicyResultEventArgs args)
        {
            if (args.PairePoint.Closed)
            {
                RaiseResult(sender,args);
            }
        }

        public override void Stop()
        {
            runningThread.Abort();
            for (int i = 0; i < policys.Count; i++)
            {
                policys[i].Stop();
                policys[i] = null;
            }
        }

        public override TradeParameter getTradeParameter()
        {
            return new TradeParameter(btParameter.Fee, btParameter.Qty, btParameter.StartValue,btParameter.StartDate,btParameter.EndDate);
        }
    }
}
