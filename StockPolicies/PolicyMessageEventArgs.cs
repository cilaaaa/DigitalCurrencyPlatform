
namespace StockPolicies
{
    public class PolicyMessageEventArgs
    {
        string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        public PolicyMessageEventArgs(string message)
        {
            this._message = message;
        }
    }
}
