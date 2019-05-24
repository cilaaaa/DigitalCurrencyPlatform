using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using StockData;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using WebSocket4Net;
using SuperSocket.ClientEngine.Proxy;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace StockTradeAPI
{
    public delegate void StockWeiTuoArrival(object sender, List<StockWeiTuo> args);

    public delegate void QueryDataArrival(object sender, TickData args);

    public delegate void TradeQueryArrival(object sender, TickData args);

    public delegate void ChiCangArrival(object sender, List<StockChiCang> args);

    public delegate void ZiJinArrival(object sender, List<StockZiJing> args);

    public abstract class BaseTradeAPI
    {
        #region 申明变量
        public Dictionary<string, SecurityInfo> DicSi;
        public SecurityInfo _querySi;
        public bool EnableCID;
        public bool FullStop;

        #endregion
        #region 配置信息

        public string name = "";
        public string market = "";
        public string domain = "";
        public string apiKey = "";
        public string apiSecret = "";
        public string passPhrase = "";
        public string address = "";
        public double makerFeePercent = 0;
        public double takerFeePercent = 0;
        public int port = 0;
        public string WEBSOCKET_API = "";

        #endregion

        #region WebSocket
        public WebSocket websocket;
        public Dictionary<string, string> topicDic = new Dictionary<string, string>();
        public bool isOpened = false;
        

        public string MARKET_DEPTH = "{0}/depth5:{1}";
        public string TRADE = "{0}/trade:{1}";
        public string ORDER = "{0}/order:{1}";
        public string POSITION = "{0}/position:{1}";
        public string ACCOUNT = "{0}/account:{1}";

        public Thread XinTiaoTh;
        public DateTime LastTime;
        public bool SendXinTiao;
        public bool InitXinTiao = false;
        public bool AbortedWs = false;

        public bool InitWebSocket()
        {
            try
            {
                AbortedWs = false;
                websocket = new WebSocket(WEBSOCKET_API);
                websocket.Error += (sender, e) =>
                {
                    Console.WriteLine("Error:" + e.Exception.Message.ToString());
                };
                if (port != 0)
                {
                    var proxy = new HttpConnectProxy(new IPEndPoint(IPAddress.Parse(address), port));
                    websocket.Proxy = proxy;
                }
                websocket.Opened += OnOpened;
                websocket.Closed += OnClosed;
                websocket.DataReceived += ReceviedData;
                websocket.MessageReceived += ReceviedMsg;
                websocket.Open();
                if (!InitXinTiao)
                {
                    InitXinTiao = true;
                    XinTiaoTh = new Thread(new ThreadStart(CheckXinTiao));
                    LastTime = DateTime.Now;
                    XinTiaoTh.Start();
                    SendXinTiao = false;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
            return true;
        }

        public void DisposeWebSocket()
        {
            try
            {
                AbortedWs = true;
                websocket.Dispose();
                isOpened = false;
            }
            catch { }
        }

        public abstract void CheckXinTiao();

        // Opened&心跳响应&触发消息事件
        /// <summary>
        /// 连通WebSocket，发送订阅消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnOpened(object sender, EventArgs e);

        public abstract void ReceviedData(object sender, DataReceivedEventArgs args);

        public abstract void ReceviedMsg(object sender, MessageReceivedEventArgs args);

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="id"></param>
        public abstract void Subscribe(string _topic, string subType, string instrumentType);

        public void SendSubscribeTopic(string msg)
        {
            websocket.Send(msg);
        }

        public void OnClosed(object sender, EventArgs e)
        {
            isOpened = false;
            Console.WriteLine("重启websocket");
            try
            {
                websocket.Open();
            }
            catch
            {
                websocket.Dispose();
                XinTiaoTh.Abort();
                InitWebSocket();
            }
            
        }
        #endregion

        #region 辅助函数
        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        public string ConvertDateStringToJsonDate(Match m)
        {
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
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
            return builder.ToString();
        }

        public string Decompress(byte[] baseBytes)
        {
            using (var decompressedStream = new MemoryStream())
            using (var compressedStream = new MemoryStream(baseBytes))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(decompressedStream);
                decompressedStream.Position = 0;
                using (var streamReader = new StreamReader(decompressedStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            try { return b.ToString().Substring(1); }
            catch (Exception) { return ""; }
        }

        public string BuildJSON(Dictionary<string, string> param, bool array = false)
        {
            if (param == null)
                return "";

            var entries = new List<string>();
            foreach (var item in param)
                if (item.Value.Contains("["))
                {
                    entries.Add(string.Format("\"{0}\":{1}", item.Key, item.Value));
                }
                else
                {
                    entries.Add(string.Format("\"{0}\":\"{1}\"", item.Key, item.Value));
                }

            if (array)
            {
                return "[{" + string.Join(",", entries) + "}]";
            }
            else
            {
                return "{" + string.Join(",", entries) + "}";
            }
        }

        public string ByteArrayToString(byte[] ba)
        {
            return Convert.ToBase64String(ba);
        }

        public string ToBase64(string str)
        {
            System.Text.Encoding encode = System.Text.Encoding.ASCII;
            byte[] bytedata = encode.GetBytes(str);
            string strPath = Convert.ToBase64String(bytedata, 0, bytedata.Length);
            return strPath;
        }

        public double GetTimeStamp(bool ToSecond = true)
        {
            TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            long t = (long)cha.TotalMilliseconds;
            double t_r = t;
            if (ToSecond)
            {
                t_r = t / (double)1000;
            }
            return t_r;
        }

        public void ClearQuery5()
        {
            this._querySi = new SecurityInfo(string.Empty, string.Empty, "", 0, 0, 0, "", "");
        }

        public void SetQuery5(SecurityInfo securityInfo)
        {
            this._querySi = securityInfo;
        }
        #endregion

        #region 解析函数
        public abstract void QueryWeiTuo(JObject jomsg);
        public event StockWeiTuoArrival WeiTuo_Arrival;
        public void RaiseWeiTuo(List<StockWeiTuo> args)
        {
            if (WeiTuo_Arrival != null)
            {
                WeiTuo_Arrival(this, args);
            }
        }

        public abstract void QueryQuote5(JObject jomsg);
        public event QueryDataArrival queryDataArrival;
        public void RaiseQueryData(TickData args)
        {
            if (queryDataArrival != null)
            {
                queryDataArrival(this, args);
            }
        }

        public abstract void QueryChengJiaoJia(JObject jomsg);
        public event TradeQueryArrival tradeQueryArrival;
        public void RaiseTradeQueryData(TickData args)
        {
            if (tradeQueryArrival != null)
            {
                tradeQueryArrival(this, args);
            }
        }

        public event ChiCangArrival ChiCang_Arrival;
        public void RaiseChiCang(List<StockChiCang> args)
        {
            if (ChiCang_Arrival != null)
            {
                ChiCang_Arrival(this, args);
            }
        }

        public event ZiJinArrival ZiJin_Arrival;
        public void RaiseZiJin(List<StockZiJing> args)
        {
            if (ZiJin_Arrival != null)
            {
                ZiJin_Arrival(this, args);
            }
        }
        #endregion
        #region REST请求

        #region BaseRestFun

        public abstract string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false, bool array = false);
        
        #endregion

        #region orderBook
        public abstract JArray GetOrderBook(string symbol, int depth);

        public abstract JObject GetKLine(SecurityInfo si, CandleResolution resolution, string from, string to);
        #endregion

        #region account
        public abstract JObject GetAccount();

        public abstract JObject GetBalance(string code, string type);
        #endregion

        #region position
        public abstract JObject GetPosition(string code, string type);

        public abstract JObject ChangeLeverage(string symbol,string leverage);
        #endregion

        #region orders
        public abstract JObject GetOrder(SecurityInfo si, string orderId = "");

        public abstract JObject GetOrders(string code,string type,bool keche = false,string status = "");

        public abstract JArray GetMatchOrders();

        public abstract JObject PostOrders(string symbol, string tsoc, string price, string orderQty, string tsopt, string symbolType, string clOrdID = "", bool maker = false, string leverage = "10");

        public abstract JObject DeleteOrder(SecurityInfo si,string orderId);

        public abstract JObject DeleteOrders(List<Dictionary<string,string>> orderIds);
        #endregion

        #endregion
    }

    public class TMessageReceivedEventArgs : EventArgs
    {
        public TMessageReceivedEventArgs(JObject message)
        {
            this.Message = message;
        }

        public JObject Message { get; set; }

    }
}
