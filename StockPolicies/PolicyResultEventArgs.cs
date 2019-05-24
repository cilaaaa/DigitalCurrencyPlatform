using StockData;
using StockTradeAPI;

namespace StockPolicies
{
    //策略的结果和事件参数的类
    public class PolicyResultEventArgs
    {
        SecurityInfo secInfo;

        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        //string stockCode;

        //public string StockCode
        //{
        //    get { return stockCode; }
        //    set { stockCode = value; }
        //}
        //string stockName;

        //public string StockName
        //{
        //    get { return stockName; }
        //    set { stockName = value; }
        //}
        string PolicyName;

        public string PolicyName1
        {
            get { return PolicyName; }
            set { PolicyName = value; }
        }
        //double currentPrice;

        //public double CurrentPrice
        //{
        //    get { return currentPrice; }
        //    set { currentPrice = value; }
        //}
        //OpenType openType;

        //public OpenType OpenType
        //{
        //    get { return openType; }
        //    set { openType = value; }
        //}
        //DateTime time;

        //public DateTime Time
        //{
        //    get { return time; }
        //    set { time = value; }
        //}
        bool isReal;

        public bool IsReal
        {
            get { return isReal; }
            set { isReal = value; }
        }
        private TradePoints pairePoint;

        public TradePoints PairePoint
        {
            get { return pairePoint; }
            set { pairePoint = value; }
        }
        TickData tickdata;

        public TickData Tickdata
        {
            get { return tickdata; }
            set { tickdata = value; }
        }

        object policyObject;

        public object PolicyObject
        {
            get { return policyObject; }
            set { policyObject = value; }
        }
        private string openRmks;

        public string OpenRmks
        {
            get { return openRmks; }
            set { openRmks = value; }
        }

        private string closermks;

        public string CloseRmks
        {
            get { return closermks; }
            set { closermks = value; }
        }

        bool isSim;
        public bool IsSim
        {
            get { return isSim; }
            set { isSim = value; }
        }

    }
}
