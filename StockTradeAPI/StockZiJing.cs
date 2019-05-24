
namespace StockTradeAPI
{
    public class StockZiJing
    {
        string equity;
        /// <summary>
        /// 可用余额
        /// </summary>
        public string Equity
        {
            get { return equity; }
            set { equity = value; }
        }

        string instrument_id;
        /// <summary>
        /// 合约名称、币种
        /// </summary>
        public string Instrument_id
        {
            get { return instrument_id; }
            set { instrument_id = value; }
        }

        string margin;
        /// <summary>
        /// 已用保证金
        /// </summary>
        public string Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        string frozen;
        /// <summary>
        /// 开仓冻结保证金、冻结币数
        /// </summary>
        public string Frozen
        {
            get { return frozen; }
            set { frozen = value; }
        }

        string margin_ratio;
        /// <summary>
        /// 保证金率
        /// </summary>
        public string Margin_ratio
        {
            get { return margin_ratio; }
            set { margin_ratio = value; }
        }

        string realized_pnl;
        /// <summary>
        /// 已实现盈亏
        /// </summary>
        public string Realized_pnl
        {
            get { return realized_pnl; }
            set { realized_pnl = value; }
        }

        string timestamp;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        string total_avail_balance;
        /// <summary>
        /// 账户余额
        /// </summary>
        public string Total_avail_balance
        {
            get { return total_avail_balance; }
            set { total_avail_balance = value; }
        }

        string unrealized_pnl;
        /// <summary>
        /// 未实现盈亏
        /// </summary>
        public string Unrealized_pnl
        {
            get { return unrealized_pnl; }
            set { unrealized_pnl = value; }
        }

        string fixed_balance;
        /// <summary>
        /// 逐仓账户余额
        /// </summary>
        public string Fixed_balance
        {
            get { return fixed_balance; }
            set { fixed_balance = value; }
        }

        string mode;
        /// <summary>
        /// 账户类型
        /// </summary>
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        
    }
}
