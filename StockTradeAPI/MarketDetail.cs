using StockData;
using System;

namespace StockTradeAPI
{
    public class MarketDetail
    {

        string _clientBianHao;

        public string ClientBianHao
        {
            get { return _clientBianHao; }
            set { _clientBianHao = value; }
        }

        double openQty;
        public double OpenQty
        {
            get { return openQty; }
            set { openQty = value; }
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

        PolicyTradeType tradeType;

        public PolicyTradeType TradeType
        {
            get { return tradeType; }
            set { tradeType = value; }
        }
    }
}
