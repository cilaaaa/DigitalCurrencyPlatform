using Newtonsoft.Json.Linq;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace PolicyBtcFuture0111
{
    /*
     * 期货趋势策略
     * 
    */
    public class Policy : StockPolicies.RunningPolicy
    {
        Parameter parameter;
        DateTime currentDay;
        string stockAccount;
        ConcurrentDictionary<Guid,PTradePoints> tps;
        bool isSim;

        TickData currentTick;
        DateTime StopTradeTime;
        List<DateTime> OpenTimeList = new List<DateTime>() { };
        List<DieBar> LiveBars = new List<DieBar>();
        DateTime NextOpenTime;
        double OpenPrice = 0;
        double SighHighPrice = 0;
        double SighLowPrice = 1000000;
        double RegionHighPrice = 0;
        double RegionLowPrice = 1000000;
        bool StartTrade = false;
        bool InitKaiCangBar = false;
        bool K1 = false;
        bool K2 = false;
        bool arriveHigh = false;
        bool arriveLow = false;
        bool firstDuoOpen = true;
        bool firstKongOpen = true;
        double DuoChuChangPrice = -1;
        double KongChuChangPrice = 1000000;
        double ZHighPrice = 0;
        double ZLowPrice = 1000000;
        int OpenDuoTimes = 0;
        int OpenKongTimes = 0;
        double Qty = 0;

        public static string PName
        {
            get { return "PolicyBtcFuture0111"; }
        }

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = PName + "-" + parameter.BarInteval + "-" + parameter.BarCount + "-" + parameter.TimeCycle.ToString() + "-" + parameter.ZhiYingBeiShu.ToString();
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
            tps = new ConcurrentDictionary<Guid, PTradePoints>();
        }

        public override void FunGetResult(string funName,object result)
        {
            JObject res = (JObject)result;
            JArray history = (JArray)res["data"];
            OpenPrice = System.Convert.ToDouble(history[0][1].ToString());
            for (int i = 0; i < history.Count; i++)
            {
                double high = System.Convert.ToDouble(history[i][2].ToString());
                double low = System.Convert.ToDouble(history[i][3].ToString());
                DieBar bar = new DieBar();
                bar.High = high;
                bar.Low = low;
                if (high > SighHighPrice)
                {
                    SighHighPrice = high;
                }
                if (low < SighLowPrice)
                {
                    SighLowPrice = low;
                }
                LiveBars.Add(bar);
            }
            if (LiveBars.Count >= parameter.BarCount)
            {
                StartTrade = true;
                RegionHighPrice = SighHighPrice;
                RegionLowPrice = SighLowPrice;
            }
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
                if (args.Bar.Finish)
                {
                    if (!InitKaiCangBar)
                    {
                        if (LiveBars.Count < parameter.BarCount)
                        {
                            DieBar bar = new DieBar();
                            bar.High = args.Bar.High;
                            bar.Low = args.Bar.Low;
                            if (bar.High > SighHighPrice)
                            {
                                SighHighPrice = bar.High;
                            }
                            if (bar.Low < SighLowPrice)
                            {
                                SighLowPrice = bar.Low;
                            }
                            LiveBars.Add(bar);
                        }
                        else
                        {
                            InitKaiCangBar = true;
                            StartTrade = true;
                            RegionHighPrice = SighHighPrice;
                            RegionLowPrice = SighLowPrice;
                        }
                    }
                    else
                    {
                        if (args.Bar.High > RegionHighPrice)
                        {
                            RegionHighPrice = args.Bar.High;
                        }
                        if (args.Bar.Low < RegionLowPrice)
                        {
                            RegionLowPrice = args.Bar.Low;
                        }
                        if (args.Bar.Close < SighHighPrice && K1)
                        {
                            K1 = false;
                            arriveHigh = false;
                        }
                        if (args.Bar.Close > SighLowPrice && K2)
                        {
                            K2 = false;
                            arriveLow = false;
                        }
                        if (arriveHigh && !K1)
                        {
                            if (args.Bar.Close < args.Bar.Open)
                            {
                                if (args.Bar.Close < args.Bar.PreBar.Low)
                                {
                                    K1 = true;
                                }
                            }
                        }
                        if (arriveLow && !K2)
                        {
                            if (args.Bar.Close > args.Bar.Open)
                            {
                                if (args.Bar.Close > args.Bar.PreBar.High)
                                {
                                    K2 = true;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void resetArgs()
        {
            InitKaiCangBar = false;
            StopTradeTime = NextOpenTime.AddMinutes(-10);
            LiveBars.Clear();
            SighHighPrice = 0;
            SighLowPrice = 100000;
            firstDuoOpen = true;
            firstKongOpen = true;
            DuoChuChangPrice = -1;
            KongChuChangPrice = 1000000;
            ZHighPrice = 0;
            ZLowPrice = 1000000;
            StartTrade = false;
            arriveHigh = false;
            arriveLow = false;
            K1 = false;
            K2 = false;
        }

        public override void InitArgs()
        {
            if (!parameter.DebugModel)
            {
                int openhour = parameter.TimeCycle;
                DateTime fdt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                OpenTimeList.Add(fdt);
                while (openhour <= 24)
                {
                    OpenTimeList.Add(fdt.AddHours(openhour));
                    openhour += parameter.TimeCycle;
                }
                OpenTimeList.Add(fdt.AddHours(openhour));
                DateTime now = DateTime.Now;
                DateTime hour = DateTime.Now;
                currentDay = now.Date;
                for (int i = 1; i < OpenTimeList.Count(); i++)
                {
                    if (now <= OpenTimeList[i])
                    {
                        hour = OpenTimeList[i - 1];
                        NextOpenTime = OpenTimeList[i];
                        break;
                    }
                }
                StopTradeTime = NextOpenTime.AddMinutes(-10);
                DateTime enddt = hour.AddMinutes(parameter.BarCount);
                string start = hour.AddHours(-8).ToString("yyyy-MM-ddTHH:mm:ssZ");
                string end = enddt.AddHours(-8).ToString("yyyy-MM-ddTHH:mm:ssZ");

                object[] parameters = new object[] { SecInfo, CandleResolution.M1, start, end };
                PolicyFunCGetEventArgs args = new PolicyFunCGetEventArgs();
                args.Parameters = parameters;
                args.PolicyObject = this;
                args.SecInfo = SecInfo;
                args.FunName = "GetKLine";
                RaiseFunCGet(args);
            }
            else
            {
                NextOpenTime = parameter.StartDate;
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
                currentTick = tickdata;
                liveDataProcessor.ReceiveTick(tickdata);
                if (tickdata.Time > StopTradeTime)
                {
                    StartTrade = false;
                }
                if (currentDay != tickdata.Time.Date)
                {
                    currentDay = tickdata.Time.Date;
                    this.Reset();
                }
                if (tickdata.Last == 0)
                {
                    return;
                }
                if (tickdata.Time >= NextOpenTime)
                {
                    foreach (var item in tps)
                    {
                        PTradePoints tp = item.Value;
                        if (tickdata.IsReal ? !tp.Finished : !tp.Closed)
                        {
                            double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                            if (qty > 0)
                            {
                                Leave(tp.EnterPoint.OpenType == OpenType.KaiDuo ? tickdata.Ask : tickdata.Bid, qty, tp.TradeGuid);
                            }
                        }
                    }
                    NextOpenTime = NextOpenTime.AddHours(parameter.TimeCycle);
                    this.resetArgs();
                }
                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {
                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        if (StartTrade)
                        {
                            if (tickdata.Last > ZHighPrice)
                            {
                                ZHighPrice = tickdata.Last;
                            }
                            if (tickdata.Last < ZLowPrice)
                            {
                                ZLowPrice = tickdata.Last;
                            }
                            if (tickdata.Last > RegionHighPrice && !arriveHigh)
                            {
                                RegionHighPrice = tickdata.Last;
                                arriveHigh = true;
                            }
                            if (tickdata.Last < RegionLowPrice && !arriveLow)
                            {
                                RegionLowPrice = tickdata.Last;
                                arriveLow = true;
                            }
                            if (K1 && OpenDuoTimes < parameter.JiaCangCiShu)
                            {
                                if (tickdata.Last > RegionHighPrice)
                                {
                                    Qty = parameter.qty;
                                    if (OpenDuoTimes > 0)
                                    {
                                        for (int j = 1; j <= OpenDuoTimes; j++)
                                        {
                                            Qty = Qty * parameter.JiaCangJiaGeBeiShu;
                                        }
                                    }
                                    OpenDuoTimes += 1;
                                    Open(tickdata.Ask, Qty, OpenType.KaiDuo);
                                }
                            }
                            if (K2 && OpenKongTimes < parameter.JiaCangCiShu)
                            {
                                if (tickdata.Last < RegionLowPrice)
                                {
                                    Qty = parameter.qty;
                                    if (OpenKongTimes > 0)
                                    {
                                        for (int j = 1; j <= OpenKongTimes; j++)
                                        {
                                            Qty = Qty * parameter.JiaCangJiaGeBeiShu;
                                        }
                                    }
                                    OpenKongTimes += 1;
                                    Open(tickdata.Bid, Qty, OpenType.KaiKong);
                                }
                            }
                            #region 止损
                            if (tickdata.Last < DuoChuChangPrice)
                            {
                                foreach (var item in tps)
                                {
                                    PTradePoints tp = item.Value;
                                    if (tickdata.IsReal ? !tp.Finished : !tp.Closed)
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
                            if (tickdata.Last > KongChuChangPrice)
                            {
                                foreach (var item in tps)
                                {
                                    PTradePoints tp = item.Value;
                                    if (tickdata.IsReal ? !tp.Finished : !tp.Closed)
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
                            #endregion
                            #region 止盈
                            foreach (var item in tps)
                            {
                                PTradePoints tp = item.Value;
                                if (tickdata.IsReal ? !tp.Finished : !tp.Closed)
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
                                            double beiShu = 0;
                                            double huice = 0.6;
                                            if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                            {
                                                beiShu = (tickdata.Last - tp.EnterPoint.OpenPrice) / (tp.Y - tp.EnterPoint.OpenPrice);

                                            }
                                            else
                                            {
                                                beiShu = (tp.EnterPoint.OpenPrice - tickdata.Last) / (tp.EnterPoint.OpenPrice - tp.Y);
                                            }
                                            if (beiShu >= 4)
                                            {
                                                huice = 0.3;
                                            }
                                            else if (beiShu >= 3.5)
                                            {
                                                huice = 0.35;
                                            }
                                            else if (beiShu >= 3)
                                            {
                                                huice = 0.4;
                                            }
                                            else if (beiShu >= 2.5)
                                            {
                                                huice = 0.45;
                                            }
                                            else if (beiShu >= 2)
                                            {
                                                huice = 0.5;
                                            }
                                            else if (beiShu >= 1.5)
                                            {
                                                huice = 0.55;
                                            }
                                            if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                                            {
                                                if ((tp.Y - tickdata.Last) / (tp.Y - tp.EnterPoint.OpenPrice) > huice)
                                                {
                                                    Leave(tickdata.Bid, qty, tp.TradeGuid);
                                                }
                                            }
                                            else
                                            {
                                                if ((tickdata.Last - tp.Y) / (tp.EnterPoint.OpenPrice - tp.Y) > huice)
                                                {
                                                    Leave(tickdata.Ask, qty, tp.TradeGuid);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region 出场
                            #endregion
                        }
                    }
                }
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
            if (firstDuoOpen)
            {
                firstDuoOpen = false;
                DuoChuChangPrice = SighHighPrice;
            }
            else
            {
                DuoChuChangPrice = ZLowPrice;
            }
            if (firstKongOpen)
            {
                firstKongOpen = false;
                KongChuChangPrice = SighLowPrice;
            }
            else
            {
                KongChuChangPrice = ZHighPrice;
            }
            PTradePoints tp = new PTradePoints(op, 0);
            if (ot == OpenType.KaiDuo)
            {
                tp.Y = (1 + parameter.ZhiYingBeiShu) * price;
                arriveHigh = false;
                K1 = false;
                ZLowPrice = 1000000;
            }
            if (ot == OpenType.KaiKong)
            {
                tp.Y = (1 - parameter.ZhiYingBeiShu) * price;
                arriveLow = false;
                K2 = false;
                ZHighPrice = 0;
            }
            
            tp.Fee = parameter.Fee;
            /////////////add//////////////////////
            tp.IsReal = arg.IsReal;
            this.tps.TryAdd(tp.TradeGuid, tp);
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



        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append(string.Format("{0},", parameter.BarInteval));
            stb.Append(string.Format("{0},", parameter.TimeCycle));
            stb.Append(string.Format("{0},", parameter.BarCount));
            stb.Append(string.Format("{0},", parameter.ZhiYingBeiShu));
            stb.Append(string.Format("{0},", parameter.Qty));
            return stb.ToString();
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

        public override void ManualOpen()
        {

        }

        protected override PolicyParameter getParameter()
        {
            return this.parameter;
        }

        public override void ManualClose(Guid tradeGuid)
        {
            if(tps.ContainsKey(tradeGuid))
            {
                PTradePoints tp = tps[tradeGuid];
                if (currentTick.IsReal ? !tp.Finished : !tp.Closed)
                {
                    double qty = tp.EnterPoint.DealQty - (tp.OutPoint != null ? tp.OutPoint.OpenQty : 0);
                    if (qty > 0)
                    {
                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                        {
                            Leave(currentTick.Bid, qty, tp.TradeGuid);
                        }
                        else
                        {
                            Leave(currentTick.Ask, qty, tp.TradeGuid);
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
                    //t.EnterPoint.DealPrice = (dealPrice + t.EnterPoint.OpenPrice) / t.EnterPoint.DealQty;
                    if (currentTick.IsReal)
                    {
                        t.Status = OpenStatus.Opened;
                        RaiseMessage(new PolicyMessageEventArgs("策略已接受开仓通知"));
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
                            OpenDuoTimes -= 1;
                        }
                        else
                        {
                            OpenKongTimes -= 1;
                        }
                        t.Finished = true;
                        PTradePoints tptemp;
                        tps.TryRemove(tradeGuid, out tptemp);
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
                    //for (int j = 0; j < t.OutPoint.OpenPointWeiTuo.Count; j++)
                    //{
                    //    if (t.OutPoint.OpenPointWeiTuo[j].Weituobianhao == weituobianhao)
                    //    {
                    //        t.OutPoint.OpenPointWeiTuo[j].PartDealQty = t.OutPoint.OpenPointWeiTuo[j].DealQty;
                    //        t.OutPoint.OpenPointWeiTuo[j].Weituobianhao = pendWeituobianhao;
                    //    }
                    //}
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
