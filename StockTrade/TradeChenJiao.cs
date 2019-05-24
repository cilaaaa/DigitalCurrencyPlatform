
namespace StockTrade
{
    public class TradeChenJiao
    {
        string bianhao;

        public string Bianhao
        {
            get { return bianhao; }
            set { bianhao = value; }
        }
        double price;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        double qty;

        public double Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        double fee;

        public double Fee
        {
            get { return fee; }
            set { fee = value; }
        }
        int buySellType;


        public int BuySellType
        {
            get { return buySellType; }
            set { buySellType = value; }
        }

        string buySellDesc;

        public string BuySellDesc
        {
            get { return buySellDesc; }
            set { buySellDesc = value; }
        }

        string weituoBianhao;

        public string WeituoBianhao
        {
            get { return weituoBianhao; }
            set { weituoBianhao = value; }
        }

        string clientBianhao;

        public string ClientBianhao
        {
            get { return clientBianhao; }
            set { clientBianhao = value; }
        }

    }
}
