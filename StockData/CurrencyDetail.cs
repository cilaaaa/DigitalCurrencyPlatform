
namespace StockData
{
    /// <summary>
    /// Tick数据结构
    /// </summary>
    public class CurrencyDetail
    {
        string _currency;

        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        double _balance;

        public double Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }
    }
}
