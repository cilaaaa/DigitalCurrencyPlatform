using System;
using System.Collections.Generic;
using System.Linq;

namespace StockData
{
    public delegate void LiveBarArrivalEventHandler(object sender,LiveBarArrivalEventArgs args);
    public class LiveBars
    {
        List<double> _closePriceQueue;
        List<double> _tickAverageQueue;
        List<double> _barTickPriceQueue;

        bool _reachHigh;

        public bool ReachHigh
        {
            get { return _reachHigh; }
            set { _reachHigh = value; }
        }

        double _currentHigh;

        public double CurrentHigh
        {
            get { return _currentHigh; }
            set { _currentHigh = value; }
        }

        Dictionary<TimeSpan, LiveBar> _bars;

        public Dictionary<TimeSpan, LiveBar> Bars
        {
            get { return _bars; }
            set { _bars = value; }
        }
        public event LiveBarArrivalEventHandler LiveBar_Arrival;
        private void PushBarArrivalEvent(LiveBar bar)
        {

            if (LiveBar_Arrival != null)
            {
                LiveBar_Arrival(this, new LiveBarArrivalEventArgs(bar, this._inteval));
            }
        }
        //TimeSpan _openTimeSpan;
        TimeSpan _currentOpenTimeSpan;
        double _lastDayClose;
        double _todayOpen;
        double _lastVolumn;
        LiveBar _currentBar;
        TimeSpan _dayOpenTime;
        public LiveBar CurrentBar
        {
            get { return _currentBar; }
            set { _currentBar = value; }
        }

        int _inteval;

        public int Inteval
        {
            get { return _inteval; }
            set { _inteval = value; }
        }

        double _tickInteval;
        LiveBar _lastBar;

