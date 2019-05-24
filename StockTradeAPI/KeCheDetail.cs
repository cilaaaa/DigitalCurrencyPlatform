using StockData;
using System;

namespace StockTradeAPI
{
    public class KeCheDetail
    {
        bool _dealed;

        public bool Dealed
        {
            get { return _dealed; }
            set { _dealed = value; }
        }

        SecurityInfo _si;

        public SecurityInfo Si
        {
            get { return _si; }
            set { _si = value; }
        }
        string _bianHao;

        public string BianHao
        {
            get { return _bianHao; }
            set { _bianHao = value; }
        }
        string _clientBianHao;

        public string ClientBianHao
        {
            get { return _clientBianHao; }
            set { _clientBianHao = value; }
        }
        TimeSpan _createTime;

        public TimeSpan CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        int _waitSecond;

        public int WaitSecond
        {
            get { return _waitSecond; }
            set { _waitSecond = value; }
        }

        double markPrice;

        public double MarkPrice
        {
            get { return markPrice; }
            set { markPrice = value; }
        }

        int _cancelTimes;

        public int CancelTimes
        {
            get { return _cancelTimes; }
            set { _cancelTimes = value; }
        }
        string _exchangeID;

        public string ExchangeID
        {
            get { return _exchangeID; }
            set { _exchangeID = value; }
        }
        PolicyTradeType _tradeType;

        public PolicyTradeType TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }
        bool _canceled;

        public bool Canceled
        {
            get { return _canceled; }
            set { _canceled = value; }
        }
        bool _finished;

        public bool Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }
        public double UnDealQty
        {
            get
            {
                return _orderQty - _dealedQty;
            }
        }
        TradeSendOrderPriceType _reEnterPriceType;

        public TradeSendOrderPriceType ReEnterPriceType
        {
            get { return _reEnterPriceType; }
            set { _reEnterPriceType = value; }
        }

        double _orderPrice;

        public double OrderPrice
        {
            get { return _orderPrice; }
            set { _orderPrice = value; }
        }

        double _dealedPrice;

        public double DealedPrice
        {
            get { return _dealedPrice; }
            set { _dealedPrice = value; }
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

        object _policyObject;

        public object PolicyObject
        {
            get { return _policyObject; }
            set { _policyObject = value; }
        }

        double _orderQty;

        public double OrderQty
        {
            get { return _orderQty; }
            set { _orderQty = value; }
        }

        double _dealedQty;

        public double DealedQty
        {
            get { return _dealedQty; }
            set { _dealedQty = value; }
        }

        bool _forceToCancel;

        public bool ForceToCancel
        {
            get { return _forceToCancel; }
            set { _forceToCancel = value; }
        }

        bool _notified;

        public bool Notified
        {
            get { return _notified; }
            set { _notified = value; }
        }

        private bool maker;

        public bool Maker
        {
            get { return maker; }
            set { maker = value; }
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

        private double reEnterPercentage;

        public double ReEnterPercentage
        {
            get { return reEnterPercentage; }
            set { reEnterPercentage = value; }
        }

        public bool OverPrice(double lastPrice, double x)
        {
            if (lastPrice > OrderPrice + x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LowerPrice(double lastPrice,double x)
        {
            if (lastPrice < OrderPrice - x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OverTime()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if ((now - this._createTime).TotalMilliseconds > _waitSecond)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
