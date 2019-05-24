
namespace StockTradeAPI
{
    public  enum TradeSendOrderPriceType
    {
        Limit,// = "limit";

        Market,//public const string ShiJiaWeiTuo = "market";

        Stop, //public const string IOC = "ioc";

        StopLimit,

        MarketIfTouched,

        LimitIfTouched,

        MarketWithLeftOverAsLimit,

        Pegged
    }
}
