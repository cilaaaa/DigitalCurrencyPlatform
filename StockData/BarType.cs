
namespace StockData
{
    public enum BarType
    {
        /// <summary>
        /// 真阳线 
        /// </summary>
        RealRaise,
        /// <summary>
        /// 真阴线
        /// </summary>
        RealDecent,
        /// <summary>
        /// 假阳线
        /// </summary>
        FakeRaise,
        /// <summary>
        /// 假阴线
        /// </summary>
        FakeDecent,
        /// <summary>
        /// 平线
        /// </summary>
        Even,
        /// <summary>
        /// 阳平
        /// </summary>
        RaiseEven,
        /// <summary>
        /// 阴平
        /// </summary>
        DecentEven
        
    }

    public enum AbsoluteBarType
    {
        Raise,
        Decent,
        Even
    }
}
