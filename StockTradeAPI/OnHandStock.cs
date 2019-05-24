using StockData;

namespace StockTradeAPI
{
    public class OnHandStock
    {
        
        int stockOnHand;

        public int StockOnHand
        {
            get { return stockOnHand; }
            set { stockOnHand = value; }
        }
        double stockAvailable;


        int stockAvailableRevise;

        public int StockAvailableRevise
        {
            get { return stockAvailableRevise; }
            set { stockAvailableRevise = value; }
        }







        public double StockAvailable
        {
            get { return stockAvailable; }
            set { stockAvailable = value; }
        }
        SecurityInfo si;

        public SecurityInfo Si
        {
            get { return si; }
            set { si = value; }
        }
        //public static void Update(SecurityInfo si,int onhand,int available)
        //{

        //    for(int i=0;i<OnHandStocks.Count;i++)
        //    {
        //        if(OnHandStocks[i].si == si)
        //        {
        //            OnHandStocks[i].stockOnHand = onhand;
        //            OnHandStocks[i].atockAvailable = available;
        //            return;
        //        }
        //    }
        //    OnHandStocks.Add(new OnHandStock(si, onhand, available));
        //}
        public OnHandStock(SecurityInfo si, int onhand, int available)
        {
            this.si = si;
            this.stockOnHand = onhand;
            this.stockAvailable = available;
            this.stockAvailableRevise = 0;
            
        }
        
    }
}
