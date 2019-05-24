using StockData;
using StockPolicies;
using System.ComponentModel;

namespace PolicyBtc0601
{
    public class Parameter : PolicyParameter
    {
        private string bz1;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("套利币种1")]
        [Description("套利币种1")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Bz1
        {
            get { return bz1; }
            set { bz1 = value; }
        }

        private string bz2;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("套利币种2")]
        [Description("套利币种2")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string Bz2
        {
            get { return bz2; }
            set { bz2 = value; }
        }

        private double huadian;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("滑点")]
        [Description("滑点")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Huadian
        {
            get { return huadian; }
            set { huadian = value; }
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
            this.startDate = System.DateTime.Now.Date;
            this.endDate = System.DateTime.Now.Date;
            this.inteval = 0;
            this.isReal = true;
            this.save = true;
            this.SI = si;
            this.Account = account;
            this.Filename = filename;
            this.ProgramName = "PolicyBtc0601.DLL";
            this.Bz1 = "EOS-USDT";
            this.Bz2 = "EOS-BTC";
            this.qty = 1.5;
            this.fee = 0.0003;
            this.lirun = 0.0003;
            this.ReEnterPercent = 0.1;
            this.EnterOrderWaitSecond = 1;
            this.CloseOrderWaitSecond = 2000;
        }

        internal void Save()
        {
        }
    }
}

