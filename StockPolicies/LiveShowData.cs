using StockData;

namespace StockPolicies
{
    public class LiveShowData
    {
        LiveBars _bar1M;

        public LiveBars Bar1M
        {
            get { return _bar1M; }
            set { _bar1M = value; }
        }
        double _lastClose;

        public double LastClose
        {
            get { return _lastClose; }
            set { _lastClose = value; }
        }

        RiscPoints _riscPoints;

        public RiscPoints RiscPoints
        {
            get { return _riscPoints; }
            set { _riscPoints = value; }
        }
        public LiveShowData(LiveBars bar1M, double lastClose,RiscPoints riscpoints)
        {
            this._bar1M = bar1M;
            this._lastClose = lastClose;
            this._riscPoints = riscpoints;
        }
    }
}
