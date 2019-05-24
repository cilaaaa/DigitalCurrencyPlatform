
namespace StockTradeAPI
{
    public class ZhangHaoEventArgs
    {
        StockZhanghao _zhangHao;

        public StockZhanghao ZhangHao
        {
            get { return _zhangHao; }
            set { _zhangHao = value; }
        }
        DataChangeType _changType;

        public DataChangeType ChangType
        {
            get { return _changType; }
            set { _changType = value; }
        }
        public ZhangHaoEventArgs(StockZhanghao zhanghao,DataChangeType changtype)
        {
            this._zhangHao = zhanghao;
            this._changType = changtype;
        }
    }
}
