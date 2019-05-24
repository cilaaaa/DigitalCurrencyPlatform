using StockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHub
{
    //创建一个数据接收类
    public class DataReceiver
    {
        public static object simLockingObj = new object();
        public static object realLockingObj = new object();
        
        //public static List<DataReceiver> SimulateDataReceivers = new List<DataReceiver>();
        public static bool hasTealTimeDataReceiver(SecurityInfo si)
        {
            lock(realLockingObj)
            {
                foreach(var x in RealTimeDataReceivers)
                {
                    if (x.SecInfo == si)
                        return true;
                }
                return false;
            }
        }
        public static List<DataReceiver> RealTimeDataReceivers = new List<DataReceiver>();
        public static DataReceiver AddDataReceiver(SecurityInfo si,Guid _policyGuid,bool isReal)
        {
            DataReceiver da = new DataReceiver();
            da.SecInfo = si;
            da.policyGuid = _policyGuid;
            if (isReal)
            {
                lock (realLockingObj)
                {
                    RealTimeDataReceivers.Add(da);
                }
            }
            else
            {
                
            }
            return da;
        }


        public static void RemoveDataReceiver(Guid _policyGuid,bool isReal)
        {
            if(isReal)
            {
                lock (realLockingObj)
                {
                    for(int j=RealTimeDataReceivers.Count -1;j>=0;j--)
                    {
                        if(RealTimeDataReceivers[j].policyGuid == _policyGuid)
                        {
                            RealTimeDataReceivers.RemoveAt(j);
                        }
                    }
                }
            }
            //else
            //{
            //    lock(simLockingObj)
            //    {
            //        for(int j=SimulateDataReceivers.Count -1;j>=0;j--)
            //        {
            //            if(SimulateDataReceivers[j].policyGuid == _policyGuid)
            //            {
            //                SimulateDataReceivers.RemoveAt(j);
            //            }
            //        }
            //    }
            //}
        }

        //封装字段
        SecurityInfo secInfo;

        public SecurityInfo SecInfo
        {
            get { return secInfo; }
            set { secInfo = value; }
        }
        //string stockcode;
        
        //public string Stockcode
        //{
        //    get { return stockcode; }
        //    set { stockcode = value; }
        //}

        //byte market;
        ////市场
        //public byte Market
        //{
        //    get { return market; }
        //    set { market = value; }
        //}
        Guid policyGuid;

        public Guid PolicyGuid
        {
            get { return policyGuid; }
            set { policyGuid = value; }
        }
        //通过托管理在控件中定义一个事件
        public DataArrivalEventHandler Data_Arrival;
        //定义了一个托管方法
        public delegate void DataArrivalEventHandler(object sender, TickData tickdata);
        //定义一个带有参数的方法
        public void DataArrival(TickData tickdata)
        {
            if (Data_Arrival != null)
            {
                Data_Arrival(this, tickdata);
            }
        }

        //通过托管理在控件中定义一个事件
        public event DataBarArrivalEventHandler Data_Bar_Arrival;
        //定义了一个托管方法
        public delegate void DataBarArrivalEventHandler(object sender, LiveBarArrivalEventArgs args);
        //定义一个带有参数的方法
        public void DataBarArrival(LiveBarArrivalEventArgs args)
        {
            if (Data_Bar_Arrival != null)
            {
                Data_Bar_Arrival(this, args);
            }
        }
    }
}
      