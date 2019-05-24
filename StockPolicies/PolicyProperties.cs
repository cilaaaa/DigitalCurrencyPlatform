
namespace StockPolicies
{
    public class PolicyProperties
    {
        bool isLianDong;

        public bool IsLianDong
        {
            get { return isLianDong; }
            set { isLianDong = value; }
        }
        bool isSim;

        public bool IsSim
        {
            get { return isSim; }
            set { isSim = value; }
        }
        string account;

        public string Account
        {
            get { return account; }
            set { account = value; }
        }

    }
}
