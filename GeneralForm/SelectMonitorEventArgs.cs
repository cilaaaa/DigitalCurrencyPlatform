using System.Collections.Generic;

namespace GeneralForm
{
    public class SelectMonitorEventArgs
    {
        private List<StockPolicies.RunningPolicy> _policies;

        public List<StockPolicies.RunningPolicy> Policies
        {
            get { return _policies; }
            set { _policies = value; }
        }

        public SelectMonitorEventArgs(List<StockPolicies.RunningPolicy> policies)
        {
            // TODO: Complete member initialization
            this._policies = policies;
        }
    }
}
