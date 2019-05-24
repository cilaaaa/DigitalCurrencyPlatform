using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockTradeAPI;
using DataBase;
using System.Windows.Forms;
using System.Security.Cryptography;
using WebSocket4Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StockData;
using DataAPI;
using System.Net;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;

namespace TradeAPIFcoin
{
    public class TradeAPI : StockTradeAPI.BaseTradeAPI
    {
        bool LoadAccount = false;
        List<string> LoadOrder = new List<string>();
        List<StockZiJing> ZiJinList;
        Dictionary<string, double> OrderUpdateTimeStamp = new Dictionary<string, double>();
        public TradeAPI()
        {
            MARKET_DEPTH = "depth.L20.{0}";
            TRADE = "trade.{0}";
            ORDER = "";
            POSITION = "";
            ACCOUNT = "";

            IniFile inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
            market = "fcoin";
            EnableCID = System.Convert.ToBoolean(inifile.GetString(market, "enableCID", "false"));
            name = inifile.GetString(market, "name", "WangQi");
            domain = inifile.GetString(market, "domain", "");
            apiKey = inifile.GetString(market, "apiKey", "");
            apiSecret = inifile.GetString(market, "apiSecret", "");
            passPhrase = inifile.GetString(market, "passPhrase", "");
            makerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "makerFeePercent", "0"));
            takerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "takerFeePercent", "0"));
            address = inifile.GetString("System", "address", "127.0.0.1");
            port = System.Convert.ToInt16(inifile.GetString("System", "port", "0"));
            WEBSOCKET_API = inifile.GetString(market, "wsdomain", "");
            ZiJinList = new List<StockZiJing>();
            DicSi = new Dictionary<string, SecurityInfo>();
            _querySi = new SecurityInfo(string.Empty, string.Empty, "", 0, 0, 0, "", "");

            this.InitWebSocket();
        }

        public override void CheckXinTiao()
        {
            while (true)
            {
                try
                {
                    TimeSpan ts = DateTime.Now - LastTime;
                    if (ts.TotalSeconds >= 30)
                    {
                        if (!SendXinTiao)
                        {
                            SendXinTiao = true;
                            string pingStr = string.Format("{{\"cmd\":\"ping\",\"args\":[{0}],\"id\":\"client.id\"}}", GetTimeStamp(false).ToString());
                            SendSubscribeTopic(pingStr);
                        }
                        if (ts.TotalSeconds >= 300)
                        {
                            SendXinTiao = false;
                            Console.WriteLine("服务器没有及时做出响应");
                            LastTime = DateTime.Now;
                            websocket.Close("服务器没有及时做出响应");
                        }
                    }
                }
                catch { }
                Thread.Sleep(1000);
            }
        }

        private byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        public override void OnOpened(object sender, EventArgs e)
        {
            isOpened = true;
            foreach (var item in topicDic)
            {
                SendSubscribeTopic(item.Value);
            }
        }

        public override void ReceviedData(object sender, DataReceivedEventArgs args)
        {

        }

        public override void ReceviedMsg(object sender, MessageReceivedEventArgs args)
        {
            var msg = args.Message;
            JObject jomsg = (JObject)JsonConvert.DeserializeObject(msg);
            string type = jomsg["type"].ToString();
            if (type != "hello") //响应心跳包
            {
                if (type.Contains("depth"))
                {
                    QueryQuote5(jomsg);
                }
                else if (type.Contains("trade"))
                {
                    QueryChengJiaoJia(jomsg);
                }
                else if (type.Contains("ping"))
                {
                    LastTime = DateTime.Now;
                    SendXinTiao = false;
                }
            }

        }

        public override void Subscribe(string code, string subType, string instrumentType)
        {
            if (subType == "marketDepth" || subType == "trade")
            {
                string topic = "";
                if (subType == "marketDepth")
                {
                    topic = string.Format(MARKET_DEPTH, code);
                }
                else if (subType == "trade")
                {
                    topic = string.Format(TRADE, code);
                }
                var msg = string.Format("{{\"cmd\":\"sub\",\"args\":[\"{0}\"]}}", topic);
                if (!topicDic.ContainsKey(topic + subType))
                    topicDic.Add(topic + subType, msg);
                if (isOpened)
                {
                    SendSubscribeTopic(msg);
                }
            }
            if (subType == "order")
            {
                if (!LoadOrder.Contains(code))
                {
                    LoadOrder.Add(code);
                    ParameterizedThreadStart pstart = new ParameterizedThreadStart(GetWeiTuo);
                    Thread thread = new Thread(pstart);
                    Dictionary<string, string> thParam = new Dictionary<string, string>();
                    OrderUpdateTimeStamp.Add(code, 0);
                    thParam.Add("code", code);
                    thParam.Add("type", instrumentType);
                    thread.Start(thParam);
                }
            }
            else if (subType == "account")
            {
                if (!LoadAccount)
                {
                    LoadAccount = true;
                    Thread ZiJinThread = new Thread(GetZiJin);
                    ZiJinThread.Start();
                }
            }
        }

        delegate JObject GetOrdersDelete(Dictionary<string, string> param);
        private JObject GetOrdersTh(Dictionary<string, string> param)
        {
            return GetOrders(param["code"], param["type"], false, param["state"]);
        }
        private void GetWeiTuo(object thParam)
        {
            Dictionary<string, string> param = (Dictionary<string, string>)thParam;
            while (!FullStop)
            {
                param["state"] = "submitted,partial_filled,partial_canceled,filled,canceled";
                GetOrdersDelete god = new GetOrdersDelete(GetOrdersTh);
                god.BeginInvoke(param, WeiTuoCallBack, god);
                Thread.Sleep(200);
            }
        }

        private void WeiTuoCallBack(IAsyncResult ar)
        {
            GetOrdersDelete god = ar.AsyncState as GetOrdersDelete;
            JObject jomsg = god.EndInvoke(ar);
            if (jomsg != null)
            {
                double time = System.Convert.ToDouble(jomsg["OrderUpdateTimeStamp"].ToString());
                string code = jomsg["code"].ToString();
                if (time > OrderUpdateTimeStamp[code])
                {
                    OrderUpdateTimeStamp[code] = time;
                    QueryWeiTuo(jomsg);
                }
            }
        }

        private void GetZiJin()
        {
            while (!FullStop)
            {
                JObject jomsg = GetAccount();
                if (jomsg != null)
                {
                    QueryZiJin(jomsg);
                }
                Thread.Sleep(500);
            }
        }

        #region 解析函数
        #region 委托
        public override void QueryWeiTuo(JObject jomsg)
        {
            JObject WeiTuoInfo = jomsg;
            JArray jorders = new JArray();
            if (WeiTuoInfo.Property("data") != null)
            {
                jorders = (JArray)WeiTuoInfo["data"];
            }
            List<StockWeiTuo> WeiTuoList = new List<StockWeiTuo>();
            for (int i = 0; i < jorders.Count; i++)
            {
                JObject jorder = (JObject)jorders[i];
                StockWeiTuo wt = new StockWeiTuo();
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                System.DateTime dt = startTime.AddMilliseconds((long)jorder["created_at"]);
                wt.Time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                wt.Code = jorder["symbol"].ToString();
                wt.Name = wt.Code;
                wt.Price = System.Convert.ToDouble(jorders[i]["price"].ToString());
                wt.Qty = System.Convert.ToDouble(jorders[i]["amount"].ToString());
                wt.Qty_deal = System.Convert.ToDouble(jorders[i]["filled_amount"].ToString());
                wt.Fee = -System.Convert.ToDouble(jorders[i]["fill_fees"].ToString());
                if (wt.Qty_deal != 0)
                {
                    wt.Price_deal = Math.Round(System.Convert.ToDouble(jorders[i]["executed_value"].ToString()) / wt.Qty_deal, 8, MidpointRounding.AwayFromZero);
                }
                else
                {
                    wt.Price_deal = 0;
                }
                wt.CancelTime = "";
                wt.WTnbr = jorders[i]["id"].ToString();
                wt.WeiTuo_Type = string.Format("{0}-{1}", jorders[i]["side"].ToString(), jorders[i]["type"].ToString());
                string status = jorder["state"].ToString();
                switch (status)
                {
                    case "submitted":
                        wt.Status = TradeOrderStatus.Open;
                        break;
                    case "partial_filled":
                        wt.Status = TradeOrderStatus.Part_Filled;
                        break;
                    case "filled":
                        wt.Status = TradeOrderStatus.Filled;
                        break;
                    case "canceled":
                        wt.Status = TradeOrderStatus.Cancelled;
                        break;
                    case "partial_canceled":
                        wt.Status = TradeOrderStatus.Cancelled;
                        break;
                    case "pending_cancel":
                        wt.Status = TradeOrderStatus.Canceling;
                        break;
                    default:
                        wt.Status = TradeOrderStatus.UnKnow;
                        break;
                }
                string[] sendtype = wt.WeiTuo_Type.Split(new char[] { '-' });
                if (sendtype[0] == "buy")
                {
                    wt.Fee = wt.Fee * wt.Price_deal;
                    wt.Title1 = "0";
                    wt.Title2 = "买入";
                }
                else
                {
                    wt.Title1 = "1";
                    wt.Title2 = "卖出";
                }
                if (wt.Status == TradeOrderStatus.Cancelled)
                {
                    wt.Qty_cancel = wt.Qty - wt.Qty_deal;
                }
                else
                {
                    wt.Qty_cancel = 0;
                }
                WeiTuoList.Add(wt);
            }
            RaiseWeiTuo(WeiTuoList);
        }
        #endregion

        #region 获取行情
        public override void QueryQuote5(JObject jomsg)
        {
            #region 全量
            TickData t = new TickData();
            JObject jtick_depth = jomsg;
            if (jtick_depth["ts"] != null && jtick_depth["bids"].Count() > 10)
            {
                string[] s = jtick_depth["type"].ToString().Split(new char[] { '.' });
                SecurityInfo si = DicSi[s[2]];
                t.SecInfo = si;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                t.Time = startTime.AddMilliseconds(System.Convert.ToDouble(jtick_depth["ts"].ToString()));
                t.Name = si.Name;
                t.Code = si.Code;
                t.IsReal = true;
                t.Bid = System.Convert.ToDouble(jtick_depth["bids"][0].ToString());
                t.Ask = System.Convert.ToDouble(jtick_depth["asks"][0].ToString());
                double last = CurrentTicker.getCurrentTickerPrice(market + s[2]);
                t.Last = last == 0 ? (t.Bid + t.Ask) / 2 : last;
                for (int i = 0; i < 20; i++)
                {
                    if (i % 2 == 0)
                    {
                        int index = i / 2;
                        t.Bids[index] = System.Convert.ToDouble(jtick_depth["bids"][i].ToString());
                        t.Asks[index] = System.Convert.ToDouble(jtick_depth["asks"][i].ToString());
                    }
                    else
                    {
                        int index = i / 2;
                        t.Bidsizes[index] = System.Convert.ToDouble(jtick_depth["bids"][i].ToString());
                        t.Asksizes[index] = System.Convert.ToDouble(jtick_depth["asks"][i].ToString());
                    }
                }
                RaiseQueryData(t);
                if (si.Code == this._querySi.Code && si.Market == market)
                {
                    RaiseTradeQueryData(t);
                }
            }
            #endregion
        }
        #endregion

        #region 获取成交价
        public override void QueryChengJiaoJia(JObject jomsg)
        {
            JObject OrderBookInfo = jomsg;
            string[] s = OrderBookInfo["type"].ToString().Split(new char[] { '.' });
            string symbol = s[1];
            CurrentTicker.Update(market + symbol, System.Convert.ToDouble(OrderBookInfo["price"].ToString()));
        }
        #endregion

        #region 现货资金
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        void QueryZiJin(JObject jomsg)
        {
            JObject ZiJinInfo = jomsg;
            JArray zijins = (JArray)ZiJinInfo["data"];
            List<CurrentBanalce> cbs = new List<CurrentBanalce>();
            for (int i = 0; i < zijins.Count; i++)
            {
                if (System.Convert.ToDecimal(zijins[i]["balance"].ToString()) > 0)
                {
                    StockZiJing zj = new StockZiJing();
                    zj.Equity = zijins[i]["available"].ToString().Substring(0,10);
                    zj.Instrument_id = zijins[i]["currency"].ToString();
                    zj.Frozen = zijins[i]["frozen"].ToString().Substring(0, 10);
                    zj.Total_avail_balance = zijins[i]["balance"].ToString().Substring(0, 10);
                    bool find = false;
                    for (int m = 0; m < ZiJinList.Count; m++)
                    {
                        if (zj.Instrument_id == ZiJinList[m].Instrument_id)
                        {
                            find = true;
                            ZiJinList[m] = zj;
                            break;
                        }
                    }
                    if (!find)
                    {
                        ZiJinList.Add(zj);
                    }
                    CurrentBanalce cb = new CurrentBanalce();
                    cb.Code = zj.Instrument_id;
                    cb.Ava = System.Convert.ToDouble(zj.Equity);
                    cb.Total = System.Convert.ToDouble(zj.Total_avail_balance);
                    cb.Frz = System.Convert.ToDouble(zj.Frozen);
                    cbs.Add(cb);
                }
            }
            CurrentBalances.Update(market, cbs);
            RaiseZiJin(ZiJinList);
        }
        #endregion
        #endregion

        #region rest请求函数
        #region 加密函数
        private string GetSignatureStr(string method, string host, string resourcePath, string parameters, string timestamp)
        {
            var sign = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(method.ToString().ToUpper())
                .Append(host)
                .Append(resourcePath)
                .Append(timestamp);
            //参数排序
            if (parameters != "")
            {
                var paraArray = parameters.Split('&');
                List<string> parametersList = new List<string>();
                foreach (var item in paraArray)
                {
                    parametersList.Add(item);
                }
                parametersList.Sort(delegate(string s1, string s2) { return string.CompareOrdinal(s1, s2); });
                foreach (var item in parametersList)
                {
                    sb.Append(item).Append("&");
                }
                sign = sb.ToString().TrimEnd('&');
            }
            else
            {
                sign = sb.ToString();
            }
            sign = ToBase64(sign);
            //计算签名，将以下两个参数传入加密哈希函数
            using (var SHA1 = new HMACSHA1(Encoding.UTF8.GetBytes(apiSecret)))
            {
                byte[] hashmessage = SHA1.ComputeHash(Encoding.UTF8.GetBytes(sign));
                sign = Convert.ToBase64String(hashmessage);
            }
            return sign;
        }
        #endregion

        public new string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, item.Value));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        public override string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false,bool array = false)
        {
            string paramData = BuildQueryData(param);
            string url = function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
                
            string postData = (method != "GET") ? paramData : "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;
            if (auth)
            {
                string ts = GetTimeStamp(false).ToString();
                string signatureString = GetSignatureStr(method, domain, url, postData, ts);
                webRequest.Headers.Clear();
                webRequest.Headers.Add("FC-ACCESS-KEY", apiKey);
                webRequest.Headers.Add("FC-ACCESS-TIMESTAMP", ts);
                webRequest.Headers.Add("FC-ACCESS-SIGNATURE", signatureString);
            }
            try
            {
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";
                webRequest.ContentType = "application/json";
                if (postData != "")
                {
                    var data = Encoding.UTF8.GetBytes(BuildJSON(param, array));
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                using (Stream str = webResponse.GetResponseStream())
                using (StreamReader sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        return "";


                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        public override JArray GetOrderBook(string symbol, int depth)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["depth"] = depth.ToString();
            string res = Query("GET", "/orderBook/L2", param);
            return (JArray)JsonConvert.DeserializeObject(res);
        }

        public override JObject GetKLine(SecurityInfo si, CandleResolution resolution, string from, string to)
        {
            string res = Query("GET", string.Format("/market/candles/{0}/{1}", resolution.ToString(), si.Code), null, false);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                if (jo["status"].ToString() == "0")
                {
                    return jo;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public override JObject GetAccount()
        {
            string res = Query("GET", "/accounts/balance", null, true);
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                    if (jo["status"].ToString() == "0")
                    {
                        return jo;
                    }
                }
            }
            return null;
        }

        public override JObject GetBalance(string code, string type)
        {
            return null;
        }

        public override JObject GetPosition(string code, string type)
        {
            return null;
        }

        public override JObject ChangeLeverage(string symbol, string leverage)
        {
            return null;
        }

        public override JObject GetOrder(SecurityInfo si, string orderId = "")
        {
            string res = "";
            switch (si.Type)
            {
                case "swap":
                    break;
                case "futures":
                    break;
                case "spot":
                    res = Query("GET", string.Format("/orders/{0}", orderId), null, true);
                    break;
                default:
                    break;
            }
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject Jorders = (JObject)JsonConvert.DeserializeObject(res);
                    if (Jorders["status"].ToString() == "0")
                    {
                        JObject jdata = new JObject();
                        jdata["data"] = new JArray() { Jorders["data"] };
                        QueryWeiTuo(jdata);
                        return Jorders;
                    }
                }
            }
            return null;
        }

        public override JObject GetOrders(string code, string type, bool keche = false, string status = "")
        {
            double time = GetTimeStamp(false);
            var param = new Dictionary<string, string>();
            param["limit"] = "20";
            param["states"] = status;
            param["symbol"] = code;
            string res = Query("GET", "/orders", param, true);
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject Jorders = (JObject)JsonConvert.DeserializeObject(res);
                    if (Jorders["status"].ToString() == "0")
                    {
                        Jorders["OrderUpdateTimeStamp"] = time;
                        Jorders["code"] = code;
                        return Jorders;
                    }
                    else
                    {
                        Console.WriteLine("C:   " + res);
                    }
                }
                else
                {
                    Console.WriteLine("A:   " + res);
                }
            }
            else
            {
                Console.WriteLine("B:   " + res);
            }
            return null;
        }

        public override JArray GetMatchOrders()
        {
            var param = new Dictionary<string, string>();
            param["count"] = 100.ToString();
            param["filter"] = "{\"execType\": \"Trade\"}";
            param["reverse"] = true.ToString();
            string res = Query("GET", "/execution", param, true);
            try
            {
                JArray jo = (JArray)JsonConvert.DeserializeObject(res);
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject PostOrders(string symbol, string tsoc, string price, string orderQty, string tsopt, string symbolType, string clOrdID = "",bool maker = false, string leverage = "10")
        {
            string res = "";
            switch (symbolType)
            {
                case "swap":
                    break;
                case "futures" :
                    break;
                case "spot":
                    {
                        var param = new Dictionary<string, string>();
                        param["amount"] = orderQty;
                        if (tsopt != "Market")
                        {
                            param["price"] = price;
                            param["type"] = "limit";
                        }
                        else
                        {
                            param["type"] = "market";
                            param["amount"] = Math.Round(System.Convert.ToDouble(orderQty) * CurrentTicker.getCurrentTickerPrice(market + symbol),2).ToString();
                        }
                        //if (maker)
                        //{
                        //    param["order_type"] = "1";
                        //}
                        param["symbol"] = symbol;
                        if (tsoc == "5")
                        {
                            param["side"] = "buy";
                        }
                        else
                        {
                            param["side"] = "sell";
                        }
                        //param["exchange"] = "main";
                        
                        //if (clOrdID != "")
                        //{
                        //    param["client_oid"] = clOrdID;
                        //}
                        res = Query("POST", "/orders", param, true);
                    }
                    break;
                default:
                    break;
            }
            JObject resultJo = new JObject();
            resultJo["data"] = "网络错误";
            resultJo["status"] = false;
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                    switch (symbolType)
                    {
                        case "swap":
                            break;
                        case "futures":
                            break;
                        case "spot":
                            {
                                if (jo["status"].ToString() == "0")
                                {
                                    resultJo["data"] = jo["data"].ToString();
                                    resultJo["status"] = true;
                                }
                                else
                                {
                                    resultJo["data"] = jo["msg"].ToString();
                                    resultJo["status"] = false;
                                }
                            }
                            break;
                        default:
                            resultJo["data"] = "类型错误";
                            resultJo["status"] = false;
                            break;
                    }
                    return resultJo;
                }
            }
            return resultJo;
        }

        public override JObject DeleteOrder(SecurityInfo si, string orderId)
        {
            string res = Query("POST", string.Format("/orders/{0}/submit-cancel", orderId), null, true);
            JObject resultJO = new JObject();
            resultJO["data"] = "网络错误";
            resultJO["status"] = false;
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject cancelInfo = (JObject)JsonConvert.DeserializeObject(res);
                    if (cancelInfo["status"].ToString() == "0")
                    {
                        resultJO["data"] = "";
                        resultJO["status"] = true;
                    }
                    else
                    {
                        resultJO["data"] = cancelInfo["msg"].ToString();
                        resultJO["status"] = false;
                    }
                }
            }
            return resultJO;
        }

        public override JObject DeleteOrders(List<Dictionary<string, string>> orderIds)
        {
            Dictionary<string, List<string>> cancelArray = new Dictionary<string, List<string>>();
            foreach (var orderInfo in orderIds)
            {
                if (!cancelArray.ContainsKey(orderInfo["code"]))
                {
                    cancelArray.Add(orderInfo["code"], new List<string>());
                }
                cancelArray[orderInfo["code"]].Add(orderInfo["orderId"]);
            }
            JObject jo = null;
            foreach (var codeItem in cancelArray)
            {
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(codeItem.Key,market);
                foreach (string orderId in codeItem.Value)
                {
                    switch (si.Type)
                    {
                        case "swap":
                            break;
                        case "futures":
                            break;
                        case "spot":
                            var res = DeleteOrder(si, orderId);
                            break;
                        default:
                            break;
                    }
                }
            }
            return jo;
        }
        #endregion
    }
}
