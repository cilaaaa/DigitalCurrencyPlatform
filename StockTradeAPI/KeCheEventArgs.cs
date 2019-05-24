using System.Collections.Generic;

namespace StockTradeAPI
{
    public class KeCheEventArgs
    {
        private Dictionary<string, StockWeiTuo> _keCheList;

        public Dictionary<string, StockWeiTuo> KeCheList
        {
            get { return _keCheList; }
            set { _keCheList = value; }
        }

        public KeCheEventArgs(Dictionary<string, StockWeiTuo> kechelist)
        {
            // TODO: Complete member initialization
            this._keCheList = kechelist;
        }
    }
}
