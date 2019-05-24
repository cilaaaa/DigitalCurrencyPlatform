using StockData;

namespace GeneralForm
{
    public class MonitorStockEventArgs
    {
        SecurityInfo _si;
        

        public MonitorStockEventArgs(SecurityInfo si)
        {
            // TODO: Complete member initialization
            this._si = si;
        }

        public SecurityInfo SI
        {
            get { return _si; }
            set { _si = value; }
        }

    }
}
