
using DataBase;
using StockData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace StockData
{
    public static class GlobalValue
    {
        private static object requestLockObj = new object();
        private static object maxOrderRefLockObj = new object();
        public static SortedList<string, SecurityInfo> G_SortedFutureInfos;

        public static List<SecurityInfo> SecurityList;

        public static void Initialize()
        {
            SecurityList = new List<SecurityInfo>();
            if (!File.Exists(ConfigFileName.ReceiveCodeFileName))
            {
                StreamWriter sw = new StreamWriter(ConfigFileName.ReceiveCodeFileName, true, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<ReceiveCode>");
                sw.WriteLine("</ReceiveCode>");
                sw.Close();
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFileName.ReceiveCodeFileName);
            XmlNode root = doc.SelectSingleNode("ReceiveCode");
            XmlNodeList items = root.SelectNodes("Item");
            if (!object.Equals(null, items))
            {
                foreach (XmlNode item in items)
                {
                    string code = item.Attributes["code"].Value;
                    string name = item.Attributes["name"].Value;
                    string market = item.Attributes["market"].Value.ToString();
                    double minqty = System.Convert.ToDouble(item.Attributes["minqty"].Value);
                    int jingdu = System.Convert.ToInt32(item.Attributes["jingdu"].Value);
                    double pricejingdu = System.Convert.ToDouble(item.Attributes["pricejingdu"].Value);
                    string type = item.Attributes["type"].Value;
                    string contractVal = item.Attributes["contractval"].Value;
                    SecurityInfo si = new SecurityInfo(code, name, market, minqty, jingdu, pricejingdu, type, contractVal);
                    SecurityList.Add(si);
                }
            }
            GetFutureList();
        }

        private static void GetFutureList()
        {
            G_SortedFutureInfos = new SortedList<string, SecurityInfo>();
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFileName.FutureCodeFileName);
            XmlNode root = doc.SelectSingleNode("FutureTimeRange");
            XmlNodeList items = root.SelectNodes("Item");
            foreach (XmlNode item in items)
            {
                string code = item.Attributes["code"].Value;
                string name = item.Attributes["name"].Value;
                string market = item.Attributes["market"].Value.ToString();
                double minqty = System.Convert.ToDouble(item.Attributes["minqty"].Value);
                int jingdu = System.Convert.ToInt32(item.Attributes["jingdu"].Value);
                double pricejingdu = System.Convert.ToDouble(item.Attributes["pricejingdu"].Value);
                string type = item.Attributes["type"].Value;
                string contractVal = item.Attributes["contractval"].Value;
                SecurityInfo si = new SecurityInfo(code, name, market, minqty, jingdu, pricejingdu, type, contractVal);
                G_SortedFutureInfos.Add(si.Code + market, si);
            }
        }

        /// <summary>
        /// 查找代码
        /// </summary>
        /// <param name="code">代码</param>
        /// <returns>FutureInfo实例，如没有找到，返回null</returns>
        public static List<SecurityInfo> GetFutureByCode(string code)
        {
            List<SecurityInfo> ls = new List<SecurityInfo>();
            foreach (var t in G_SortedFutureInfos)
            {
                if (t.Value.Code == code)
                {
                    ls.Add(t.Value);
                }
            }
            return ls;
        }

        public static SecurityInfo GetFutureByCodeAndMarket(string code, string market)
        {
            if (G_SortedFutureInfos.ContainsKey(code + market))
            {
                return G_SortedFutureInfos[code + market];
            }
            else
            {
                return null;
            }
        }
    }
}
