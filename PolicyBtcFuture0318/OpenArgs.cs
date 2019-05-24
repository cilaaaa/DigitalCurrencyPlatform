using StockData;
using System;

namespace PolicyBtcFuture0318
{
    class OpenArgs
    {
        double price;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        double qty;

        public double Qty
        {
            get { return qty; }
            set { qty = value; }
        }

        OpenType opentype;

        public OpenType Opentype
        {
            get { return opentype; }
            set { opentype = value; }
        }

        SecurityInfo si;

        public SecurityInfo Si
        {
            get { return si; }
            set { si = value; }
        }

        TickData tickdata;

        public TickData Tickdata
        {
            get { return tickdata; }
            set { tickdata = value; }
        }

        String tr;

        public String Tr
        {
            get { return tr; }
            set { tr = value; }
        }

        public OpenArgs(double price, double qty, OpenType ot, SecurityInfo si, TickData td, string tr)
        {
            this.price = price;
            this.qty = qty;
            this.opentype = ot;
            this.si = si;
            this.tickdata = td;
            this.tr = tr;
        }
    }
}
