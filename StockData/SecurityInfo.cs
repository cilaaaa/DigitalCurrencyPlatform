using System;

namespace StockData
{
    public class SecurityInfo
    {
        public static TimeSpan TodayFirstTime = new TimeSpan(0, 0, 0);
        //public static TimeSpan[] CloseTime = new TimeSpan[3] {new TimeSpan(14,53,0),new TimeSpan(14,58,0) ,new TimeSpan(23,58,0)};
        //public static TimeSpan[] OpenStartTime = new TimeSpan[3] { new TimeSpan(9, 35, 0), new TimeSpan(9, 35, 0),new TimeSpan(0,5,0) };
        //public static TimeSpan[] OpenEndTime = new TimeSpan[3] { new TimeSpan(14, 50, 0), new TimeSpan(14, 57, 0) ,new TimeSpan(23,58,0)};
        //public static TimeSpan[] NormalStartTime = new TimeSpan[3] { new TimeSpan(9, 31, 0), new TimeSpan(9, 31, 0),new TimeSpan(0,1,0) };

        string name;

        MarketTimeRange timeRange;

        public MarketTimeRange TimeRange
        {
            get { return timeRange; }
        }

        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }
        string code;

        public string Code
        {
            get { return code; }
        }

        string market;

        public string Market
        {
            get { return market; }
        }

        string type;

        public string Type
        {
            get { return type; }
        }

        double minQty;

        public double MinQty
        {
            get { return minQty; }
        }

        int jingDu;

        public int JingDu
        {
            get { return jingDu; }
        }

        double priceJingDu;

        public double PriceJingDu
        {
            get { return priceJingDu; }
        }

        string contractVal;

        public string ContractVal
        {
            get { return contractVal; }
        }
        //public SecurityInfo()
        //{
        //    name = string.Empty;
        //    code = string.Empty;
        //    Market = 0;
        //}
        public SecurityInfo(string _code, string _name, string _market, double _minQty, int _jingdu, double _priceJingDu, string _type,string _contractVal)
        {
            this.name = _name;
            this.code = _code;
            this.market = _market;
            this.minQty = _minQty;
            this.jingDu = _jingdu;
            this.priceJingDu = _priceJingDu;
            this.type = _type;
            this.contractVal = _contractVal;
            timeRange = MarketTimeRange.getTimeRange(_market);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 9点35开始
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        //public bool isInOpenTime(TimeSpan ts)
        //{
        //    if(ts >= OpenStartTime[System.Convert.ToInt32(this.Market)] && ts <= OpenEndTime[System.Convert.ToInt32(this.Market)])
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        /// <summary>
        /// 9点31分开始
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public bool isInNormalOpenTIme(TimeSpan ts)
        {
            if (ts >= timeRange.PolicyOpenStartTime && ts <= timeRange.PolicyOpenEndTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isInCloseTime(TimeSpan ts)
        {
            if (ts >= timeRange.ForceCloseTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            try
            {
                SecurityInfo si = (SecurityInfo)obj;
                if (si.code == this.code)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public string Key
        {
            get
            {
                return string.Format("{0}{1}", this.code, this.Market);
            }
        }

        public int calculateSeconds(TimeSpan start, TimeSpan end)
        {
            if (end <= start)
            {
                return 0;
            }
            int startPosition = -1;
            int endPosition = -1;

            for (int i = 0; i < this.timeRange.TimeRanges.Count; i++)
            {
                TimeRange tr = this.timeRange.TimeRanges[i];
                if (startPosition == -1)
                {
                    if (start < tr.Start)
                    {
                        start = tr.Start;
                        startPosition = i;
                    }
                    else if (start <= tr.End)
                    {
                        startPosition = i;
                    }
                }
                if (endPosition == -1)
                {
                    if (end < tr.Start)
                    {
                        end = tr.Start;
                        endPosition = i;
                    }
                    else if (end <= tr.End)
                    {
                        endPosition = i;
                    }
                }
                if (startPosition != -1 && endPosition != -1)
                {
                    break;
                }
            }
            if (startPosition == -1)
            {
                return 0;
            }
            else if (endPosition == -1)
            {
                endPosition = timeRange.TimeRanges.Count - 1;
                end = timeRange.EndTime;
            }


            int totalSeconds = System.Convert.ToInt32((end - start).TotalSeconds);
            for (int i = startPosition; i < endPosition; i++)
            {
                totalSeconds -= System.Convert.ToInt32(this.timeRange.TimeRanges[i].ToNextOpenHour * 60 * 60);
            }

            return totalSeconds;
        }

        public bool isLive(TimeSpan timeSpan)
        {
            foreach (var timerange in this.timeRange.TimeRanges)
            {
                if (timeSpan >= timerange.Start && timeSpan <= timerange.End)
                {
                    return true;
                }
            }
            return false;
        }

        public int calculateMinutes(TimeSpan timeSpan)
        {
            return calculateSeconds(this.timeRange.StartTime, timeSpan) / 60;
        }

        public int calculateSeconds(TimeSpan timeSpan)
        {
            return calculateSeconds(this.timeRange.StartTime, timeSpan);
        }
    }
}
