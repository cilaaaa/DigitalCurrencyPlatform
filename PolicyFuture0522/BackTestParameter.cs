using DataBase;
using StockPolicies;
using System.ComponentModel;
using System.Windows.Forms;

namespace PolicyFuture0522
{
    public class BackTestParameter : PolicyBackTestParameter
    {

        private int barCountFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开始周期")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountFrom
        {
            get { return barCountFrom; }
            set { barCountFrom = value; }
        }

        private int barCountTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开始周期")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountTo
        {
            get { return barCountTo; }
            set { barCountTo = value; }
        }

        private int barCountStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("开始周期")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountStep
        {
            get { return barCountStep; }
            set { barCountStep = value; }
        }

        private double zhiYingBeiShuFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("止盈倍数")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiYingBeiShuFrom
        {
            get { return zhiYingBeiShuFrom; }
            set { zhiYingBeiShuFrom = value; }
        }

        private double zhiYingBeiShuTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("止盈倍数")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiYingBeiShuTo
        {
            get { return zhiYingBeiShuTo; }
            set { zhiYingBeiShuTo = value; }
        }

        private double zhiYingBeiShuStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("止盈倍数")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiYingBeiShuStep
        {
            get { return zhiYingBeiShuStep; }
            set { zhiYingBeiShuStep = value; }
        }

        private double huiCheBiLiFrom;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("回撤比例")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double HuiCheBiLiFrom
        {
            get { return huiCheBiLiFrom; }
            set { huiCheBiLiFrom = value; }
        }

        private double huiCheBiLiTo;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("回撤比例")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double HuiCheBiLiTo
        {
            get { return huiCheBiLiTo; }
            set { huiCheBiLiTo = value; }
        }

        private double huiCheBiLiStep;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("回撤比例")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double HuiCheBiLiStep
        {
            get { return huiCheBiLiStep; }
            set { huiCheBiLiStep = value; }
        }

        private double zhiSunBiLiFrom;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("止损比例")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiSunBiLiFrom
        {
            get { return zhiSunBiLiFrom; }
            set { zhiSunBiLiFrom = value; }
        }

        private double zhiSunBiLiTo;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("止损比例")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiSunBiLiTo
        {
            get { return zhiSunBiLiTo; }
            set { zhiSunBiLiTo = value; }
        }

        private double zhiSunBiLiStep;

        [Browsable(true)]
        [Category("3-出场参数")]
        [DisplayName("止损比例")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double ZhiSunBiLiStep
        {
            get { return zhiSunBiLiStep; }
            set { zhiSunBiLiStep = value; }
        }

        public BackTestParameter()
        {
            IniFile ini = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "backtest.ini"));
            string section = "PolicyFuture0522";
            //this.isReal = false;
            this.inteval = 0;
            this.startDate = System.Convert.ToDateTime(ini.GetString(section, "startdate", this.startDate.ToString("yyyy-MM-dd"))).Date;
            this.endDate = System.Convert.ToDateTime(ini.GetString(section, "enddate", this.endDate.ToString("yyyy-MM-dd"))).Date;


            int barCountFrom;
            try
            {
                barCountFrom = System.Convert.ToInt16(ini.GetString(section, "barCountFrom", "10"));
            }
            catch
            {
                barCountFrom = 10;
            }
            this.barCountFrom = barCountFrom;

            int barCountTo;
            try
            {
                barCountTo = System.Convert.ToInt16(ini.GetString(section, "barCountTo", "100"));
            }
            catch
            {
                barCountTo = 100;
            }
            this.barCountTo = barCountTo;

            int barCountStep;
            try
            {
                barCountStep = System.Convert.ToInt16(ini.GetString(section, "barCountStep", "10"));
            }
            catch
            {
                barCountStep = 10;
            }
            this.barCountStep = barCountStep;

