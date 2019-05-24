using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using StockTradeAPI;
using System.Windows.Forms;

namespace PolicyBtcFuture1225
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

        double openHands;
        Decimal closeHands;

        //0，没开；1，开多；2，开空
        int openSide = 0;
        bool dealState = false;
        bool openComplete = true;
        bool closeComplete = true;

        Decimal frozenBuyHands;
        Decimal frozenSellHands;

        Decimal HandBuyOrder;
        Decimal HandSellOrder;

        List<DieBar> LiveBarsIn;
        List<DieBar> LiveBarsOut;
        bool init = false;
        double signInHighPrice;
        double signOutHighPrice;
        double signInLowPrice;
        double signOutLowPrice;
        double Ratio;
        OpenType ot;

        public static string PName
        {
            get { return "PolicyBtcFuture1225"; }
        }

        object lockObject;

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = string.Format("{0}-{1}s-{2}-{3}-{4}", PName, rpp.BarInteval.ToString(), rpp.BarCountIn.ToString(), rpp.Ratio.ToString(), rpp.CodeName);
            this.startDate = rpp.StartDate;
            this.endDate = rpp.EndDate;
            this.inteval = rpp.Inteval;
            this.isReal = rpp.IsReal;
            this.policyguid = Guid.NewGuid();

            LiveBarsIn = new List<DieBar>();
            LiveBarsOut = new List<DieBar>();
            //if (!parameter.Debug)
            //if (!parameter.Debug)
            //{
            //LoadHistoryBars();
            //}

            //LoadHistoryBars();

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

        private void LoadHistoryBars()
        {

            int resolution = parameter.BarInteval;

            string nowTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

            JArray history = TradeAPI.GetKLine(SecInfo, resolution.ToString(), "", nowTime);

            for (int i = 1; i < parameter.BarCountIn + 1; i++)
            {
                DieBar bar = new DieBar();
                bar.Open = System.Convert.ToDouble(history[i][1].ToString());
                bar.High = System.Convert.ToDouble(history[i][2].ToString());
                bar.Low = System.Convert.ToDouble(history[i][3].ToString());
                bar.Close = System.Convert.ToDouble(history[i][4].ToString());
                LiveBarsIn.Add(bar);
            }
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
                if (LiveBarsIn.Count == 0)
                {
                    LoadHistoryBars();
                }
                else
                {
                    DieBar bar = new DieBar();
                    bar.Open = args.Bar.Open;
                    bar.High = args.Bar.High;
                    bar.Low = args.Bar.Low;
                    bar.Close = args.Bar.Close;
                    bar.Finish = args.Bar.Finish;

                    if (bar.Close == 0)
                    {
                        return;
                    }

                    if (LiveBarsIn.Count < parameter.BarCountIn)
                    {
                        if (bar.Finish)
                        {
                            LiveBarsIn.Add(bar);
                        }
                    }
                    else
                    {
                        if (!init)
                        {
                            foreach (DieBar tempBar in LiveBarsIn)
                            {
                                if (tempBar.High > signInHighPrice)
                                {
                                    signInHighPrice = tempBar.High;
                                }
                                if (tempBar.Low < signInLowPrice)
                                {
                                    signInLowPrice = tempBar.Low;
                                }
                            }
                            Ratio = ((signInHighPrice - signInLowPrice) / ((signInHighPrice + signInLowPrice) * 0.5)) * 100;
                            init = true;
                        }
                        if (bar.Finish)
                        {
                            DieBar removeBar = LiveBarsIn[0];
                            LiveBarsIn.RemoveAt(0);
                            LiveBarsIn.Add(bar);

                            if (bar.High > signInHighPrice)
                            {
                                signInHighPrice = bar.High;
                            }
                            else
                            {
                                if (removeBar.High == signInHighPrice)
                                {
                                    signInHighPrice = 0;
                                    foreach (DieBar tempBar in LiveBarsIn)
                                    {
                                        if (tempBar.High > signInHighPrice)
                                        {
                                            signInHighPrice = tempBar.High;
                                        }
                                    }
                                }
                            }

                            if (bar.Low < signInLowPrice)
                            {
                                signInLowPrice = bar.Low;
                            }
                            else
                            {
                                if (removeBar.Low == signInLowPrice)
                                {
                                    signInLowPrice = 1000000;
                                    foreach (DieBar tempBar in LiveBarsIn)
                                    {
                                        if (tempBar.Low < signInLowPrice)
                                        {
                                            signInLowPrice = tempBar.Low;
                                        }
                                    }
                                }
                            }
                            Ratio = ((signInHighPrice - signInLowPrice) / ((signInHighPrice + signInLowPrice) * 0.5)) * 100;
                        }
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

            //Console.WriteLine("openSide:{0},signInHighPrice:{1},signInLowPrice:{2},tickdata.Last:{3},dealState:{4},openComplete:{5},closeComplete:{6}", openSide, signInHighPrice, signInLowPrice, tickdata.Last, dealState, openComplete, closeComplete);
            //Console.WriteLine("Ratio:{0}", Ratio);
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
                liveDataProcessor.ReceiveTick(tickdata);
                currentTick = tickdata;
                if (parameter.StartTime <= tickdata.Time.TimeOfDay && parameter.EndTime >= tickdata.Time.TimeOfDay && init)
                {
                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        if (dealState == true && openComplete && closeComplete)
                        {
                            dealState = false;
                        }

                        if (dealState == false)
                        {
                            if (openHands > 0)
                            {

                            }
                            if (tickdata.Last > signInHighPrice && openSide != 1)
                            {
                                if (openSide == 2)
                                {
                                    foreach (TradePoints tp in tps)
                                    {
                                        if (!tp.Finished)
                                        {
                                            if (!tickdata.IsReal)
                                            {
                                                tp.Finished = true;
                                                Leave(tp.EnterPoint.OpenQty, tickdata.Last, tp.TradeGuid);

                                            }
                                            else
                                            {
                                                //平空    
                                                closeComplete = false;
                                                dealState = true;
                                                CloseDelete od = new CloseDelete(CloseThread);
                                                od.BeginInvoke(parameter.qty, tickdata.Last, tp.TradeGuid, null, null);

                                            }
                                        }
                                    }
                                }

                                if (Ratio <= parameter.Ratio)
                                {
                                    if (!tickdata.IsReal)
                                    {
                                        openSide = 1;
                                        openComplete = false;
                                        dealState = true;
                                        openHands = parameter.qty;
                                        OpenArgs oa = new OpenArgs(
                                                tickdata.Last,
                                                parameter.qty,
                                                OpenType.KaiDuo,
                                                SecInfo,
                                                tickdata,
                                                -1,
                                                "");
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                    }
                                    else
                                    {
                                        openSide = 1;
                                        holdHands += (decimal)parameter.Qty;
                                        Open(tickdata.Last, parameter.qty, OpenType.KaiDuo, SecInfo, tickdata, -1, "");
                                    }
                                }
                            }
                            else if (tickdata.Last < signInLowPrice && openSide != 2)
                            {
                                if (openSide == 1)
                                {
                                    foreach (TradePoints tp in tps)
                                    {
                                        if (!tp.Finished)
                                        {
                                            if (!tickdata.IsReal)
                                            {
                                                tp.Finished = true;
                                                Leave(tp.EnterPoint.OpenQty, tickdata.Last, tp.TradeGuid);

                                            }
                                            else
                                            {
                                                //平多
                                                closeComplete = false;
                                                dealState = true;
                                                CloseDelete od = new CloseDelete(CloseThread);
                                                od.BeginInvoke(parameter.qty, tickdata.Last, tp.TradeGuid, null, null);

                                            }
                                        }
                                    }
                                }

                                if (Ratio <= parameter.Ratio)
                                {
                                    if (!tickdata.IsReal)
                                    {
                                        openSide = 2;
                                        openComplete = false;
                                        dealState = true;
                                        openHands = parameter.qty;
                                        OpenArgs oa = new OpenArgs(
                                                tickdata.Last,
                                                parameter.qty,
                                                OpenType.KaiKong,
                                                SecInfo,
                                                tickdata,
                                                -1,
                                                "");
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                    }
                                    else
                                    {
                                        openSide = 2;
                                        Open(tickdata.Last, parameter.qty, OpenType.KaiKong, SecInfo, tickdata, -1, "");
                                    }
                                }
                            }
                        }
                    }
                }
                LiveDataUpdate(tickdata);
                if (parameter.IsReal)
                {
                    try
                    {
                        UpdateChart();
                    }
                    catch { }
                }
            }
        }

        delegate void OpenDelete(OpenArgs oa);
        private void OpenThread(OpenArgs oa)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Open(oa.Price, oa.Qty, oa.Opentype, oa.Si, oa.Tickdata, oa.Zhuidan, oa.Tr);
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

        private void Open(double price, double qty, OpenType ot, SecurityInfo si, TickData td, double zhuidan, string _tr = "")
        {
            PolicyResultEventArgs arg = new PolicyResultEventArgs();
            arg.PolicyName1 = this.policyName;
            arg.SecInfo = si;
            arg.IsReal = currentTick.IsReal;
            OpenPoint op = new OpenPoint();
            op.SecInfo = si;
            op.OpenTime = td.Time;
            op.OpenPrice = price;
            op.OpenType = ot;
            op.OpenQty = qty;
            op.DealQty = 0;
            op.Openop = true;
            if (zhuidan < 0)
            {
                op.FirstTradePriceType = parameter.EnterOrderPriceType;
                op.CancelLimitTime = parameter.EnterOrderWaitSecond;
                op.ReEnterPecentage = zhuidan;

            }
            else
            {
                op.FirstTradePriceType = TradeSendOrderPriceType.Market;
                op.CancelLimitTime = parameter.EnterOrderWaitSecond;
                op.ReEnterPecentage = zhuidan;

            }
            openPoints.Add(op);
            string tr = _tr;
            if (tr == "")
            {
                tr = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            TradePoints tp = new TradePoints(op, 0);
            tp.TpRemark = tr;
            tp.Fee = parameter.Fee;
            tp.IsReal = arg.IsReal;
            this.tps.Add(tp);
            arg.PairePoint = tp;
            arg.Tickdata = currentTick;
            RaiseResult(arg);
        }

        delegate void CloseDelete(double qty, double price, Guid guid);
        private void CloseThread(double qty, double price, Guid guid)
        {
            Leave(qty, price, guid);
        }

        private void Leave(double qty, double price, Guid tradeGuid)
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
                    break;
                }
            }
        }


        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(string.Format("{0},", parameter.BarCountIn));
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
                if (t.TradeGuid == tradeGuid && currentTick.IsReal)
                {
                    if (status == OpenStatus.Opened)
                    {
                        t.EnterPoint.OpenPrice = dealPrice;
                        decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                        t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                        t.Status = OpenStatus.Opened;
                        openComplete = true;
                        openHands = 0;
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
                                t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                find = true;
                            }
                            RealTotalDeal += t.OutPoint.OpenPointWeiTuo[j].DealQty;

                        }
                        if (t.EnterPoint.DealQty == RealTotalDeal)
                        {
                            closeComplete = true;
                            t.Finished = true;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓通知-{1}-有单子：{2}", this.policyName, weituobianhao, find.ToString())));
                    }
                    else if (status == OpenStatus.OpenPending)
                    {
                        t.EnterPoint.PartDealQty = t.EnterPoint.DealQty;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略HoldHands:{1}", this.policyName, holdHands)));
                    }
                    else if (status == OpenStatus.ClosePending)
                    {
                        for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                        {
                            if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                            {
                                t.OutPoint.OpenPointWeiTuo[j].PartDealQty = t.OutPoint.OpenPointWeiTuo[j].DealQty;
                                t.OutPoint.OpenPointWeiTuo[j].Weituobianhao = pendWeituobianhao;
                            }
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略HoldHands:{1}", this.policyName, holdHands)));
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
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分入场成交通知，成交数量:{1}", this.policyName, dealQty)));
                        }
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
                                    t.OutPoint.OpenPointWeiTuo[j].DealQty = dealQty + opwt.PartDealQty;
                                }
                                break;
                            }
                        }
                    }
                    else if (status == OpenStatus.OpenCanceled)
                    {
                        if (t.EnterPoint.DealQty == 0)
                        {
                            t.Finished = true;
                            openSide = 0;
                            openComplete = true;
                            openHands = 0;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.CloseCanceled)
                    {
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
