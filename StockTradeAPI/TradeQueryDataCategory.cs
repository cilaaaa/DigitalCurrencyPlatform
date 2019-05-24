
namespace StockTradeAPI
{
    public class TradeQueryDataCategory
    {
        /// <summary>
        /// 资金
        /// </summary>
        public const int ZiJin = 0;
        /// <summary>
        /// 股份
        /// </summary>
        public const int GuFen = 1;
        /// <summary>
        /// 委托
        /// </summary>
        public const string WeiTuo = "submitted,partial_filled,partial_canceled,filled,canceled";
        /// <summary>
        /// 当日成交
        /// </summary>
        public const int DangRiChengJiao = 3;
        /// <summary>
        /// 可撤单
        /// </summary>
        public const string KeCheDan = "submitted,partial_filled";
        /// <summary>
        /// 股东代码
        /// </summary>
        public const int GuDongDaiMa = 5;
        /// <summary>
        /// 融资余额
        /// </summary>
        public const int RongZiYuE = 6;
        /// <summary>
        /// 融券余额
        /// </summary>
        public const int RongQuanYuE = 7;
        //可容证券
        public const int KeRongZhengQuan = 8;
    }
}
