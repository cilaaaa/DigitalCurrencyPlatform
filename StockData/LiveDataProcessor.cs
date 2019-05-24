using System;
using System.Collections.Generic;

namespace StockData
{
    public class LiveDataProcessor
    {
        List<LiveBars> _Live_Bars;
        //bool _dataisOk;

        //public bool DataisOk
        //{
        //    get { return _dataisOk; }
        //    set { _dataisOk = value; }
        //}
        LiveBars _bar1M;

        public LiveBars Bar1M
        {
            get { return _bar1M; }
            set { _bar1M = value; }
        }

        LiveBars _bar5M;

        public LiveBars Bar5M
        {
            get { return _bar5M; }
            set { _bar5M = value; }
        }

        LiveBars _bar15S;

        public LiveBars Bar15S
        {
            get { return _bar15S; }
            set { _bar15S = value; }
        }

        LiveBars _barDay;

        public LiveBars BarDay
        {
            get { return _barDay; }
            set { _barDay = value; }
        }

        //TimeSpan _lastTime;

        //public TimeSpan LastTime
        //{
        //    get { return _lastTime; }
        //    set { _lastTime = value; }
        //}

        double _lastClose;

        public double LastClose
        {
            get { return _lastClose; }
            set { _lastClose = value; }
        }

        double _todayOpen;

        public double TodayOpen
        {
            get { return _todayOpen; }
            set { _todayOpen = value; }
        }

        double _raisePercent;

        public double RaisePercent
        {
            get { return _raisePercent; }
            set { _raisePercent = value; }
        }

        double _averageDealVolumn;

        public double AverageDealVolumn
        {
            get { return _averageDealVolumn; }
            set { _averageDealVolumn = value; }
        }

        TickData _currentTick;

        public TickData CurrentTick
        {
            get { return _currentTick; }
            set { _currentTick = value; }
        }
        /// <summary>
        /// 添加只有一分钟bar的数据处理器
        /// </summary>
        /// 
        SecurityInfo _security;

        public LiveDataProcessor(SecurityInfo security, DateTime? firstBarTime = null)
        {
            //_lastTime = new TimeSpan(9, 30, 0);
            //_lastClose = -1;
            //_todayOpen = -1;
            //_raisePercent = 0;
            //_bar1M = new LiveBars(60);
            //_bar1M.LiveBar_Arrival += LiveBar_Arrival;
            //_currentTick = new TickData();
            //_dataisOk = false;
            this._security = security;
            List<int> intevals = new List<int>();
            intevals.Add(60);
            initializeClass(intevals, firstBarTime);
           
        }
        /// <summary>
        /// 添加除1分钟bar以外的K线的数据处理器
        /// </summary>
        /// <param name="intevals">需要的bar时间间隔</param>
        public LiveDataProcessor(List<int> intevals, SecurityInfo security ,DateTime? firstBarTime = null)
        {
            this._security = security;
            if (!intevals.Contains(60))
            {
                intevals.Add(60);
            }
            initializeClass(intevals, firstBarTime);
        }
        private void initializeClass(List<int> intevals, DateTime? firstBarTime = null)
        {
            _lastClose = -1;
            _todayOpen = -1;
            _raisePercent = 0;
            intevals.Sort();
            _Live_Bars = new List<LiveBars>();
            for (int i = 0; i < intevals.Count;i++)
            {
                LiveBars bar = new LiveBars(intevals[i], _security.TimeRange, firstBarTime);
                bar.LiveBar_Arrival += LiveBar_Arrival;
                _Live_Bars.Add(bar);
                if(intevals[i] == 60)
                {
                    this._bar1M = bar;
                }
                else if(intevals[i] == 15)
                {
                    this._bar15S = bar;
                }
                else if (intevals[i] == 300)
                {
                    this._bar5M = bar;
                }
            }
                //bar1M = new LiveBars(60);
            //_bar1M.LiveBar_Arrival += LiveBar_Arrival;
            _currentTick = new TickData();
        }


        public virtual void Reset()
        {
            //_lastTime = new TimeSpan(9, 30, 0);
            _lastClose = -1;
            _todayOpen = -1;
            _raisePercent = 0;
            foreach(LiveBars bar in _Live_Bars)
            {
                bar.Reset();
            }
            //_bar1M.Reset();// = new LiveBars(60);
            //_bar1M.LiveBar_Arrival += LiveBar_Arrival;
            _currentTick = new TickData();
            //_dataisOk = false;
        }


        public event LiveBarArrivalEventHandler OnLiveBarArrival;
        protected void LiveBar_Arrival(object sender, LiveBarArrivalEventArgs args)
        {
            //DebugLog.Log(string.Format("Bar-{0}", args.Bar.BarTime));
            PushOneMinuteBarEvent(sender, args);
        }

        private void PushOneMinuteBarEvent(object sender, LiveBarArrivalEventArgs args)
        {
            if (OnLiveBarArrival != null)
            {
                OnLiveBarArrival(sender, args);
            }
        }
        public virtual void ReceiveTick(TickData tickdata)
        {
            //if(SecurityMarket.isLive(tickdata.Time.TimeOfDay))
            {
                //if(tickdata.Time.TimeOfDay == _lastTime)
                //{
                //    _dataisOk = false;
                //    return;
                //}
                if (tickdata.Last != 0)
                {
                    if (_lastClose == -1)
                    {
                        _lastClose = tickdata.Last;
                        _todayOpen = tickdata.Last;
                    }
                    //if (tickdata.Preclose != 0)
                    //{
                    //    this._raisePercent = Math.Round((tickdata.Last - tickdata.Preclose) / tickdata.Preclose * 100, 2, MidpointRounding.AwayFromZero);
                    //}

                    //int seconds = _security.calculateSeconds(tickdata.Time.TimeOfDay);

                    //if(seconds != 0)
                    //{
                    //    this._averageDealVolumn = tickdata.Volume / seconds * 60;
                    //}

                    //try
                    //{
                    //    this._averageDealVolumn = tickdata.Volume / (SecurityMarket.calculateSeconds(new TimeSpan(9, 30, 0), tickdata.Time.TimeOfDay) / 60);
                    //}
                    //catch
                    //{
                    //    this._averageDealVolumn = 0;
                    //}
                    //LastTime = tickdata.Time.TimeOfDay;
                    //_bar1M.ReceiveTick(tickdata);
                    foreach (LiveBars bar in _Live_Bars)
                    {
                        bar.ReceiveTick(tickdata);
                    }
                    //_dataisOk = true;
                }
                else
                {
                    //_dataisOk = false;
                }

            }
        }
    }
}
