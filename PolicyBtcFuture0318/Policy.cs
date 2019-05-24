using DataAPI;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PolicyBtcFuture0318
{
    /*
     * 期货趋势策略
     * 
    */
    public class Policy : StockPolicies.RunningPolicy
    {
        Parameter parameter;
        DateTime currentDay;
        ConcurrentDictionary<Guid, TradePoints> tps;
        string stockAccount;

        bool isSim;

        TickData currentTick;

        List<double> LiveBars = new List<double>();
        TickData SecondTick;
        SecurityInfo SecondSi;
        bool firstDeal = true;
        bool secondDeal = true;
        bool Code1AtoB = false;
        bool Code1BtoA = false;
        bool Code2AtoB = false;
        bool Code2BtoA = false;
        List<int> loseArr = new List<int>();
        int loseCount = 0;

        public static string PName
        {
            get { return "PolicyBtcFuture0318"; }
        }

        public Policy(SecurityInfo si, Parameter rpp, PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = PName;
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
            tps = new ConcurrentDictionary<Guid, TradePoints>();
            SecondSi = GlobalValue.GetFutureByCodeAndMarket(parameter.SecondCode, parameter.SecondMarket);
            SecondTick = new TickData();
        }

        public override void FunGetResult(string funName, object result)
        {
        }

        public override void InitArgs()
        {
        }


        private void SaveParameter(Parameter rpp)
        {
            rpp.Save();
        }

        void liveDataProcessor_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int inteval = ((LiveBars)sender).Inteval;
        }


        public override void Reset()
        {
            liveDataProcessor.Reset();
        }

        private bool CanOpen()
        {
            return (firstDeal && secondDeal);
        }

        public int Rand6Int()
        {
            int rtn = 0;
            Random r = new Random();
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iSeed = BitConverter.ToInt32(buffer, 0);
            r = new Random(iSeed);
            rtn = r.Next(100000, 999999);
            return rtn;
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
                    loseArr.Add(loseCount);
                    currentDay = tickdata.Time.Date;
                    this.Reset();
                }
                currentTick = tickdata;
                SecondTick = CurrentStockData.GetTick(SecondSi);
                liveDataProcessor.ReceiveTick(tickdata);

                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {
                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        if (CanOpen())
                        {
                            if (Math.Abs((tickdata.Time - SecondTick.Time).Seconds) <= 1)
                            {
                                double a = tickdata.Bid - SecondTick.Ask;
                                double b = (tickdata.Bid * parameter.Fee + SecondTick.Ask * parameter.SecondFee + tickdata.Last * parameter.Lirun);
                                double c = SecondTick.Bid - tickdata.Ask;
                                double d = (tickdata.Ask * parameter.Fee + SecondTick.Bid * parameter.SecondFee + tickdata.Last * parameter.Lirun);
                                List<CurrentBanalce> ACBS = CurrentBalances.getCurrentBalance(SecInfo.Market);
                                List<CurrentBanalce> BCBS = CurrentBalances.getCurrentBalance(parameter.SecondMarket);
                                foreach (CurrentBanalce aCBS in ACBS)
                                {
                                    if (aCBS.Code.ToUpper() == parameter.Code.ToUpper() || aCBS.Code.ToUpper() == parameter.Code.ToUpper() + "-SPOT")
                                    {
                                        if (aCBS.Ava > parameter.Qty * 2)
                                        {
                                            Code1AtoB = true;
                                        }
                                        else
                                        {
                                            Code1AtoB = false;
                                        }
                                    }
                                    if (aCBS.Code.ToUpper() == parameter.Code2.ToUpper() || aCBS.Code.ToUpper() == parameter.Code2.ToUpper() + "-SPOT")
                                    {
                                        if (aCBS.Ava > parameter.Qty * tickdata.Last * 2)
                                        {
                                            Code2BtoA = true;
                                        }
                                        else
                                        {
                                            Code2BtoA = false;
                                        }
                                    }
                                }
                                foreach (CurrentBanalce bCBS in BCBS)
                                {
                                    if (bCBS.Code.ToUpper() == parameter.Code.ToUpper() || bCBS.Code.ToUpper() == parameter.Code.ToUpper() + "-SPOT")
                                    {
                                        if (bCBS.Ava > parameter.Qty * 2)
                                        {
                                            Code1BtoA = true;
                                        }
                                        else
                                        {
                                            Code1BtoA = false;
                                        }
                                    }
                                    if (bCBS.Code.ToUpper() == parameter.Code2.ToUpper() || bCBS.Code.ToUpper() == parameter.Code2.ToUpper() + "-SPOT")
                                    {
                                        if (bCBS.Ava > parameter.Qty * tickdata.Last * 2)
                                        {
                                            Code2AtoB = true;
                                        }
                                        else
                                        {
                                            Code2AtoB = false;
                                        }
                                    }
                                }
                                if (a > b)
                                {
                                    if (Code1AtoB && Code2AtoB)
                                    {
                                        firstDeal = false;
                                        secondDeal = false;
                                        string tr = DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
                                        OpenArgs oa = new OpenArgs(
                                                    tickdata.Bid - parameter.HuaDian,
                                                    parameter.qty,
                                                    OpenType.Sell,
                                                    SecInfo,
                                                    tickdata,
                                                    tr);
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                        OpenArgs oa2 = new OpenArgs(
                                                    SecondTick.Ask + parameter.HuaDian,
                                                    parameter.qty,
                                                    OpenType.Buy,
                                                    SecondSi,
                                                    SecondTick,
                                                    tr);
                                        OpenDelete od2 = new OpenDelete(OpenThread);
                                        od2.BeginInvoke(oa2, null, null);
                                    }
                                    else
                                    {
                                        loseCount += 1;
                                    }
                                }
                                if (c > d)
                                {
                                    if (Code1BtoA && Code2BtoA)
                                    {
                                        firstDeal = false;
                                        secondDeal = false;
                                        string tr = DateTime.Now.ToString("yyyyMMddHHmmss") + Rand6Int().ToString();
                                        OpenArgs oa = new OpenArgs(
                                                    tickdata.Ask + parameter.HuaDian,
                                                    parameter.qty,
                                                    OpenType.Buy,
                                                    SecInfo,
                                                    tickdata,
                                                    tr);
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                        OpenArgs oa2 = new OpenArgs(
                                                    SecondTick.Bid - parameter.HuaDian,
                                                    parameter.qty,
                                                    OpenType.Sell,
                                                    SecondSi,
                                                    SecondTick,
                                                    tr);
                                        OpenDelete od2 = new OpenDelete(OpenThread);
                                        od2.BeginInvoke(oa2, null, null);
                                    }
                                    else
                                    {
                                        loseCount += 1;
                                    }
                                }
                            }
                        }
                    }
                }
                LiveDataUpdate(tickdata);
            }
        }

        delegate void OpenDelete(OpenArgs oa);
        private void OpenThread(OpenArgs oa)
        {
            Open(oa.Price, oa.Qty, oa.Opentype, oa.Si, oa.Tickdata, oa.Tr);
        }

        private void Open(double price, double qty, OpenType ot, SecurityInfo si, TickData td, string tr)
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
            op.FirstTradePriceType = parameter.EnterOrderPriceType;
            op.CancelLimitTime = parameter.EnterOrderWaitSecond;
            op.ReEnterPecentage = parameter.ReEnterPercent;

            TradePoints tp = new TradePoints(op, 0);
            tp.Fee = parameter.Fee;
            tp.TpRemark = tr;
            /////////////add//////////////////////
            tp.IsReal = arg.IsReal;
            this.tps.TryAdd(tp.TradeGuid, tp);
            arg.PairePoint = tp;
            arg.PolicyObject = this;
            arg.Tickdata = td;
            RaiseResult(arg);
        }

        public override CheDanArgs PolicyCheDan(KeCheDetail kcd)
        {
            CheDanArgs cda = new CheDanArgs();
            cda.Tsopt = TradeSendOrderPriceType.Limit;
            TickData tick = CurrentStockData.GetTick(kcd.Si);
            if (kcd.UnDealQty >= kcd.Si.MinQty)
            {
                if (kcd.OverTime())
                {
                    if (kcd.OpenType == OpenType.Buy)
                    {
                        decimal newPrice = (decimal)(Math.Floor(tick.Ask / SecInfo.PriceJingDu) * SecInfo.PriceJingDu) + (decimal)parameter.HuaDian;
                        if ((decimal)kcd.OrderPrice < newPrice)
                        {
                            cda.Cancel = true;
                            cda.ZhuiDan = true;
                            cda.NewOrderPrice = (double)newPrice;
                        }
                    }
                    else if (kcd.OpenType == OpenType.Sell)
                    {
                        decimal newPrice = (decimal)(Math.Floor(tick.Bid / SecInfo.PriceJingDu) * SecInfo.PriceJingDu) - (decimal)parameter.HuaDian;
                        if ((decimal)kcd.OrderPrice > newPrice)
                        {
                            cda.Cancel = true;
                            cda.ZhuiDan = true;
                            cda.NewOrderPrice = (double)newPrice;
                        }
                    }
                }
            }
            else
            {
                if (kcd.Si.Key == SecInfo.Key)
                {
                    firstDeal = true;
                }
                if (kcd.Si.Key == SecondSi.Key)
                {
                    secondDeal = true;
                }
            }

            return cda;
        }

        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
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
            if (tps.ContainsKey(tradeGuid))
            {
                TradePoints t = tps[tradeGuid];
                if (t.TradeGuid == tradeGuid)
                {
                    if (status == OpenStatus.Opened)
                    {
                        if (t.EnterPoint.SecInfo.Key == SecInfo.Key)
                        {
                            firstDeal = true;
                        }
                        if (t.EnterPoint.SecInfo.Key == SecondSi.Key)
                        {
                            secondDeal = true;
                        }
                        t.Status = OpenStatus.Opened;
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓通知，成交数量:{1}", this.policyName, dealQty)));
                    }
                    else if (status == OpenStatus.Open)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓下单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.OpenPending)
                    {
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收入场追单通知", this.policyName)));
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
                    else if (status == OpenStatus.OpenRest)
                    {
                        if (t.EnterPoint.SecInfo.Key == SecInfo.Key)
                        {
                            firstDeal = true;
                        }
                        if (t.EnterPoint.SecInfo.Key == SecondSi.Key)
                        {
                            secondDeal = true;
                        }
                        if (t.EnterPoint.DealQty == 0)
                        {
                            t.Finished = true;
                        }
                        RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                    }
                    else if (status == OpenStatus.OpenCanceled)
                    {
                        if (t.EnterPoint.SecInfo.Key == SecInfo.Key)
                        {
                            double openPrice = 0;
                            if (t.EnterPoint.OpenType == OpenType.Buy)
                            {
                                openPrice = currentTick.Ask;
                            }
                            else
                            {
                                openPrice = currentTick.Bid;
                            }
                            OpenArgs oa = new OpenArgs(
                                                openPrice,
                                                parameter.qty,
                                                t.EnterPoint.OpenType,
                                                SecInfo,
                                                currentTick,
                                                t.TpRemark);
                            OpenDelete od = new OpenDelete(OpenThread);
                            od.BeginInvoke(oa, null, null);
                        }
                        if (t.EnterPoint.SecInfo.Key == SecondSi.Key)
                        {
                            double openPrice = 0;
                            if (t.EnterPoint.OpenType == OpenType.Buy)
                            {
                                openPrice = SecondTick.Ask;
                            }
                            else
                            {
                                openPrice = SecondTick.Bid;
                            }
                            OpenArgs oa = new OpenArgs(
                                                openPrice,
                                                parameter.qty,
                                                t.EnterPoint.OpenType,
                                                SecondSi,
                                                SecondTick,
                                                t.TpRemark);
                            OpenDelete od = new OpenDelete(OpenThread);
                            od.BeginInvoke(oa, null, null);
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
            liveDataProcessor = new LiveDataProcessor(intevals, this.SecInfo, DateTime.Now);
            liveDataProcessor.OnLiveBarArrival += liveDataProcessor_OnLiveBarArrival;
            this.isLiveDataProcessor = true;
        }
    }
}
