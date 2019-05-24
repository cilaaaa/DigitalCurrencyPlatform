using System.Collections.Generic;

namespace DataAPI
{

    public class CurrentTicker
    {
        static Dictionary<string, CurrentTick> _ticker = new Dictionary<string, CurrentTick>();


        public static void Insert(string code)
        {
            _ticker.Add(code, new CurrentTick());
        }

        public static void Update(string code, double lastPrice)
        {
            _ticker[code].LastPrice = lastPrice;
        }

        public static double getCurrentTickerPrice(string code)
        {
            if (_ticker.ContainsKey(code))
            {
                return _ticker[code].LastPrice;
            }
            else
            {
                return 0;
            }
        }
    }

    public class CurrentTick
    {
        double _lastPrice;

        public double LastPrice
        {
            get { return _lastPrice; }
            set { _lastPrice = value; }
        }
    }

}
