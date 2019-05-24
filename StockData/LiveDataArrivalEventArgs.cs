
namespace StockData
{
    public class LiveDataArrivalEventArgs
    {
        TickData _tickdata;

        public TickData Tickdata
        {
            get { return _tickdata; }
            set { _tickdata = value; }
        }
        public LiveDataArrivalEventArgs(TickData tickdata)
        {
            this._tickdata = tickdata;
        }
    }
}
