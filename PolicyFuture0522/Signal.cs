using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolicyFuture0522
{
    class Signal
    {
        int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        double price;
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        string info;
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        public Signal(int index, double price, string info)
        {
            this.index = index;
            this.price = price;
            this.info = info;
        }
    }
}
