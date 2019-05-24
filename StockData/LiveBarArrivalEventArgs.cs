
namespace StockData
{
    public class LiveBarArrivalEventArgs
    {
        int _inteval;

        public int Inteval
        {
            get { return _inteval; }
            set { _inteval = value; }
        }
        LiveBar _bar;

        public LiveBar Bar
        {
            get { return _bar; }
            set { _bar = value; }
        }
        public LiveBarArrivalEventArgs(LiveBar bar,int inteval)
        {
            this._bar = bar;
            this._inteval = inteval;
        }
    }
    
}
