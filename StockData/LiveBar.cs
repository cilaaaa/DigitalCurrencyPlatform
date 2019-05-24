using System;

namespace StockData
{
    public class LiveBar
    {
        

        MAValue _MA;
        /// <summary>
        /// 收盘价MA
        /// </summary>
        public MAValue MA
        {
            get { return _MA; }
            set { _MA = value; }
        }
        
        MAValue _AMA;
        /// <summary>
        /// Tick均价MA
        /// </summary>
        public MAValue AMA
        {
            get { return _AMA; }
            set { _AMA = value; }
        }
        double _average;

        public double Average
        {
            get { return _average; }
            set { _average = value; }
        }

        double _open;

        public double Open
        {
            get { return _open; }
            set { _open = value; }
        }

        double _close;

        public double Close
        {
            get { return _close; }
            set { _close = value; }
        }

        double _high;

        public double High
        {
            get { return _high; }
            set { _high = value; }
        }

        double _low;

        public double Low
        {
            get { return _low; }
            set { _low = value; }
        }

        double _preClose;

        public double PreClose
        {
            get { return _preClose; }
            set { _preClose = value; }
        }

        double _volumn;

        public double Volumn
        {
            get { return _volumn; }
            set { _volumn = value; }
        }

        double _estVolumn;

        public double EstVolumn
        {
            get { return _estVolumn; }
            set { _estVolumn = value; }
        }

        AbsoluteBarType _absoluteRaiseType;

        public AbsoluteBarType AbsoluteRaiseType
        {
            get { return _absoluteRaiseType; }
            set { _absoluteRaiseType = value; }
        }

        BarType _relativeRaiseType;

        public BarType RelativeRaiseType
        {
            get { return _relativeRaiseType; }
            set { _relativeRaiseType = value; }
        }

        TimeSpan _barOpenTime;

        public TimeSpan BarOpenTime
        {
            get { return _barOpenTime; }
            set { _barOpenTime = value; }
        }

        TimeSpan _barCloseTime;

        public TimeSpan BarCloseTime
        {
            get { return _barCloseTime; }
            set { _barCloseTime = value; }
        }


        TimeSpan _nextOpenTime;

        public TimeSpan NextOpenTime
        {
            get { return _nextOpenTime; }
            set { _nextOpenTime = value; }
        }

        TimeSpan _lastTickTime;

        public TimeSpan LastTickTime
        {
            get { return _lastTickTime; }
            set { _lastTickTime = value; }
        }

        DateTime _barTime;

        public DateTime BarTime
        {
            get { return _barTime; }
            set { _barTime = value; }
        }

        //TimeSpan _barTime;

        //public TimeSpan BarTime
        //{
        //    get { return _barTime; }
        //    set { _barTime = value; }
        //}
        bool _finish;
        public bool Finish
        {
            get { return _finish; }
            set { _finish = value; }
        }

        LiveBar _preBar;

        public LiveBar PreBar
        {
            get { return _preBar; }
            set { _preBar = value; }
        }


        internal void Reset()
        {
            _open = 0;
            _close = 0;
            _high = 0;
            _low = 0;
            _estVolumn = 0;
            _volumn = 0;
            _preClose = 0;
            _finish = false;
            MA = new MAValue();
            AMA = new MAValue();
        }
        public LiveBar()
        {
            _finish = false;
            MA = new MAValue();
            AMA = new MAValue();
        }
    }
}
