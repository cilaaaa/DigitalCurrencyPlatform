
namespace StockTradeAPI
{
    public class KeCheArgs
    {
        int openDuoIndex;
        public int OpenDuoIndex
        {
            get { return openDuoIndex; }
            set { openDuoIndex = value; }
        }

        int openKongIndex;
        public int OpenKongIndex
        {
            get { return openKongIndex; }
            set { openKongIndex = value; }
        }

        string policyName;
        public string PolicyName
        {
            get { return policyName; }
            set { policyName = value; }
        }

    }
}
