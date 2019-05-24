using System;
using System.ComponentModel;

namespace StockPolicies
{
    public class PolicyBackTestParameter
    {
        protected int inteval;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("Tick间隔")]
        [Description("策略执行Tick间隔")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int Inteval
        {
            get { return inteval; }
            set { inteval = value; }
        }
        protected DateTime startDate;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("开始日期")]
        [Description("策略执行开始日期")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        protected DateTime endDate;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("结束日期")]
        [Description("策略执行结束日期")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }



        private double qty = 0.1;
        [Browsable(true)]
        [Category("X-交易参数")]
        [DisplayName("单次交易股数")]
        [Description("单次交易股数")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        private double fee = 0.16;
        [Browsable(true)]
        [Category("X-交易参数")]
        [DisplayName("交易手续费")]
        [Description("双向交易手续费")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Fee
        {
            get { return fee; }
            set { fee = value; }
        }

        private int startValue;
        [Browsable(true)]
        [Category("X-交易参数")]
        [DisplayName("起始金额（元）")]
        [Description("起始金额")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int StartValue
        {
            get { return startValue; }
            set { startValue = value; }
        }
    }
}
