using StockData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StockPolicies
{
    public abstract class RunningBackTest
    {
        private SecurityInfo secInfo;

        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        public delegate void BackTestFinished(object sender, EventArgs e);
        public event BackTestFinished BackTest_Finished;
        public void RaiseFinished(EventArgs e)
        {
            if(BackTest_Finished != null)
            {
                BackTest_Finished(this, e);
            }
        }
        public delegate void BackTestMessageArrival(object sender, PolicyMessageEventArgs args);
        public event BackTestMessageArrival BackTestMessage_Arrival;
        public void RaiseMessage(PolicyMessageEventArgs args)
        {
            if(BackTestMessage_Arrival != null)
            {
                BackTestMessage_Arrival(this, args);
            }
        }


        public delegate void BackTestResultArrival(object sender, PolicyResultEventArgs args);
        public event BackTestResultArrival BackTestResult_Arrival;
        public void RaiseResult(object policy,PolicyResultEventArgs args)
        {
            if(BackTestResult_Arrival != null)
            {
                BackTestResult_Arrival(policy, args);
            }
        }
       
        public abstract List<DataGridViewColumn> GridColumns();
        public abstract int getPolicyCount();
        public abstract void Start();
        public abstract void Stop();
        public delegate void BackTestPolictFinished(object sender,BackTestFinishedArgs args);
        public event BackTestPolictFinished BackTestPolicy_Finished;
        public void PolicyFinished(Guid g,TimeSpan ts)
        {
            if(BackTestPolicy_Finished != null)
            {
                BackTestPolicy_Finished(this, new BackTestFinishedArgs(g, ts));
            }
        }
        public delegate void BackTestPolictDataFinished(object sender);
        public event BackTestPolictDataFinished BackTestPolicy_DataFinished;
        public void PolicyDataFinished()
        {
            if (BackTestPolicy_DataFinished != null)
            {
                BackTestPolicy_DataFinished(this);
            }
        }
        public delegate void BackTestPolicyStarted(object sender, BackTestStartEventArgs args);
        public event BackTestPolicyStarted BackTestPolicy_Started;
        public void PolicyStarted(BackTestStartEventArgs args)
        {
            if(BackTestPolicy_Started != null)
            {
                BackTestPolicy_Started(this, args);
            }
        }
        public delegate void BackTestPolicyLoading(object sender, string text);
        public event BackTestPolicyLoading BackTestPolicy_Loading;
        public void PolicyLoading(string text)
        {
            if (BackTestPolicy_Loading != null)
            {
                BackTestPolicy_Loading(this, text);
            }
        }
        public abstract TradeParameter getTradeParameter();  
    }
}
