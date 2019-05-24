using System;

namespace StockData
{
    public class Bar
    {
        //封装字段
        double open;

        public double Open
        {
            get { return open; }
            set { open = value; }
        }
        double close;

        public double Close
        {
            get { return close; }
            set { close = value; }
        }
        double high;

        public double High
        {
            get { return high; }
            set { high = value; }
        }
        double low;

        public double Low
        {
            get { return low; }
            set { low = value; }
        }

        Int64 volumn;

        public Int64 Volumn
        {
            get { return volumn; }
            set { volumn = value; }
        }

        double preClose;

        public double PreClose
        {
            get { return preClose; }
            set { preClose = value; }
        }

        Int64 estVolumn;

        public Int64 EstVolumn
        {
            get { return estVolumn; }
            set { estVolumn = value; }
        }

        Int64 ev5;

        public Int64 Ev5
        {
            get { return ev5; }
            set { ev5 = value; }
        }

        Int64 ev10;

        public Int64 Ev10
        {
            get { return ev10; }
            set { ev10 = value; }
        }

        BarType raiseType;

        public BarType RaiseType
        {
            get { return raiseType; }
            set { raiseType = value; }
        }

        //Int64 amount;

        //public Int64 Amount
        //{
        //    get { return amount; }
        //    set { amount = value; }
        //}
        //Int64 estAmount;

        //public Int64 EstAmount
        //{
        //    get { return estAmount; }
        //    set { estAmount = value; }
        //}

        public Bar()
        {
            this.open = 0;
            this.close = 0;
            this.high = 0;
            this.close = 0;
            this.estVolumn = 0;
            this.ev10 = 0;
            this.ev5 = 0;
        }

        TimeSpan barTime;

        public TimeSpan BarTime
        {
            get { return barTime; }
            set { barTime = value; }
        }
    }
}
