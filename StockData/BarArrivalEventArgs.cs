using System;

namespace StockData
{
    public class BarArrivalEventArgs
    {
        TimeSpan barTimeSpan;

        public TimeSpan BarTimeSpan
        {
            get { return barTimeSpan; }
            set { barTimeSpan = value; }
        }
        TickData lastTickData;

        public TickData LastTickData
        {
            get { return lastTickData; }
            set { lastTickData = value; }
        }
        public BarArrivalEventArgs(TimeSpan ts,TickData td)
        {
            this.barTimeSpan = ts;
            this.lastTickData = td;
        }
    }
}
