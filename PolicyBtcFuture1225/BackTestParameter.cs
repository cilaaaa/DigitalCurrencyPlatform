using DataBase;
using StockPolicies;
using System.ComponentModel;
using System.Windows.Forms;

namespace PolicyBtcFuture1225
{
    public class BackTestParameter : PolicyBackTestParameter  {

        private int barIntevalFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间间隔")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarIntevalFrom
        {
            get { return barIntevalFrom; }
            set { barIntevalFrom = value; }
        }

        private int barIntevalTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间间隔")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarIntevalTo
        {
            get { return barIntevalTo; }
            set { barIntevalTo = value; }
        }

        private int barIntevalStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("时间间隔")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarIntevalStep
        {
            get { return barIntevalStep; }
            set { barIntevalStep = value; }
        }

        private int barCountInFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("进场Bar数量")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountInFrom
        {
            get { return barCountInFrom; }
            set { barCountInFrom = value; }
        }

        private int barCountInTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("进场Bar数量")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountInTo
        {
            get { return barCountInTo; }
            set { barCountInTo = value; }
        }

        private int barCountInStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("进场Bar数量")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountInStep
        {
            get { return barCountInStep; }
            set { barCountInStep = value; }
        }

        private int barCountOutFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("出场Bar数量")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountOutFrom
        {
            get { return barCountOutFrom; }
            set { barCountOutFrom = value; }
        }

        private int barCountOutTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("出场Bar数量")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountOutTo
        {
            get { return barCountOutTo; }
            set { barCountOutTo = value; }
        }

        private int barCountOutStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("出场Bar数量")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int BarCountOutStep
        {
            get { return barCountOutStep; }
            set { barCountOutStep = value; }
        }

        private double ratioFrom;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("斜率")]
        [Description("从")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double RatioFrom
        {
            get { return ratioFrom; }
            set { ratioFrom = value; }
        }

        private double ratioTo;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("斜率")]
        [Description("至")]
        //[DisplayName("连续阴线下跌的百分比")]
        //[Description("监控连续阴线下跌的百分比")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double RatioTo
        {
            get { return ratioTo; }
            set { ratioTo = value; }
        }

        private double ratioStep;

        [Browsable(true)]
        [Category("2-入场参数")]
        [DisplayName("斜率")]
        [Description("步长")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double RatioStep
        {
            get { return ratioStep; }
            set { ratioStep = value; }
        }

        public BackTestParameter()
        {
            IniFile ini = new IniFile(string.Format("{0}\\{1}",Application.StartupPath,"backtest.ini"));
            string section = "PolicyBtcFuture1108";
            //this.isReal = false;
            this.inteval = 0;
            this.startDate = System.Convert.ToDateTime(ini.GetString(section, "startdate", this.startDate.ToString("yyyy-MM-dd"))).Date;
            this.endDate = System.Convert.ToDateTime(ini.GetString(section, "enddate", this.endDate.ToString("yyyy-MM-dd"))).Date;

            int barIntevalFrom;
            try
            {
                barIntevalFrom = System.Convert.ToInt16(ini.GetString(section, "barIntevalFrom", "900"));
            }
            catch
            {
                barIntevalFrom = 900;
            }
            this.barIntevalFrom = barIntevalFrom;

            int barIntevalTo;
            try
            {
                barIntevalTo = System.Convert.ToInt16(ini.GetString(section, "barIntevalTo", "3600"));
            }
            catch
            {
                barIntevalTo = 3600;
            }
            this.barIntevalTo = barIntevalTo;

            int barIntevalStep;
            try
            {
                barIntevalStep = System.Convert.ToInt16(ini.GetString(section, "barIntevalStep", "900"));
            }
            catch
            {
                barIntevalStep = 900;
            }
            this.barIntevalStep = barIntevalStep;

            int barCountInFrom;
            try
            {
                barCountInFrom = System.Convert.ToInt16(ini.GetString(section, "barCountInFrom", "10"));
            }
            catch
            {
                barCountInFrom = 10;
            }
            this.barCountInFrom = barCountInFrom;

            int barCountInTo;
            try
            {
                barCountInTo = System.Convert.ToInt16(ini.GetString(section, "barCountInTo", "30"));
            }
            catch
            {
                barCountInTo = 30;
            }
            this.barCountInTo = barCountInTo;

            int barCountInStep;
            try
            {
                barCountInStep = System.Convert.ToInt16(ini.GetString(section, "barCountInStep", "5"));
            }
            catch
            {
                barCountInStep = 5;
            }
            this.barCountInStep = barCountInStep;

            int barCountOutFrom;
            try
            {
                barCountOutFrom = System.Convert.ToInt16(ini.GetString(section, "barCountOutFrom", "5"));
            }
            catch
            {
                barCountOutFrom = 5;
            }
            this.barCountOutFrom = barCountOutFrom;

            int barCountOutTo;
            try
            {
                barCountOutTo = System.Convert.ToInt16(ini.GetString(section, "barCountOutTo", "30"));
            }
            catch
            {
                barCountOutTo = 30;
            }
            this.barCountOutTo = barCountOutTo;

            int barCountOutStep;
            try
            {
                barCountOutStep = System.Convert.ToInt16(ini.GetString(section, "barCountOutStep", "5"));
            }
            catch
            {
                barCountOutStep = 5;
            }
            this.barCountOutStep = barCountOutStep;

            double ratioFrom;
            try
            {
                ratioFrom = System.Convert.ToInt16(ini.GetString(section, "ratioFrom", "1"));
            }
            catch
            {
                ratioFrom = 1;
            }
            this.ratioFrom = ratioFrom;

            double ratioTo;
            try
            {
                ratioTo = System.Convert.ToInt16(ini.GetString(section, "ratioTo", "10"));
            }
            catch
            {
                ratioTo = 10;
            }
            this.ratioTo = ratioTo;

            double ratioStep;
            try
            {
                ratioStep = System.Convert.ToInt16(ini.GetString(section, "ratioStep", "0.5"));
            }
            catch
            {
                ratioStep = 0.5;
            }
            this.ratioStep = ratioStep;

            this.Qty = 1000;
            this.Fee = 0.00075;
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
