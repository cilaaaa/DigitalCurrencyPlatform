﻿using DataBase;
using DataHub;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolicyBtcFuture0111
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
        List<int> BarIntevals = new List<int>() { 60, 180, 300, 900, 1800 };
        public BackTest(SecurityInfo si, BackTestParameter btp, bool isLianDong)
        {
            this.isLianDong = isLianDong;
            this.btParameter = btp;
            this.securityInfo = si;
            //this.totalCount = 0;
            //da = new DataMonitor();
            try
            {
                IniFile ini = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "backtest.ini"));
                string section = "PolicyBtcFuture0111";
                ini.WriteString(section, "startdate", btp.StartDate.ToString("yyyy-MM-dd"));
                ini.WriteString(section, "enddate", btp.EndDate.ToString("yyyy-MM-dd"));
                ini.WriteString(section, "timeCycleFrom", btp.TimeCycleFrom.ToString());
                ini.WriteString(section, "timeCycleTo", btp.TimeCycleTo.ToString());
                ini.WriteString(section, "timeCycleStep", btp.TimeCycleStep.ToString());
                ini.WriteString(section, "barCountFrom", btp.BarCountFrom.ToString());
                ini.WriteString(section, "barCountTo", btp.BarCountTo.ToString());
                ini.WriteString(section, "barCountStep", btp.BarCountStep.ToString());
                ini.WriteString(section, "zhiYingBeiShuFrom", btp.ZhiYingBeiShuFrom.ToString());
                ini.WriteString(section, "zhiYingBeiShuTo", btp.ZhiYingBeiShuTo.ToString());
                ini.WriteString(section, "zhiYingBeiShuStep", btp.ZhiYingBeiShuStep.ToString());
                ini.WriteString(section, "barInterval", btp.BarInterval.ToString());
                ini.WriteString(section, "qty", btp.Qty.ToString());
                ini.WriteString(section, "fee", btp.Fee.ToString());
            }
            catch { }
            policys = new List<Policy>();
            RunPolicies(si, true);
        }

        private void RunPolicies(SecurityInfo si, bool getCount)
        {
            for (int timeCycle = btParameter.TimeCycleFrom; timeCycle <= btParameter.TimeCycleTo; timeCycle += btParameter.TimeCycleStep)
            {
                for (double zhiYingBeiShu = btParameter.ZhiYingBeiShuFrom; zhiYingBeiShu <= btParameter.ZhiYingBeiShuTo; zhiYingBeiShu += btParameter.ZhiYingBeiShuStep)
                {
                    for (int barCount = btParameter.BarCountFrom; barCount <= btParameter.BarCountTo; barCount += btParameter.BarCountStep)
                    {
                        for (int i = 0; i < BarIntevals.Count; i++)
                        {
                            if (BarIntevals[i] / 60 * barCount < 240)
                            {
                                Parameter parameter = new Parameter();
                                parameter.DebugModel = true;
                                parameter.TimeCycle = timeCycle;
                                parameter.ZhiYingBeiShu = zhiYingBeiShu;
                                parameter.BarCount = barCount;
                                parameter.BarInteval = BarIntevals[i];
                                parameter.JiaCangCiShu = 3;
                                parameter.JiaCangJiaGeBeiShu = 0.5;
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
        }

        public override List<System.Windows.Forms.DataGridViewColumn> GridColumns()
        {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            DataGridViewColumn col = new DataGridViewColumn();
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
            col.HeaderText = "分钟bar";
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
            col.HeaderText = "开始区间";
            col.Width = 80;
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.SortMode = DataGridViewColumnSortMode.Automatic;
            columns.Add(col);

            col = new DataGridViewColumn();
            col.HeaderText = "止盈倍数";
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
                string name = ConfigFileName.HistoryDataFileName + "\\Okex%" + securityInfo.Code + "%" + today.ToString("yyyyMMdd") + ".csv";
                //string name = ConfigFileName.HistoryDataFileName + "\\XBTUSD-" + today.ToString("yyyyMMdd") + ".csv";
                PolicyLoading(name);
                DataTable tickdatas = CSVFileHelper.OpenCSV(name);
                List<TickData> list_tickdata = new List<TickData>();
                for (int i = 0; i < tickdatas.Rows.Count; i++)
                {
                    DataRow dr = tickdatas.Rows[i];
                    DateTime tickTime = System.Convert.ToDateTime(dr["timestamp"].ToString().Replace("D", " ").Substring(0, 23));

                    //TickData tickdata = TickData.ConvertFromDataRow(dr);

                    TickData tickdata = new TickData();
                    tickdata.Code = dr["symbol"].ToString();
                    tickdata.SecInfo = this.securityInfo;
                    tickdata.Time = tickTime;
                    tickdata.Preclose = 0;
                    tickdata.Open = 0;
                    tickdata.High = 0;
                    tickdata.Low = 0;
                    tickdata.Ask = System.Convert.ToDouble(dr["askPrice"]);
                    tickdata.Bid = System.Convert.ToDouble(dr["bidPrice"]);
                    tickdata.Last = System.Convert.ToDouble(dr["lastPrice"]);
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
                    PolicyDataFinished();
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
            if (!args.PairePoint.Closed)
            {
                ((RunningPolicy)sender).Notify(args.PairePoint.TradeGuid, OpenStatus.Opened, args.PairePoint.EnterPoint.OpenQty);
            }
            else
            {
                ((RunningPolicy)sender).Notify(args.PairePoint.TradeGuid, OpenStatus.Close, args.PairePoint.OutPoint.OpenQty, 0, "1234");
                ((RunningPolicy)sender).Notify(args.PairePoint.TradeGuid, OpenStatus.Closed, args.PairePoint.OutPoint.OpenQty, args.PairePoint.OutPoint.OpenPrice, "1234");
                RaiseResult(sender, args);
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
            return new TradeParameter(btParameter.Fee, btParameter.Qty, btParameter.StartValue, btParameter.StartDate, btParameter.EndDate);
        }
    }
}
