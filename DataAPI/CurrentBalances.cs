using System.Collections.Generic;

namespace DataAPI
{

    public class CurrentBalances
    {
        static Dictionary<string, List<CurrentBanalce>> _balances = new Dictionary<string, List<CurrentBanalce>>();


        public static void Insert(string market)
        {
            _balances.Add(market, new List<CurrentBanalce>());
        }

        public static void Update(string market,List<CurrentBanalce> cbs)
        {
            _balances[market] = cbs;
        }

        public static List<CurrentBanalce> getCurrentBalance(string market)
        {
            if (_balances.ContainsKey(market))
            {
                return _balances[market];
            }
            else
            {
                return null;
            }
        }
    }

    public class CurrentBanalce
    {
        string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        double _ava;

        public double Ava
        {
            get { return _ava; }
            set { _ava = value; }
        }
        double _total;

        public double Total
        {
            get { return _total; }
            set { _total = value; }
        }
        double _frz;

        public double Frz
        {
            get { return _frz; }
            set { _frz = value; }
        }
    }
    
}
