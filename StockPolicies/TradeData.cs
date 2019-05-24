using StockData;
using StockTradeAPI;

namespace StockPolicies
{
    public class TradeData
    {
        string _policyName;

        public string PolicyName
        {
            get { return _policyName; }
            set { _policyName = value; }
        }

        TradePoints _pairPoints;

        public TradePoints PairPoints
        {
            get { return _pairPoints; }
            set { _pairPoints = value; }
        }

        SecurityInfo _sI;

        public SecurityInfo SI
        {
            get { return _sI; }
            set { _sI = value; }
        }

        object _policy;

        public object Policy
        {
            get { return _policy; }
            set { _policy = value; }
        }

        PolicyTradeType _tradeType;

        public PolicyTradeType TradeType
        {
            get { return _tradeType; }
            set { _tradeType = value; }
        }
    }
}
