
namespace StockTradeAPI
{
    public class ChengJiaoEventArgs
    {
        private StockChengJiao cj;

        public StockChengJiao Cj
        {
            get { return cj; }
            set { cj = value; }
        }
        private DataChangeType dataChangeType;

        public DataChangeType ChangeType
        {
            get { return dataChangeType; }
            set { dataChangeType = value; }
        }

        public ChengJiaoEventArgs(StockChengJiao chenjiao, DataChangeType dataChangeType)
        {
            // TODO: Complete member initialization
            this.cj = chenjiao;
            this.dataChangeType = dataChangeType;
        }
    }
}
