using Newtonsoft.Json.Linq;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace PolicyBtcFuture0226
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
        List<double> LiveBars = new List<double>();
        bool init = false;
        double MarkHigh = 0;
        double LastHigh = 0;
        double LastLow = 0;
        double MarkLow = 0;
        Decimal DuoHands = 0;
        Decimal FrozenDuoHands = 0;
        Decimal KongHands = 0;
        Decimal FrozenKongHands = 0;

        public static string PName
        {
            get { return "PolicyBtcFuture0226"; }
        }

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            string name = PName + "-" + parameter.BarInteval.ToString() + "-" + parameter.BarCount.ToString() + "-" + parameter.ZhiYingBeiShu.ToString() + "-" + parameter.HuiCheBiLi.ToString() + "-" + parameter.ZhiSunBiLi.ToString();
            name = name.Replace(".", "");
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
            //isOpened = false;
            if (rpp.save)
            {
                SaveParameter(rpp);
            }
            currentTick = new TickData();
            this.isLiveDataProcessor = true;
            tps = new ConcurrentDictionary<Guid,PTradePoints>();
            ArrayX = new double[parameter.BarCount];
            for (int i = 0; i < parameter.BarCount; i++)
            {
                ArrayX[i] = i;
            }
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
                double close = System.Convert.ToDouble(history[i][4].ToString());
                LiveBars.Add(close);
            }
            if (LiveBars.Count >= parameter.BarCount)
            {
                while (LiveBars.Count > parameter.BarCount)
                {
                    LiveBars.RemoveAt(0);
                }
                CalArgs();
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

        public double CalRatio(double[] arrayY)
        {
            int Len = parameter.BarCount;
            double var2 = Len * (Len - 1) * 0.5;
            double var3 = Len * (Len - 1) * (2 * Len - 1) / 6;
            double var5 = Math.Sqrt(var2) - Len * var3;
            double var0 = 0;
            for (int i = 0; i < arrayY.Count(); i++)
            {
                var0 = var0 + i * arrayY[i];
            }
            double var1 = arrayY.Sum();
            return (Len * var0 - var2 * var1) / var5;
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
                while (LiveBars.Count > parameter.BarCount)
                {
                    LiveBars.RemoveAt(0);
                }
                if (LiveBars.Count == parameter.BarCount)
                {
                    CalArgs();
                }
                LiveBars.Add(args.Bar.Close);
            }
            #endregion
        }

        private void CalArgs()
        {
            LastRatio = CalRatio(LiveBars.ToArray());
            if (!init)
            {
                init = true;
                MarkRatio = LastRatio;
            }
            Console.WriteLine(LastRatio * MarkRatio);
            if (LastRatio * MarkRatio < 0)
            {
                MarkRatio = LastRatio;
                if (LastRatio < 0)
                {
                    MarkHigh = Math.Max(LiveBars[LiveBars.Count - 2], LiveBars[LiveBars.Count - 1]);
                }
                else
                {
                    MarkLow = Math.Min(LiveBars[LiveBars.Count - 2], LiveBars[LiveBars.Count - 1]);
                }
            }
        }

        public override void Reset()
        {
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
                        if (MarkHigh != 0 && MarkLow != 0)
                        {
                            if (tickdata.Last > MarkHigh && MarkHigh > MarkLow)
                            {
                                if (DuoHands + FrozenDuoHands == 0)
                                {
                                    if (KongHands > 0)
                                    {
                                        foreach (var item in tps)
                                        {
                                            PTradePoints tp = item.Value;
                                            if (!tp.Finished)
                                            {
                                                if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                                {
                                                    double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                                                    if (qty > 0)
                                                    {
                                                        Leave(tickdata.Ask, qty, tp.TradeGuid);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    FrozenDuoHands += (Decimal)parameter.qty;
                                    Open(tickdata.Bid, parameter.qty, OpenType.KaiDuo);
                                }
                            }
                            if (tickdata.Last < MarkLow && MarkHigh > MarkLow)
                            {
                                if (KongHands + FrozenKongHands == 0)
                                {
                                    if (DuoHands > 0)
                                    {
                                        foreach (var item in tps)
                                        {
                                            PTradePoints tp = item.Value;
                                            if (!tp.Finished)
                                            {
                                                if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                                {
                                                    double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                                                    if (qty > 0)
                                                    {
                                                        Leave(tickdata.Bid, qty, tp.TradeGuid);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    FrozenKongHands += (Decimal)parameter.qty;
                                    Open(tickdata.Ask, parameter.qty, OpenType.KaiKong);
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
                                    if (!tp.StartZhiYing)
                                    {
                                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                        {
                                            if (tickdata.Last >= tp.Y)
                                            {
                                                tp.StartZhiYing = true;
                                            }
                                        }
                                        else if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                        {
                                            if (tickdata.Last <= tp.Y)
                                            {
                                                tp.StartZhiYing = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                        {
                                            if (tickdata.Last > tp.Y)
                                            {
                                                tp.Y = tickdata.Last;
                                            }
                                            if ((tp.Y - tickdata.Last) / (tp.Y - tp.EnterPoint.OpenPrice) > parameter.HuiCheBiLi)
                                            {
                                                Leave(tickdata.Ask, qty, tp.TradeGuid);
                                            }
                                        }
                                        else
                                        {
                                            if (tickdata.Last < tp.Y)
                                            {
                                                tp.Y = tickdata.Last;
                                            }
                                            if ((tickdata.Last - tp.Y) / (tp.EnterPoint.OpenPrice - tp.Y) > parameter.HuiCheBiLi)
                                            {
                                                Leave(tickdata.Bid, qty, tp.TradeGuid);
                                            }
                                        }
                                    }
                                    if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                    {
                                        if (tickdata.Last <= tp.ZhiSun)
                                        {
                                            Leave(tickdata.Ask, qty, tp.TradeGuid);
                                        }
                                    }
                                    else if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                                    {
                                        if (tickdata.Last >= tp.ZhiSun)
                                        {
                                            Leave(tickdata.Bid, qty, tp.TradeGuid);
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
                tp.Y = (1 + parameter.ZhiYingBeiShu) * price;
                tp.ZhiSun = (1 - parameter.ZhiSunBiLi) * price;
            }
            if (ot == OpenType.KaiKong)
            {
                tp.Y = (1 - parameter.ZhiYingBeiShu) * price;
                tp.ZhiSun = (1 + parameter.ZhiSunBiLi) * price;
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
            TickData tick = CurrentStockData.GetTick(kcd.Si);
            if (kcd.OverTime())
            {
                if ((kcd.OpenType == OpenType.KaiDuo || kcd.OpenType == OpenType.PingKong) && kcd.OrderPrice != tick.Ask)
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = true;
                    cda.NewOrderPrice = tick.Ask;
                }
                else if ((kcd.OpenType == OpenType.KaiKong || kcd.OpenType == OpenType.PingDuo) && kcd.OrderPrice != tick.Bid)
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = true;
                    cda.NewOrderPrice = tick.Bid;
                }
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
                    decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                    t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                    if (t.EnterPoint.OpenType == OpenType.KaiDuo)
                    {
                        DuoHands += (Decimal)t.EnterPoint.DealQty;
                        FrozenDuoHands -= (Decimal)t.EnterPoint.DealQty;
                    }
                    else
                    {
                        KongHands += (Decimal)t.EnterPoint.DealQty;
                        FrozenKongHands -= (Decimal)t.EnterPoint.DealQty;
                    }
                    if (currentTick.IsReal)
                    {
                        t.Status = OpenStatus.Opened;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓通知，成交数量:{1}", this.policyName, dealQty)));
                    }

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
                        if (t.EnterPoint.OpenType == OpenType.KaiDuo)
                        {
                            DuoHands -= (decimal)RealTotalDeal;
                        }
                        else
                        {
                            KongHands -= (decimal)RealTotalDeal;
                        }
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
                    if (t.EnterPoint.DealQty == 0)
                    {
                        t.Finished = true;
                    }
                    RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                }
                else if (status == OpenStatus.CloseCanceled)
                {
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