            double zhiYingBeiShuFrom;
            try
            {
                zhiYingBeiShuFrom = System.Convert.ToInt16(ini.GetString(section, "zhiYingBeiShuFrom", "0.01"));
            }
            catch
            {
                zhiYingBeiShuFrom = 0.01;
            }
            this.zhiYingBeiShuFrom = zhiYingBeiShuFrom;

            double zhiYingBeiShuTo;
            try
            {
                zhiYingBeiShuTo = System.Convert.ToInt16(ini.GetString(section, "zhiYingBeiShuTo", "0.1"));
            }
            catch
            {
                zhiYingBeiShuTo = 0.1;
            }
            this.zhiYingBeiShuTo = zhiYingBeiShuTo;

            double zhiYingBeiShuStep;
            try
            {
                zhiYingBeiShuStep = System.Convert.ToInt16(ini.GetString(section, "zhiYingBeiShuStep", "0.01"));
            }
            catch
            {
                zhiYingBeiShuStep = 0.01;
            }
            this.zhiYingBeiShuStep = zhiYingBeiShuStep;

            double huiCheBiLiFrom;
            try
            {
                huiCheBiLiFrom = System.Convert.ToInt16(ini.GetString(section, "huiCheBiLiFrom", "0.1"));
            }
            catch
            {
                huiCheBiLiFrom = 0.1;
            }
            this.huiCheBiLiFrom = huiCheBiLiFrom;

            double huiCheBiLiTo;
            try
            {
                huiCheBiLiTo = System.Convert.ToInt16(ini.GetString(section, "huiCheBiLiTo", "0.6"));
            }
            catch
            {
                huiCheBiLiTo = 0.6;
            }
            this.huiCheBiLiTo = huiCheBiLiTo;

            double huiCheBiLiStep;
            try
            {
                huiCheBiLiStep = System.Convert.ToInt16(ini.GetString(section, "huiCheBiLiStep", "0.1"));
            }
            catch
            {
                huiCheBiLiStep = 0.1;
            }
            this.huiCheBiLiStep = huiCheBiLiStep;

            double zhiSunBiLiFrom;
            try
            {
                zhiSunBiLiFrom = System.Convert.ToInt16(ini.GetString(section, "zhiSunBiLiFrom", "0.01"));
            }
            catch
            {
                zhiSunBiLiFrom = 0.01;
            }
            this.zhiSunBiLiFrom = zhiSunBiLiFrom;

            double zhiSunBiLiTo;
            try
            {
                zhiSunBiLiTo = System.Convert.ToInt16(ini.GetString(section, "zhiSunBiLiTo", "0.1"));
            }
            catch
            {
                zhiSunBiLiTo = 0.1;
            }
            this.zhiSunBiLiTo = zhiSunBiLiTo;

            double zhiSunBiLiStep;
            try
            {
                zhiSunBiLiStep = System.Convert.ToInt16(ini.GetString(section, "zhiSunBiLiStep", "0.01"));
            }
            catch
            {
                zhiSunBiLiStep = 0.01;
            }
            this.zhiSunBiLiStep = zhiSunBiLiStep;

            this.Qty = 1;
            this.Fee = 0.0003;
            this.StartValue = 1;
            //this.largeCountFrom = ini.GetInt(section, "largeCountFrom", 2);
            //this.largeCountTo = ini.GetInt(section, "largeCountTo", 3);
            //this.largeCountStep = ini.GetInt(section, "largeCountStep", 1);
            //this.largePercentFrom = ini.GetInt(section, "largePercentFrom", 80);
            //this.largePercenTo = ini.GetInt(section, "largePercentTo", 120);
            //this.largePercentStep = ini.GetInt(section, "largePercentSetp", 20);
            //this.stopLossPercentFrom = ini.GetInt(section, "stopLossPercentFrom", 30);
            //this.stopLossPercentTo = ini.GetInt(section, "stopLossPercentTo", 50);
            //this.stopLossPercentStep = ini.GetInt(section, "stopLossPercentStep", 10);

        }
    }
}
