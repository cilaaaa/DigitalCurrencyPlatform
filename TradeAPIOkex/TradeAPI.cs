using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockTradeAPI;
using DataBase;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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

namespace TradeAPIOkex
{
    public class TradeAPI : StockTradeAPI.BaseTradeAPI
    {
        List<StockZiJing> ZiJinList;
        List<StockChiCang> ChiCangList;
        public TradeAPI()
        {
            MARKET_DEPTH = "{0}/depth5:{1}";
            TRADE = "{0}/trade:{1}";
            ORDER = "{0}/order:{1}";
            POSITION = "{0}/position:{1}";
            ACCOUNT = "{0}/account:{1}";

            IniFile inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
            market = "okex";
            EnableCID = System.Convert.ToBoolean(inifile.GetString(market, "enableCID", "false"));
            name = inifile.GetString(market, "name", "WangQi");
            domain = inifile.GetString(market, "domain", "");
            apiKey = inifile.GetString(market, "apiKey", "");
            apiSecret = inifile.GetString(market, "apiSecret", "");
            passPhrase = inifile.GetString(market, "passPhrase", "init1234");
            makerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "makerFeePercent", "0"));
            takerFeePercent = System.Convert.ToDouble(inifile.GetString(market, "takerFeePercent", "0"));
            address = inifile.GetString("System", "address", "127.0.0.1");
            port = System.Convert.ToInt16(inifile.GetString("System", "port", "0"));
            WEBSOCKET_API = inifile.GetString(market, "wsdomain", "");

            ZiJinList = new List<StockZiJing>();
            ChiCangList = new List<StockChiCang>();
            DicSi = new Dictionary<string, SecurityInfo>();
            _querySi = new SecurityInfo(string.Empty, string.Empty, "", 0, 0, 0, "", "");

