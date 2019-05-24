
namespace StockTradeAPI
{
    public class StockChiCang
    {
        string instrument_id;

        public string Instrument_id
        {
            get { return instrument_id; }
            set { instrument_id = value; }
        }

        string margin_mode;

        public string Margin_mode
        {
            get { return margin_mode; }
            set { margin_mode = value; }
        }

        string liquidation_price;

        public string Liquidation_price
        {
            get { return liquidation_price; }
            set { liquidation_price = value; }
        }

        string long_qty;

        public string Long_qty
        {
            get { return long_qty; }
            set { long_qty = value; }
        }

        string long_avail_qty;

        public string Long_avail_qty
        {
            get { return long_avail_qty; }
            set { long_avail_qty = value; }
        }

        string long_avg_cost;

        public string Long_avg_cost
        {
            get { return long_avg_cost; }
            set { long_avg_cost = value; }
        }

        string long_settlement_price;

        public string Long_settlement_price
        {
            get { return long_settlement_price; }
            set { long_settlement_price = value; }
        }

        string realized_pnl;

        public string Realized_pnl
        {
            get { return realized_pnl; }
            set { realized_pnl = value; }
        }

        string leverage;

        public string Leverage
        {
            get { return leverage; }
            set { leverage = value; }
        }

        string short_qty;

        public string Short_qty
        {
            get { return short_qty; }
            set { short_qty = value; }
        }

        string short_avail_qty;

        public string Short_avail_qty
        {
            get { return short_avail_qty; }
            set { short_avail_qty = value; }
        }

        string short_avg_cost;

        public string Short_avg_cost
        {
            get { return short_avg_cost; }
            set { short_avg_cost = value; }
        }

        string short_settlement_price;

        public string Short_settlement_price
        {
            get { return short_settlement_price; }
            set { short_settlement_price = value; }
        }

        string created_at;

        public string Created_at
        {
            get { return created_at; }
            set { created_at = value; }
        }

        string updated_at;

        public string Updated_at
        {
            get { return updated_at; }
            set { updated_at = value; }
        }

        bool exist;
        public bool Exist
        {
            get { return exist; }
            set { exist = value; }
        }
    }
}
