using StockData;
using System;

namespace StockTradeAPI
{
    public class WeiTuoMonitor
    {
        //static List<WeiTuoMonitor> allWeiTuo = new List<WeiTuoMonitor>();
        //public static bool isRunning = false;
        //public static List<WeiTuoMonitor> AllWeiTuo
        //{
        //    get { return WeiTuoMonitor.allWeiTuo; }
        //}
        string bianHao;

        public string BianHao
        {
            get { return bianHao; }
            set { bianHao = value; }
        }

        TimeSpan createTime;

        public TimeSpan CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        int waitSecond;

        public int WaitSecond
        {
            get { return waitSecond; }
            set { waitSecond = value; }
        }

        string exchangeID;

        public string ExchangeID
        {
            get { return exchangeID; }
            set { exchangeID = value; }
        }

        PolicyTradeType tradeType;

        public PolicyTradeType TradeType
        {
            get { return tradeType; }
            set { tradeType = value; }
        }

        bool dealed;

        public bool Dealed
        {
            get { return dealed; }
            set { dealed = value; }
        }

        bool canCancel;

        public bool CanCancel
        {
            get { return canCancel; }
            set { canCancel = value; }
        }

        int unDealedQty;

        public int UnDealedQty
        {
            get { return unDealedQty; }
            set { unDealedQty = value; }
        }

        double reEnterPercentage;

        public double ReEnterPercentage
        {
            get { return reEnterPercentage; }
            set { reEnterPercentage = value; }
        }

        bool monitored;

        public bool Monitored
        {
            get { return monitored; }
            set { monitored = value; }
        }

        public SecurityInfo SI;

        double orderPrice;

        public double OrderPrice
        {
            get { return orderPrice; }
            set { orderPrice = value; }
        }

        OpenType openType;

        public OpenType OpenType
        {
            get { return openType; }
            set { openType = value; }
        }

        Guid tradeGuid;

        public Guid TradeGuid
        {
            get { return tradeGuid; }
            set { tradeGuid = value; }
        }


        Object policyObject;

        public Object PolicyObject
        {
            get { return policyObject; }
            set { policyObject = value; }
        }

        bool notified;

        public bool Notified1
        {
            get { return notified; }
            set { notified = value; }
        }

        int orderQTY;

        public int OrderQTY
        {
            get { return orderQTY; }
            set { orderQTY = value; }
        }

        bool forceToCancel;

        public bool ForceToCancel
        {
            get { return forceToCancel; }
            set { forceToCancel = value; }
        }
        public bool Notified
        {
            get { return notified; }
            set { notified = value; }
        }
        //public static void AddNewWeiTuo(string bianhao, int cancelLimitTime, double reEnterPercentage, PolicyTradeType tradetype, SecurityInfo si, double orderprice, int orderQty, OpenType opentype, Guid tradeguid, Object runningPolicy)
        //{
        //    WeiTuoMonitor wtm = new WeiTuoMonitor();
        //    wtm.bianHao = bianhao;
        //    wtm.createTime = System.DateTime.Now.TimeOfDay;
        //    wtm.waitSecond = cancelLimitTime;
        //    wtm.tradeType = tradetype;
        //    if (tradetype == PolicyTradeType.Open)
        //    {
        //        if (cancelLimitTime == 0)
        //        {
        //            wtm.canCancel = false;
        //        }
        //        else
        //        {
        //            wtm.canCancel = true;
        //        }
        //    }
        //    else
        //    {
        //        wtm.canCancel = false;
        //    }
        //    wtm.SI = si;
        //    wtm.unDealedQty = orderQty;
        //    wtm.dealed = false;
        //    wtm.reEnterPercentage = reEnterPercentage;
        //    wtm.monitored = false;
        //    wtm.orderPrice = orderprice;
        //    wtm.openType = opentype;
        //    wtm.tradeGuid = tradeguid;
        //    wtm.policyObject = runningPolicy;
        //    wtm.notified = false;
        //    wtm.orderQTY = orderQty;
        //    wtm.forceToCancel = false;
        //    allWeiTuo.Add(wtm);
        //}

