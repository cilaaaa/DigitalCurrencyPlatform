using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace PolicyBtc0601
{   
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
        Decimal frozenBuyHands;
        Decimal frozenSellHands;
        List<double> MomList;
        List<double> MomAbsList;
        List<DieBar> DieBars;
        SecurityInfo eosusdtsi;
        SecurityInfo eosbtcsi;
        TickData eosusdt;
        TickData eosbtc;
        Stopwatch sw;
        bool test;
        List<OpenArgs> openbtcusdtargsQueue;
        List<OpenArgs> openeosusdtargsQueue;
        Thread btcusdtThread;
        Thread eosbtcThread;

        public static string PName
        {
            get { return "PolicyBtc0601"; }
        }

        object lockObject;

        public Policy(SecurityInfo si,Parameter rpp,PolicyProperties pp)
        {
            this.isSim = pp.IsSim;
            this.SecInfo = si;
            this.stockAccount = pp.Account;
            this.parameter = rpp;
            this.policyName = string.Format("{0}%{1}%{2}%{3}",PName,si.Code,parameter.Bz1, parameter.Bz2);
            this.startDate = rpp.StartDate;
            this.endDate = rpp.EndDate;
            this.inteval = rpp.Inteval;
            this.isReal = rpp.IsReal;
            this.policyguid = Guid.NewGuid();
            
            initialDataReceiver();
            InitialDataProcessor();
            currentDay = DateTime.MinValue.Date;
            openPoints = new List<OpenPoint>();
            IsSimulateFinished = false;
            //isOpened = false;
            if(rpp.save)
            {
                SaveParameter(rpp);
            }

            currentTick = new TickData();
            lockObject = new object();
            this.isLiveDataProcessor = true;
            MomList = new List<double>();
            MomAbsList = new List<double>();
            DieBars = new List<DieBar>();
            tps = new List<TradePoints>();
            lock_tps = new object();
            eosusdtsi = GlobalValue.GetFutureByCodeAndMarket(parameter.Bz1, si.Market);
            eosbtcsi = GlobalValue.GetFutureByCodeAndMarket(parameter.Bz2, si.Market);
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
            DateTime lastday = currentDay;
            lastday = lastday.AddDays(-1);
            openPoints = new List<OpenPoint>();
        }

        bool EosUsdtStatus = true;
        bool EosBtcStatus = true;
        bool BtcUsdtStatus = true;

        private bool CanOpen()
        {
            if (EosBtcStatus && EosUsdtStatus && BtcUsdtStatus)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                eosusdt = CurrentStockData.GetTick(eosusdtsi);
                eosbtc = CurrentStockData.GetTick(eosbtcsi);

                liveDataProcessor.ReceiveTick(tickdata);
                if (this.SecInfo.isLive(tickdata.Time.TimeOfDay))
                {
                    if (IsSimulateFinished || (!IsSimulateFinished && !tickdata.IsReal))
                    {
                        if (Math.Abs((tickdata.Time - eosbtc.Time).Seconds) <= 1 && Math.Abs((tickdata.Time - eosusdt.Time).Seconds) <= 1 && Math.Abs((eosusdt.Time - eosbtc.Time).Seconds) <= 1)
                        {
                            if (CanOpen())
                            {
                                double a = eosusdt.Bid - tickdata.Ask * eosbtc.Ask;
                                double b = tickdata.Bid * eosbtc.Bid - eosusdt.Ask;
                                double ChengBenAndLiRun = (eosusdt.Last) * (0.0006 + 0.0002);
                                if (a > ChengBenAndLiRun)
                                {
                                    Console.WriteLine(string.Format("{3}-a开仓,时间：{0},价格A：{1}，成本C：{2}", DateTime.Now.ToLongTimeString(), a, ChengBenAndLiRun, SecInfo.Code + "%" + eosusdtsi.Code + "%" + eosbtc.Code));
                                    //EosBtcStatus = false;
                                    //OpenArgs oa = new OpenArgs(
                                    //            eosbtc.Ask,
                                    //            parameter.qty,
                                    //            OpenType.Buy,
                                    //            eosbtcsi,
                                    //            eosbtc,
                                    //            "");
                                    //OpenDelete od = new OpenDelete(OpenThread);
                                    //od.BeginInvoke(oa, null, null);
                                }
                                if (b > ChengBenAndLiRun)
                                {
                                    Console.WriteLine(string.Format("{3}-b开仓,时间：{0},价格B：{1}，成本C：{2}", DateTime.Now.ToLongTimeString(), b, ChengBenAndLiRun, SecInfo.Code + "%" + eosusdtsi.Code + "%" + eosbtc.Code));
                                    //EosBtcStatus = false;
                                    //OpenArgs oa = new OpenArgs(
                                    //            eosbtc.Bid,
                                    //            parameter.qty,
                                    //            OpenType.Sell,
                                    //            eosbtcsi,
                                    //            eosbtc,
                                    //            "");
                                    //OpenDelete od = new OpenDelete(OpenThread);
                                    //od.BeginInvoke(oa, null, null);
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
            Open(oa.Price, oa.Qty, oa.Opentype, oa.Si, oa.Tickdata, oa.Tr);
        }

        private void Open(double price, double qty, OpenType ot, SecurityInfo si, TickData td,string _tr = "")
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
            lock (lock_tps)
            {
                this.tps.Add(tp);
            }
            arg.PairePoint = tp;
            arg.Tickdata = currentTick;
            RaiseResult(arg);
        }

        public override string getBackTestTitle()
        {
            StringBuilder stb = new StringBuilder();
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
            
        }

        public override CheDanArgs PolicyCheDan(KeCheDetail kcd)
        {
            CheDanArgs cda = new CheDanArgs();
            TickData tick = CurrentStockData.GetTick(kcd.Si);
            if (kcd.OverTime())
            {
                if (kcd.OpenType == OpenType.Buy)
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = true;
                    cda.NewOrderPrice = tick.Ask;
                }
                else
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = true;
                    cda.NewOrderPrice = tick.Bid;
                }
            }
            return cda;
        }

        public override void Notify(Guid tradeGuid, OpenStatus status, double dealQty = 0, double dealPrice = 0, string weituobianhao = "", string pendWeituobianhao = "")
        {
            lock (lock_tps)
            {
                for (int i = 0;i<tps.ToArray().Count();i++)
                {
                    var t = tps[i];
                    if (t.TradeGuid == tradeGuid)
                    {
                        if (status == OpenStatus.Opened)
                        {
                            t.EnterPoint.OpenPrice = dealPrice;
                            decimal realDealQty = (decimal)dealQty + (decimal)t.EnterPoint.PartDealQty - (decimal)t.EnterPoint.DealQty;
                            holdHands += realDealQty;
                            t.Status = OpenStatus.Opened;
                            frozenBuyHands -= realDealQty;
                            t.EnterPoint.DealQty = (double)((decimal)dealQty + (decimal)t.EnterPoint.PartDealQty);
                            RaiseMessage(new PolicyMessageEventArgs("策略已接受开仓通知"));
                            if (t.EnterPoint.SecInfo.Key == eosbtcsi.Key)
                            {
                                double huanbiqty = Math.Floor(dealPrice * (double)realDealQty * SecInfo.JingDu) / SecInfo.JingDu;
                                double eosqty = Math.Floor((double)realDealQty * eosusdtsi.JingDu) / eosusdtsi.JingDu;
                                if (t.EnterPoint.OpenType == OpenType.Buy)
                                {
                                    if ((double)realDealQty > eosusdtsi.MinQty)
                                    {
                                        OpenArgs oa = new OpenArgs(
                                        eosusdt.Bid,
                                        eosqty,
                                        OpenType.Sell,
                                        eosusdtsi,
                                        eosusdt,
                                        t.TpRemark);
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                        EosUsdtStatus = false;
                                    }
                                    if (huanbiqty >= SecInfo.MinQty)
                                    {
                                        OpenArgs oa2 = new OpenArgs(
                                        currentTick.Ask,
                                        huanbiqty,
                                        OpenType.Buy,
                                        SecInfo,
                                        currentTick,
                                        t.TpRemark);
                                        OpenDelete od2 = new OpenDelete(OpenThread);
                                        od2.BeginInvoke(oa2, null, null);
                                        BtcUsdtStatus = false;
                                    }
                                }
                                else
                                {
                                    if ((double)realDealQty > eosusdtsi.MinQty)
                                    {
                                        OpenArgs oa = new OpenArgs(
                                            eosusdt.Ask,
                                            eosqty,
                                            OpenType.Buy,
                                            eosusdtsi,
                                            eosusdt,
                                            t.TpRemark);
                                        OpenDelete od = new OpenDelete(OpenThread);
                                        od.BeginInvoke(oa, null, null);
                                        EosUsdtStatus = false;
                                    }
                                    if (huanbiqty >= SecInfo.MinQty)
                                    {
                                        OpenArgs oa2 = new OpenArgs(
                                        currentTick.Bid,
                                        huanbiqty,
                                        OpenType.Sell,
                                        SecInfo,
                                        currentTick,
                                        t.TpRemark);
                                        OpenDelete od2 = new OpenDelete(OpenThread);
                                        od2.BeginInvoke(oa2, null, null);
                                        BtcUsdtStatus = false;
                                    }
                                }

                                EosBtcStatus = true;
                            }
                            else if (t.EnterPoint.SecInfo.Key == eosusdtsi.Key)
                            {
                                EosUsdtStatus = true;
                            }
                            else if (t.EnterPoint.SecInfo.Key == SecInfo.Key)
                            {
                                BtcUsdtStatus = true;
                            }
                        }
                        else if (status == OpenStatus.Open)
                        {
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受开仓下单通知", this.policyName)));
                        }
                        else if (status == OpenStatus.Close)
                        {
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓下单通知", this.policyName)));
                        }
                        else if (status == OpenStatus.Closed)
                        {
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受平仓通知", this.policyName)));
                        }
                        else if (status == OpenStatus.OpenPending)
                        {
                            t.EnterPoint.PartDealQty = t.EnterPoint.DealQty;
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略HoldHands:{1}", this.policyName, holdHands)));
                        }
                        else if (status == OpenStatus.ClosePending)
                        {
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
                                holdHands += realDealQty;
                                frozenBuyHands -= realDealQty;
                                t.EnterPoint.DealQty = dealQty + t.EnterPoint.PartDealQty;
                                if (t.EnterPoint.SecInfo.Key == eosbtcsi.Key)
                                {
                                    double huanbiqty = Math.Floor(dealPrice * (double)realDealQty * SecInfo.JingDu) / SecInfo.JingDu;
                                    double eosqty = Math.Floor((double)realDealQty * eosusdtsi.JingDu) / eosusdtsi.JingDu;
                                    if (t.EnterPoint.OpenType == OpenType.Buy)
                                    {
                                        if ((double)realDealQty > eosusdtsi.MinQty)
                                        {
                                            OpenArgs oa = new OpenArgs(
                                            eosusdt.Bid,
                                            eosqty,
                                            OpenType.Sell,
                                            eosusdtsi,
                                            eosusdt,
                                            t.TpRemark);
                                            OpenDelete od = new OpenDelete(OpenThread);
                                            od.BeginInvoke(oa, null, null);
                                            EosUsdtStatus = false;
                                        }
                                        if (huanbiqty >= SecInfo.MinQty)
                                        {
                                            OpenArgs oa2 = new OpenArgs(
                                            currentTick.Ask,
                                            huanbiqty,
                                            OpenType.Buy,
                                            SecInfo,
                                            currentTick,
                                            t.TpRemark);
                                            OpenDelete od2 = new OpenDelete(OpenThread);
                                            od2.BeginInvoke(oa2, null, null);
                                            BtcUsdtStatus = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((double)realDealQty > eosusdtsi.MinQty)
                                        {
                                            OpenArgs oa = new OpenArgs(
                                                eosusdt.Ask,
                                                eosqty,
                                                OpenType.Buy,
                                                eosusdtsi,
                                                eosusdt,
                                                t.TpRemark);
                                            OpenDelete od = new OpenDelete(OpenThread);
                                            od.BeginInvoke(oa, null, null);
                                            EosUsdtStatus = false;
                                        }
                                        if (huanbiqty >= SecInfo.MinQty)
                                        {
                                            OpenArgs oa2 = new OpenArgs(
                                            currentTick.Bid,
                                            huanbiqty,
                                            OpenType.Sell,
                                            SecInfo,
                                            currentTick,
                                            t.TpRemark);
                                            OpenDelete od2 = new OpenDelete(OpenThread);
                                            od2.BeginInvoke(oa2, null, null);
                                            BtcUsdtStatus = false;
                                        }
                                    }
                                    EosBtcStatus = true;
                                }
                                RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分入场成交通知，成交数量:{1}", this.policyName, dealQty)));
                            }
                        }
                        else if (status == OpenStatus.PartClosed)
                        {
                            if (dealQty > (t.OutPoint.DealQty - t.OutPoint.PartDealQty))
                            {
                                RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受部分出场成交通知，成交数量:{1}", this.policyName, dealQty)));
                                decimal realDealQty = (decimal)dealQty + (decimal)t.OutPoint.PartDealQty - (decimal)t.OutPoint.DealQty;
                                holdHands -= realDealQty;
                                frozenSellHands -= realDealQty;
                                t.OutPoint.DealQty = dealQty + t.OutPoint.PartDealQty;
                            }
                        }
                        else if (status == OpenStatus.OpenCanceled)
                        {
                            if (t.EnterPoint.SecInfo.Key == eosbtcsi.Key)
                            {
                                EosBtcStatus = true;
                            }
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受入场撤单通知", this.policyName)));
                        }
                        else if (status == OpenStatus.CloseCanceled)
                        {
                            frozenSellHands -= (decimal)dealQty;
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接受出场撤单通知", this.policyName)));
                        }
                        else if (status == OpenStatus.OpenRest)
                        {
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收开仓剩余通知,数量：{1}", this.policyName, dealQty)));
                        }
                        else if (status == OpenStatus.CloseRest)
                        {
                            holdHands -= (decimal)dealQty;
                            frozenSellHands -= (decimal)dealQty;
                            RaiseMessage(new PolicyMessageEventArgs(string.Format("{0}-策略已接收平仓剩余通知,数量：{1}", this.policyName, dealQty)));
                        }

                    }
                }
            }
        }

        public override void InitialDataProcessor()
        {
            //this.marketTimeRange = MarketTimeRange.getTimeRange(this.SecInfo.Market1);
            List<int> intevals = new List<int>();
            liveDataProcessor = new LiveDataProcessor(intevals, this.SecInfo,DateTime.Now);
            liveDataProcessor.OnLiveBarArrival += liveDataProcessor_OnLiveBarArrival;
            this.isLiveDataProcessor = true;
        }
    }  
}
