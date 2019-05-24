using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockPolicies
{
    public class BackTestStartEventArgs
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private Guid policyGuid;

        public Guid PolicyGuid
        {
            get { return policyGuid; }
            set { policyGuid = value; }
        }
        public BackTestStartEventArgs(string title,Guid policyguid)
        {
            this.title = title;
            this.policyGuid = policyguid;
        }
    }
}
