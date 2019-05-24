
namespace StockPolicies
{
    //策略的结果和事件参数的类
    public class PolicyParamEventArgs
    {
        string paramName;

        public string ParamName
        {
            get { return paramName; }
            set { paramName = value; }
        }

        string paramValue;

        public string ParamValue
        {
            get { return paramValue; }
            set { paramValue = value; }
        }

    }
}