            this.InitWebSocket();
        }

        public override void CheckXinTiao()
        {
            while (true)
            {
                TimeSpan ts = DateTime.Now - LastTime;
                if (ts.TotalSeconds >= 5)
                {
                    if (!SendXinTiao)
                    {
                        SendXinTiao = true;
                        SendSubscribeTopic("ping");
                    }
                    if (ts.TotalSeconds >= 20)
                    {
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
            string ts = this.GetTimeStamp().ToString();
            string message = ts + "GET/users/self/verify";
            byte[] signatureBytes = hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
            string signatureString = ByteArrayToString(signatureBytes);
            string login_msg = "{\"op\": \"login\", \"args\": [\"" + apiKey + "\",\"" + passPhrase + "\", \"" + ts + "\", \"" + signatureString + "\"]}";
            SendSubscribeTopic(login_msg);
        }

        public override void ReceviedData(object sender, DataReceivedEventArgs args)
        {
            LastTime = DateTime.Now;
            SendXinTiao = false;
            var msg = Decompress(args.Data);


            string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            msg = reg.Replace(msg, matchEvaluator);
            if (msg != "pong")
            {
                JObject jomsg = (JObject)JsonConvert.DeserializeObject(msg);
                if (jomsg.Property("event") != null)
                {
                    if (jomsg["event"].ToString() == "login")
                    {
                        if (jomsg.Property("success") != null)
                        {
                            StringBuilder msgsb = new StringBuilder();
                            msgsb.Append("{\"op\":\"subscribe\",\"args\":[");
                            foreach (var item in topicDic)
                            {
                                msgsb.Append("\"" + item.Value + "\",");
                            }
                            string subMsg = msgsb.ToString().TrimEnd(',') + "]}";
                            SendSubscribeTopic(subMsg);
                            isOpened = true;
                        }
                    }
                }
                if (jomsg.Property("table") != null) //响应心跳包
                {
                    switch (jomsg["table"].ToString())
                    {
                        case "swap/depth5":
                            QueryQuote5(jomsg);
                            break;
                        case "swap/trade":
                            QueryChengJiaoJia(jomsg);
                            break;
                        case "swap/order":
                            QueryWeiTuo(jomsg);
                            break;
                        case "swap/position":
                            QuerySwapChiCang(jomsg);
                            break;
                        case "swap/account":
                            QuerySwapZiJing(jomsg);
                            break;
                        case "futures/depth5":
                            QueryQuote5(jomsg);
                            break;
                        case "futures/trade":
                            QueryChengJiaoJia(jomsg);
                            break;
                        case "futures/order":
                            QueryWeiTuo(jomsg);
                            break;
                        case "futures/position":
                            QueryFutureChiCang(jomsg);
                            break;
                        case "futures/account":
                            QueryFutureZiJing(jomsg);
                            break;
                        case "spot/depth5":
                            QueryQuote5(jomsg);
                            break;
                        case "spot/trade":
                            QueryChengJiaoJia(jomsg);
                            break;
                        case "spot/order":
                            QueryWeiTuo(jomsg);
                            break;
                        case "spot/account":
                            QuerySpotZiJing(jomsg);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override void ReceviedMsg(object sender, MessageReceivedEventArgs args)
        {
        }

        public override void Subscribe(string _topic, string subType, string instrumentType)
        {
            string topic = "";
            switch (subType)
            {
                case "marketDepth":
                    topic = string.Format(MARKET_DEPTH, instrumentType, _topic);
                    break;
                case "trade":
                    topic = string.Format(TRADE, instrumentType, _topic);
                    break;
                case "order":
                    GetOpenWeiTuoDelegate gowtd = new GetOpenWeiTuoDelegate(GetOpenWeiTuo);
                    if (instrumentType == "swap")
                    {
                        gowtd.BeginInvoke(_topic, instrumentType, "0", null, null);
                        gowtd.BeginInvoke(_topic, instrumentType, "1", null, null);
                    }
                    else
                    {
                        gowtd.BeginInvoke(_topic, instrumentType, "", null, null);
                    }
                    topic = string.Format(ORDER, instrumentType, _topic);
                    break;
                case "position":
                    GetPositionDelegate gpd = new GetPositionDelegate(GetOpenPosition);
                    gpd.BeginInvoke(_topic, instrumentType, null, null);
                    topic = string.Format(POSITION, instrumentType, _topic);
                    break;
                case "account":
                    if (instrumentType == "futures")
                    {
                        _topic = _topic.Split(new char[] { '-' })[0];
                    }
                    else if (instrumentType == "spot")
                    {
                        string[] splitArray = _topic.Split(new char[] { '-' });
                        string topic2 = string.Format(ACCOUNT, instrumentType, splitArray[1]);
                        if (!topicDic.ContainsKey(topic2))
                        {
                            GetZijinDelegate gzd2 = new GetZijinDelegate(GetZijin);
                            gzd2.BeginInvoke(splitArray[1], instrumentType, null, null);
                            var msg = string.Format("{{\"op\":\"subscribe\",\"args\":[\"{0}\"]}}", topic2);
                            if (isOpened)
                            {
                                SendSubscribeTopic(msg);
                            }
                            topicDic.Add(topic2, topic2);
                        }
                        _topic = splitArray[0];
                    }
                    topic = string.Format(ACCOUNT, instrumentType, _topic);
                    if (!topicDic.ContainsKey(topic))
                    {
                        GetZijinDelegate gzd = new GetZijinDelegate(GetZijin);
                        gzd.BeginInvoke(_topic, instrumentType, null, null);
                    }
                    break;
                default:
                    topic = _topic;
                    break;
            }
            if (!topicDic.ContainsKey(topic))
            {
                var msg = string.Format("{{\"op\":\"subscribe\",\"args\":[\"{0}\"]}}", topic);
                if (isOpened)
                {
                    SendSubscribeTopic(msg);
                }
                topicDic.Add(topic, topic);
            }
        }

        delegate void GetOpenWeiTuoDelegate(string code, string type, string status);
        private void GetOpenWeiTuo(string code, string type, string status)
        {
            var jomsg = GetOrders(code, type, true, status);
            QueryWeiTuo(jomsg);
        }

        delegate void GetPositionDelegate(string code, string type);
        private void GetOpenPosition(string code, string type)
        {
            var jomsg = GetPosition(code, type);
            switch (type)
            {
                case "futures":
                    QueryFutureChiCang(jomsg);
                    break;
                case "swap":
                    JObject swapChiCangJo = new JObject();
                    JArray swapChiCangJa = new JArray();
                    JObject detail = new JObject();
                    detail["margin_mode"] = jomsg["margin_mode"];
                    detail["holding"] = jomsg["holding"];
                    detail["instrument_id"] = jomsg["holding"][0]["instrument_id"];
                    swapChiCangJa.Add(detail);
                    swapChiCangJo["data"] = swapChiCangJa;
                    QuerySwapChiCang(swapChiCangJo);
                    break;
            }
        }

        delegate void GetZijinDelegate(string code, string type);
        private void GetZijin(string code, string type)
        {
            var jomsg = GetBalance(code, type);
            switch (type)
            {
                case "futures":
                    JObject zijinJo = new JObject();
                    zijinJo[code] = jomsg;
                    JArray zijinJa = new JArray();
                    zijinJa.Add(zijinJo);
                    JObject zijin = new JObject();
                    zijin["data"] = zijinJa;
                    QueryFutureZiJing(zijin);
                    break;
                case "swap":
                    JObject swapZijinJo = new JObject();
                    JArray swapZijinJa = new JArray();
                    swapZijinJa.Add(jomsg["info"]);
                    swapZijinJo["data"] = swapZijinJa;
                    QuerySwapZiJing(swapZijinJo);
                    break;
                case "spot":
                    JObject spotZijin = new JObject();
                    JArray spotZijinJa = new JArray();
                    spotZijinJa.Add(jomsg);
                    spotZijin["data"] = spotZijinJa;
                    QuerySpotZiJing(spotZijin);
                    break;
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
            else if (WeiTuoInfo.Property("order_info") != null)
            {
                jorders = (JArray)WeiTuoInfo["order_info"];
            }
            List<StockWeiTuo> WeiTuoList = new List<StockWeiTuo>();
            for (int i = 0; i < jorders.Count; i++)
            {
                JObject jorder = (JObject)jorders[i];
                StockWeiTuo wt = new StockWeiTuo();
                wt.Time = jorder["timestamp"].ToString();
                wt.Code = jorder["instrument_id"].ToString();
                wt.Name = wt.Code;
                if (jorder["price"].ToString() == "")
                {
                    wt.Price = 0;
                }
                else
                {
                    wt.Price = System.Convert.ToDouble(jorder["price"].ToString());
                }
                
                wt.Qty = System.Convert.ToDouble(jorder["size"].ToString());
                string status = jorder["status"].ToString();
                if (jorder.Property("side") != null)
                {
                    wt.Qty_deal = System.Convert.ToDouble(jorder["filled_size"].ToString());
                    if (wt.Qty_deal == 0) 
                    {
                        wt.Price_deal = 0;
                    }
                    else
                    {
                        wt.Price_deal = Math.Round(System.Convert.ToDouble(jorder["filled_notional"].ToString()) / wt.Qty_deal, 8, MidpointRounding.AwayFromZero);
                    }
                    wt.Fee = -Math.Round(wt.Price_deal * wt.Qty_deal * takerFeePercent, 8, MidpointRounding.AwayFromZero);
                    switch (status)
                    {
                        case "open":
                            wt.Status = TradeOrderStatus.Open;
                            break;
                        case "part_filled":
                            wt.Status = TradeOrderStatus.Part_Filled;
                            break;
                        case "filled":
                            wt.Status = TradeOrderStatus.Filled;
                            break;
                        case "cancelled":
                            wt.Status = TradeOrderStatus.Cancelled;
                            break;
                        case "failure":
                            wt.Status = TradeOrderStatus.Failure;
                            break;
                        default:
                            wt.Status = TradeOrderStatus.UnKnow;
                            break;
                    }
                }
                else
                {
                    wt.Qty_deal = System.Convert.ToDouble(jorder["filled_qty"].ToString());
                    wt.Price_deal = System.Convert.ToDouble(jorder["price_avg"].ToString());
                    wt.Fee = System.Convert.ToDouble(jorder["fee"].ToString());
                    switch (status)
                    {
                        case "-2":
                            wt.Status = TradeOrderStatus.Failure;
                            break;
                        case "-1":
                            wt.Status = TradeOrderStatus.Cancelled;
                            break;
                        case "0":
                            wt.Status = TradeOrderStatus.Open;
                            break;
                        case "1":
                            wt.Status = TradeOrderStatus.Part_Filled;
                            break;
                        case "2":
                            wt.Status = TradeOrderStatus.Filled;
                            break;
                        default:
                            wt.Status = TradeOrderStatus.UnKnow;
                            break;
                    }
                }
                wt.CancelTime = "";
                wt.WTnbr = jorder["order_id"].ToString();
                try
                {
                    wt.CWTnbr = jorder["client_oid"].ToString();
                }
                catch
                {
                    wt.CWTnbr = "";
                }
                wt.WeiTuo_Type = jorder["type"].ToString();

                switch (wt.WeiTuo_Type)
                {
                    case "1":
                        wt.Title1 = "1";
                        wt.Title2 = "开多";
                        break;
                    case "2":
                        wt.Title1 = "2";
                        wt.Title2 = "开空";
                        break;
                    case "3":
                        wt.Title1 = "3";
                        wt.Title2 = "平多";
                        break;
                    case "4":
                        wt.Title1 = "4";
                        wt.Title2 = "平空";
                        break;
                    case "limit":
                        if (jorder["side"].ToString() == "buy")
                        {
                            wt.Title1 = "5";
                            wt.Title2 = "限价买入";
                        }
                        else
                        {
                            wt.Title1 = "6";
                            wt.Title2 = "限价卖出";
                        }
                        break;
                    case "market":
                        if (jorder["side"].ToString() == "buy")
                        {
                            wt.Title1 = "7";
                            wt.Title2 = "市价买入";
                        }
                        else
                        {
                            wt.Title1 = "8";
                            wt.Title2 = "市价卖出";
                        }
                        break;
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
            #region 增量
            //JObject OrderBookInfo = e.Message;
            //string action = e.Action;
            //JArray orderBooks = (JArray)OrderBookInfo["data"];
            //string symbol = orderBooks[0]["symbol"].ToString();
            //if (action == "partial")
            //{
            //    if (!dsob.ContainsKey(symbol))
            //    {
            //        Dictionary<string, SortedDictionary<long, StockOrderBook>> ddsob = new Dictionary<string, SortedDictionary<long, StockOrderBook>>();
            //        ddsob.Add("Buy", new SortedDictionary<long, StockOrderBook>());
            //        ddsob.Add("Sell", new SortedDictionary<long, StockOrderBook>());
            //        dsob.Add(symbol, ddsob);
            //    }
            //    else
            //    {
            //        dsob[symbol]["Buy"].Clear();
            //        dsob[symbol]["Sell"].Clear();
            //    }
            //    for (int i = 0; i < orderBooks.Count; i++)
            //    {
            //        StockOrderBook item = new StockOrderBook();
            //        item.Id = System.Convert.ToInt64(orderBooks[i]["id"].ToString());
            //        item.Price = System.Convert.ToDouble(orderBooks[i]["price"].ToString());
            //        item.Size = System.Convert.ToInt64(orderBooks[i]["size"].ToString());
            //        item.Side = orderBooks[i]["side"].ToString();
            //        item.Symbol = symbol;
            //        dsob[symbol][item.Side].Add(item.Id, item);
            //    }
            //}
            //else if (action == "update")
            //{
            //    for (int i = 0; i < orderBooks.Count; i++)
            //    {
            //        string side = orderBooks[i]["side"].ToString();
            //        long id = System.Convert.ToInt64(orderBooks[i]["id"].ToString());
            //        if(orderBooks[i]["price"] != null){
            //            dsob[symbol][side][id].Price = System.Convert.ToDouble(orderBooks[i]["price"].ToString());
            //        }
            //        if(orderBooks[i]["size"] != null){
            //            dsob[symbol][side][id].Size = System.Convert.ToInt64(orderBooks[i]["size"].ToString());
            //        }
            //    }
            //}
            //else if (action == "delete")
            //{
            //    for (int i = 0; i < orderBooks.Count; i++)
            //    {
            //        string side = orderBooks[i]["side"].ToString();
            //        dsob[symbol][side].Remove(System.Convert.ToInt64(orderBooks[i]["id"].ToString()));
            //    }
            //}
            //else if (action == "insert")
            //{
            //    for (int i = 0; i < orderBooks.Count; i++)
            //    {
            //        StockOrderBook item = new StockOrderBook();
            //        item.Id = System.Convert.ToInt64(orderBooks[i]["id"].ToString());
            //        item.Price = System.Convert.ToDouble(orderBooks[i]["price"].ToString());
            //        item.Size = System.Convert.ToInt64(orderBooks[i]["size"].ToString());
            //        item.Side = orderBooks[i]["side"].ToString();
            //        item.Symbol = orderBooks[i]["symbol"].ToString();
            //        dsob[symbol][item.Side].Add(item.Id, item);
            //    }
            //}

            //var BidKeys = dsob[symbol]["Buy"].Keys.ToList();
            //var SellKeys = dsob[symbol]["Sell"].Keys.ToList();
            //if (BidKeys.Count >= 10 && SellKeys.Count >= 10)
            //{
            //    TickData t = new TickData();
            //    SecurityInfo si = DicSi[symbol];
            //    t.SecInfo = si;
            //    t.Time = DateTime.Now;
            //    t.Name = si.Name;
            //    t.Code = si.Code;
            //    t.IsReal = true;

            //    t.Bid = dsob[symbol]["Buy"][BidKeys[0]].Price;
            //    t.Ask = dsob[symbol]["Sell"][SellKeys[SellKeys.Count - 1]].Price;
            //    t.Last = (t.Bid + t.Ask) / 2;
            //    for (int i = 0; i < 10; i++)
            //    {
            //        t.Bids[i] = dsob[symbol]["Buy"][BidKeys[i]].Price;
            //        t.Asks[i] = dsob[symbol]["Sell"][SellKeys[SellKeys.Count - 1 - i]].Price;
            //        t.Bidsizes[i] = dsob[symbol]["Buy"][BidKeys[i]].Size;
            //        t.Asksizes[i] = dsob[symbol]["Sell"][SellKeys[SellKeys.Count - 1 - i]].Size;
            //    }
            //    if (queryDataArrival != null)
            //    {
            //        queryDataArrival(this, t);
            //    }
            //    if (si.Code == this._querySi.Code)
            //    {
            //        tradeQueryArrival(this, t);
            //    }
            //}
            #endregion
            #region 全量

            JObject OrderBookInfo = jomsg;
            JObject orderBooks = (JObject)OrderBookInfo["data"][0];
            string symbol = orderBooks["instrument_id"].ToString();
            TickData t = new TickData();
            SecurityInfo si = DicSi[symbol];
            t.SecInfo = si;
            t.Time = System.Convert.ToDateTime(orderBooks["timestamp"].ToString());
            t.Name = si.Name;
            t.Code = si.Code;
            t.IsReal = true;
            if (orderBooks["bids"].Count() > 0)
            {
                t.Bid = System.Convert.ToDouble(orderBooks["bids"][0][0].ToString());
                t.Ask = System.Convert.ToDouble(orderBooks["asks"][0][0].ToString());
                double last = CurrentTicker.getCurrentTickerPrice(market + symbol);
                t.Last = last == 0 ? (t.Bid + t.Ask) / 2 : last;
                for (int i = 0; i < 5; i++)
                {
                    t.Bids[i] = System.Convert.ToDouble(orderBooks["bids"][i][0].ToString());
                    t.Asks[i] = System.Convert.ToDouble(orderBooks["asks"][i][0].ToString());
                    t.Bidsizes[i] = System.Convert.ToDouble(orderBooks["bids"][i][1].ToString());
                    t.Asksizes[i] = System.Convert.ToDouble(orderBooks["asks"][i][1].ToString());
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
            JObject orderBooks = (JObject)OrderBookInfo["data"][0];
            string symbol = orderBooks["instrument_id"].ToString();
            CurrentTicker.Update(market + symbol, System.Convert.ToDouble(orderBooks["price"].ToString()));
        }
        #endregion

        #region 永续持仓
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void QuerySwapChiCang(JObject jomsg)
        {
            JObject ChiCangInfo = jomsg;
            JArray positions = new JArray();
            if (ChiCangInfo.Property("data") != null)
            {
                positions = (JArray)ChiCangInfo["data"];
            }
            else if (ChiCangInfo.Property("holding") != null)
            {
                positions = (JArray)ChiCangInfo["holding"];
            }
            for (int i = 0; i < positions.Count; i++)
            {
                JObject p = (JObject)positions[i];
                JArray holding = (JArray)p["holding"];
                for (int j = 0; j < holding.Count; j++)
                {
                    JObject position = (JObject)holding[i];
                    StockChiCang chicang = new StockChiCang();
                    chicang.Instrument_id = p["instrument_id"].ToString();
                    chicang.Margin_mode = p["margin_mode"].ToString();
                    chicang.Liquidation_price = position["liquidation_price"].ToString();
                    string side = position["side"].ToString();
                    if (side == "long")
                    {
                        chicang.Long_qty = position["position"].ToString();
                        chicang.Long_avail_qty = position["avail_position"].ToString();
                        chicang.Long_avg_cost = position["avg_cost"].ToString();
                    }
                    else
                    {
                        chicang.Short_qty = position["position"].ToString();
                        chicang.Short_avail_qty = position["avail_position"].ToString();
                        chicang.Short_avg_cost = position["avg_cost"].ToString();
                    }
                    chicang.Leverage = position["leverage"].ToString();
                    chicang.Created_at = position["timestamp"].ToString();
                    chicang.Realized_pnl = position["realized_pnl"].ToString();
                    bool find = false;
                    for (int m = 0; m < ChiCangList.Count; m++)
                    {
                        if (chicang.Instrument_id == ChiCangList[m].Instrument_id && chicang.Leverage == ChiCangList[m].Leverage)
                        {
                            find = true;
                            ChiCangList[m] = chicang;
                            break;
                        }
                    }
                    if (!find)
                    {
                        ChiCangList.Add(chicang);
                    }
                }
            }
            RaiseChiCang(ChiCangList);
        }
        #endregion

        #region 交割持仓
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void QueryFutureChiCang(JObject jomsg)
        {
            JObject ChiCangInfo = jomsg;
            JArray positions = new JArray();
            if (ChiCangInfo.Property("data") != null)
            {
                positions = (JArray)ChiCangInfo["data"];
            }
            else if (ChiCangInfo.Property("holding") != null)
            {
                positions = (JArray)ChiCangInfo["holding"];
            }
            for (int i = 0; i < positions.Count; i++)
            {
                JObject position = (JObject)positions[i];
                if (position["margin_mode"].ToString() == "crossed")
                {
                    StockChiCang chicang = new StockChiCang();
                    chicang.Instrument_id = position["instrument_id"].ToString();
                    chicang.Margin_mode = position["margin_mode"].ToString();
                    chicang.Liquidation_price = position["liquidation_price"].ToString();
                    chicang.Long_qty = position["long_qty"].ToString();
                    chicang.Long_avail_qty = position["long_avail_qty"].ToString();
                    chicang.Long_avg_cost = position["long_avg_cost"].ToString();
                    chicang.Long_settlement_price = position["long_settlement_price"].ToString();
                    chicang.Realized_pnl = position["realised_pnl"].ToString();
                    chicang.Leverage = position["leverage"].ToString();
                    chicang.Short_qty = position["short_qty"].ToString();
                    chicang.Short_avail_qty = position["short_avail_qty"].ToString();
                    chicang.Short_avg_cost = position["short_avg_cost"].ToString();
                    chicang.Short_settlement_price = position["short_settlement_price"].ToString();
                    chicang.Created_at = position["created_at"].ToString();
                    chicang.Updated_at = position["updated_at"].ToString();
                    bool find = false;
                    for (int m = 0; m < ChiCangList.Count; m++)
                    {
                        if (chicang.Instrument_id == ChiCangList[m].Instrument_id && chicang.Leverage == ChiCangList[m].Leverage)
                        {
                            find = true;
                            ChiCangList[m] = chicang;
                            break;
                        }
                    }
                    if (!find)
                    {
                        ChiCangList.Add(chicang);
                    }
                }

            }
            RaiseChiCang(ChiCangList);
        }
        #endregion

        #region 永续资金
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        void QuerySwapZiJing(JObject jomsg)
        {
            JObject ZiJinInfo = jomsg;
            JArray zijins = (JArray)ZiJinInfo["data"];
            for (int i = 0; i < zijins.Count; i++)
            {
                StockZiJing zj = new StockZiJing();
                zj.Instrument_id = zijins[i]["instrument_id"].ToString();
                zj.Equity = zijins[i]["equity"].ToString();
                zj.Margin = zijins[i]["margin"].ToString();
                zj.Frozen = zijins[i]["margin_frozen"].ToString();
                zj.Margin_ratio = zijins[i]["margin_ratio"].ToString();
                zj.Realized_pnl = zijins[i]["realized_pnl"].ToString();
                zj.Timestamp = zijins[i]["timestamp"].ToString();
                zj.Total_avail_balance = zijins[i]["total_avail_balance"].ToString();
                zj.Unrealized_pnl = zijins[i]["unrealized_pnl"].ToString();
                zj.Fixed_balance = zijins[i]["fixed_balance"].ToString();
                zj.Mode = zijins[i]["margin_mode"].ToString();
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
            }
            RaiseZiJin(ZiJinList);
        }
        #endregion

        #region 期货资金
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        void QueryFutureZiJing(JObject jomsg)
        {
            JObject ZiJinInfo = jomsg;
            JArray zijins = (JArray)ZiJinInfo["data"];
            for (int i = 0; i < zijins.Count; i++)
            {
                foreach (var item in zijins[i])
                {
                    StockZiJing zj = new StockZiJing();
                    zj.Instrument_id = ((JProperty)item).Name + "-FUTURE";
                    zj.Mode = ((JProperty)item).Value["margin_mode"].ToString();
                    zj.Equity = ((JProperty)item).Value["equity"].ToString();
                    zj.Total_avail_balance = ((JProperty)item).Value["total_avail_balance"].ToString();
                    zj.Margin = ((JProperty)item).Value["margin"].ToString();
                    zj.Realized_pnl = ((JProperty)item).Value["realized_pnl"].ToString();
                    zj.Unrealized_pnl = ((JProperty)item).Value["unrealized_pnl"].ToString();
                    zj.Margin_ratio = ((JProperty)item).Value["margin_ratio"].ToString();
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
                }
            }
            RaiseZiJin(ZiJinList);
        }
        #endregion
        
        #region 现货资金
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        void QuerySpotZiJing(JObject jomsg)
        {
            JObject ZiJinInfo = jomsg;
            JArray zijins = (JArray)ZiJinInfo["data"];
            
            for (int i = 0; i < zijins.Count; i++)
            {
                StockZiJing zj = new StockZiJing();
                zj.Instrument_id = zijins[i]["currency"].ToString() + "-SPOT";
                zj.Frozen = zijins[i]["hold"].ToString();
                zj.Equity = zijins[i]["available"].ToString();
                zj.Total_avail_balance = zijins[i]["balance"].ToString();
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
                
            }
            List<CurrentBanalce> cbs = new List<CurrentBanalce>();
            for (int i = 0; i < ZiJinList.Count; i++)
            {
                CurrentBanalce cb = new CurrentBanalce();
                cb.Code = ZiJinList[i].Instrument_id;
                cb.Ava = System.Convert.ToDouble(ZiJinList[i].Equity);
                cb.Total = System.Convert.ToDouble(ZiJinList[i].Total_avail_balance);
                cb.Frz = System.Convert.ToDouble(ZiJinList[i].Frozen);
                cbs.Add(cb);
            }
            CurrentBalances.Update(market, cbs);
            RaiseZiJin(ZiJinList);
        }
        #endregion
        #endregion

        #region rest请求函数
        public override string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false,bool array = false)
        {
            string paramData = json ? BuildJSON(param,array) : BuildQueryData(param);
            string url = function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
                
            string postData = (method != "GET") ? paramData : "";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
            webRequest.Method = method;
            if (auth)
            {
                string ts = GetTimeStamp().ToString();
                string message = ts + method + url + postData;
                byte[] signatureBytes =  hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);
                webRequest.Headers.Clear();
                webRequest.Headers.Add("OK-ACCESS-KEY", apiKey);
                webRequest.Headers.Add("OK-ACCESS-PASSPHRASE", passPhrase);
                webRequest.Headers.Add("OK-ACCESS-TIMESTAMP", ts);
                webRequest.Headers.Add("OK-ACCESS-SIGN", signatureString);
            }
            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
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
            var param = new Dictionary<string, string>();
            param["start"] = from;
            param["end"] = to;
            string r = "";
            switch (resolution)
            {
                case CandleResolution.M1:
                    r = "60";
                    break;
                case CandleResolution.M3:
                    r = "180";
                    break;
                case CandleResolution.M5:
                    r = "300";
                    break;
                case CandleResolution.M15:
                    r = "900";
                    break;
                case CandleResolution.M30:
                    r = "1800";
                    break;
                case CandleResolution.H1:
                    r = "3600";
                    break;
                case CandleResolution.H4:
                    r = "14400";
                    break;
                case CandleResolution.H6:
                    r = "21600";
                    break;
                case CandleResolution.D1:
                    r = "86400";
                    break;
                case CandleResolution.W1:
                    r = "604800";
                    break;
            }
            param["granularity"] = r;
            string res = Query("GET", string.Format("/api/{0}/v3/instruments/{1}/candles", si.Type, si.Code), param, false, false);
            try
            {
                JArray ja = (JArray)JsonConvert.DeserializeObject(res);
                JObject jo = new JObject();
                jo["data"] = ja;
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject GetAccount()
        {
            string res = Query("GET", "/api/swap/v3/accounts", null, true);
            JObject jo = (JObject)JsonConvert.DeserializeObject(res);
            return jo;
        }

        public override JObject GetBalance(string code, string type)
        {
            string res = "";
            if (type == "swap")
            {
                res = Query("GET", string.Format("/api/{0}/v3/{1}/accounts", type, code), null, true, false);
            }
            else if (type == "futures" || type == "spot")
            {
                res = Query("GET", string.Format("/api/{0}/v3/accounts/{1}", type, code), null, true, false);
            }

            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject GetPosition(string code, string type)
        {
            string res = Query("GET", string.Format("/api/{0}/v3/{1}/position", type, code), null, true, false);
            try
            {
                string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
                MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                Regex reg = new Regex(p);
                res = reg.Replace(res, matchEvaluator);
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject ChangeLeverage(string symbol, string leverage)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = symbol;
            param["leverage"] = leverage;
            string res = Query("GET", "/position/leverage", null, true);
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject GetOrder(SecurityInfo si, string orderId = "")
        {
            var param = new Dictionary<string, string>();
            string res = "";
            switch (si.Type)
            {
                case "swap":
                    res = Query("GET", string.Format("/api/swap/v3/orders/{0}/{1}", si.Code, orderId), param, true, false);
                    break;
                case "futures":
                    res = Query("GET", string.Format("/api/futures/v3/orders/{0}/{1}", si.Code, orderId), param, true, false);
                    break;
                case "spot":
                    param["instrument_id"] = si.Code;
                    res = Query("GET", string.Format("/api/spot/v3/orders/{0}", orderId), param, true, false);
                    break;
                default:
                    break;
            }
            try
            {
                string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
                MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                Regex reg = new Regex(p);
                res = reg.Replace(res, matchEvaluator);
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                JObject jdata = new JObject();
                jdata["data"] = new JArray() { jo };
                QueryWeiTuo(jdata);
                return jo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject GetOrders(string code, string type, bool keche = false, string status = "")
        {
            var param = new Dictionary<string, string>();
            param["limit"] = "50";

            string res = "";
            switch (type)
            {
                case "swap":
                    {
                        if (keche)
                        {
                            param["status"] = status;
                        }
                        res = Query("GET", string.Format("/api/swap/v3/orders/{0}", code), param, true, false);
                        string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
                        MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                        Regex reg = new Regex(p);
                        res = reg.Replace(res, matchEvaluator);
                        JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                        return jo;
                    }
                case "futures":
                    {
                        if (keche)
                        {
                            param["status"] = "6";
                        }
                        res = Query("GET", string.Format("/api/futures/v3/orders/{0}", code), param, true, false);
                        string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
                        MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                        Regex reg = new Regex(p);
                        res = reg.Replace(res, matchEvaluator);
                        JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                        return jo;
                    }
                case "spot":
                    {
                        param["instrument_id"] = code;
                        res = Query("GET", "/api/spot/v3/orders_pending", param, true, false);
                        string p = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z";
                        MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
                        Regex reg = new Regex(p);
                        res = reg.Replace(res, matchEvaluator);
                        JArray jarray = (JArray)JsonConvert.DeserializeObject(res);
                        JObject jo = new JObject();
                        jo["order_info"] = jarray;
                        return jo;
                    }
                default:
                    break;
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
                    {
                        var param = new Dictionary<string, string>();
                        param["price"] = price;
                        if (tsopt != "Market")
                        {
                            param["match_price"] = "0";
                        }
                        else
                        {
                            param["match_price"] = "1";
                        }
                        if (maker)
                        {
                            param["order_type"] = "1";
                        }
                        param["instrument_id"] = symbol;
                        param["type"] = tsoc;
                        param["size"] = orderQty;
                        if (clOrdID != "")
                        {
                            param["client_oid"] = clOrdID;
                        }
                        res = Query("POST", "/api/swap/v3/order", param, true,true);
                    }
                    break;
                case "futures" :
                    {
                        var param = new Dictionary<string, string>();
                        param["price"] = price;
                        if (tsopt != "Market")
                        {
                            param["match_price"] = "0";
                        }
                        else
                        {
                            param["match_price"] = "1";
                        }
                        if (maker)
                        {
                            param["order_type"] = "1";
                        }
                        param["leverage"] = leverage;
                        param["instrument_id"] = symbol;
                        param["type"] = tsoc;
                        param["size"] = orderQty;
                        if (clOrdID != "")
                        {
                            param["client_oid"] = clOrdID;
                        }
                        res = Query("POST", "/api/futures/v3/order", param, true,true);
                    }
                    break;
                case "spot":
                    {
                        var param = new Dictionary<string, string>();

                        if (tsopt != "Market")
                        {
                            param["price"] = price;
                            param["type"] = "limit";
                        }
                        else
                        {
                            param["notional"] = Math.Round(System.Convert.ToDouble(orderQty) * CurrentTicker.getCurrentTickerPrice(market + symbol), 2).ToString();
                            param["type"] = "market";
                        }
                        if (maker)
                        {
                            param["order_type"] = "1";
                        }
                        param["instrument_id"] = symbol;
                        if (tsoc == "5")
                        {
                            param["side"] = "buy";
                        }
                        else
                        {
                            param["side"] = "sell";
                        }

                        param["size"] = orderQty;
                        if (clOrdID != "")
                        {
                            param["client_oid"] = clOrdID;
                        }
                        res = Query("POST", "/api/spot/v3/orders", param, true,true);
                    }
                    break;
                default:
                    break;
            }
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                JObject resultJo = new JObject();
                if (jo.Property("order_id") != null && jo["order_id"].ToString() != "-1")
                {
                    resultJo["data"] = jo["order_id"].ToString();
                    resultJo["status"] = true;
                }
                else
                {
                    resultJo["data"] = jo.ToString();
                    resultJo["status"] = false;
                }
                return resultJo;
            }
            catch
            {
                return null;
            }
        }

        public override JObject DeleteOrder(SecurityInfo si, string orderId)
        {
            var param = new Dictionary<string, string>();
            string res = "";
            switch (si.Type)
            {
                case "swap":
                    res = Query("POST", string.Format("/api/swap/v3/cancel_order/{0}/{1}",si.Code,orderId), param, true, true);
                    break;
                case "futures":
                    res = Query("POST", string.Format("/api/futures/v3/cancel_order/{0}/{1}", si.Code, orderId), param, true, true);
                    break;
                case "spot":
                    param["instrument_id"] = si.Code;
                    res = Query("POST", string.Format("/api/spot/v3/cancel_orders/{0}", orderId), param, true, true);
                    break;
                default:
                    break;
            }
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                JObject resultJO = new JObject();
                if (jo.Property("error_message") != null && jo["error_message"].ToString() != "")
                {
                    resultJO["data"] = jo["error_message"].ToString();
                    resultJO["status"] = false;
                }
                else if (jo.Property("result") != null && jo["result"].ToString() == "True")
                {
                    resultJO["data"] = "";
                    resultJO["status"] = true;
                }
                else
                {
                    resultJO["data"] = jo.ToString();
                    resultJO["status"] = false;
                }
                return resultJO;
            }
            catch
            {
                return null;
            }
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
                string ids = "";
                var filterStr = new StringBuilder();
                filterStr.Append("[");
                foreach (string orderId in codeItem.Value)
                {
                    filterStr.Append("\"" + orderId + "\",");
                }
                ids = filterStr.ToString().TrimEnd(',') + "]";
                var param = new Dictionary<string, string>();

                string res = "";
                switch (si.Type)
                {
                    case "swap":
                        param["ids"] = ids;
                        res = Query("POST", "/api/swap/v3/cancel_batch_orders/" + codeItem.Key, param, true,true);
                        break;
                    case "futures":
                        param["order_ids"] = ids;
                        res = Query("POST", "/api/futures/v3/cancel_batch_orders/" + codeItem.Key, param, true, true);
                        break;
                    case "spot":
                        param["order_ids"] = ids;
                        param["instrument_id"] = codeItem.Key;
                        res = Query("POST", "/api/spot/v3/cancel_batch_orders", param, true, true,true);
                        break;
                    default:
                        break;
                }
                jo = (JObject)JsonConvert.DeserializeObject(res);
            }
            return jo;
        }
        #endregion
    }
}
