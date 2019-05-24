using StockData;

namespace StockPolicies
{
    //策略的结果和事件参数的类
    public class PolicyFunCGetEventArgs
    {
        SecurityInfo secInfo;

        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        string funName;

        public string FunName
        {
            get { return funName; }
            set { funName = value; }
        }

        object[] parameters;

        public object[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        object policyObject;

        public object PolicyObject
        {
            get { return policyObject; }
            set { policyObject = value; }
        }
    }
}
