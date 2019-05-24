using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockData
{
    public class MarketTimeRange
    {
        int _hourShift;
        
        double _mintickTime;

        public double MinTickTime
        {
            get { return _mintickTime; }
            set { _mintickTime = value; }
        }


        double _tickInteval;

        public double TickInteval
        {
            get { return _tickInteval; }
            set { _tickInteval = value; }
        }

        public int HourShift
        {
            get { return _hourShift; }
            set { _hourShift = value; }
        }
        List<TimeRange> _timeRanges;

        public List<TimeRange> TimeRanges
        {
            get { return _timeRanges; }
            set { _timeRanges = value; }
        }

        string _market;

        public string Market
        {
            get { return _market; }
            set { _market = value; }
        }

        private TimeSpan _startTime;

        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        private TimeSpan _endTime;

        public TimeSpan EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        private TimeSpan _tradeEndTime;

        public TimeSpan TradeEndTime
        {
            get { return _tradeEndTime; }
            set { _tradeEndTime = value; }
        }

        private TimeSpan _policyOpenStartTime;

        public TimeSpan PolicyOpenStartTime
        {
            get { return _policyOpenStartTime; }
            set { _policyOpenStartTime = value; }
        }
        private TimeSpan _policyOpenEndTime;

        public TimeSpan PolicyOpenEndTime
        {
            get { return _policyOpenEndTime; }
            set { _policyOpenEndTime = value; }
        }

        private TimeSpan _forceCloseTime;

        public TimeSpan ForceCloseTime
        {
            get { return _forceCloseTime; }
            set { _forceCloseTime = value; }
        }
        public static MarketTimeRange getTimeRange(string market)
        {
            MarketTimeRange mtr = new MarketTimeRange();
            if(market == "future"){
                mtr._market = market;
                mtr._hourShift = 0;
                List<TimeRange> _t = new List<TimeRange>();
                _t.Add(new TimeRange(new TimeSpan(09, 30, 0), new TimeSpan(15, 00, 00), 0));
                mtr._startTime = new TimeSpan(09, 30, 0);
                mtr._endTime = new TimeSpan(15, 00, 00);
                mtr._tradeEndTime = new TimeSpan(14, 57, 0);
                mtr._policyOpenStartTime = new TimeSpan(9, 31, 0);
                mtr._policyOpenEndTime = new TimeSpan(15, 0, 0);
                mtr._forceCloseTime = new TimeSpan(14, 56, 0);
                mtr._mintickTime = 1;
                mtr._tickInteval = 1;
                mtr._timeRanges = _t;
            }
            else
            {
                mtr._market = market;
                mtr._hourShift = 0;
                List<TimeRange> _t = new List<TimeRange>();
                _t.Add(new TimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59), 0));
                mtr._startTime = new TimeSpan(0, 0, 0);
                mtr._endTime = new TimeSpan(23, 59, 59);
                mtr._tradeEndTime = new TimeSpan(14, 57, 0);
                mtr._policyOpenStartTime = new TimeSpan(9, 31, 0);
                mtr._policyOpenEndTime = new TimeSpan(15, 0, 0);
                mtr._forceCloseTime = new TimeSpan(14, 56, 0);
                mtr._mintickTime = 1;
                mtr._tickInteval = 1;
                mtr._timeRanges = _t;
            }
            return mtr;
        }
        public bool isLive(TimeSpan timespan)
        {
            foreach(var t in this._timeRanges)
            {
                if(timespan >= t.Start && timespan <= t.End)
                {
                    return true;
                }
            }
            return false;
        }




        internal int calculateSeconds(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
    public class TimeRange
    {
        TimeSpan _start;

        public TimeSpan Start
        {
            get { return _start; }
            set { _start = value; }
        }
        TimeSpan _end;

        public TimeSpan End
        {
            get { return _end; }
            set { _end = value; }
        }

        double _toNextOpenHour;

        public double ToNextOpenHour
        {
            get { return _toNextOpenHour; }
            set { _toNextOpenHour = value; }
        }

        public TimeRange(TimeSpan start,TimeSpan end,double toNextOpenHour)
        {
            this._start = start;
            this._end = end;
            this._toNextOpenHour = toNextOpenHour;
        }
    }
}
