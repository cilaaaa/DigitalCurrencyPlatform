using System;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace StockData
{
    /// <summary>
    /// Tick数据结构
    /// </summary>
    public class TickData
    {
        SecurityInfo secInfo;
        /// <summary>
        /// 股票信息
        /// </summary>
        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        string code;
        /// <summary>
        /// 股票代码
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("股票代码")]
        [Description("股票代码")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        string name;
        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string market;
        /// <summary>
        /// 市场
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("市场")]
        [Description("市场")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Market
        {
            get { return market; }
            set { market = value; }
        }

        DateTime time;
        /// <summary>
        /// 时间
        /// </summary>
        /// 
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("时间")]
        [Description("时间")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        double preclose;
        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public double Preclose
        {
            get { return preclose; }
            set { preclose = value; }
        }
        double open;
        /// <summary>
        /// 当天开盘价
        /// </summary>
        public double Open
        {
            get { return open; }
            set { open = value; }
        }
        double high;
        /// <summary>
        /// 当天最高价格
        /// </summary>
        public double High
        {
            get { return high; }
            set { high = value; }
        }

        double bidHigh;
        /// <summary>
        /// 当天买入最高价格
        /// </summary>
        public double BidHigh
        {
            get { return bidHigh; }
            set { bidHigh = value; }
        }
        double low;
        /// <summary>
        /// 当天最低价格
        /// </summary>
        public double Low
        {
            get { return low; }
            set { low = value; }
        }
        double last;
        /// <summary>
        /// 最新价格
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("当前价")]
        [Description("当前价")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Last
        {
            get { return last; }
            set { last = value; }
        }
        double ask;
        /// <summary>
        /// 卖价
        /// </summary>
        public double Ask
        {
            get { return ask; }
            set { ask = value; }
        }
        double bid;
        /// <summary>
        /// 买家
        /// </summary>
        public double Bid
        {
            get { return bid; }
            set { bid = value; }
        }


        double volume;
        /// <summary>
        /// 成交量
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("量")]
        [Description("量")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        double amt;
        /// <summary>
        /// 成交额
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("金额")]
        [Description("金额")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Amt
        {
            get { return amt; }
            set { amt = value; }
        }
        double[] asks;
        /// <summary>
        /// 卖1至卖10价格(0-9)
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("卖1-卖10价格")]
        [Description("卖1-卖10价格")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double[] Asks
        {
            get { return asks; }
            set { asks = value; }
        }
        double[] bids;
        /// <summary>
        /// 买1至买10价格(0-9)
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("买1-买10价格")]
        [Description("买1-买10价格")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double[] Bids
        {
            get { return bids; }
            set { bids = value; }
        }
        double[] asksizes;
        /// <summary>
        /// 卖1至卖10量(0-9)
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("卖1-卖10量")]
        [Description("卖1-卖10量")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double[] Asksizes
        {
            get { return asksizes; }
            set { asksizes = value; }
        }
        double[] bidsizes;
        /// <summary>
        /// 买1至买10量(0-9)
        /// </summary>
        [Browsable(true)]
        [Category("Tick数据")]
        [DisplayName("买1-买10量")]
        [Description("买1-买10量")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double[] Bidsizes
        {
            get { return bidsizes; }
            set { bidsizes = value; }
        }

        bool isReal;
        /// <summary>
        /// 是否实时数据
        /// </summary>
        public bool IsReal
        {
            get { return isReal; }
            set { isReal = value; }
        }

        public TickData()
        {
            code = string.Empty;
            last = 0;
            preclose = 0;
            amt = 0;
            asks = new double[10];
            bids = new double[10];
            asksizes = new double[10];
            bidsizes = new double[10];
        }

        //public static bool SaveFromSource(StringBuilder Result)
        //{
        //    ArrayList strTran = new ArrayList();
        //    string[] lists = Result.ToString().Split(new char[] { '\n' });
        //    for(int i=1;i<lists.Length;i++)
        //    {
        //        string[] stocks = lists[i].Split(new char[] { '\t' });
        //        TickData td = new TickData();
        //        td.code = stocks[1];
        //        td.name = stocks[1];
        //        td.amt = System.Convert.ToDouble(stocks[12]);
        //        td.ask = System.Convert.ToDouble(stocks[18]);
        //        td.bid = System.Convert.ToDouble(stocks[17]);
        //        td.high = System.Convert.ToDouble(stocks[6]);
        //        td.last = System.Convert.ToDouble(stocks[3]);
        //        td.low = System.Convert.ToDouble(stocks[7]);
        //        td.open = System.Convert.ToDouble(stocks[5]);
        //        td.preclose = System.Convert.ToDouble(stocks[4]);
        //        td.volume = System.Convert.ToInt32(stocks[10]);
        //        td.time = PhraseTimeSpan(stocks[8]);
        //        for(int j=0;j<5;j++)
        //        {
        //            td.asks[j] = System.Convert.ToDouble(stocks[18 + 4 * j]);
        //            td.bids[j] = System.Convert.ToDouble(stocks[17 + 4 * j]);
        //            td.Asksizes[j] = System.Convert.ToInt32(stocks[20 + 4 * j]);
        //            td.bidsizes[j] = System.Convert.ToInt32(stocks[19 + 4 * j]);
        //        }
        //        for (int j = 5; j < 10; j++)
        //        {
        //            td.asks[j] = System.Convert.ToDouble(stocks[43 + 4 * (j - 5)]);
        //            td.bids[j] = System.Convert.ToDouble(stocks[42 + 4 * (j - 5)]);
        //            td.Asksizes[j] = System.Convert.ToInt32(stocks[45 + 4 * (j - 5)]);
        //            td.bidsizes[j] = System.Convert.ToInt32(stocks[44 + 4 * (j - 5)]);
        //        }
        //        strTran.Add(td.toInsertString());
        //    }
        //    try
        //    {
        //        LocalSQL.ExecuteSqlTran(strTran);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}


        private static DateTime PhraseTimeSpan(string timestring)
        {
            timestring = timestring.Insert(2, ":").Insert(5, ":");
            return System.DateTime.Now.Date.Add(TimeSpan.Parse(timestring));

        }

        public static TickData ConvertFromDataRow(DataRow dr)
        {
            TickData td = new TickData();
            td.Code = dr["tick_code"].ToString();
            //td.market = System.Convert.ToByte(dr["tick_market"]);
            td.secInfo = GlobalValue.GetFutureByCode(td.code)[0];
            td.Time = System.Convert.ToDateTime(dr["tick_time"]);
            td.Preclose = 0;
            td.Open = 0;
            td.High = 0;
            td.Low = 0; 
            td.Ask = System.Convert.ToDouble(dr["tick_ask1"]);
            td.Bid = System.Convert.ToDouble(dr["tick_bid1"]);
            td.Last = Math.Round((td.Ask + td.Bid) / 2, 2,MidpointRounding.AwayFromZero);
            td.Volume = 0;
            td.Amt = 0;

            td.isReal = false;
            for (int i = 0; i < 10; i++)
            {
                td.Asks[i] = System.Convert.ToDouble(dr[string.Format("tick_ask{0}", i + 1)]);
                td.Bids[i] = System.Convert.ToDouble(dr[string.Format("tick_bid{0}", i + 1)]);
                td.Asksizes[i] = System.Convert.ToDouble(dr[string.Format("tick_asks{0}", i + 1)]);
                td.Bidsizes[i] = System.Convert.ToDouble(dr[string.Format("tick_bids{0}", i + 1)]);
            }
            return td;
        }

        public static TickData ConvertFromArray(string[] datas)
        {
            TickData td = new TickData();
            if (datas[8] == "1")
            {
                td.code = string.Empty;
                return td;
            }
            try
            {
                td.time = PhraseTimeSpan(datas[8]);
            }
            catch
            {
                td.code = string.Empty;
                return td;
            }
            //td.market = System.Convert.ToByte(datas[0]);
            td.code = datas[1];
            td.secInfo = new SecurityInfo(td.code, string.Empty, "", 0, 0, 0, "","");
            td.name = datas[1];
            //td.amt = System.Convert.ToDouble(datas[12]);
            td.ask = System.Convert.ToDouble(datas[18]);
            td.bid = System.Convert.ToDouble(datas[17]);
            td.high = System.Convert.ToDouble(datas[6]);
            td.last = System.Convert.ToDouble(datas[3]);
            td.low = System.Convert.ToDouble(datas[7]);
            td.open = System.Convert.ToDouble(datas[5]);
            td.preclose = System.Convert.ToDouble(datas[4]);
            td.volume = System.Convert.ToInt32(datas[10]);
            td.isReal = true;
            for (int j = 0; j < 5; j++)
            {
                td.asks[j] = System.Convert.ToDouble(datas[18 + 4 * j]);
                td.bids[j] = System.Convert.ToDouble(datas[17 + 4 * j]);
                td.Asksizes[j] = System.Convert.ToInt32(datas[20 + 4 * j]);
                td.bidsizes[j] = System.Convert.ToInt32(datas[19 + 4 * j]);
            }
            for (int j = 5; j < 10; j++)
            {
                td.asks[j] = System.Convert.ToDouble(datas[43 + 4 * (j - 5)]);
                td.bids[j] = System.Convert.ToDouble(datas[42 + 4 * (j - 5)]);
                td.Asksizes[j] = System.Convert.ToInt32(datas[45 + 4 * (j - 5)]);
                td.bidsizes[j] = System.Convert.ToInt32(datas[44 + 4 * (j - 5)]);
            }
            return td;
        }

        public double getOpenPrice(OpenType openType)
        {
            if (this.bid == 0 && this.ask != 0) //跌停
            {
                if (openType == OpenType.Buy)
                {
                    return this.ask;
                }
                else
                {
                    return this.ask;
                }
            }
            else if (this.bid != 0 && this.ask == 0) //涨停
            {
                if (openType == OpenType.Sell)
                {
                    return this.bid;
                }
                else
                {
                    return this.bid;
                }
            }
            else
            {
                double gap = Math.Round(this.ask - this.bid, 2, MidpointRounding.AwayFromZero);
                if (gap == 0.01) //无跳价
                {
                    if (openType == OpenType.Buy)
                    {
                        return this.ask;
                    }
                    else
                    {
                        return this.bid;
                    }
                }
                else
                {
                    return Math.Round(this.ask - gap / 2, 2, MidpointRounding.AwayFromZero);
                }
            }
        }
    }
}