        public LiveBars(int inteval, MarketTimeRange timeRange, DateTime? firstBarTime = null)
        {
            _dayOpenTime = timeRange.StartTime;
            _bars = new Dictionary<TimeSpan, LiveBar>();
            
            _lastBar = new LiveBar();
            _lastBar.BarOpenTime = _dayOpenTime;
            TimeSpan open = _dayOpenTime;
            _tickInteval = timeRange.TickInteval;
            foreach(var range in timeRange.TimeRanges)
            {
                TimeSpan close = range.End;
                while(open < close)
                {
                    LiveBar bar = new LiveBar();
                    bar.BarOpenTime = open;
                    _bars.Add(bar.BarOpenTime, bar);
                    bar.PreBar = _lastBar;
                    _lastBar = bar;
                    open = open.Add(TimeSpan.FromSeconds(inteval));
                    if(open < close)
                    {
                        bar.BarCloseTime = open.Add(TimeSpan.FromMilliseconds(-timeRange.MinTickTime));
                        bar.LastTickTime = bar.BarCloseTime.Add(TimeSpan.FromMilliseconds(-timeRange.TickInteval));
                        bar.NextOpenTime = open;
                    }
                    else if(open == close)
                    {
                        bar.BarCloseTime = close.Add(TimeSpan.FromMilliseconds(-timeRange.MinTickTime));
                        bar.LastTickTime = bar.BarCloseTime.Add(TimeSpan.FromMilliseconds(-timeRange.TickInteval));
                        open = open.Add(TimeSpan.FromHours(range.ToNextOpenHour));
                        bar.NextOpenTime = open;
                    }
                    else
                    {
                        open = open.Add(TimeSpan.FromHours(range.ToNextOpenHour));
                        bar.BarCloseTime = open.Add(TimeSpan.FromSeconds(-timeRange.MinTickTime));
                        if(bar.BarCloseTime < close)
                        {
                            bar.BarCloseTime = bar.BarCloseTime.Add(TimeSpan.FromHours(-range.ToNextOpenHour));
                            bar.LastTickTime = bar.BarCloseTime.Add(TimeSpan.FromSeconds(-timeRange.TickInteval));
                        }
                        bar.LastTickTime = bar.BarCloseTime.Add(TimeSpan.FromSeconds(-timeRange.TickInteval));
                        if(bar.LastTickTime < close)
                        {
                            bar.LastTickTime = bar.LastTickTime.Add(TimeSpan.FromHours(-range.ToNextOpenHour));
                        }
                        bar.NextOpenTime = open;
                    }
                }
            }
            //while (open < close)
            //{
            //    LiveBar bar = new LiveBar();

            //    if (open >= new TimeSpan(11, 30, 0))
            //    {
            //        bar.BarTime = open.Add(TimeSpan.FromHours(1.5));
            //    }
            //    else
            //    {
            //        bar.BarTime = open;
            //    }
            //    _bars.Add(bar.BarTime, bar);
            //    bar.PreBar = _lastBar;
            //    _lastBar = bar;
            //    open = open.Add(TimeSpan.FromSeconds(inteval));
            //}

            _currentOpenTimeSpan = _dayOpenTime;
            _currentBar = _bars[_currentOpenTimeSpan];
            _inteval = inteval;
            _closePriceQueue = new List<double>();
            _tickAverageQueue = new List<double>();
            _barTickPriceQueue = new List<double>();
            _lastVolumn = 0;

        }
        internal void Reset()
        {
            List<TimeSpan> t = new List<TimeSpan>();
            t.AddRange(Bars.Keys);
            foreach (TimeSpan tm in t)
            {
                _bars[tm].Reset();
            }
            _currentOpenTimeSpan = _dayOpenTime;
            _currentBar = _bars[_currentOpenTimeSpan];
            _closePriceQueue = new List<double>();
            _tickAverageQueue = new List<double>();
            _barTickPriceQueue = new List<double>();
            _lastVolumn = 0;

        }
        public void ReceiveTick(TickData tickdata)
        {
            if (_lastDayClose == 0)
            {
                _lastDayClose = tickdata.Preclose;
                _todayOpen = tickdata.Open;
            }


            if (_currentHigh < tickdata.Last)
            {
                _currentHigh = tickdata.Last;
                _reachHigh = true;
            }

            //TimeSpan closeTimeSpan = getNextCloseTimeSpan(_currentOpenTimeSpan);
            //TimeSpan closeTimeSpan = _currentBar.BarCloseTime;
            //TimeSpan LastTickTime = _currentBar.LastTickTime;
            TimeSpan CurrentTickTime = tickdata.Time.TimeOfDay;
            //TimeSpan NextTickTime = tickdata.Time.TimeOfDay.Add(TimeSpan.FromSeconds(_tickInteval));


            //.Log(string.Format("{0}-{1}",currentTickTime,closeTimeSpan));
            if (CurrentTickTime <= _currentBar.BarCloseTime) //当前时间小于等于本根bar的收盘时间
            {
                UpdateCurrentBar(tickdata, CurrentTickTime);
                //if (CurrentTickTime > _currentBar.LastTickTime)
                //{
                //    _currentBar.EstVolumn = _currentBar.Volumn;
                //    _lastVolumn += _currentBar.Volumn;
                //    CalcuateBar();
                //    PushBarArrivalEvent(_currentBar);
                //    _currentOpenTimeSpan = _currentBar.NextOpenTime;
                //    if (Bars.ContainsKey(_currentOpenTimeSpan))
                //    {
                //        _currentBar = Bars[_currentOpenTimeSpan];
                //    }
                //}

                //PushBarArrivalEvent(_currentBar);
            }
            else //当前时间大于本根bar收盘时间
            {
                while (CurrentTickTime > _currentBar.BarCloseTime)
                {
                    LiveBar nextBar;
                    if (_currentBar.Open == 0)
                    {
                        _currentBar.Open = tickdata.Last;
                        _currentBar.High = tickdata.Last;
                        _currentBar.Close = tickdata.Last;
                        _currentBar.Low = tickdata.Last;
                    }
                    //_lastVolumn += _currentBar.Volumn;
                    //_currentBar.EstVolumn = _currentBar.Volumn;
                    //CalcuateBar();
                    //MakeRaiseType();
                    //PushBarArrivalEvent(_currentBar);

                    _currentOpenTimeSpan = _currentBar.NextOpenTime;
                    if(Bars.ContainsKey(_currentOpenTimeSpan))
                    {
                        nextBar = Bars[_currentOpenTimeSpan];
                    }
                    else
                    {
                        return;
                    }
                    //LastTickTime = Bars[_currentOpenTimeSpan].LastTickTime;
                    //closeTimeSpan = getNextCloseTimeSpan(closeTimeSpan);
                    if (CurrentTickTime <= nextBar.BarCloseTime)  //当前时间小于等于下一根bar收盘时间
                    {
                        if(CurrentTickTime > nextBar.LastTickTime) //当前时间大于最后tick时间
                        {
                            //push nextbar
                            CalcuateBar();
                            MakeRaiseType();
                            _lastVolumn += _currentBar.Volumn;
                            _currentBar = Bars[_currentOpenTimeSpan];
                            _currentBar.Open = tickdata.Last;
                            _currentBar.High = tickdata.Last;
                            _currentBar.Close = tickdata.Last;
                            _currentBar.Low = tickdata.Last;
                            _currentBar.EstVolumn = _currentBar.Volumn;
                            CalcuateBar();
                            MakeRaiseType();
                            _currentBar.Finish = true;
                            PushBarArrivalEvent(_currentBar);
                            _currentOpenTimeSpan = _currentBar.NextOpenTime;
                            if(Bars.ContainsKey(_currentOpenTimeSpan))
                            {
                                _currentBar = Bars[_currentOpenTimeSpan];
                            }
                        }
                        else // 当前时间小于最后tick时间
                        {
                            //push currentbar
                            CalcuateBar();
                            MakeRaiseType();
                            _lastVolumn += _currentBar.Volumn;
                            _currentBar.Finish = true;
                            PushBarArrivalEvent(_currentBar);
                            _currentBar = Bars[_currentOpenTimeSpan];
                            _currentBar.Open = tickdata.Last;
                            _currentBar.High = tickdata.Last;
                            _currentBar.Close = tickdata.Last;
                            _currentBar.Low = tickdata.Last;

                        }
                        
                    }
                    else
                    {
                        CalcuateBar();
                        MakeRaiseType();
                        _currentBar = Bars[_currentOpenTimeSpan];
                        _currentBar.Open = tickdata.Last;
                        _currentBar.High = tickdata.Last;
                        _currentBar.Close = tickdata.Last;
                        _currentBar.Low = tickdata.Last;
                        _currentBar.EstVolumn = _currentBar.Volumn;
                        CalcuateBar();
                        MakeRaiseType();
                        _lastVolumn += _currentBar.Volumn;
                        //PushBarArrivalEvent(_currentBar);
                        _currentOpenTimeSpan = _currentBar.NextOpenTime;
                        if (Bars.ContainsKey(_currentOpenTimeSpan))
                        {
                            _currentBar = Bars[_currentOpenTimeSpan];
                        }
                    }

                }
            }
        }

