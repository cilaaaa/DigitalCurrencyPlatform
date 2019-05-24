using StockData;
using System;


namespace StockTradeAPI
{

    public class CanceledWeiTuo
    {
        string _bianhao;

        public string Bianhao
        {
            get { return _bianhao; }
            set { _bianhao = value; }
        }

        string _clientbianhao;

        public string Clientbianhao
        {
            get { return _clientbianhao; }
            set { _clientbianhao = value; }
        }

        CanceledWeiTuoStatus _status;

        public CanceledWeiTuoStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        int _cancelTimes;

        public int CancelTimes
        {
            get { return _cancelTimes; }
            set { _cancelTimes = value; }
        }

        double _orderQty;

        public double OrderQty
        {
            get { return _orderQty; }
            set { _orderQty = value; }
        }

        double _unDealedQty;

        public double UnDealedQty
        {
            get { return _unDealedQty; }
            set { _unDealedQty = value; }
        }

        SecurityInfo _si;

        public SecurityInfo Si
        {
            get { return _si; }
            set { _si = value; }
        }

        double _orderPrice;

        public double OrderPrice
        {
            get { return _orderPrice; }
            set { _orderPrice = value; }
        }

        double markPrice;

        public double MarkPrice
        {
            get { return markPrice; }
            set { markPrice = value; }
        }

        OpenType _openType;

        public OpenType OpenType
        {
            get { return _openType; }
            set { _openType = value; }
        }

        Guid _tradeGuid;

        public Guid TradeGuid
        {
            get { return _tradeGuid; }
            set { _tradeGuid = value; }
        }

        PolicyTradeType _tradeType;

        public PolicyTradeType TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }

        TradeSendOrderPriceType _reTradePriceType;

        public TradeSendOrderPriceType ReTradePriceType
        {
            get { return _reTradePriceType; }
            set { _reTradePriceType = value; }
        }

        object _policyObject;


        public object PolicyObject
        {
            get { return _policyObject; }
            set { _policyObject = value; }
        }

        private int _cancelLimitTime;

        public int CancelLimitTime
        {
            get { return _cancelLimitTime; }
            set { _cancelLimitTime = value; }
        }

        private Boolean res;

        public Boolean Res
        {
            get { return res; }
            set { res = value; }
        }

        private double reEnterPercentage;

        public double ReEnterPercentage
        {
            get { return reEnterPercentage; }
            set { reEnterPercentage = value; }
        }

        string leverage;

        /// <summary>
        /// 杠杆倍数
        /// </summary>
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

    }

    public enum CanceledWeiTuoStatus
    {
        WaitForConfirm,
        CancelConfirmed,
        Pending,
        ReEntered
    }
}
