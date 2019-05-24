using StockData;
using System;
using System.Collections.Generic;

namespace StockTradeAPI
{
    public class OpenPoint
    {
        SecurityInfo secInfo;

        public SecurityInfo SecInfo//股票的信息
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        DateTime openTime;//开始时间

        public DateTime OpenTime
        {
            get { return openTime; }
            set { openTime = value; }
        }
        double openPrice;//开始价格

        public double OpenPrice
        {
            get { return openPrice; }
            set { openPrice = value; }
        }
        OpenType openType;//开始的类型

        public OpenType OpenType
        {
            get { return openType; }
            set { openType = value; }
        }

        private string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        double openQty;

        public double OpenQty
        {
            get { return openQty; }
            set { openQty = value; }
        }

        double dealQty;

        public double DealQty
        {
            get { return dealQty; }
            set { dealQty = value; }
        }

        double dealPrice;

        public double DealPrice
        {
            get { return dealPrice; }
            set { dealPrice = value; }
        }

        double partDealQty;

        public double PartDealQty
        {
            get { return partDealQty; }
            set { partDealQty = value; }
        }


        int cancelTimes;

        public int CancelTimes
        {
            get { return cancelTimes; }
            set { cancelTimes = value; }
        }

        List<OpenPointWeiTuo> openPointWeiTuo;

        public List<OpenPointWeiTuo> OpenPointWeiTuo
        {
            get { return openPointWeiTuo; }
        }

        public OpenPoint()
        {
            this.remark = string.Empty;
            this.reEnterPecentage = -1;
            this.cancelLimitTime = 0;
            this.openPointWeiTuo = new List<OpenPointWeiTuo>();
            this.leverage = "10";
        }

        TradeSendOrderPriceType firstTradePriceType;

        public TradeSendOrderPriceType FirstTradePriceType
        {
            get { return firstTradePriceType; }
            set { firstTradePriceType = value; }
        }
        int cancelLimitTime;

        /// <summary>
        /// 单位秒
        /// </summary>
        public int CancelLimitTime
        {
            get { return cancelLimitTime; }
            set { cancelLimitTime = value; }
        }

        double reEnterPecentage;
        /// <summary>
        /// 单位1
        /// </summary>
        public double ReEnterPecentage
        {
            get { return reEnterPecentage; }
            set { reEnterPecentage = value; }
        }

        TradeSendOrderPriceType reTradePriceType;

        public TradeSendOrderPriceType ReTradePriceType
        {
            get { return reTradePriceType; }
            set { reTradePriceType = value; }
        }

        bool openop;
        public bool Openop
        {
            get { return openop; }
            set { openop = value; }
        }

        string leverage;
        public string Leverage
        {
            get { return leverage; }
            set { leverage = value; }
        }

        private bool maker;

        public bool Maker
        {
            get { return maker; }
            set { maker = value; }
        }

        //string remarks;
        ///// <summary>
        ///// 备注
        ///// </summary>
        //public string Remarks
        //{
        //    get { return remarks; }
        //    set { remarks = value; }
        //}

        
    }
}
