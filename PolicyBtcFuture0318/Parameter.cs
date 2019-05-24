using StockData;
using StockPolicies;
using StockTradeAPI;
using System.ComponentModel;

namespace PolicyBtcFuture0318
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

        private string code;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("套币代码")]
        [Description("套币代码")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        private string code2;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("套币2代码")]
        [Description("套币2代码")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Code2
        {
            get { return code2; }
            set { code2 = value; }
        }

        private string secondMarket;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("另一个市场ID")]
        [Description("另一个市场ID")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string SecondMarket
        {
            get { return secondMarket; }
            set { secondMarket = value; }
        }

        private string secondCode;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("另一个市场币种代码")]
        [Description("另一个市场币种代码")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string SecondCode
        {
            get { return secondCode; }
            set { secondCode = value; }
        }

        private double secondFee;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("另一个市场手续费")]
        [Description("另一个市场手续费")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double SecondFee
        {
            get { return secondFee; }
            set { secondFee = value; }
        }

        private double lirun;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("利润")]
        [Description("利润")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Lirun
        {
            get { return lirun; }
            set { lirun = value; }
        }

        private double huaDian;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("滑点")]
        [Description("滑点")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double HuaDian
        {
            get { return huaDian; }
            set { huaDian = value; }
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
            this.ProgramName = "PolicyBtcFuture0318.DLL";
            this.qty = 0.1;
            this.secondMarket = "fcoin";
            this.secondCode = "eosusdt";
            this.secondFee = 0.0005;
            this.fee = 0.0003;
            this.lirun = 0.0005;
            this.EnterOrderPriceType = TradeSendOrderPriceType.Limit;
            this.EnterOrderWaitSecond = 1;
            this.code = "eos";
            this.code2 = "usdt";
            this.huaDian = 0.001;
        }

        internal void Save()
        {
        }
    }
}

