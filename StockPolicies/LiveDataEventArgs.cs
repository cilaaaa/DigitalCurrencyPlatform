using StockData;

namespace StockPolicies
{
    public class LiveDataEventArgs
    {
        SecurityInfo si;

        public SecurityInfo Si
        {
            get { return si; }
            set { si = value; }
        }
        string stockcode;

        public string Stockcode
        {
            get { return stockcode; }
            set { stockcode = value; }
        }
        string stockname;

        public string Stockname
        {
            get { return stockname; }
            set { stockname = value; }
        }
        double currentPrice;

        public double CurrentPrice
        {
            get { return currentPrice; }
            set { currentPrice = value; }
        }
        double amount;

        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        double preClose;

        public double PreClose
        {
            get { return preClose; }
            set { preClose = value; }
        }

        bool isReal;

        public bool IsReal
        {
            get { return isReal; }
            set { isReal = value; }
        }
        double todayOpen;

        public double TodayOpen
        {
            get { return todayOpen; }
            set { todayOpen = value; }
        }
        double todayHigh;

        public double TodayHigh
        {
            get { return todayHigh; }
            set { todayHigh = value; }
        }
        double todayLow;

        public double TodayLow
        {
            get { return todayLow; }
            set { todayLow = value; }
        }

        double ask;

        public double Ask
        {
            get { return ask; }
            set { ask = value; }
        }
        double bid;

        public double Bid
        {
            get { return bid; }
            set { bid = value; }
        }
        public static LiveDataEventArgs ConvertFromTickData(TickData tickdata)
        {
            LiveDataEventArgs args = new LiveDataEventArgs();
            args.si = tickdata.SecInfo;
            args.Stockcode = tickdata.Code;
            args.CurrentPrice = tickdata.Last;
            args.PreClose = tickdata.Preclose;
            //args.Amount = tickdata.Amt;
            args.Stockname = tickdata.Name;
            args.isReal = tickdata.IsReal;
            args.todayOpen = tickdata.Open;
            args.todayHigh = tickdata.High;
            args.todayLow = tickdata.Low;
            args.bid = tickdata.Bid;
            args.ask = tickdata.Ask;
            return args;
        }
    }
}