        private void UpdateCurrentBar(TickData tickdata, TimeSpan CurrentTickTime)
        {
            if (_currentBar.Open == 0)
            {
                _currentBar.Open = tickdata.Last;
                _currentBar.High = tickdata.Last;
                _currentBar.Close = tickdata.Last;
                _currentBar.Low = tickdata.Last;
            }
            else
            {
                _currentBar.Close = tickdata.Last;
                _currentBar.High = _currentBar.High < tickdata.Last ? tickdata.Last : _currentBar.High;
                _currentBar.Low = _currentBar.Low > tickdata.Last ? tickdata.Last : _currentBar.Low;
            }
            _currentBar.Volumn = tickdata.Volume - _lastVolumn;
            double seconds = (CurrentTickTime - _currentOpenTimeSpan).TotalSeconds;
            if (seconds == 0)
            {
                seconds = 1;
            }
            _currentBar.EstVolumn = System.Convert.ToInt64(_currentBar.Volumn / seconds * Inteval);
            _barTickPriceQueue.Add(tickdata.Last);
            MakeRaiseType();
            //return CurrentTickTime;
        }

        private void MakeRaiseType()
        {
            if (_currentBar.Open > _currentBar.Close)
            {
                _currentBar.AbsoluteRaiseType = AbsoluteBarType.Decent;
                if (_currentBar.Close >= _currentBar.PreBar.Close)
                {
                    _currentBar.RelativeRaiseType = BarType.FakeDecent;
                }
                else
                {
                    _currentBar.RelativeRaiseType = BarType.RealDecent;
                }
            }
            else if (_currentBar.Open < _currentBar.Close)
            {
                _currentBar.AbsoluteRaiseType = AbsoluteBarType.Raise;// BarType.RealRaise;
                if (_currentBar.Close >= _currentBar.PreBar.Close)
                {
                    _currentBar.RelativeRaiseType = BarType.RealRaise;
                }
                else
                {
                    _currentBar.RelativeRaiseType = BarType.FakeRaise;
                }
            }
            else
            {
                _currentBar.AbsoluteRaiseType = AbsoluteBarType.Even;// BarType.Even;
                if (_currentBar.Close > _currentBar.PreBar.Close)
                {
                    _currentBar.RelativeRaiseType = BarType.RealRaise;
                }
                else if (_currentBar.Close < _currentBar.PreBar.Close)
                {
                    _currentBar.RelativeRaiseType = BarType.RealDecent;
                }
                else
                {
                    _currentBar.RelativeRaiseType = BarType.Even;
                }
            }




        }

