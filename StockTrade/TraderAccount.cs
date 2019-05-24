using StockData;
using StockTradeAPI;
using System.Runtime.ExceptionServices;
using System.Security;

namespace StockTrade
{
    public class TraderAccount
    {
        int _clientID;

        public int ClientID
        {
            get { return _clientID; }
            set { _clientID = value; }
        }

        string _accountID;

        public string AccountID
        {
            get { return _accountID; }
            set { _accountID = value; }
        }

        string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        string _state;

        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        BaseTradeAPI bta;
        public BaseTradeAPI Bta
        {
            get { return bta; }
            set { bta = value; }
        }

        public void ClearQuery5()
        {
            bta.ClearQuery5();
        }

        public void SetQuery5(SecurityInfo securityInfo)
        {
            bta.SetQuery5(securityInfo);
        }

        public TraderAccount(BaseTradeAPI bta)
        {
            // TODO: Complete member initialization
            this._accountID = bta.name;
            this.ClientID = -1;
            this.bta = bta;
        }

        #region 手工下单
        internal bool ManualXiaDan(string tradeCategory, string priceType, string code, float price, double qty, ref string weituobianhao,string symbolType ,string clientweituobianhao, ref string errmsg,bool maker, string leverage = "10")
        {
            var orderResult = bta.PostOrders(code, tradeCategory, price.ToString(), qty.ToString(), priceType, symbolType, clientweituobianhao,maker, leverage);
            if (orderResult != null)
            {
                bool status = System.Convert.ToBoolean(orderResult["status"].ToString());
                if (status)
                {
                    weituobianhao = orderResult["data"].ToString();
                    return true;
                }
                else
                {
                    errmsg = orderResult["data"].ToString();
                    return false;
                }
            }
            else
            {
                errmsg = "网络错误";
                return false;
            }
        }
        #endregion


       // public static TraderAccount SimulateAccount = new TraderAccount("模拟账号");

        internal bool AutoXiaDan(OpenType openType, string code, string TradePriceType, float price, double orderQty, string symbolType, ref string weituobianhao, string clientweituobianhao, ref string errmsg,bool maker, string leverage = "")
        {
            return XiaDan(openType, TradePriceType, code, price, orderQty, symbolType, maker, leverage, ref weituobianhao, clientweituobianhao, ref errmsg);
        }

        internal bool CancelOrder(SecurityInfo si, string order_id, ref string errmsg)
        {
            var cancelResult = bta.DeleteOrder(si, order_id);
            if (cancelResult != null)
            {
                bool status = System.Convert.ToBoolean(cancelResult["status"].ToString());
                if (status)
                {
                    return true;
                }
                else
                {
                    errmsg = cancelResult["data"].ToString();
                    return false;
                }
            }
            else
            {
                errmsg = "网络错误";
            }
            return false;
        }

        #region 下单
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        internal bool XiaDan(OpenType opentype, string tradeSendOrderPriceType, string code, float price, double qty,string symbolType,bool maker,string leverage, ref string weituobianhao, string clientweituobianhao, ref string errmsg)
        {
            string tradeSendOrderCategory = "";
            switch (opentype)
            {
                case OpenType.Buy:
                    tradeSendOrderCategory = TradeSendOrderCategory.Buy;
                    break;
                case OpenType.Sell:
                    tradeSendOrderCategory = TradeSendOrderCategory.Sell;
                    break;
                case OpenType.KaiDuo:
                    tradeSendOrderCategory = TradeSendOrderCategory.KaiDuo;
                    break;
                case OpenType.KaiKong:
                    tradeSendOrderCategory = TradeSendOrderCategory.KaiKong;
                    break;
                case OpenType.PingDuo:
                    tradeSendOrderCategory = TradeSendOrderCategory.PingDuo;
                    break;
                case OpenType.PingKong:
                    tradeSendOrderCategory = TradeSendOrderCategory.PingKong;
                    break;
            }
            var orderResult = bta.PostOrders(code, tradeSendOrderCategory, price.ToString(), qty.ToString(), tradeSendOrderPriceType, symbolType, clientweituobianhao, maker, leverage);
            if (orderResult != null)
            {
                bool status = System.Convert.ToBoolean(orderResult["status"].ToString());
                if (status)
                {
                    weituobianhao = orderResult["data"].ToString();
                    return true;
                }
                else
                {
                    errmsg = orderResult["data"].ToString();
                    return false;
                }
            }
            else
            {
                errmsg = "网络错误";
                return false;
            }
        }
        #endregion

    }
}
