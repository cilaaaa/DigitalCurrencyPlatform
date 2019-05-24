using DataHub;
using StockData;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StockPolicies
{


    public delegate void PolicyMessageArrival(object sender, PolicyMessageEventArgs args);

    public delegate void PolicyFunCGetArrival(object sender, PolicyFunCGetEventArgs args);

    public delegate void PolicyResultArrival(object sender, PolicyResultEventArgs args);

    public delegate void PolicyParamArrival(object sender, PolicyParamEventArgs args);
    
    public delegate void LiveDataArrival(object sender, LiveDataEventArgs args);

    public delegate void HistoryDataLoadFinished(object sender, EventArgs args);


    public abstract class RunningPolicy
    {

        //protected MarketTimeRange marketTimeRange;

        protected bool isLiveDataProcessor;

        //frm_Monitor monitorForm;

        frm_DataShow dataShowFrom;

        //protected StockDataAnalisys dataAnalisys;

        protected LiveDataProcessor liveDataProcessor;

        protected List<OpenPoint> openPoints;

        private DataReceiver dataReceiver;

        protected RiscPoints riscPoints;

        public KeCheArgs kca;

        public bool start;

        public bool CanStart
        {
            get
            {
                return this.getParameter().CanStart;
            }
            set
            {
                this.getParameter().CanStart = value;
            }
        }

        public DataReceiver PolicyDataReceiver
        {
            get { return dataReceiver; }
        }
        protected void initialDataReceiver()
        {
            if (object.Equals(null,secInfo))
            {
                throw new Exception("StockCode Not Set");
            }
            else
            {
                this.dataReceiver = DataReceiver.AddDataReceiver(this.secInfo, this.policyguid, this.isReal);// DataMonitor.AddDataReceiver(this.stockCode, this.PolicyGuid, this.isReal);
                this.dataReceiver.Data_Arrival += dataReceiver_Data_Arrival;
                start = true;
            }
        }
        //public RunningPolicy()
        //{
        //    initialDataReceiver();
        //}
        public void Stop()
        {
            DataReceiver.RemoveDataReceiver(this.policyguid,this.isReal);//this.stockCode, this.isReal, this.PolicyGuid);
            this.dataReceiver.Data_Arrival -= dataReceiver_Data_Arrival;
            start = false;
        }

        public void Start()
        {
            this.dataReceiver = DataReceiver.AddDataReceiver(this.secInfo, this.policyguid, this.isReal);// DataMonitor.AddDataReceiver(this.stockCode, this.PolicyGuid, this.isReal);
            this.dataReceiver.Data_Arrival += dataReceiver_Data_Arrival;
            start = true;
        }
        public abstract void ManualOpen();
        public abstract void ManualClose(Guid guid);
        public abstract void Reset();
        public void UpdateChart()
        {
            
                try
                {
                    dataShowFrom.UpdateChart(new LiveShowData(liveDataProcessor.Bar1M, liveDataProcessor.LastClose, riscPoints), openPoints , policyName);
                }
                catch
                {

                }
            
        }
        
        public void showMonitor(Screen screen)
        {
            
                try
                {
                    dataShowFrom = new frm_DataShow();
                    dataShowFrom.Left = screen.Bounds.X;
                    dataShowFrom.Top = screen.Bounds.Y;
                    dataShowFrom.Text = string.Format("{0}-{1}-图形", this.secInfo.Code, this.policyName);
                    dataShowFrom.Show();
                    dataShowFrom.UpdateChart(new LiveShowData(liveDataProcessor.Bar1M, liveDataProcessor.LastClose, riscPoints), openPoints,policyName);
                }
                catch { }
            
            
        }

        public abstract void InitArgs();
        public virtual void FunGetResult(string funName,object result) { }

        public virtual CheDanArgs PolicyCheDan(KeCheDetail kcd) 
        {
            CheDanArgs cda = new CheDanArgs();
            TickData tick = CurrentStockData.GetTick(kcd.Si);
            if (kcd.OverTime())
            {
                if (kcd.TradeType == PolicyTradeType.Open) //入场单
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = false;
                }
                else//出场单
                {
                    cda.Cancel = true;
                    cda.ZhuiDan = true;
                    cda.NewOrderPrice = tick.Last;
                }
            }
            return cda;
        }
        //public abstract void Notify(Guid tradeGuid,OpenStatus status,int dealQty);
        public abstract void Notify(Guid tradeGuid, OpenStatus status, double dealQty = 0, double dealPrice = 0, string weituobianhao = "", string pendWeituobianhao = "");
        public abstract void InitialDataProcessor();
        protected Guid policyguid;
        public Guid PolicyGuid
        {
            get { return this.policyguid; }
        }
        protected abstract void dataReceiver_Data_Arrival(object sender, TickData tickdata);

        private SecurityInfo secInfo;

        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        protected int inteval;

        public int Inteval
        {
            get { return inteval; }
        }
        protected string policyName;

        public string PolicyName
        {
            get { return policyName; }
        }

        protected bool isReal;

        public bool IsReal
        {
            get { return isReal; }
        }

        protected DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
        }

        protected DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
        }

        private bool isSimulateFinished;

        protected bool IsSimulateFinished
        {
            get { return isSimulateFinished; }
            set { isSimulateFinished = value; }
        }

        bool isSim;

        public bool IsSim
        {
            get { return isSim; }
            set { isSim = value; }
        }


        //delegate是C#中的一种类型，它实际上是一个能够持有对某个方法的引用的类。
        //与其它的类不同，delegate类能够拥有一个签名（signature），并且它只能持有与它的签名相匹配的方法的引用。

        public event PolicyMessageArrival PolicyMessage_Arrival;
        public void RaiseMessage(PolicyMessageEventArgs args)
        {
            if (PolicyMessage_Arrival != null)
            {
                PolicyMessage_Arrival(this, args);
            }
        }
        public event PolicyFunCGetArrival PolicyFunCGet_Arrival;
        public void RaiseFunCGet(PolicyFunCGetEventArgs args)
        {
            if (PolicyFunCGet_Arrival != null)
            {
                PolicyFunCGet_Arrival(this, args);
            }
        }
        public event PolicyResultArrival PolicyResult_Arrival;
        public void RaiseResult(PolicyResultEventArgs args)
        {
            if (PolicyResult_Arrival != null)
            {
                PolicyResult_Arrival(this, args);
            }
        }
        public event LiveDataArrival LiveData_Arrival;
        public void LiveDataUpdate(TickData tickdata)
        {
            if (LiveData_Arrival != null)
            {
                LiveData_Arrival(this, LiveDataEventArgs.ConvertFromTickData(tickdata));
            }
        }

        public event PolicyParamArrival PolicyParam_Arrival;
        public void RaiseParamChange(PolicyParamEventArgs args){
            if (PolicyParam_Arrival != null)
            {
                PolicyParam_Arrival(this, args);
            }
        }

        public event HistoryDataLoadFinished HistoryDataLoad_Finished;
        public void HistoryFinished()
        {
            if(HistoryDataLoad_Finished != null)
            {
                HistoryDataLoad_Finished(this, new EventArgs());
            }
        }

        public abstract string getBackTestTitle();
        protected abstract PolicyParameter getParameter();
        public void showParameter(IWin32Window owner,Screen screen)
        {
            frm_Parameter frm = new frm_Parameter();
            frm.setParameter(this.getParameter(),this.secInfo ,this.policyName);
            frm.ShowDialog(owner);
        }

        
    }
}