        //public static void AddNewWeiTuoWithMonitored(string bianhao, int cancelLimitTime, double reEnterPercentage, PolicyTradeType tradetype, SecurityInfo si, double orderprice, int orderQty, OpenType opentype, Guid tradeguid, Object runningPolicy)
        //{
        //    WeiTuoMonitor wtm = new WeiTuoMonitor();
        //    wtm.bianHao = bianhao;
        //    wtm.createTime = System.DateTime.Now.TimeOfDay;
        //    wtm.waitSecond = cancelLimitTime;
        //    wtm.tradeType = tradetype;
        //    if (tradetype == PolicyTradeType.Open)
        //    {
        //        if (cancelLimitTime == 0)
        //        {
        //            wtm.canCancel = false;
        //        }
        //        else
        //        {
        //            wtm.canCancel = true;
        //        }
        //    }
        //    else
        //    {
        //        wtm.canCancel = false;
        //    }
        //    wtm.SI = si;
        //    wtm.unDealedQty = orderQty;
        //    wtm.dealed = false;
        //    wtm.reEnterPercentage = reEnterPercentage;
        //    wtm.monitored = true;
        //    wtm.orderPrice = orderprice;
        //    wtm.openType = opentype;
        //    wtm.tradeGuid = tradeguid;
        //    wtm.policyObject = runningPolicy;
        //    wtm.notified = false;
        //    wtm.orderQTY = orderQty;
        //    allWeiTuo.Add(wtm);
        //}





