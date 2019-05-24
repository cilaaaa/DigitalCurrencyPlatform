using StockData;
using StockPolicies;
using System;
using System.ComponentModel;

namespace PolicyBtcFuture1225
{
    public class Parameter : PolicyParameter
    {
        private bool debug;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("调试模式")]
        [Description("调试模式")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        private TimeSpan startTime;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("开始时间")]
        [Description("策略执行开始时间")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public TimeSpan StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        private TimeSpan endTime;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("结束时间")]
        [Description("策略执行结束时间")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public TimeSpan EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private Decimal totalHands;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("总购买手数")]
        [Description("BCIN")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Decimal TotalHands
        {
            get { return totalHands; }
            set { totalHands = value; }
        }

        private int barInteval;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("bar间隔，秒")]
        [Description("BCIN")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarInteval
        {
            get { return barInteval; }
            set { barInteval = value; }
        }

        private int barCountIn;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("入场BAR数量")]
        [Description("入场BAR数量")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountIn
        {
            get { return barCountIn; }
            set { barCountIn = value; }
        }

        private string codeName;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("代号")]
        [Description("代号")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string CodeName
        {
            get { return codeName; }
            set { codeName = value; }
        }

        private double ratio;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("通道窄幅")]
        [Description("通道窄幅")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Ratio
        {
            get { return ratio; }
            set { ratio = value; }
        }

        private double stoploss;
        [Browsable(false)]
        //[Browsable(true)]
        //[Category("3-出场参数")]
        //[DisplayName("止损点")]
        //[Description("止损百分比")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        public double Stoploss
        {
            get { return stoploss; }
            set { stoploss = value; }
        }

        private int stopLossPercent;
        [Browsable(false)]
        //[Browsable(true)]
        //[Category("3-出场参数")]
        //[DisplayName("止损大单减少到原来的百分比")]
        //[Description("值为20表示止损大单的数量小于原先的20%止损")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        public int StopLossPercent
        {
            get { return stopLossPercent; }
            set { stopLossPercent = value; }
        }


        public bool save;


        public Parameter()
        {
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = false;
        }


        public Parameter(SecurityInfo si, PolicyParameter pp, string account)
            : this(si, account, "Parameter.xml")
        {
            this.startDate = pp.StartDate;
            this.endDate = pp.EndDate;
            this.inteval = 0;
            this.isReal = false;

        }
        public Parameter(SecurityInfo si, string account, string filename)
        {
            GetGeneralValue();
            this.codeName = "1";
            this.debug = true;
            this.startTime = new TimeSpan(0, 0, 0);
            this.EndTime = new TimeSpan(23, 59, 59);
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = true;
            this.SI = si;
            this.Account = account;
            this.Filename = filename;
            this.ProgramName = "PolicyBtcFuture1225.DLL";
            this.totalHands = 1;
            this.qty = 1;
            this.fee = 0.0003;
            this.ReEnterPercent = -1;
            this.EnterOrderWaitSecond = 1000;
            this.CloseOrderWaitSecond = 1000;
            this.barInteval = 60;
            this.barCountIn = 14;
            //this.barCountOut = 5;
            this.ratio = 9.8;
        }

        internal void Save()
        {
        }
    }
}

