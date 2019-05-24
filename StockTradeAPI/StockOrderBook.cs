
namespace StockTradeAPI
{
    public class StockOrderBook
    {
        string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        long _id;

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        string _side;

        public string Side
        {
            get { return _side; }
            set { _side = value; }
        }

        double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        long _size;

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        
    }
}
