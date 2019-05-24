using System.Collections.Generic;

namespace StockTradeAPI
{
    public class ChiCangEventArgs
    {
        private List<StockChiCang> _chicangs;

        public List<StockChiCang> Chicangs
        {
            get { return _chicangs; }
            set { _chicangs = value; }
        }
        public ChiCangEventArgs(List<StockChiCang> chicangs)
        {
            this._chicangs = chicangs;
        }
    }
}
