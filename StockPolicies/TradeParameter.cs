using System;

namespace StockPolicies
{
    public class TradeParameter
    {
        double fee;

        public double Fee
        {
            get { return fee; }
            set { fee = value; }
        }
        double amount;

        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        double startvalue;

        public double Startvalue
        {
            get { return startvalue; }
            set { startvalue = value; }
        }

        DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        DateTime endTime;

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }
        public TradeParameter(double fee, double amount, double startvalue,DateTime starttime,DateTime endtime)
        {
            this.fee = fee;
            this.amount = amount;
            this.startvalue = startvalue;
            this.startTime = starttime;
            this.endTime = endtime;
        }
    }
}
