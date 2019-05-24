using DataBase;
using StockPolicies;
using System.ComponentModel;
using System.Windows.Forms;

namespace PolicyBtcFuture0111
{
    public class BackTestParameter : PolicyBackTestParameter
    {
        private int barInterval;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("分钟bar")]
        [Description("值")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarInterval
        {
            get { return barInterval; }
            set { barInterval = value; }
        }

        private int timeCycleFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间周期")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int TimeCycleFrom
        {
            get { return timeCycleFrom; }
            set { timeCycleFrom = value; }
        }

        private int timeCycleTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间周期")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int TimeCycleTo
        {
            get { return timeCycleTo; }
            set { timeCycleTo = value; }
        }

        private int timeCycleStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间周期")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int TimeCycleStep
        {
            get { return timeCycleStep; }
            set { timeCycleStep = value; }
        }

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

        public BackTestParameter()
        {
            IniFile ini = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "backtest.ini"));
            string section = "PolicyBtcFuture0111";
            //this.isReal = false;
            this.inteval = 0;
            this.startDate = System.Convert.ToDateTime(ini.GetString(section, "startdate", this.startDate.ToString("yyyy-MM-dd"))).Date;
            this.endDate = System.Convert.ToDateTime(ini.GetString(section, "enddate", this.endDate.ToString("yyyy-MM-dd"))).Date;

            int timeCycleFrom;
            try
            {
                timeCycleFrom = System.Convert.ToInt16(ini.GetString(section, "timeCycleFrom", "12"));
            }
            catch
            {
                timeCycleFrom = 12;
            }
            this.timeCycleFrom = timeCycleFrom;

            int timeCycleTo;
            try
            {
                timeCycleTo = System.Convert.ToInt16(ini.GetString(section, "timeCycleTo", "24"));
            }
            catch
            {
                timeCycleTo = 24;
            }
            this.timeCycleTo = timeCycleTo;

            int timeCycleStep;
            try
            {
                timeCycleStep = System.Convert.ToInt16(ini.GetString(section, "timeCycleStep", "12"));
            }
            catch
            {
                timeCycleStep = 12;
            }
            this.timeCycleStep = timeCycleStep;

            int barCountFrom;
            try
            {
                barCountFrom = System.Convert.ToInt16(ini.GetString(section, "barCountFrom", "30"));
            }
            catch
            {
                barCountFrom = 30;
            }
            this.barCountFrom = barCountFrom;

            int barCountTo;
            try
            {
                barCountTo = System.Convert.ToInt16(ini.GetString(section, "barCountTo", "180"));
            }
            catch
            {
                barCountTo = 180;
            }
            this.barCountTo = barCountTo;

            int barCountStep;
            try
            {
                barCountStep = System.Convert.ToInt16(ini.GetString(section, "barCountStep", "30"));
            }
            catch
            {
                barCountStep = 30;
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
                zhiYingBeiShuTo = System.Convert.ToInt16(ini.GetString(section, "zhiYingBeiShuTo", "0.05"));
            }
            catch
            {
                zhiYingBeiShuTo = 0.05;
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
            this.Qty = 1000;
            this.Fee = 0.0003;
            this.StartValue = 1;
            this.barInterval = 60;
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
