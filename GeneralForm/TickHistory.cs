using StockData;
using System;
using System.Collections.Generic;
using System.Data;

namespace GeneralForm
{
    public class TickHistory
    {
        public string code;
        public DateTime datetime;
        public DataTable lists;
        public double lastclose;
        public double high;
        public double low;
        public SortedDictionary<TimeSpan, TickData> ticks;
    }
}
