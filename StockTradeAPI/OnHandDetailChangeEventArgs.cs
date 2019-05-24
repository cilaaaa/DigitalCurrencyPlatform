
namespace StockTradeAPI
{
    public class OnHandDetailChangeEventArgs
    {
        private OnHandStock _onHandStock;

        public OnHandStock OnHandStock
        {
            get { return _onHandStock; }
            set { _onHandStock = value; }
        }

        public OnHandDetailChangeEventArgs(OnHandStock onHandStock)
        {
            this._onHandStock = onHandStock;
        }
    }
}
