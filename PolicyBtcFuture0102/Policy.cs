using Newtonsoft.Json.Linq;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PolicyBtcFuture0102
{
    /*
     * 期货策略
     * 
     * 趋势策略
     * 
    */
    public class Policy : StockPolicies.RunningPolicy
    {
        object lock_tps;
        Parameter parameter;
        DateTime currentDay;
        List<TradePoints> tps;
        string stockAccount;

        bool isSim;

        TickData currentTick;

        Decimal holdHands;
        Decimal frozenOpenHands;
        Decimal frozenCloseHands;

        Decimal HandBuyOrder;
        Decimal HandSellOrder;

        List<DieBar> LiveBarsIn;
        List<DieBar> LiveBarsOut;
        List<double> list_efast;
        List<double> list_xfast;
        List<double> list_eslow;
        List<double> list_xslow;
        List<double> list_init_all;
        //long lastminute = -1;
        double last_high;
        double last_low;
        double init_high;
        double init_low;
        bool has_pull_data;
        double last_maefast;
        double last_maxfast;
        double last_maeslow;
        double last_maxslow;
        int long_position;
        int short_position;
        int open_complete;
        int close_complete;
        int step;


        bool init = false;
        double signInHighPrice;
        double signOutHighPrice;
        double signInLowPrice;
        double signOutLowPrice;
        double Ratio;
        OpenType ot;

        public static string PName
        {
            get { return "PolicyBtcFuture0102"; }
        }

        object lockObject;

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = PName + "_" + parameter.EFast.ToString() + "_" + parameter.XFast.ToString() + "_" + parameter.ESlow.ToString() + "_" + parameter.XSlow.ToString();
            this.startDate = rpp.StartDate;
            this.endDate = rpp.EndDate;
            this.inteval = rpp.Inteval;
            this.isReal = rpp.IsReal;
            this.policyguid = Guid.NewGuid();

            LiveBarsIn = new List<DieBar>();
            LiveBarsOut = new List<DieBar>();
            list_efast = new List<double>();
            list_xfast = new List<double>();
            list_eslow = new List<double>();
            list_xslow = new List<double>();
            list_init_all = new List<double>();
            last_maefast = 0;
            last_maxfast = 0;
            last_maeslow = 0;
            last_maxslow = 0;
            //lastminute = -1;
            last_high = 0;
            last_low = 9999;
            init_high = 0;
            init_low = 9999;
            has_pull_data = false;
            long_position = 0;
            short_position = 0;
            open_complete = 0;
            close_complete = 0;
            step = 0;

            //if (!parameter.Debug)
            //if (parameter.Debug)
            //{
            //    LoadHistoryBars();
            //}

            initialDataReceiver();
            InitialDataProcessor();
            currentDay = DateTime.MinValue.Date;
            openPoints = new List<OpenPoint>();
            IsSimulateFinished = false;
            //isOpened = false;
            if (rpp.save)
            {
                SaveParameter(rpp);
            }
            currentTick = new TickData();
            lockObject = new object();
            this.isLiveDataProcessor = true;
            tps = new List<TradePoints>();
            lock_tps = new object();
            
            signInHighPrice = 0;
            signOutHighPrice = 0;
            signInLowPrice = 1000000;
            signOutLowPrice = 1000000;
            Ratio = 0;            

        }

        /**
         * 计算最大值 
        */
        private int Max(params int[] a1)
        {
            int max = -111;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] > max)
                    max = a1[i];
            return max;
        }

        public void FunGetResult(object result)
        {
            int BarCount = Max(parameter.EFast, parameter.XFast, parameter.ESlow, parameter.XSlow);
            JObject res = (JObject)result;
            JArray history = (JArray)res["data"];
            //lastminute = long.Parse(history[0][0].ToString());
            init_high = double.Parse(history[0][2].ToString());
            init_low = double.Parse(history[0][3].ToString());
            for (int i = 1; i < BarCount + 1; i++)
            {
                double close = System.Convert.ToDouble(history[i][4].ToString());
                list_init_all.Add(close);
            }
            double sum = 0;
            for (int i = 0; i < parameter.EFast; i++)
            {
                list_efast.Add(list_init_all[i]);
                sum += list_init_all[i];
            }
            last_maefast = sum / parameter.EFast;//平均值

            sum = 0;
            for (int i = 0; i < parameter.XFast; i++)
            {
                list_xfast.Add(list_init_all[i]);
                sum += list_init_all[i];
            }
            last_maxfast = sum / parameter.XFast;

            sum = 0;
            for (int i = 0; i < parameter.ESlow; i++)
            {
                list_eslow.Add(list_init_all[i]);
                sum += list_init_all[i];
            }
            last_maeslow = sum / parameter.ESlow;

            sum = 0;
            for (int i = 0; i < parameter.XSlow; i++)
            {
                list_xslow.Add(list_init_all[i]);
                sum += list_init_all[i];
            }
            last_maxslow = sum / parameter.XSlow;
        }

        public override void InitArgs()
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long MarkTimeStamp = (long)cha.TotalSeconds;

            //int InitSecond = 0;
            //int resolution = 60;
            
            string ISO8601time = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            object[] parameters = new object[] { SecInfo, parameter.BarInteval.ToString(), "", ISO8601time };
            PolicyFunCGetEventArgs args = new PolicyFunCGetEventArgs();
            args.Parameters = parameters;
            args.PolicyObject = this;
            args.SecInfo = SecInfo;
            args.FunName = "GetKLine";
            RaiseFunCGet(args);
        }

        private void SaveParameter(Parameter rpp)
        {
            rpp.Save();
        }

        void liveDataProcessor_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int inteval = ((LiveBars)sender).Inteval;
            if (inteval == parameter.BarInteval)
            {
                //DieBar bar = new DieBar();
                //bar.Open = args.Bar.Open;
                //bar.High = args.Bar.High;
                //bar.Low = args.Bar.Low;
                //bar.Close = args.Bar.Close;
                //bar.Finish = args.Bar.Finish;

                if (args.Bar.Close == 0)
                {
                    return;
                }
                if (args.Bar.Finish)
                {
                    // fast enter bar
                    list_efast.RemoveAt(parameter.EFast - 1);
                    list_efast.Insert(0,args.Bar.Close);
                    double sum = 0;
                    for(int i = 0; i < list_efast.Count; i++)
                    {
                        sum += list_efast[i];
                    }
                    last_maefast = sum / parameter.EFast;

                    // fast exit bar
                    list_xfast.RemoveAt(parameter.XFast - 1);
                    list_xfast.Insert(0,args.Bar.Close);
                    sum = 0;
                    for(int i = 0; i < list_xfast.Count; i++)
                    {
                        sum += list_xfast[i];
                    }
                    last_maxfast = sum / parameter.XFast;

                    // slow enter bar
                    list_eslow.RemoveAt(parameter.ESlow - 1);
                    list_eslow.Insert(0,args.Bar.Close);
                    sum = 0;
                    for (int i = 0; i < list_eslow.Count; i++)
                    {
                        sum += list_eslow[i];
                    }
                    last_maeslow = sum / parameter.ESlow;

                    // slow exit bar
                    list_xslow.RemoveAt(parameter.XSlow - 1);
                    list_xslow.Insert(0,args.Bar.Close);
                    sum = 0;
                    for(int i = 0; i < list_xslow.Count; i++)
                    {
                        sum += list_xslow[i];
                    }
                    last_maxslow = sum / parameter.XSlow;

                    if (!has_pull_data)
                    {
                        if (args.Bar.High >= init_high)
                        {
                            last_high = args.Bar.High;
                        }
                        else
                        {
                            last_high = init_high;
                        }
                        if (args.Bar.Low <= init_low)
                        {
                            last_low = args.Bar.Low;
                        }
                        else
                        {
                            last_low = init_low;
                        }
                        has_pull_data = true;
                    }
                    else
                    {
                        last_high = args.Bar.High;
                        last_low = args.Bar.Low;
                    }

                }

                
            }
        }

        public override void Reset()
        {
            liveDataProcessor.Reset();
            DateTime lastday = currentDay;
            lastday = lastday.AddDays(-1);
            openPoints = new List<OpenPoint>();
        }

        protected override void dataReceiver_Data_Arrival(object sender, StockData.TickData tickdata)
        {
            if (tickdata.Code == string.Empty)
            {
                IsSimulateFinished = true;
                HistoryFinished();
                LiveDataUpdate(tickdata);
            }
            else
            {
                if (currentDay != tickdata.Time.Date)
                {
                    currentDay = tickdata.Time.Date;
                    this.Reset();
                }
                if (tickdata.Last == 0)
                {
                    return;
                }

                currentTick = tickdata;
                //dataAnalisys.ReceiveTick(tickdata);

                liveDataProcessor.ReceiveTick(tickdata);
                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {

                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        #region 4MA策略
                        double bid1 = tickdata.Bid;
                        double ask1 = tickdata.Ask;
                        double mid_price = (tickdata.Ask + tickdata.Bid) / 2;
                        if(open_complete == 0 && close_complete == 0)
                        {
                            //开仓
                            if (last_maefast > last_maeslow && last_maxfast > last_maxslow && mid_price >= last_high)
                            {
                                if (long_position == 0)
                                {//开多
                                    Open(ask1, parameter.qty, OpenType.KaiDuo);
                                }
                            }
                            else if (last_maefast < last_maeslow && last_maxfast < last_maxslow && mid_price <= last_low)
                            {
                                if (short_position == 0)
                                {//开空
                                    Open(bid1, parameter.qty, OpenType.KaiKong);
                                }
                            }
                            //平仓
                            if ((last_maxfast < last_maxslow) || (last_maefast < last_maeslow && last_maxfast < last_maxslow && mid_price <= last_low))
                            {
                                if (long_position > 0)
                                {//平多
                                    foreach (TradePoints tp in tps)
                                    {
                                        if (!tp.Finished)
                                        {
                                            Leave(long_position, bid1, tp.TradeGuid);
                                        }
                                    }
                                }
                            }
                            else if ((last_maxfast > last_maxslow) || (last_maefast > last_maeslow && last_maxfast > last_maxslow && mid_price >= last_high))
                            {
                                if (short_position > 0)
                                {//平空
                                    foreach (TradePoints tp in tps)
                                    {
                                        if (!tp.Finished)
                                        {
                                            Leave(short_position, ask1, tp.TradeGuid);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion                   
                    }
                }
            }
        }

        delegate void OpenDelete(OpenArgs oa);
        private void OpenThread(OpenArgs oa)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Open(oa.Price, oa.Qty, oa.Opentype, oa.Si, oa.Tickdata, oa.Zhuidan, oa.Tr);
            sw.Stop();
            try
            {
                using (StreamWriter file = new StreamWriter(string.Format("{0}\\{1}.log", Application.StartupPath, "speed"), true))
                {
                    file.WriteLine(string.Format("下单耗时：{0}", sw.Elapsed.TotalMilliseconds));
                    file.Close();
                }
            }
            catch { }
        }

        private void Open(double price, double qty, OpenType ot)
        {
            PolicyResultEventArgs arg = new PolicyResultEventArgs();
            arg.PolicyName1 = this.policyName;
            arg.SecInfo = this.SecInfo;
            arg.IsReal = currentTick.IsReal;
            OpenPoint op = new OpenPoint();
            op.SecInfo = currentTick.SecInfo;
            op.OpenTime = currentTick.Time;
            op.OpenPrice = price;
            op.OpenType = ot;
            op.OpenQty = qty;
            op.DealQty = 0;
            op.Openop = true;
            op.FirstTradePriceType = parameter.EnterOrderPriceType;
            op.CancelLimitTime = parameter.EnterOrderWaitSecond;
            op.ReEnterPecentage = parameter.ReEnterPercent;
            //op.CancelLimitTime = 60;
            //op.ReEnterPecentage = 0.05;
            openPoints.Add(op);

            TradePoints tp = new TradePoints(op, 0);
            tp.Fee = parameter.Fee;
            /////////////add//////////////////////
            tp.IsReal = arg.IsReal;
            this.tps.Add(tp);
            arg.PairePoint = tp;
            arg.Tickdata = currentTick;

            open_complete = 1;

            RaiseResult(arg);
        }

        delegate void CloseDelete(double qty,double price, Guid guid);
        private void CloseThread(double qty, double price, Guid guid)
        {
            Leave(qty,price, guid);
        }

        private void Leave(double qty,double price, Guid tradeGuid)
        {
            foreach (var tp in tps)
            {
                if (tp.TradeGuid == tradeGuid)
                {
                    if (tp.OutPoint == null)
                    {
                        OpenPoint op = new OpenPoint();
                        op.SecInfo = currentTick.SecInfo;
                        //op.FirstTradePriceType = TradeSendOrderPriceType.ShiJiaWeiTuo_SH_SZ_WuDangJiChenShenChe;
                        op.FirstTradePriceType = parameter.CloseOrderPriceType;
                        op.CancelLimitTime = parameter.CloseOrderWaitSecond;
                        op.ReTradePriceType = parameter.ReCloseOrderPriceType;
                        op.ReEnterPecentage = 1;
                        op.Openop = false;
                        //op.Remark = string.Format("MA2:{0},Close{1}", args.Bar.MA.MA2, args.Bar.Close);
                        tp.OutPoint = op;
                        openPoints.Add(op);
                        tp.HowToClose = CloseType.Normal;
                        tp.Status = OpenStatus.Closed;
                    }
                    tp.OutPoint.OpenTime = currentTick.Time;
                    tp.OutPoint.OpenQty = qty;
                    if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                    {
                        tp.OutPoint.OpenType = OpenType.PingDuo;
                        tp.OutPoint.OpenPrice = price;
                    }
                    else
                    {
                        tp.OutPoint.OpenType = OpenType.PingKong;
                        tp.OutPoint.OpenPrice = price;
                    }
                    PolicyResultEventArgs arg = new PolicyResultEventArgs();
                    arg.Tickdata = currentTick;
                    arg.PolicyName1 = this.policyName;
                    arg.SecInfo = this.SecInfo;
                    arg.IsReal = tp.IsReal;
                    arg.PairePoint = tp;
                    arg.OpenRmks = tp.EnterPoint.Remark;

                    RaiseResult(arg);

                    close_complete = 1;
                    break;
                }
            }
        }


        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(string.Format("{0},", parameter.BarCountIn));
            stb.Append(string.Format("{0},", parameter.BarCountOut));
            stb.Append(string.Format("{0},", parameter.BarInteval));
            stb.Append(string.Format("{0},", parameter.Ratio));
            stb.Append(string.Format("{0},", parameter.Qty));
            return stb.ToString();
        }

        public override void ManualOpen()
        {

        }

        protected override PolicyParameter getParameter()
        {
            return this.parameter;
        }

        public override void ManualClose()
        {

        }

        public override void Notify(Guid tradeGuid, OpenStatus status, double dealQty = 0, double dealPrice = 0, string weituobianhao = "", string pendWeituobianhao = "")
        {
            for (int i = 0; i < tps.ToArray().Count(); i++)
            {
                var t = tps[i];
                if (t.TradeGuid == tradeGuid)
                {
                    if (status == OpenStatus.Opened)
                    {
                        decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                        t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                        t.EnterPoint.DealPrice = (dealPrice + t.EnterPoint.OpenPrice) / t.EnterPoint.DealQty;
                        holdHands += realDealQty;

                        if(t.EnterPoint.OpenType == OpenType.KaiDuo)
                        {
                            long_position += (int)dealQty;
                        }else if(t.EnterPoint.OpenType == OpenType.KaiKong)
                        {
                            short_position += (int)dealQty;
                        }
                        if(long_position == (int)parameter.qty || short_position == (int)parameter.qty)
                        {
                            open_complete = 0;
                        }
                        
                        t.Status = OpenStatus.Opened;
                        frozenOpenHands -= realDealQty;
                        RaiseMessage(new PolicyMessageEventArgs("策略已接受开仓通知"));
                    }
                    else if (status == OpenStatus.Open)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓下单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.Close)
                    {
                        OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                        opwt.Weituobianhao = weituobianhao;
                        opwt.OpenQty = dealQty;
                        t.OutPoint.OpenPointWeiTuo.Add(opwt);
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓下单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.Closed)
                    {
                        double RealTotalDeal = 0;
                        bool find = false;
                        for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                        {
                            if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                            {
                                OpenPointWeiTuo opwt = t.OutPoint.OpenPointWeiTuo[j];
                                decimal realDealQty = (decimal)dealQty + (decimal)opwt.PartDealQty - (decimal)opwt.DealQty;
                                frozenCloseHands -= realDealQty;
                                holdHands -= realDealQty;
                                t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                find = true;
                            }
                            RealTotalDeal += t.OutPoint.OpenPointWeiTuo[j].DealQty;

                        }


                        if(t.OutPoint.OpenType == OpenType.PingDuo)
                        {
                            long_position -= (int)dealQty;
                        }
                        else if(t.OutPoint.OpenType == OpenType.PingKong)
                        {
                            short_position -= (int)dealQty;
                        }

                        if(long_position == 0 || short_position == 0)
                        {
                            close_complete = 0;
                        }


                        if (t.EnterPoint.DealQty == RealTotalDeal)
                        {
                            t.Finished = true;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓通知-{1}-有单子：{2}", this.policyName, weituobianhao, find.ToString())));
                    }
                    else if (status == OpenStatus.OpenPending)
                    {
                        t.EnterPoint.PartDealQty = t.EnterPoint.DealQty;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收入场追单通知-HoldHands:{1}", this.policyName, holdHands)));
                    }
                    else if (status == OpenStatus.ClosePending)
                    {
                        OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                        opwt.Weituobianhao = pendWeituobianhao;
                        opwt.OpenQty = dealQty;
                        t.OutPoint.OpenPointWeiTuo.Add(opwt);
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收出场追单通知HoldHands:{1}", this.policyName, holdHands)));
                    }
                    else if (status == OpenStatus.Failed)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受失败通知", this.policyName)));
                    }
                    else if (status == OpenStatus.PartOpend)
                    {
                        if (dealQty > (t.EnterPoint.DealQty - t.EnterPoint.PartDealQty))
                        {
                            decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                            t.EnterPoint.DealQty = dealQty + t.EnterPoint.PartDealQty;
                            frozenOpenHands -= realDealQty;
                            holdHands += realDealQty;
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分入场成交通知，成交数量:{1}", this.policyName, dealQty)));
                        }
                        long_position += (int)dealQty;
                        short_position += (int)dealQty;
                    }
                    else if (status == OpenStatus.PartClosed)
                    {
                        for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                        {
                            if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                            {
                                OpenPointWeiTuo opwt = t.OutPoint.OpenPointWeiTuo[j];
                                if (dealQty > (opwt.DealQty - opwt.PartDealQty))
                                {
                                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分出场成交通知，成交数量:{1}", this.policyName, dealQty)));
                                    decimal realDealQty = (decimal)dealQty + (decimal)opwt.PartDealQty - (decimal)opwt.DealQty;
                                    frozenCloseHands -= realDealQty;
                                    holdHands -= realDealQty;
                                    t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                }
                                break;
                            }
                        }
                        long_position -= (int)dealQty;
                        short_position -= (int)dealQty;
                    }
                    else if (status == OpenStatus.OpenCanceled)
                    {
                        frozenOpenHands -= (decimal)dealQty;
                        if (t.EnterPoint.DealQty == 0)
                        {
                            t.Finished = true;
                        }

                        open_complete = 0;
                        close_complete = 0;


                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.CloseCanceled)
                    {
                        frozenCloseHands -= (decimal)dealQty;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受出场撤单通知", this.policyName)));
                    }
                }
            }
        }

        public override void InitialDataProcessor()
        {
            List<int> intevals = new List<int>();
            if (parameter.BarInteval != 60)
            {
                intevals.Add(parameter.BarInteval);
            }
            liveDataProcessor = new LiveDataProcessor(intevals, this.SecInfo, DateTime.Now);
            liveDataProcessor.OnLiveBarArrival += liveDataProcessor_OnLiveBarArrival;
            this.isLiveDataProcessor = true;
        }
    }
}
