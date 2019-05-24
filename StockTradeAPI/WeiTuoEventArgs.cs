
namespace StockTradeAPI
{
    public class WeiTuoEventArgs
    {
        private StockWeiTuo wt;

        public StockWeiTuo Wt
        {
            get { return wt; }
            set { wt = value; }
        }
        private DataChangeType dataChangeType;

        public DataChangeType ChangeType
        {
            get { return dataChangeType; }
            set { dataChangeType = value; }
        }

        private bool done;

        public bool Done
        {
            get { return done; }
            set { done = value; }
        }

        public WeiTuoEventArgs(StockWeiTuo wt, DataChangeType dataChangeType)
        {
            // TODO: Complete member initialization
            this.wt = wt;
            this.done = false;
            this.dataChangeType = dataChangeType;
        }
    }
}
