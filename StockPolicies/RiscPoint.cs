using System;

namespace StockPolicies
{
    public class RiscPoint
    {
        TimeSpan time;

        public TimeSpan Time
        {
            get { return time; }
            set { time = value; }
        }
        double price;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        public RiscPoint(TimeSpan time, double price)
        {
            this.time = time;
            this.price = price;
        }
    }
}
