using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;

namespace StockTrade
{
    public class TradeDetail
    {
        RunningPolicy policyObject;

        public RunningPolicy PolicyObject
        {
            get { return policyObject; }
            set { policyObject = value; }
        }

        bool closed;

        public bool Closed
        {
            get { return closed; }
            set { closed = value; }
        }

        Guid tradeid;

        public Guid Tradeid
        {
            get { return tradeid; }
            set { tradeid = value; }
        }
        SecurityInfo tradeSi;

        public SecurityInfo TradeSi
        {
            get { return tradeSi; }
            set { tradeSi = value; }
        }
        double tradeOpenOrderQty;

        public double TradeOpenOrderQty
        {
            get { return tradeOpenOrderQty; }
            set { tradeOpenOrderQty = value; }
        }
        double tradeOHQty;

        public double TradeCloseOrderAmount
        {
            get
            {
                return this.TradeCloseDealQty *(TradeOpenDealPrice + TradeCloseDealPrice);
            }
        }


        /// <summary>
        /// 开仓时间，第一笔委托时间
        /// </summary>
        public string TradeOpenOrderTime
        {
            get
            {
                foreach (TradeWeiTuo twt in this.openWeiTuo)
                {
                    if (twt.Time != null)
                    {
                        return twt.Time;
                    }
                }
                return string.Empty;
            }
        }

        double tradeOpenOrderPrice;

        public double TradeOpenOrderPrice
        {
            get { return tradeOpenOrderPrice; }
            set { tradeOpenOrderPrice = value; }
        }

        double tradeCloseOrderPrice;

        public double TradeCloseOrderPrice
        {
            get { return tradeCloseOrderPrice; }
            set { tradeCloseOrderPrice = value; }
        }

        OpenType tradeCloseOrderType;

        public OpenType TradeCloseOrderType
        {
            get { return tradeCloseOrderType; }
            set { tradeCloseOrderType = value; }
        }


        OpenType tradeOpenOrderType;

        public OpenType TradeOpenOrderType
        {
            get { return tradeOpenOrderType; }
            set { tradeOpenOrderType = value; }
        }

        /// <summary>
        /// 策略持仓数量=入场委托成交数量-出场委托成交数量
        /// </summary>
        public double TradeOHQty
        {
            get
            {
                decimal ohQty = 0;
                for (int i = 0; i < this.openWeiTuo.Count; i++)
                {
                    ohQty = ohQty + (decimal)openWeiTuo[i].WeiTuoDealQty;
                }
                for (int i = 0; i < this.closeWeiTuo.Count; i++)
                {
                    ohQty = ohQty - (decimal)closeWeiTuo[i].WeiTuoDealQty;
                }
                return (double)ohQty;
            }
        }


        /// <summary>
        /// 入场成交数量
        /// </summary>
        public double TradeOpenDealQty
        {
            get
            {
                decimal dealQty = 0;
                for (int i = 0; i < this.openWeiTuo.Count; i++)
                {
                    dealQty += (decimal)openWeiTuo[i].WeiTuoDealQty;
                }
                return (double)dealQty;

            }
        }

        //入场手续费

        public double TradeOpenFee
        {
            get
            {
                double dealQty = 0;
                for (int i = 0; i < this.openWeiTuo.Count; i++)
                {
                    dealQty += openWeiTuo[i].WeiTuoDealFee;
                }
                return dealQty;

            }
        }
        /// <summary>
        /// 出场成交数量
        /// </summary>
        public double TradeCloseDealQty
        {
            get
            {
                decimal dealQty = 0;
                for (int i = 0; i < this.closeWeiTuo.Count; i++)
                {
                    dealQty += (decimal)closeWeiTuo[i].WeiTuoDealQty;
                }
                return (double)dealQty;

            }
        }

        //出场手续费

