
namespace StockTradeAPI
{
    public class OpenPointWeiTuo
    {
        string weituobianhao;

        public string Weituobianhao
        {
            get { return weituobianhao; }
            set { weituobianhao = value; }
        }

        double openQty;

        public double OpenQty
        {
            get { return openQty; }
            set { openQty = value; }
        }

        double dealQty;

        public double DealQty
        {
            get { return dealQty; }
            set { dealQty = value; }
        }

        double partDealQty;

        public double PartDealQty
        {
            get { return partDealQty; }
            set { partDealQty = value; }
        }

    }
}
