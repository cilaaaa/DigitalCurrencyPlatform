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
using System.IO.Compression;
using System.Web;
using SuperSocket.ClientEngine.Proxy;

namespace TradeAPIHuoBi
{
    public class TradeAPI : StockTradeAPI.BaseTradeAPI
    {
        bool LoadAccount = false;
        string AccountId = "";
        WebSocket HQWs;
        bool HQAbortedWs;
        bool InitHQXinTiao;
        Thread HQXinTiaoTh;
        DateTime HQLastTime;
        bool SendHQXinTiao;
        bool isHQOpened = false;
        Dictionary<string, string> topicHQDic = new Dictionary<string, string>();
        public TradeAPI()
        {
            MARKET_DEPTH = "market.{0}.depth.step0";
            TRADE = "market.{0}.trade.detail";
            ORDER = "orders.{0}";
            POSITION = "";
            ACCOUNT = "accounts.list";

            IniFile inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
            market = "huobi";
            EnableCID = System.Convert.ToBoolean(inifile.GetString(market, "enableCID", "false"));
            name = inifile.GetString(market, "name", "WangQi");
            domain = inifile.GetString(market, "domain", "");
            apiKey = inifile.GetString(market, "apiKey", "");
            apiSecret = inifile.GetString(market, "apiSecret", "");
            makerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "makerFeePercent", "0"));
            takerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "takerFeePercent", "0"));
            address = inifile.GetString("System", "address", "127.0.0.1");
            port = System.Convert.ToInt16(inifile.GetString("System", "port", "0"));
            WEBSOCKET_API = inifile.GetString(market, "wsdomain", "");
            DicSi = new Dictionary<string, SecurityInfo>();
            _querySi = new SecurityInfo(string.Empty, string.Empty, "", 0, 0, 0, "", "");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.InitWebSocket();
            this.InitHangQingWebSocket();
        }

        private void InitHangQingWebSocket()
        {
            try
            {
                HQAbortedWs = false;
                HQWs = new WebSocket(WEBSOCKET_API.Substring(0,WEBSOCKET_API.Length - 3));
                HQWs.Error += (sender, e) =>
                {
                    Console.WriteLine("Error:" + e.Exception.Message.ToString());
                };
                if (port != 0)
                {
                    var proxy = new HttpConnectProxy(new IPEndPoint(IPAddress.Parse(address), port));
                    HQWs.Proxy = proxy;
                }
                HQWs.Opened += OnHQOpened;
                HQWs.Closed += OnHQClosed;
                HQWs.DataReceived += ReceviedHQData;
                HQWs.Open();
                if (!InitHQXinTiao)
                {
                    InitHQXinTiao = true;
                    HQXinTiaoTh = new Thread(new ThreadStart(CheckHQXinTiao));
                    HQLastTime = DateTime.Now;
                    HQXinTiaoTh.Start();
                    SendHQXinTiao = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }

        private void CheckHQXinTiao()
        {
            while (true)
            {
                TimeSpan ts = DateTime.Now - LastTime;
                if (ts.TotalSeconds >= 30)
                {
                    if (!SendHQXinTiao)
                    {
                        SendHQXinTiao = true;
                        string pingStr = string.Format("{{\"ping\":{0}}}", Math.Floor(GetTimeStamp(false)));
                        SendHQSubscribeTopic(pingStr);
                    }
                    if (ts.TotalSeconds >= 60)
                    {
                        SendHQXinTiao = false;
                        Console.WriteLine("HQ服务器没有及时做出响应");
                        HQLastTime = DateTime.Now;
                        websocket.Close("HQ服务器没有及时做出响应");
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void SendHQSubscribeTopic(string msg)
        {
            HQWs.Send(msg);
        }

        private void ReceviedHQData(object sender, DataReceivedEventArgs args)
        {
            var msg = GZipHelper.GZipDecompressString(args.Data);
            if (msg.IndexOf("ping") != -1) //响应心跳包
            {
                HQLastTime = DateTime.Now;
                SendHQXinTiao = false;
                var reponseData = msg.Replace("ping", "pong");
                SendHQSubscribeTopic(reponseData);
            }else
            {
                JObject jomsg = (JObject)JsonConvert.DeserializeObject(msg);
                if (jomsg.Property("ch") != null)
                {
                    string type = jomsg["ch"].ToString();
                    if (type.Contains("depth"))
                    {
                        QueryQuote5(jomsg);
                    }
                    else if (type.Contains("trade"))
                    {
                        QueryChengJiaoJia(jomsg);
                    }
                }
            }
        }

        private void OnHQClosed(object sender, EventArgs e)
        {
            isHQOpened = false;
            Console.WriteLine("重启HQwebsocket");
            try
            {
                HQWs.Open();
            }
            catch
            {
                HQWs.Dispose();
                HQXinTiaoTh.Abort();
                InitHangQingWebSocket();
            }
        }

        private void OnHQOpened(object sender, EventArgs e)
        {
            foreach (var item in topicHQDic)
            {
                SendHQSubscribeTopic(item.Value);
            }
            isHQOpened = true;
        }

        public override void CheckXinTiao()
        {
            while (true)
            {
                TimeSpan ts = DateTime.Now - LastTime;
                if (ts.TotalSeconds >= 30)
                {
                    if (!SendXinTiao)
                    {
                        SendXinTiao = true;
                        string pingStr = string.Format("{{\"ping\":{0}}}", Math.Floor(GetTimeStamp(false)));
                        SendSubscribeTopic(pingStr);
                    }
                    if (ts.TotalSeconds >= 60)
                    {
                        SendXinTiao = false;
                        Console.WriteLine("服务器没有及时做出响应");
                        LastTime = DateTime.Now;
                        websocket.Close("服务器没有及时做出响应");
                    }
                }
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
            string ts = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string signatureString = GetSignatureStr("GET", domain.Substring(8), "/ws/v1", GetCommonParameters(ts),false);
            string login_msg = "{\"op\": \"auth\", \"AccessKeyId\": \"" + apiKey + "\",\"SignatureMethod\": \"HmacSHA256\", \"SignatureVersion\": \"2\",\"Timestamp\": \"" + ts + "\", \"Signature\":\"" + signatureString + "\"}";
            SendSubscribeTopic(login_msg);
        }

        public override void ReceviedData(object sender, DataReceivedEventArgs args)
        {
            var msg = GZipHelper.GZipDecompressString(args.Data);
            if (msg.IndexOf("ping") != -1) //响应心跳包
            {
                LastTime = DateTime.Now;
                SendXinTiao = false;
                var reponseData = msg.Replace("ping", "pong");
                SendSubscribeTopic(reponseData);
            }
            else if (msg.IndexOf("auth") != -1)
            {
                JObject jomsg = (JObject)JsonConvert.DeserializeObject(msg);
                if (jomsg["err-code"].ToString() == "0")
                {
                    AccountId = jomsg["data"]["user-id"].ToString();
                    foreach (var item in topicDic)
                    {
                        SendSubscribeTopic(item.Value);
                    }
                    isOpened = true;
                }
            }
            else
            {
                JObject jomsg = (JObject)JsonConvert.DeserializeObject(msg);
                if (jomsg.Property("topic") != null)
                {
                    string type = jomsg["topic"].ToString();
                    if (type.Contains("orders"))
                    {
                        QueryWeiTuo(jomsg);
                    }
                    else if (type.Contains("accounts.list"))
                    {
                        QueryZiJin(jomsg);
                    }
                }
                
            }
        }

        public override void ReceviedMsg(object sender, MessageReceivedEventArgs args)
        {

        }

        public override void Subscribe(string code, string subType, string instrumentType)
        {
            if (subType == "marketDepth" || subType == "trade")
            {
                string topic = "";
                string msg = "";
                if (subType == "marketDepth")
                {
                    topic = string.Format(MARKET_DEPTH, code);
                    msg = string.Format("{{\"sub\":\"{0}\",\"pick\":[\"bids.20\",\"asks.20\"]}}", topic);
                }
                else if (subType == "trade")
                {
                    topic = string.Format(TRADE, code);
                    msg = string.Format("{{\"sub\":\"{0}\"}}", topic);
                }

                if (topicHQDic.ContainsKey(topic + subType))
                    return;
                
                topicHQDic.Add(topic + subType, msg);
                if (isHQOpened)
                {
                    SendHQSubscribeTopic(msg);
                }
            }
            if (subType == "order")
            {
                ParameterizedThreadStart pstart = new ParameterizedThreadStart(GetWeiTuo);
                Thread thread = new Thread(pstart);
                Dictionary<string, string> thParam = new Dictionary<string, string>();
                thParam.Add("code", code);
                thParam.Add("type", instrumentType);
                thread.Start(thParam);
                string topic = string.Format(ORDER, code);
                string msg = string.Format("{{\"op\":\"sub\",\"topic\":\"{0}\"}}", topic);
                if (topicDic.ContainsKey(topic + subType))
                    return;
                topicDic.Add(topic + subType,msg);
                if (isOpened)
                {
                    SendSubscribeTopic(msg);
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

        private void GetWeiTuo(object thParam)
        {
            Dictionary<string, string> param = (Dictionary<string, string>)thParam;
            JObject jomsg = GetOrders(param["code"], param["type"], false, "submitted,partial-filled,partial-canceled,filled,canceled");
            if (jomsg != null)
            {
                QueryWeiTuo(jomsg);
            }
        }

        private void GetZiJin()
        {
            while (!FullStop)
            {
                string msg = string.Format("{{\"op\":\"req\",\"topic\":\"{0}\"}}", ACCOUNT);
                SendSubscribeTopic(msg);
                Thread.Sleep(1000);
            }
        }

        #region 解析函数
        #region 委托
        public override void QueryWeiTuo(JObject jomsg)
        {
            JObject WeiTuoInfo = jomsg;
            JObject jorder = null;
            if (WeiTuoInfo.Property("data") != null)
            {
                jorder = (JObject)WeiTuoInfo["data"];
            }
            if (jorder != null)
            {
                List<StockWeiTuo> WeiTuoList = new List<StockWeiTuo>();
                StockWeiTuo wt = new StockWeiTuo();
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                System.DateTime dt = startTime.AddMilliseconds((long)jorder["created-at"]);
                wt.Time = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                wt.Code = jorder["symbol"].ToString();
                wt.Name = wt.Code;
                wt.Price = System.Convert.ToDouble(jorder["order-price"].ToString());
                wt.Qty = System.Convert.ToDouble(jorder["order-amount"].ToString());
                wt.Qty_deal = System.Convert.ToDouble(jorder["filled-amount"].ToString());
                wt.Fee = -System.Convert.ToDouble(jorder["filled-fees"].ToString());
                wt.Price_deal = System.Convert.ToDouble(jorder["price"].ToString());
                wt.CancelTime = "";
                wt.WTnbr = jorder["order-id"].ToString();
                wt.WeiTuo_Type = jorder["order-type"].ToString();
                string status = jorder["order-state"].ToString();
                switch (status)
                {
                    case "submitted":
                        wt.Status = TradeOrderStatus.Open;
                        break;
                    case "partial-filled":
                        wt.Status = TradeOrderStatus.Part_Filled;
                        break;
                    case "filled":
                        wt.Status = TradeOrderStatus.Filled;
                        break;
                    case "canceled":
                        wt.Status = TradeOrderStatus.Cancelled;
                        break;
                    case "partial-canceled":
                        wt.Status = TradeOrderStatus.Cancelled;
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
                RaiseWeiTuo(WeiTuoList);
            }
        }
        #endregion

        #region 获取行情
        public override void QueryQuote5(JObject jomsg)
        {
            #region 全量
            TickData t = new TickData();
            JObject jtick_depth = jomsg;
            string[] s = jtick_depth["ch"].ToString().Split(new char[] { '.' });
            SecurityInfo si = DicSi[s[1]];
            t.SecInfo = si;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            t.Time = startTime.AddMilliseconds(System.Convert.ToDouble(jtick_depth["ts"].ToString()));
            t.Name = si.Name;
            t.Code = si.Code;
            t.IsReal = true;
            t.Bid = System.Convert.ToDouble(jtick_depth["tick"]["bids"][0][0].ToString());
            t.Ask = System.Convert.ToDouble(jtick_depth["tick"]["asks"][0][0].ToString());
            double last = CurrentTicker.getCurrentTickerPrice(market + s[1]);
            t.Last = last == 0 ? (t.Bid + t.Ask) / 2 : last;
            for (int i = 0; i < 10; i++)
            {
                t.Bids[i] = System.Convert.ToDouble(jtick_depth["tick"]["bids"][i][0].ToString());
                t.Bidsizes[i] = System.Convert.ToDouble(jtick_depth["tick"]["bids"][i][1].ToString());
                t.Asks[i] = System.Convert.ToDouble(jtick_depth["tick"]["asks"][i][0].ToString());
                t.Asksizes[i] = System.Convert.ToDouble(jtick_depth["tick"]["asks"][i][1].ToString());
            }
            RaiseQueryData(t);
            if (si.Code == this._querySi.Code && si.Market == market)
            {
                RaiseTradeQueryData(t);
            }
            #endregion
        }
        #endregion

        #region 获取成交价
        public override void QueryChengJiaoJia(JObject jomsg)
        {
            JObject OrderBookInfo = jomsg;
            string[] s = OrderBookInfo["ch"].ToString().Split(new char[] { '.' });
            string symbol = s[1];
            CurrentTicker.Update(market + symbol, System.Convert.ToDouble(OrderBookInfo["tick"]["data"][0]["price"].ToString()));
        }
        #endregion

        #region 现货资金
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        void QueryZiJin(JObject jomsg)
        {
            JObject ZiJinInfo = jomsg;
            JArray ziJins = (JArray)ZiJinInfo["data"];
            if (ziJins != null)
            {
                List<CurrentBanalce> cbs = new List<CurrentBanalce>();
                for (int j = 0; j < ziJins.Count; j++)
                {
                    if (ziJins[j]["type"].ToString() == "spot")
                    {
                        JArray zijins = (JArray)ziJins[j]["list"];
                        for (int i = 0; i < zijins.Count; i++)
                        {
                            bool find = false;
                            int index = 0;
                            for (int m = 0; m < cbs.Count; m++)
                            {
                                if (cbs[m].Code == zijins[i]["currency"].ToString())
                                {
                                    find = true;
                                    index = m;
                                    break;
                                }
                            }
                            if (find)
                            {
                                if (zijins[i]["type"].ToString() == "trade")
                                {
                                    cbs[index].Ava = System.Convert.ToDouble(zijins[i]["balance"].ToString());
                                }
                                else
                                {
                                    cbs[index].Frz = System.Convert.ToDouble(zijins[i]["balance"].ToString());
                                }
                                cbs[index].Total = cbs[index].Ava + cbs[index].Frz;
                            }
                            else
                            {
                                CurrentBanalce cb = new CurrentBanalce();
                                if (zijins[i]["type"].ToString() == "trade")
                                {
                                    cb.Ava = System.Convert.ToDouble(zijins[i]["balance"].ToString());
                                    cb.Code = zijins[i]["currency"].ToString();
                                    cb.Frz = 0;
                                    cb.Total = cb.Ava;
                                }
                                else
                                {
                                    cb.Ava = 0;
                                    cb.Code = zijins[i]["currency"].ToString();
                                    cb.Frz = System.Convert.ToDouble(zijins[i]["balance"].ToString());
                                    cb.Total = cb.Frz;
                                }
                                cbs.Add(cb);
                            }
                        }
                    }

                }
                for (int i = 0; i < cbs.Count(); i++)
                {
                    if (cbs[i].Total == 0)
                    {
                        cbs.RemoveAt(i);
                        i--;
                    }
                }
                CurrentBalances.Update(market, cbs);
                List<StockZiJing> ZiJinList = new List<StockZiJing>();
                for (int i = 0; i < cbs.Count; i++)
                {
                    StockZiJing szj = new StockZiJing();
                    szj.Instrument_id = cbs[i].Code;
                    szj.Total_avail_balance = cbs[i].Total.ToString();
                    szj.Frozen = cbs[i].Frz.ToString();
                    szj.Equity = cbs[i].Ava.ToString();
                    ZiJinList.Add(szj);
                }
                RaiseZiJin(ZiJinList);
            }
        }
        #endregion
        #endregion

        #region rest请求函数
        #region 加密函数
        private string GetCommonParameters(string timeStr)
        {
            return string.Format("AccessKeyId={0}&SignatureMethod={1}&SignatureVersion={2}&Timestamp={3}", apiKey, "HmacSHA256", "2", UrlEncode(timeStr));
        }

        private string GetSignatureStr(string method, string host, string resourcePath, string parameters,bool isUrl = true)
        {
            var sign = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(method.ToString().ToUpper()).Append("\n")
                .Append(host).Append("\n")
                .Append(resourcePath).Append("\n");
            //参数排序
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
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(sign));
                sign = Convert.ToBase64String(hashmessage);
            }
            if (isUrl)
            {
                StringBuilder builder = new StringBuilder();
                foreach (char c in sign)
                {
                    if (HttpUtility.UrlEncode(c.ToString(), Encoding.UTF8).Length > 1)
                    {
                        builder.Append(HttpUtility.UrlEncode(c.ToString(), Encoding.UTF8).ToUpper());
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                sign = builder.ToString();
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
                b.Append(string.Format("&{0}={1}", item.Key, UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        public override string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false,bool array = false)
        {
            string ts = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string NeedAuthData = (method == "GET" && param != null) ? GetCommonParameters(ts) + "&" + BuildQueryData(param) : GetCommonParameters(ts);
            string url = function + "?" + NeedAuthData;
            if (auth)
            {
                string signatureString = GetSignatureStr(method, domain.Substring(8), function, NeedAuthData);
                url += "&Signature=" + signatureString;
            }
            
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;
            try
            {
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36";
                webRequest.ContentType = "application/json";
                if (method == "POST" && param != null)
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
            var param = new Dictionary<string, string>();
            param["size"] = "20";
            param["states"] = status;
            param["symbol"] = code;
            string res = Query("GET", "/v1/order/orders", param, true);
            if (res.Length > 0)
            {
                if (res[0].ToString() != "<")
                {
                    JObject Jorders = (JObject)JsonConvert.DeserializeObject(res);
                    if (Jorders["status"].ToString() == "0")
                    {
                        return Jorders;
                    }
                }
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

        internal class GZipHelper
        {
            /// <summary>
            /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
            /// </summary>
            /// <param name="rawString">需要压缩的字符串</param>
            /// <returns>压缩后的Base64编码的字符串</returns>
            public static string GZipCompressString(string rawString)
            {
                if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
                {
                    return "";
                }
                else
                {
                    byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                    byte[] zippedData = Compress(rawData);
                    return (string)(Convert.ToBase64String(zippedData));
                }

            }
            /// <summary>
            /// 将传入的二进制字符串资料以GZip算法解压缩
            /// </summary>
            /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
            /// <returns>原始未压缩字符串</returns>
            public static string GZipDecompressString(byte[] zippedByte)
            {

                return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedByte)));
            }
            /// <summary>
            /// GZip压缩
            /// </summary>
            /// <param name="rawData"></param>
            /// <returns></returns>
            public static byte[] Compress(byte[] rawData)
            {
                MemoryStream ms = new MemoryStream();
                GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
                compressedzipStream.Write(rawData, 0, rawData.Length);
                compressedzipStream.Close();
                return ms.ToArray();
            }
            /// <summary>
            /// 解压
            /// </summary>
            /// <param name="zippedData"></param>
            /// <returns></returns>
            public static byte[] Decompress(byte[] zippedData)
            {
                MemoryStream ms = new MemoryStream(zippedData);
                GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
                MemoryStream outBuffer = new MemoryStream();
                byte[] block = new byte[1024];
                while (true)
                {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                        break;
                    else
                        outBuffer.Write(block, 0, bytesRead);
                }
                compressedzipStream.Close();
                return outBuffer.ToArray();
            }
        }
    }
}
