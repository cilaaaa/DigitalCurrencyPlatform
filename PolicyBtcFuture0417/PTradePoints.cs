﻿using StockTradeAPI;

namespace PolicyBtcFuture0417
{
    public class PTradePoints : TradePoints
    {
        private double _zPrice;
        public double ZPrice
        {
            get { return _zPrice; }
            set { _zPrice = value; }
        }

        private bool _startZhiYing;
        public bool StartZhiYing
        {
            get { return _startZhiYing; }
            set { _startZhiYing = value; }
        }

        private double _Y;
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        private double _ZhiSun;
        public double ZhiSun
        {
            get { return _ZhiSun; }
            set { _ZhiSun = value; }
        }

        public PTradePoints(OpenPoint openpoint, double stoplossPercent):base(openpoint,stoplossPercent)
        {
            this._zPrice = 0;
        }
    }
}
