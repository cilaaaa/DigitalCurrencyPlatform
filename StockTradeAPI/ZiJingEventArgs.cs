
namespace StockTradeAPI
{
    public class ZiJingEventArgs
    {
        private StockZiJing zj;

        public StockZiJing Zj
        {
            get { return zj; }
            set { zj = value; }
        }
        private DataChangeType dataChangeType;

        public DataChangeType ChangeType
        {
            get { return dataChangeType; }
            set { dataChangeType = value; }
        }

        public ZiJingEventArgs(StockZiJing zijing)
        {
            // TODO: Complete member initialization
            this.zj = zijing;
        }

        public ZiJingEventArgs(StockZiJing zj, DataChangeType dataChangeType)
        {
            // TODO: Complete member initialization
            this.zj = zj;
            this.dataChangeType = dataChangeType;
        }
    }
}
