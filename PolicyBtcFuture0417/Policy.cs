using Newtonsoft.Json.Linq;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace PolicyBtcFuture0417
{
    /*
     * 期货趋势策略
     * 
    */
    public class Policy : StockPolicies.RunningPolicy
    {
        Parameter parameter;
        DateTime currentDay;
        ConcurrentDictionary<Guid,PTradePoints> tps;
        string stockAccount;

        bool isSim;

        TickData currentTick;

        double MarkRatio;
        double LastRatio;
        double[] ArrayX;
        Dictionary<string, string> OpenWeiTuos = new Dictionary<string, string>();
        Dictionary<string, string> NeedCheDanWeiTuos = new Dictionary<string, string>();
        List<MarkPrice> LiveBars = new List<MarkPrice>();
        List<MarkPrice> LiveBars2 = new List<MarkPrice>();
        bool init = false;
        int UpTuPoCounts = 0;
        int DownTuPoCounts = 0;
        MarkPrice MarkHigh;
        MarkPrice MarkLow;
        bool CanOpen = false;
        bool KaiDuo = true;
        bool KaiKong = true;
        bool TuPoHigh = false;
        bool TuPoLow = false;

        public class MarkPrice
        {
            private TimeSpan _time;
            public TimeSpan Time
            {
                get { return _time; }
                set { _time = value; }
            }

            private double _price;
            public double Price
            {
                get { return _price; }
                set { _price = value; }
            }

            public MarkPrice(TimeSpan time, double price)
            {
                this._time = time;
                this._price = price;
            }
        }

        public static string PName
        {
            get { return "PolicyBtcFuture0417"; }
        }

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            string name = PName;
            this.policyName = name;
            this.startDate = rpp.StartDate;
            this.endDate = rpp.EndDate;
            this.inteval = rpp.Inteval;
            this.isReal = rpp.IsReal;
            this.policyguid = Guid.NewGuid();
            initialDataReceiver();
            InitialDataProcessor();
            currentDay = DateTime.Now;
            openPoints = new List<OpenPoint>();
            IsSimulateFinished = false;
            if (rpp.save)
            {
                SaveParameter(rpp);
            }
            currentTick = new TickData();
            this.isLiveDataProcessor = true;
            tps = new ConcurrentDictionary<Guid,PTradePoints>();
            if (!parameter.DebugModel)
            {
                InitArgs();
            }
        }

        public override void FunGetResult(string funName,object result)
        {
            JObject res = (JObject)result;
            JArray history = (JArray)res["data"];
            for (int i = history.Count - 1; i >= 0; i--)
            {
                TimeSpan time = System.Convert.ToDateTime(history[i][0].ToString()).TimeOfDay;
                double close = System.Convert.ToDouble(history[i][4].ToString());
                MarkPrice mp = new MarkPrice(time,close);
                LiveBars.Add(mp);
            }
            if (LiveBars.Count >= parameter.BarCount)
            {
                while (LiveBars.Count > parameter.BarCount)
                {
                    LiveBars.RemoveAt(0);
                }
            }
            double high = 0;
            double low = 100000;
            for (int i = 0; i < LiveBars.Count; i++)
            {
                if (LiveBars[i].Price < low)
                {
                    low = LiveBars[i].Price;
                    MarkLow = LiveBars[i];
                }
                if (LiveBars[i].Price > high)
                {
                    high = LiveBars[i].Price;
                    MarkHigh = LiveBars[i];
                }
            }
        }

        public override void InitArgs()
        {
            DateTime endDt = DateTime.UtcNow;
            DateTime startDt = endDt.AddMinutes(-parameter.BarInteval / 60 * parameter.BarCount);
            string start = startDt.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string end = endDt.ToString("yyyy-MM-ddTHH:mm:ssZ");
            object[] parameters = new object[] { SecInfo, CandleResolution.M1, start, end };
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
            #region 策略bar逻辑
            if (inteval == parameter.BarInteval)
            {
                MarkPrice mp = new MarkPrice(args.Bar.BarOpenTime, args.Bar.Close);
                if (MarkHigh == null)
                {
                    MarkHigh = new MarkPrice(args.Bar.BarOpenTime, args.Bar.High);
                    MarkLow = new MarkPrice(args.Bar.BarOpenTime, args.Bar.Low);
                }
                else
                {
                    if (LiveBars.Count == parameter.BarCount)
                    {
                        if (TuPoHigh)
                        {
                            double low = 100000;
                            for (int i = 0; i < LiveBars.Count; i++)
                            {
                                if (LiveBars[i].Price < low)
                                {
                                    low = LiveBars[i].Price;
                                    MarkLow = LiveBars[i];
                                }
                            }
                        }
                        else if (TuPoLow)
                        {
                            double high = 0;
                            for (int i = 0; i < LiveBars.Count; i++)
                            {
                                if (LiveBars[i].Price > high)
                                {
                                    high = LiveBars[i].Price;
                                    MarkHigh = LiveBars[i];
                                }
                            }
                        }
                    }
                    if (LiveBars.Count >= 2 * parameter.BarCount && !CanOpen)
                    {
                        if ((MarkHigh.Price - MarkLow.Price) < currentTick.Last * 0.015 && (MarkHigh.Price - MarkLow.Price) > currentTick.Last * 0.005)
                        {
                            CanOpen = true;
                            if (!parameter.DebugModel)
                            {
                                Open(MarkHigh.Price * 0.999, parameter.qty, OpenType.KaiKong);
                                Open(MarkLow.Price * 1.001, parameter.qty, OpenType.KaiDuo);
                            }
                        }
                    }
                    if (args.Bar.Close > MarkHigh.Price)
                    {
                        UpTuPoCounts += 1;
                    }
                    else
                    {
                        UpTuPoCounts = 0;
                    }
                    if (args.Bar.Close < MarkLow.Price)
                    {
                        DownTuPoCounts += 1;
                    }
                    else
                    {
                        DownTuPoCounts = 0;
                    }
                    if (args.Bar.Close > MarkHigh.Price * 1.002 )
                    {
                        MarkHigh = mp;
                        UpTuPoCounts = 0;
                        CanOpen = false;
                        TuPoHigh = true;
                        TuPoLow = false;
                        LiveBars.Clear();
                        foreach (var item in OpenWeiTuos)
                        {
                            NeedCheDanWeiTuos.Add(item.Key, item.Key);
                        }
                    }
                    if (args.Bar.Close < MarkLow.Price * 0.998)
                    {
                        MarkLow = mp;
                        DownTuPoCounts = 0;
                        CanOpen = false;
                        TuPoHigh = false;
                        TuPoLow = true;
                        LiveBars.Clear();
                        foreach (var item in OpenWeiTuos)
                        {
                            NeedCheDanWeiTuos.Add(item.Key, item.Key);
                        }
                    }
                    LiveBars.Add(mp);
                }
            }
            #endregion
        }

        public override void Reset()
        {
            MarkHigh = null;
            MarkLow = null;
            KaiDuo = KaiKong = true;
            CanOpen = false;
            TuPoHigh = false;
            TuPoLow = false;
            LiveBars.Clear();
            liveDataProcessor.Reset();
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
                if (tickdata.Time.TimeOfDay >= new TimeSpan(15, 00, 00))
                {
                    foreach (var item in tps)
                    {
                        PTradePoints tp = item.Value;
                        if (!tp.Finished)
                        {
                            if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                            {
                                Leave(tickdata.Bid, parameter.qty, tp.TradeGuid);
                            }
                            if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                            {
                                Leave(tickdata.Ask, parameter.qty, tp.TradeGuid);
                            }
                        }
                    }
                }
                if (currentDay != tickdata.Time.Date)
                {
                    currentDay = tickdata.Time.Date;
                    this.Reset();
                }
                currentTick = tickdata;
                liveDataProcessor.ReceiveTick(tickdata);

                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {
                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        if (parameter.DebugModel)
                        {
                            if (CanOpen)
                            {
                                if (tickdata.Bid >= MarkHigh.Price * 0.999 && KaiKong)
                                {
                                    KaiKong = false;
                                    Open(tickdata.Bid, parameter.qty, OpenType.KaiKong);
                                }
                                if (tickdata.Ask <= MarkLow.Price * 1.001 && KaiDuo)
                                {
                                    KaiDuo = false;
                                    Open(tickdata.Ask, parameter.qty, OpenType.KaiDuo);
                                }
                            }
                            foreach (var item in tps)
                            {
                                PTradePoints tp = item.Value;
                                if (!tp.Finished)
                                {
                                    if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                    {
                                        if (tickdata.Bid >= tp.Y)
                                        {
                                            KaiDuo = true;
                                            Leave(tickdata.Bid, parameter.qty, tp.TradeGuid);
                                        }
                                    }
                                    if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                    {
                                        if (tickdata.Ask <= tp.Y)
                                        {
                                            KaiKong = true;
                                            Leave(tickdata.Ask, parameter.qty, tp.TradeGuid);
                                        }
                                    }
                                }
                            }
                        }
                        #region 止盈、止损
                        foreach (var item in tps)
                        {
                            PTradePoints tp = item.Value;
                            if (!tp.Finished)
                            {
                                double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                                if (qty > 0)
                                {
                                    if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                    {
                                        if (tickdata.Bid <= tp.ZhiSun)
                                        {
                                            if (tp.OutPoint != null)
                                            {
                                                for (int i = 0; i < tp.OutPoint.OpenPointWeiTuo.Count; i++)
                                                {
                                                    if (tp.OutPoint.OpenPointWeiTuo[i].OpenQty > tp.OutPoint.OpenPointWeiTuo[i].DealQty)
                                                    {
                                                        string wtbh = tp.OutPoint.OpenPointWeiTuo[i].Weituobianhao;
                                                        NeedCheDanWeiTuos.Add(wtbh, wtbh);
                                                    }
                                                }
                                            }
                                            KaiDuo = true;
                                            Leave(tickdata.Bid, qty, tp.TradeGuid);
                                        }
                                    }
                                    else if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                    {
                                        if (tickdata.Ask >= tp.ZhiSun)
                                        {
                                            if (tp.OutPoint != null)
                                            {
                                                for (int i = 0; i < tp.OutPoint.OpenPointWeiTuo.Count; i++)
                                                {
                                                    if (tp.OutPoint.OpenPointWeiTuo[i].OpenQty > tp.OutPoint.OpenPointWeiTuo[i].DealQty)
                                                    {
                                                        string wtbh = tp.OutPoint.OpenPointWeiTuo[i].Weituobianhao;
                                                        NeedCheDanWeiTuos.Add(wtbh, wtbh);
                                                    }
                                                }
                                            }
                                            KaiKong = true;
                                            Leave(tickdata.Ask, qty, tp.TradeGuid);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                LiveDataUpdate(tickdata);
            }
        }

        delegate void OpenDelete(OpenArgs oa);
        private void OpenThread(OpenArgs oa)
        {
            Open(oa.Price, oa.Qty, oa.Opentype);
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
            PTradePoints tp = new PTradePoints(op, 0);
            if (ot == OpenType.KaiDuo)
            {
                tp.Y = MarkHigh.Price * 0.999;
                tp.ZhiSun = price - (MarkHigh.Price - MarkLow.Price) / 2;
            }
            if (ot == OpenType.KaiKong)
            {
                tp.Y = MarkLow.Price * 1.001;
                tp.ZhiSun = price + (MarkHigh.Price - MarkLow.Price) / 2;
            }
            tp.Fee = parameter.Fee;
            /////////////add//////////////////////
            tp.IsReal = arg.IsReal;
            this.tps.TryAdd(tp.TradeGuid,tp);
            arg.PairePoint = tp;
            arg.PolicyObject = this;
            arg.Tickdata = currentTick;
            RaiseResult(arg);
        }

        delegate void CloseDelete(double price, double qty, Guid guid);
        private void CloseThread(double price, double qty, Guid guid)
        {
            Leave(price, qty, guid);
        }
        private void Leave(double closePrice, double qty, Guid tradeGuid)
        {

            if(tps.ContainsKey(tradeGuid))
            {
                PTradePoints tp = tps[tradeGuid];
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
                tp.OutPoint.OpenQty += qty;
                if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                {
                    tp.OutPoint.OpenType = OpenType.PingDuo;
                    tp.OutPoint.OpenPrice = closePrice;
                }
                else
                {
                    tp.OutPoint.OpenType = OpenType.PingKong;
                    tp.OutPoint.OpenPrice = closePrice;
                }
                PolicyResultEventArgs arg = new PolicyResultEventArgs();
                arg.Tickdata = currentTick;
                arg.PolicyName1 = this.policyName;
                arg.SecInfo = this.SecInfo;
                arg.IsReal = tp.IsReal;
                arg.PairePoint = tp;
                arg.OpenRmks = tp.EnterPoint.Remark;

                RaiseResult(arg);
            }
        }

        public override CheDanArgs PolicyCheDan(KeCheDetail kcd)
        {
            CheDanArgs cda = new CheDanArgs();
            cda.Tsopt = TradeSendOrderPriceType.Limit;
            cda.ZhuiDan = false;
            if (NeedCheDanWeiTuos.ContainsKey(kcd.ClientBianHao))
            {
                cda.Cancel = true;
            }
            return cda;
        }

        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(string.Format("{0},", parameter.BarInteval));
            stb.Append(string.Format("{0},", parameter.BarCount));
            stb.Append(string.Format("{0},", parameter.ZhiYingBeiShu));
            stb.Append(string.Format("{0},", parameter.HuiCheBiLi));
            stb.Append(string.Format("{0},", parameter.ZhiSunBiLi));
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

        public override void ManualClose(Guid tradeGuid)
        {
            if (tps.ContainsKey(tradeGuid))
            {
                PTradePoints tp = tps[tradeGuid];
                if (currentTick.IsReal ? !tp.Finished : !tp.Closed)
                {
                    double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                    if (qty > 0)
                    {
                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                        {
                            Leave(currentTick.Bid, qty, tradeGuid);
                        }
                        else
                        {
                            Leave(currentTick.Ask, qty, tradeGuid);
                        }
                    }
                }
            }
        }

        public override void Notify(Guid tradeGuid, OpenStatus status, double dealQty = 0, double dealPrice = 0, string weituobianhao = "", string pendWeituobianhao = "")
        {
           if(tps.ContainsKey(tradeGuid))
            {
                PTradePoints t = tps[tradeGuid];
                if (status == OpenStatus.Opened)
                {
                    OpenWeiTuos.Remove(weituobianhao);
                    decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                    t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                    if (currentTick.IsReal)
                    {
                        t.Status = OpenStatus.Opened;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓通知，成交数量:{1}", this.policyName, dealQty)));
                    }

                }
                else if (status == OpenStatus.Open)
                {
                    OpenWeiTuos.Add(weituobianhao,weituobianhao);
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓下单通知", this.policyName)));
                }
                else if (status == OpenStatus.Close)
                {
                    OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                    opwt.Weituobianhao = weituobianhao;
                    opwt.OpenQty = dealQty;
                    t.OutPoint.OpenPointWeiTuo.Add(opwt);
                    if (currentTick.IsReal)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓下单通知", this.policyName)));
                    }
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
                        t.Finished = true;
                        PTradePoints tptemp;
                        tps.TryRemove(tradeGuid,out tptemp);
                    }
                    if (currentTick.IsReal)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓通知-{1}-有单子：{2}", this.policyName, weituobianhao, find.ToString())));
                    }
                }
                else if (status == OpenStatus.OpenPending)
                {
                    t.EnterPoint.PartDealQty = t.EnterPoint.DealQty;
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收入场追单通知", this.policyName)));
                }
                else if (status == OpenStatus.ClosePending)
                {
                    OpenPointWeiTuo opwt = new OpenPointWeiTuo();
                    opwt.Weituobianhao = pendWeituobianhao;
                    opwt.OpenQty = dealQty;
                    t.OutPoint.OpenPointWeiTuo.Add(opwt);
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收出场追单通知", this.policyName)));
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
                    if (NeedCheDanWeiTuos.ContainsKey(weituobianhao))
                    {
                        NeedCheDanWeiTuos.Remove(weituobianhao);
                    }
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                }
                else if (status == OpenStatus.CloseCanceled)
                {
                    if (NeedCheDanWeiTuos.ContainsKey(weituobianhao))
                    {
                        NeedCheDanWeiTuos.Remove(weituobianhao);
                    }
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受出场撤单通知", this.policyName)));
                }
            }
        }

        public override void InitialDataProcessor()
        {
            //this.marketTimeRange = MarketTimeRange.getTimeRange(this.SecInfo.Market1);
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
