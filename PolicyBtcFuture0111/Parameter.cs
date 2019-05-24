using StockData;
using StockPolicies;
using System.ComponentModel;

namespace PolicyBtcFuture0111
{
    public class Parameter : PolicyParameter
    {
        private bool debugModel;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("回测模式")]
        [Description("回测模式")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool DebugModel
        {
            get { return debugModel; }
            set { debugModel = value; }
        }

        private int barCount;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开盘计算BAR数量")]
        [Description("开盘计算BAR数量")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCount
        {
            get { return barCount; }
            set { barCount = value; }
        }

        private int timeCycle;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间周期")]
        [Description("时间周期")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int TimeCycle
        {
            get { return timeCycle; }
            set { timeCycle = value; }
        }

        private int barInteval;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("bar间隔，秒")]
        [Description("bar间隔，秒")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarInteval
        {
            get { return barInteval; }
            set { barInteval = value; }
        }

        private int jiaCangCiShu;

        [Browsable(true)]
        [Category("3-加仓参数")]
        [DisplayName("加仓次数")]
        [Description("加仓次数")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int JiaCangCiShu
        {
            get { return jiaCangCiShu; }
            set { jiaCangCiShu = value; }
        }

        private double jiaCangJiaGeBeiShu;
        [Browsable(true)]
        [Category("3-加仓参数")]
        [DisplayName("加仓价格倍数")]
        [Description("加仓价格倍数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double JiaCangJiaGeBeiShu
        {
            get { return jiaCangJiaGeBeiShu; }
            set { jiaCangJiaGeBeiShu = value; }
        }

        private double zhiYingBeiShu;
        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开启止盈的倍数")]
        [Description("开启止盈的倍数")]
        [EditorBrowsable(EditorBrowsableState.Always)]

        public double ZhiYingBeiShu
        {
            get { return zhiYingBeiShu; }
            set { zhiYingBeiShu = value; }
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


        public  bool save;


        public Parameter()
        {
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = false;
        }

        
        public Parameter(SecurityInfo si, PolicyParameter pp,string account)
            : this(si,account,"Parameter.xml")
        {
            this.startDate = pp.StartDate;
            this.endDate = pp.EndDate;
            this.inteval = 0;
            this.isReal = false;

        }
        public Parameter(SecurityInfo si,string account,string filename)
        {
            GetGeneralValue();
            this.debugModel = true;
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = true;
            this.SI = si;
            this.Account = account;
            this.Filename = filename;
            this.ProgramName = "PolicyBtcFuture0111.DLL";
            this.qty = 1;
            this.zhiYingBeiShu = 0.04;
            this.timeCycle = 24;
            this.BarCount = 60;
            this.barInteval = 60;
            this.fee = 0.0003;
            this.jiaCangCiShu = 3;
            this.jiaCangJiaGeBeiShu = 0.5;
        }

        internal void Save()
        {
        }
    }
}

