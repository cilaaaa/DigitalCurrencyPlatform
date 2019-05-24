using StockData;
using System.Collections.Generic;

namespace StockTradeAPI
{
    public delegate void OnHandDetailChangeDelegate(object sender,OnHandDetailChangeEventArgs args);
    public class OnHandDetail
    {
        object _lockObj;
        private List<OnHandStock> onHandStocks;

        
        public OnHandDetail()
        {
            this.onHandStocks = new List<OnHandStock>();
            _lockObj = new object();
        }

        //public void Update(SecurityInfo si, int onhand, int available)
        //{
        //    for (int i = 0; i < onHandStocks.Count; i++)
        //    {
        //        if (onHandStocks[i].Si == si)
        //        {
        //            onHandStocks[i].StockOnHand = onhand;
        //            onHandStocks[i].StockAvailable = available;
        //            return;
        //        }
        //    }
        //    onHandStocks.Add(new OnHandStock(si, onhand, available));
        //}

        //public int GetAvailable(SecurityInfo si)
        //{
        //    for (int i = 0; i < onHandStocks.Count; i++)
        //    {
        //        if (onHandStocks[i].Si == si)
        //        {
        //            return onHandStocks[i].StockAvailable;
        //        }
        //    }
        //    return 0;
        //}

        public void AddOnHandDetail(SecurityInfo securityInfo, int onh, int ava)
        {
            lock (_lockObj)
            {
                OnHandStock ohs = new OnHandStock(securityInfo, onh, ava);
                onHandStocks.Add(ohs);
                RaiseChange(ohs);
            }
        }

        //public void UpdateOnHandDetail(SecurityInfo securityInfo, int onh, int ava)
        //{
        //    lock (_lockObj)
        //    {
        //        for (int i = 0; i < onHandStocks.Count; i++)
        //        {
        //            if (onHandStocks[i].Si == securityInfo)
        //            {
        //                onHandStocks[i].StockOnHand = onh;
        //                onHandStocks[i].StockAvailable = ava;
        //                return;
        //            }
        //        }
        //    }
        //}

        //public int GetAvailableStockQty(SecurityInfo securityInfo)
        //{
        //    lock (_lockObj)
        //    {
        //        for (int i = 0; i < onHandStocks.Count; i++)
        //        {
        //            if (onHandStocks[i].Si == securityInfo)
        //            {
        //                return onHandStocks[i].StockAvailable - onHandStocks[i].StockAvailableRevise;
        //            }
        //        }
        //        return 0;
        //    }
        //}

        //public void UpdateRevise(SecurityInfo securityInfo, int Qty)
        //{
        //    lock(_lockObj)
        //    {
        //        for(int i=0;i<onHandStocks.Count;i++)
        //        {
        //            if(onHandStocks[i].Si == securityInfo)
        //            {
        //                onHandStocks[i].StockAvailableRevise += Qty;
        //                return;
        //            }
        //        }
        //    }
        //}
        public void UpdateAvailableQty(SecurityInfo si, double qty)
        {
            lock(_lockObj)
            {
                for(int i=0;i<onHandStocks.Count;i++)
                {
                    if(onHandStocks[i].Si == si)
                    {
                        onHandStocks[i].StockAvailable -= qty;
                        RaiseChange(onHandStocks[i]);
                    }
                }
            }
        }

        public bool canOpen(SecurityInfo securityInfo, int orderQty)
        {
            lock (_lockObj)
            {
                for (int i = 0; i < onHandStocks.Count; i++)
                {
                    if (onHandStocks[i].Si == securityInfo)
                    {
                        if (onHandStocks[i].StockAvailable >= orderQty)
                        {
                            onHandStocks[i].StockAvailable -= orderQty;
                            RaiseChange(onHandStocks[i]);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
        }
        public event OnHandDetailChangeDelegate Change;
        public void RaiseChange(OnHandStock onHandStock)
        {
            if(Change != null)
            {
                Change(this, new OnHandDetailChangeEventArgs(onHandStock));
            }
        }

        public void ResetQty(SecurityInfo securityInfo, int qty)
        {
            lock(_lockObj)
            {
                for(int i=0;i<onHandStocks.Count;i++)
                {
                    if(onHandStocks[i].Si == securityInfo)
                    {
                        onHandStocks[i].StockAvailable = qty;
                    }
                }
            }
        }

        public void ReMoveAll()
        {
            lock(_lockObj)
            {
                onHandStocks.Clear();
            }
        }
    }
}