        private void CalcuateBar()
        {
            _closePriceQueue.Add(_currentBar.Close);
            _currentBar.Average = _closePriceQueue.Average();
            if (_barTickPriceQueue.Count != 0)
            {
                _tickAverageQueue.Add(_barTickPriceQueue.Average());
            }
            else
            {
                _tickAverageQueue.Add(_currentBar.Close);
            }
            while (_closePriceQueue.Count > 20)
            {
                _closePriceQueue.RemoveAt(0);
            }
            if (_closePriceQueue.Count >= 5)
            {
                double sum = 0;
                for (int i = 1; i <= 5; i++)
                {
                    sum += _closePriceQueue[_closePriceQueue.Count - i];
                }
                _currentBar.MA.MA5 = Math.Round(sum / 5, 3, MidpointRounding.AwayFromZero);
                if (_closePriceQueue.Count >= 20)
                {
                    double avg = _closePriceQueue.Average();
                    double stdsum = _closePriceQueue.Sum(d => Math.Pow(d - avg, 2));
                    double std = Math.Sqrt(stdsum / _closePriceQueue.Count());
                    _currentBar.MA.STD = std;
                    _currentBar.MA.MA20 = Math.Round(avg, 3, MidpointRounding.AwayFromZero);
                    
                }
            }
            while (_tickAverageQueue.Count > 20)
            {
                _tickAverageQueue.RemoveAt(0);
            }
            if (_tickAverageQueue.Count >= 5)
            {
                double sum = 0;
                for (int i = 1; i <= 5; i++)
                {
                    sum += _tickAverageQueue[_tickAverageQueue.Count - i];
                }
                _currentBar.AMA.MA5 = Math.Round(sum / 5, 3, MidpointRounding.AwayFromZero);
                if (_tickAverageQueue.Count >= 20)
                {
                    _currentBar.AMA.MA20 = Math.Round(_tickAverageQueue.Average(), 3, MidpointRounding.AwayFromZero);
                }
            }

        }

        //private TimeSpan getNextCloseTimeSpan(TimeSpan _currentOpenTimeSpan)
        //{
        //    TimeSpan closeTimeSpan = _currentOpenTimeSpan.Add(TimeSpan.FromSeconds(_inteval));

        //    if (closeTimeSpan >= new TimeSpan(13, 0, 0))
        //    {
        //        return closeTimeSpan;
        //    }
        //    else if (closeTimeSpan < new TimeSpan(11, 30, 0))
        //    {
        //        return closeTimeSpan;
        //    }
        //    else
        //    {
        //        return closeTimeSpan.Add(TimeSpan.FromHours(1.5));
        //    }

        //}
    }
}
