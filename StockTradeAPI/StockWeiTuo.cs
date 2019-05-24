
namespace StockTradeAPI
{
    public class StockWeiTuo
    {
        string _time;

        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        string _title1;

        public string Title1
        {
            get { return _title1; }
            set { _title1 = value; }
        }

        string _title2;

        public string Title2
        {
            get { return _title2; }
            set { _title2 = value; }
        }

        double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        double _qty;

        public double Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        double _price_deal;

        public double Price_deal
        {
            get { return _price_deal; }
            set { _price_deal = value; }
        }

        double _qty_deal;

        public double Qty_deal
        {
            get { return _qty_deal; }
            set { _qty_deal = value; }
        }

        double _fee;

        public double Fee
        {
            get { return _fee; }
            set { _fee = value; }
        }

        string _cancel_time;

        public string CancelTime
        {
            get { return _cancel_time; }
            set { _cancel_time = value; }
        }

        double _qty_cancel;

        public double Qty_cancel
        {
            get { return _qty_cancel; }
            set { _qty_cancel = value; }
        }

        string _wTnbr;

        public string WTnbr
        {
            get { return _wTnbr; }
            set { _wTnbr = value; }
        }

        string _cwTnbr;

        public string CWTnbr
        {
            get { return _cwTnbr; }
            set { _cwTnbr = value; }
        }

        string _weiTuo_Type;

        public string WeiTuo_Type
        {
            get { return _weiTuo_Type; }
            set { _weiTuo_Type = value; }
        }

        TradeOrderStatus _status;

        public TradeOrderStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public void Update(StockWeiTuo wt)
        {
            this.WTnbr = wt.WTnbr;
            this.Code = wt.Code;
            this.Name = wt.Name;
            this.Time = wt.Time;
            this.Price = wt.Price;
            this.Qty_deal = wt.Qty_deal;
            this.WeiTuo_Type = wt.WeiTuo_Type;
            this.Title1 = wt.Title1;
            this.Title2 = wt.Title2;
            this.Status = wt.Status;
            this.Qty_cancel = wt.Qty_cancel;
            this.Fee = wt.Fee;
            this.Price_deal = wt.Price_deal;
        }


        string _jiaoYiSuo;

        public string JiaoYiSuo
        {
            get { return _jiaoYiSuo; }
            set { _jiaoYiSuo = value; }
        }

        bool exists;

        public bool Exists
        {
            get { return exists; }
            set { exists = value; }
        }
        
    }
}
