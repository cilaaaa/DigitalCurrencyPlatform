
namespace StockTradeAPI
{
    public class CheDanArgs
    {
        bool cancel;
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        bool zhuiDan;
        public bool ZhuiDan
        {
            get { return zhuiDan; }
            set { zhuiDan = value; }
        }

        double newOrderPrice;
        public double NewOrderPrice
        {
            get { return newOrderPrice; }
            set { newOrderPrice = value; }
        }

        double markPrice;
        public double MarkPrice
        {
            get { return markPrice; }
            set { markPrice = value; }
        }

        TradeSendOrderPriceType tsopt;
        public TradeSendOrderPriceType Tsopt
        {
            get { return tsopt; }
            set { tsopt = value; }
        }

        public CheDanArgs()
        {
            this.cancel = false;
            this.zhuiDan = false;
            this.newOrderPrice = 0;
            this.markPrice = 0;
            this.tsopt = TradeSendOrderPriceType.Limit;
        }
    }
}
