using StockData;
using System;

namespace StockTradeAPI
{
    public class TradePoints
    {
        private OpenPoint enterPoint;

        public OpenPoint EnterPoint
        {
            get { return enterPoint; }
            set { enterPoint = value; }
        }
        private OpenPoint outPoint;

        public OpenPoint OutPoint
        {
            get { return outPoint; }
            set
            {
                outPoint = value;
                //this.closed = true;
                this.status = OpenStatus.Closed;
            }
        }
        private bool closed;

        public bool Closed
        {
            get
            {
                if (this.status != OpenStatus.Closed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        private bool finished;

        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        private OpenStatus status;

        public OpenStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private string tpRemark;

        public string TpRemark
        {
            get { return tpRemark; }
            set { tpRemark = value; }
        }

        Guid tradeGuid;

        public Guid TradeGuid
        {
            get { return tradeGuid; }
            set { tradeGuid = value; }
        }
        public TradePoints(OpenPoint openpoint, double stoplossPercent)
        {
            this.tradeGuid = Guid.NewGuid();
            this.enterPoint = openpoint;
            if (this.enterPoint.OpenType == OpenType.Buy)
            {
                this.stopLossPrice = Math.Round(this.enterPoint.OpenPrice * (1 - stoplossPercent / 100), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                this.stopLossPrice = Math.Round(this.enterPoint.OpenPrice * (1 + stoplossPercent / 100), 2, MidpointRounding.AwayFromZero);

            }
            //this.closed = false;
            this.status = OpenStatus.Open;
            //this.lastRaisePrice = 0;
            this.returnToLeave = false;
            this.leavePoint = 0;
            this.preHigh = 0;
            this.returned = false;
        }

        


        private double stopLossPrice;

        public double StopLossPrice
        {
            get { return stopLossPrice; }
            set { stopLossPrice = value; }
        }

        private Boolean deepPending;

        public Boolean DeepPending
        {
            get { return deepPending; }
            set { deepPending = value; }
        }

        private CloseType howToClose;

        public CloseType HowToClose
        {
            get { return howToClose; }
            set { howToClose = value; }
        }




        //private double lastRaisePrice;
        ////最近阳线涨幅
        //public double LastRaisePrice
        //{
        //    get { return lastRaisePrice; }
        //    set { lastRaisePrice = value; }
        //}
        private bool returnToLeave;

        public bool ReturnToLeave
        {
            get { return returnToLeave; }
            set { returnToLeave = value; }
        }

        private double leavePoint;

        public double LeavePoint
        {
            get { return leavePoint; }
            set { leavePoint = value; }
        }

        private double preHigh;

        public double PreHigh
        {
            get { return preHigh; }
            set { preHigh = value; }
        }

        private bool returned;

        public bool Returned
        {
            get { return returned; }
            set { returned = value; }
        }

        double fee;

        public double Fee
        {
            get { return fee; }
            set { fee = value; }
        }

        bool isReal;

        public bool IsReal
        {
            get { return isReal; }
            set { isReal = value; }
        }

        

    }
}
