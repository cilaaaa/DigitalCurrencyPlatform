using DataBase;
using DataHub;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PolicyBtcFuture0315
{
    public class PTradePoints : TradePoints
    {
        private bool _startZhiYing;
        public bool StartZhiYing{
            get { return _startZhiYing; }
            set { _startZhiYing = value; }
        }

        private double _zhiYingDian;
        public double ZhiYingDian
        {
            get { return _zhiYingDian; }
            set { _zhiYingDian = value; }
        }

        private double _preTr;
        public double PreTr
        {
            get { return _preTr; }
            set { _preTr = value; }
        }

        private double _maxTr;
        public double MaxTr
        {
            get { return _maxTr; }
            set { _maxTr = value; }
        }

        public PTradePoints(OpenPoint openpoint, double stoplossPercent):base(openpoint,stoplossPercent)
        {
        }
    }
}
