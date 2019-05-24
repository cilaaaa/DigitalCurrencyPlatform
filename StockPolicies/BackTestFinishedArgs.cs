using System;

namespace StockPolicies
{
    public class BackTestFinishedArgs
    {
        public Guid guid;
        public TimeSpan Elapsed;
        public BackTestFinishedArgs(Guid g,TimeSpan ts)
        {
            this.guid = g;
            this.Elapsed = ts;
        }
    }
}
