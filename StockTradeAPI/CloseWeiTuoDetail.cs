using StockData;
using System;

namespace StockTradeAPI
{
    public class CloseWeiTuoDetail
    {
        string _bianHao;

        public string BianHao
        {
            get { return _bianHao; }
            set { _bianHao = value; }
        }

        bool _dealed;

        public bool Dealed
        {
            get { return _dealed; }
            set { _dealed = value; }
        }

        int _orderQty;

        public int OrderQty
        {
            get { return _orderQty; }
            set { _orderQty = value; }
        }

        int _unDealedQty;

        public int UnDealedQty
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

        bool _monitored;

        public bool Monitored
        {
            get { return _monitored; }
            set { _monitored = value; }
        }

        double _orderPrice;

        public double OrderPrice
        {
            get { return _orderPrice; }
            set { _orderPrice = value; }
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

        string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

    }
}