        public double TradeCloseFee
        {
            get
            {
                double dealQty = 0;
                for (int i = 0; i < this.closeWeiTuo.Count; i++)
                {
                    dealQty += closeWeiTuo[i].WeiTuoDealFee;
                }
                return dealQty;

            }
        }
        /// <summary>
        /// 出场平均成交价格
        /// </summary>
        public double TradeCloseDealPrice
        {
            get
            {
                decimal dealQty = 0;
                decimal dealPrice = 0;
                for (int i = 0; i < this.closeWeiTuo.Count; i++)
                {
                    dealQty += (decimal)closeWeiTuo[i].WeiTuoDealQty;
                    dealPrice += (decimal)closeWeiTuo[i].WeiTuoDealQty * (decimal)closeWeiTuo[i].WeiTuoDealPrice;
                }
                if (dealQty == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)Math.Round(dealPrice / dealQty, 8, MidpointRounding.AwayFromZero);
                }
            }
        }
        /// <summary>
        /// 入场平均成交价格
        /// </summary>
        public double TradeOpenDealPrice
        {
            get
            {
                decimal dealQty = 0;
                decimal dealPrice = 0;
                for (int i = 0; i < this.openWeiTuo.Count; i++)
                {
                    dealQty += (decimal)openWeiTuo[i].WeiTuoDealQty;
                    dealPrice += (decimal)openWeiTuo[i].WeiTuoDealQty * (decimal)openWeiTuo[i].WeiTuoDealPrice;
                }
                if (dealQty == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)Math.Round(dealPrice / dealQty, 8, MidpointRounding.AwayFromZero);
                }
            }
        }

        double tradeCloseOrderQty;

        public double TradeCloseOrderQty
        {
            get { return tradeCloseOrderQty; }
            set { tradeCloseOrderQty = value; }
        }


        public string TradeCloseOrderTime
        {
            get
            {
                foreach (TradeWeiTuo twt in this.closeWeiTuo)
                {
                    if (twt.Time != null)
                    {
                        return twt.Time;
                    }
                }
                return string.Empty;
            }
        }

        

        List<TradeWeiTuo> openWeiTuo;

        public List<TradeWeiTuo> OpenWeiTuo
        {
            get { return openWeiTuo; }
            set { openWeiTuo = value; }
        }
        List<TradeWeiTuo> closeWeiTuo;

        public List<TradeWeiTuo> CloseWeiTuo
        {
            get { return closeWeiTuo; }
            set { closeWeiTuo = value; }
        }


        string poicyName;

        public string PolicyName
        {
            get { return poicyName; }
            set { poicyName = value; }
        }

        int policyOpenQty;

        public int PolicyOpenQty
        {
            get { return policyOpenQty; }
            set { policyOpenQty = value; }
        }
        int policyCloseQty;

        public int PolicyCloseQty
        {
            get { return policyCloseQty; }
            set { policyCloseQty = value; }
        }
        OpenPoint policyOpenPoint;

        public OpenPoint PolicyOpenPoint
        {
            get { return policyOpenPoint; }
            set { policyOpenPoint = value; }
        }
        OpenPoint policyClosePoint;

        public OpenPoint PolicyClosePoint
        {
            get { return policyClosePoint; }
            set { policyClosePoint = value; }
        }

        string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        double poicyFee;

        public double PoicyFee
        {
            get { return poicyFee; }
            set { poicyFee = value; }
        }

        //RunningPolicy policy;

        //public RunningPolicy Policy
        //{
        //    get { return policy; }
        //    set { policy = value; }
        //}

        double gainPolicy;

        public double GainPolicy
        {
            get { return gainPolicy; }
            set { gainPolicy = value; }
        }
        double gainActure;

        public double GainActure
        {
            get { return gainActure; }
            set { gainActure = value; }
        }

        DateTime createTime;

        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        public TradeDetail()
        {
            this.closed = false;
            this.openWeiTuo = new List<TradeWeiTuo>();
            this.tradeCloseOrderQty = 0;
            this.tradeOHQty = 0;
            this.closeWeiTuo = new List<TradeWeiTuo>();
            this.policyClosePoint = new OpenPoint();
            this.policyOpenPoint = new OpenPoint();
            this.remark = string.Empty;
            this.poicyFee = 0;
            this.gainActure = 0;
            this.gainPolicy = 0;
            this.createTime = DateTime.Now;
        }


    }
}
