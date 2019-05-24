using DataBase;
using StockData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace DataHub
{
    //创建一个数据监控类
    public class DataMonitor
    {
        static object lockingOjb = new object();
        public DataMonitor()
        {

        }
        public void Simulator(SecurityInfo si, DateTime startDate, DateTime endDate, DataReceiver da, int interval, bool needSimulte = false)
        {
            if (needSimulte)
            {
                
                DateTime today = startDate.Date;
                MarketTimeRange marketRange = MarketTimeRange.getTimeRange(si.Market);
                while (today <= endDate)
                {
                    
                    //DataTable tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\Okex%" + si.Code + "%" + today.ToString("yyyyMMdd") + ".csv");
                    //DataTable tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\" + si.Code + "-" + today.ToString("yyyyMMdd") + ".csv");

                    DataTable tickdatas = new DataTable();
                    try
                    {
                        tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\" + si.Code + "%" + today.ToString("yyyyMMdd") + ".csv");
                    }
                    catch { }
                    if (tickdatas.Rows.Count > 0)
                    {
                        for (int i = 0; i < tickdatas.Rows.Count; i++)
                        {
                            try
                            {
                                DataRow dr = tickdatas.Rows[i];
                                DateTime tickTime = System.Convert.ToDateTime(dr["timestamp"].ToString());
                                //DateTime tickTime = System.Convert.ToDateTime(dr["timestamp"].ToString().Replace("D", " ").Substring(0, 23));

                                TickData tickdata = new TickData();
                                tickdata.Code = si.Code;
                                tickdata.SecInfo = GlobalValue.GetFutureByCodeAndMarket(tickdata.Code, si.Market);
                                tickdata.Time = tickTime;
                                tickdata.Preclose = 0;
                                tickdata.Open = 0;
                                tickdata.High = 0;
                                tickdata.Low = 0;
                                tickdata.Ask = System.Convert.ToDouble(dr["askPrice"].ToString());
                                tickdata.Bid = System.Convert.ToDouble(dr["bidPrice"].ToString());
                                //tickdata.Last = System.Convert.ToDouble(dr["lastPrice"].ToString());
                                tickdata.Last = (tickdata.Ask + tickdata.Bid) / 2;
                                tickdata.Volume = 0;
                                tickdata.Amt = 0;
                                tickdata.IsReal = false;
                                for (int j = 0; j < 10; j++)
                                {
                                    tickdata.Asks[j] = tickdata.Ask;
                                    tickdata.Bids[j] = tickdata.Bid;
                                    tickdata.Asksizes[j] = System.Convert.ToDouble(dr["askSize"]);
                                    tickdata.Bidsizes[j] = System.Convert.ToDouble(dr["bidSize"]);
                                }
                                da.DataArrival(tickdata);
                                if (interval != 0)
                                    Thread.Sleep(interval);
                            }
                            catch { }
                        }
                    }
                    today = today.AddDays(1);
                }
                //DataTable tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\rb15.csv");
                ////DataTable tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\XBTUSD-" + today.ToString("yyyyMMdd") + ".csv");
                //if (tickdatas.Rows.Count > 0)
                //{
                //    for (int i = 0; i < tickdatas.Rows.Count; i++)
                //    {
                //        DataRow dr = tickdatas.Rows[i];
                //        DateTime tickTime = System.Convert.ToDateTime(dr["timestamp"].ToString());
                //        //TickData tickdata = TickData.ConvertFromDataRow(dr);

                //        TickData tickdata = new TickData();
                //        tickdata.Code = "rb";
                //        tickdata.SecInfo = GlobalValue.GetFutureByCode(tickdata.Code);
                //        tickdata.Time = tickTime;
                //        tickdata.Preclose = 0;
                //        tickdata.Open = 0;
                //        tickdata.High = 0;
                //        tickdata.Low = 0;
                //        tickdata.Ask = System.Convert.ToDouble(dr["open"]);
                //        tickdata.Bid = System.Convert.ToDouble(dr["high"]);
                //        tickdata.Last = System.Convert.ToDouble(dr["close"]);
                //        tickdata.Volume = 0;
                //        tickdata.Amt = 0;
                //        tickdata.IsReal = false;
                //        for (int j = 0; j < 10; j++)
                //        {
                //            tickdata.Asks[j] = tickdata.Ask;
                //            tickdata.Bids[j] = tickdata.Bid;
                //        }
                //        da.DataArrival(tickdata);
                //        if (interval != 0)
                //            Thread.Sleep(interval);
                //    }
                //}
            }
            TickData td = new TickData();
            td.Code = string.Empty;
            try
            {
                da.DataArrival(td);
            }
            catch { }

        }

        public void SendTick(TickData td)
        {
            try
            {
                //RealTimeDataReceivers[td.Code].DataArrival(td);
                foreach (var dr in DataReceiver.RealTimeDataReceivers)
                {
                    if (dr.SecInfo.Key == td.SecInfo.Key)
                    {
                        dr.DataArrival(td);
                    }
                }
            }
            catch { }
        }
        public void BackSimulator(List<TickData> list_tickdata, DataReceiver dataReceiver, int p)
        {
            for (int i = 0; i < list_tickdata.Count; i++)
            {
                //TickData tdx = list_tickdata[i];
                dataReceiver.DataArrival(list_tickdata[i]);
            }
            TickData td = new TickData();
            td.Code = string.Empty;
            try
            {
                dataReceiver.DataArrival(td);
            }
            catch { }
        }
    }
}