        internal void Action()
        {
            //if (!this.dealed)
            //{
            //    #region 限价单逻辑
            //    if (canCancel) //限价挂单单
            //    {
            //        if (this.forceToCancel)
            //        {
            //            StringBuilder reslt = new StringBuilder(1024 * 1024);
            //            StringBuilder errorInfo = new StringBuilder(256);
            //            if (true)
            //            ///////////////////////////////////////////////////////////////if (StockAccount.CancelOrder(this.exchangeID, this.bianHao, reslt, errorInfo)) //撤单
            //            {
            //                TradeLog.Log(string.Format("委托单{0}自动撤单成功,代码{1}", bianHao, SI.Code));
            //                this.dealed = true;
            //            }
            //            else
            //            {
            //                TradeLog.Log(string.Format("委托单{0}撤单失败:{1}", bianHao, errorInfo.ToString()));
            //            }
            //            return;
            //        }
            //        if (unDealedQty != orderQTY && !notified)
            //        {
            //            policyObject.Notify(this.tradeGuid, OpenStatus.Opened);
            //            notified = true;
            //        }
            //        if (unDealedQty == 0)
            //        {
            //            this.dealed = true;
            //            return;
            //        }
            //        if (this.isOverTime() && !forceToCancel) //已超时
            //        {
            //            //撤单
            //            StringBuilder reslt = new StringBuilder(1024 * 1024);
            //            StringBuilder errorInfo = new StringBuilder(256);
            //            if (true)
            //            /////////////////////////////////////////////////////////if (StockAccount.CancelOrder(this.exchangeID, this.bianHao, reslt, errorInfo)) //撤单
            //            {
            //                this.dealed = true;
            //                TradeLog.Log(string.Format("委托单{0}自动撤单成功,代码{1}", bianHao, SI.Code));
            //                //追单
            //                TickData tick = CurrentStockData.GetTick(this.SI);
            //                //double currentPrice = CurrentStockData.GetTick(this.SI).Last;
            //                if (tick.Code == string.Empty)
            //                {
            //                    TradeLog.Log(string.Format("找不到{0}的当前价格数据，放弃追单", SI.Code));
            //                    if (!notified)
            //                    {
            //                        this.policyObject.Notify(this.tradeGuid, OpenStatus.Failed);
            //                        notified = true;
            //                    }
            //                }
            //                else
            //                {
            //                    double lastclose = tick.Preclose;
            //                    if (this.openType == OpenType.Buy)
            //                    {
            //                        double current = tick.Asks[0];
            //                        if (((current - orderPrice) / lastclose) > 0.005)
            //                        {
            //                            TradeLog.Log(string.Format("{0}现价超过开仓价0.5%，放弃追单", SI.Code));
            //                            if (!notified)
            //                            {
            //                                this.policyObject.Notify(this.tradeGuid, OpenStatus.Failed);
            //                                notified = true;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //追单
            //                            string weituobianhao = string.Empty;
            //                            string errinfo = string.Empty;

            //                            //if (TradeHistory.XiaDan(args.PairePoint.EnterPoint.OpenType, args.PairePoint.EnterPoint.SecInfo, args.PairePoint.EnterPoint.OpenPriceType, (float)args.PairePoint.EnterPoint.OpenPrice, orderQty, ref weituobianhao, ref errorinfo))
            //                            if (TradeHistory.XiaDan(this.openType, this.SI, TradeSendOrderPriceType.ShiJiaWeiTuo_SH_SZ_WuDangJiChenShenChe, (float)current, unDealedQty, ref weituobianhao, ref errinfo))
            //                            {
            //                                //string weituobianhao = "testopen";
            //                                //if(true)
            //                                //{ 
            //                                TradeDetail td = new TradeDetail();
            //                                TradeLog.Log(string.Format("{0}追单委托成功，数量{1}", SI.Code, this.unDealedQty));
            //                                if (TradeHistory.getTradeDetail(this.tradeGuid, ref td))
            //                                {
            //                                    td.OpenWeiTuo.Add(new TradeWeiTuo(weituobianhao, current, unDealedQty, this.openType));
            //                                }
            //                                else
            //                                {
            //                                    TradeLog.Log(string.Format("错误，{0}找不到持仓明细数据", SI.Code));

            //                                }
            //                                if (!notified)
            //                                {
            //                                    this.policyObject.Notify(this.tradeGuid, OpenStatus.Opened);
            //                                    notified = true;
            //                                }
            //                                //td.PoicyName = args.PolicyName1;
            //                                //td.Tradeid = args.PairePoint.TradeGuid;
            //                                //td.TradeOpenOrderQty = orderQty;
            //                                //td.TradeSi = args.PairePoint.EnterPoint.SecInfo;
            //                                //td.PolicyOpenPoint = args.PairePoint.EnterPoint;
            //                                //td.TradeOpenOrderPrice = args.PairePoint.EnterPoint.OpenPrice;
            //                                //td.TradeOpenOrderType = args.PairePoint.EnterPoint.OpenType;
            //                                //td.OpenWeiTuo.Add(new TradeWeiTuo(weituobianhao, args.PairePoint.EnterPoint.OpenPrice, orderQty, args.PairePoint.EnterPoint.OpenType));
            //                                //td.PoicyFee = args.PairePoint.Fee;
            //                                //TradeHistory.Add(td);
            //                                //if (args.PairePoint.EnterPoint.CancelLimitTime != 0)
            //                                //{
            //                                //    WeiTuoMonitor.AddNewWeiTuo(weituobianhao, args.PairePoint.EnterPoint.CancelLimitTime, args.PairePoint.EnterPoint.ReEnterPecentage, PolicyTradeType.Open, args.SecInfo, args.PairePoint.EnterPoint.OpenPrice, args.PairePoint.EnterPoint.OpenType);
            //                                //}

            //                            }
            //                            else
            //                            {
            //                                TradeLog.Log(string.Format("错误，{0}下单失败", SI.Code));
            //                                if (!notified)
            //                                {
            //                                    this.policyObject.Notify(this.tradeGuid, OpenStatus.Failed);
            //                                    notified = true;
            //                                }
            //                            }

            //                        }
            //                    }
            //                    //else
            //                    //{
            //                    //    double current = tick.Bids[0];
            //                    //    if(((orderPrice - current) / lastclose) > this.reEnterPercentage)
            //                    //    {
            //                    //        TradeLog.Log(string.Format("{0}现价超过开仓价0.5%，放弃追单", SI.Code));
            //                    //        this.policy.Notify(this.tradeGuid, OpenStatus.Failed);
            //                    //    }
            //                    //    else
            //                    //    {
            //                    //        //追单
            //                    //        string weituobianhao = string.Empty;
            //                    //        string errinfo = string.Empty;

            //                    //        //if (TradeHistory.XiaDan(args.PairePoint.EnterPoint.OpenType, args.PairePoint.EnterPoint.SecInfo, args.PairePoint.EnterPoint.OpenPriceType, (float)args.PairePoint.EnterPoint.OpenPrice, orderQty, ref weituobianhao, ref errorinfo))
            //                    //        if (TradeHistory.XiaDan(this.openType, this.SI, TradeSendOrderPriceType.ShiJiaWeiTuo_SH_SZ_WuDangJiChenShenChe, (float)current, unDealedQty, ref weituobianhao, ref errinfo))
            //                    //        {
            //                    //            //string weituobianhao = "testopen";
            //                    //            //if(true)
            //                    //            //{ 
            //                    //            TradeDetail td = new TradeDetail();
            //                    //            TradeLog.Log(string.Format("{0}追单委托成功，数量{1}", SI.Code, this.unDealedQty));
            //                    //            if (TradeHistory.getTradeDetail(this.tradeGuid, ref td))
            //                    //            {
            //                    //                td.OpenWeiTuo.Add(new TradeWeiTuo(weituobianhao, current, unDealedQty, this.openType));
            //                    //            }
            //                    //            else
            //                    //            {
            //                    //                TradeLog.Log(string.Format("错误，{0}找不到持仓明细数据", SI.Code));
            //                    //            }
            //                    //            this.policy.Notify(this.tradeGuid, OpenStatus.Opened);
            //                    //            //td.PoicyName = args.PolicyName1;
            //                    //            //td.Tradeid = args.PairePoint.TradeGuid;
            //                    //            //td.TradeOpenOrderQty = orderQty;
            //                    //            //td.TradeSi = args.PairePoint.EnterPoint.SecInfo;
            //                    //            //td.PolicyOpenPoint = args.PairePoint.EnterPoint;
            //                    //            //td.TradeOpenOrderPrice = args.PairePoint.EnterPoint.OpenPrice;
            //                    //            //td.TradeOpenOrderType = args.PairePoint.EnterPoint.OpenType;
            //                    //            //td.OpenWeiTuo.Add(new TradeWeiTuo(weituobianhao, args.PairePoint.EnterPoint.OpenPrice, orderQty, args.PairePoint.EnterPoint.OpenType));
            //                    //            //td.PoicyFee = args.PairePoint.Fee;
            //                    //            //TradeHistory.Add(td);
            //                    //            //if (args.PairePoint.EnterPoint.CancelLimitTime != 0)
            //                    //            //{
            //                    //            //    WeiTuoMonitor.AddNewWeiTuo(weituobianhao, args.PairePoint.EnterPoint.CancelLimitTime, args.PairePoint.EnterPoint.ReEnterPecentage, PolicyTradeType.Open, args.SecInfo, args.PairePoint.EnterPoint.OpenPrice, args.PairePoint.EnterPoint.OpenType);
            //                    //            //}

            //                    //        }
            //                    //        else
            //                    //        {
            //                    //            TradeLog.Log(string.Format("错误，{0}下单失败", SI.Code));
            //                    //            this.policy.Notify(this.tradeGuid, OpenStatus.Failed);
            //                    //        }
            //                    //    }

            //                    //}
            //                }
            //            }
            //            else
            //            {
            //                TradeLog.Log(string.Format("委托单{0}撤单失败:{1}", bianHao, errorInfo.ToString()));
            //            }

            //        }
            //    }
            //    #endregion
            //    else //5档即时成交单
            //    #region 5档即时成交逻辑
            //    {
            //        if (this.tradeType == PolicyTradeType.Close) //出场单
            //        {
            //            string weituobianhao = string.Empty;
            //            string errinfo = string.Empty;

            //            //if (TradeHistory.XiaDan(args.PairePoint.EnterPoint.OpenType, args.PairePoint.EnterPoint.SecInfo, args.PairePoint.EnterPoint.OpenPriceType, (float)args.PairePoint.EnterPoint.OpenPrice, orderQty, ref weituobianhao, ref errorinfo))
            //            if (TradeHistory.XiaDan(this.openType, this.SI, TradeSendOrderPriceType.ShiJiaWeiTuo_SH_SZ_WuDangJiChenShenChe, (float)orderPrice, unDealedQty, ref weituobianhao, ref errinfo))
            //            {
            //                this.dealed = true;
            //                //string weituobianhao = "testopen";
            //                //if(true)
            //                //{ 
            //                TradeDetail td = new TradeDetail();
            //                TradeLog.Log(string.Format("{0}追单委托成功，数量{1}", SI.Code, this.unDealedQty));
            //                if (TradeHistory.getTradeDetail(this.tradeGuid, ref td))
            //                {
            //                    td.CloseWeiTuo.Add(new TradeWeiTuo(weituobianhao, orderPrice, unDealedQty, this.openType));
            //                    WeiTuoMonitor.AddNewWeiTuo(weituobianhao, 0, 0, PolicyTradeType.Close, SI, orderPrice, unDealedQty, openType, tradeGuid, this.policyObject);
            //                }
            //                else
            //                {
            //                    TradeLog.Log(string.Format("错误，{0}找不到持仓明细数据", SI.Code));
            //                }
            //                //td.PoicyName = args.PolicyName1;
            //                //td.Tradeid = args.PairePoint.TradeGuid;
            //                //td.TradeOpenOrderQty = orderQty;
            //                //td.TradeSi = args.PairePoint.EnterPoint.SecInfo;
            //                //td.PolicyOpenPoint = args.PairePoint.EnterPoint;
            //                //td.TradeOpenOrderPrice = args.PairePoint.EnterPoint.OpenPrice;
            //                //td.TradeOpenOrderType = args.PairePoint.EnterPoint.OpenType;
            //                //td.OpenWeiTuo.Add(new TradeWeiTuo(weituobianhao, args.PairePoint.EnterPoint.OpenPrice, orderQty, args.PairePoint.EnterPoint.OpenType));
            //                //td.PoicyFee = args.PairePoint.Fee;
            //                //TradeHistory.Add(td);
            //                //if (args.PairePoint.EnterPoint.CancelLimitTime != 0)
            //                //{
            //                //    WeiTuoMonitor.AddNewWeiTuo(weituobianhao, args.PairePoint.EnterPoint.CancelLimitTime, args.PairePoint.EnterPoint.ReEnterPecentage, PolicyTradeType.Open, args.SecInfo, args.PairePoint.EnterPoint.OpenPrice, args.PairePoint.EnterPoint.OpenType);
            //                //}

            //            }
            //            else
            //            {
            //                TradeLog.Log(string.Format("错误，{0}下单失败", SI.Code));
            //            }
            //        }
            //    }
            //    #endregion
            //}
        }

        private bool isOverTime()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (this.SI.calculateSeconds(this.createTime, now) > waitSecond)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //internal static void CancelOpenOrder(Guid tradeGuid)
        //{
        //    for (int i = 0; i < allWeiTuo.Count; i++)
        //    {
        //        WeiTuoMonitor wtm = allWeiTuo[i];
        //        if (!wtm.dealed)
        //        {
        //            if (wtm.tradeType == PolicyTradeType.Open)
        //            {
        //                if (wtm.canCancel)
        //                {
        //                    StringBuilder reslt = new StringBuilder(1024 * 1024);
        //                    StringBuilder errorInfo = new StringBuilder(256);
        //                    if (true)
        //                    //////////////////////////////////////////////////////if (StockAccount.CancelOrder(wtm.exchangeID, wtm.bianHao, reslt, errorInfo)) //撤单
        //                    {
        //                        wtm.dealed = true;
        //                    }
        //                    else
        //                    {
        //                        wtm.forceToCancel = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}

///出场单挂单 
///没有全部出场
///继续挂单