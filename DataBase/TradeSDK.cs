using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;


//
//byte[ ] 转换为string
//byte[ ] image;
//string ll = Encoding.Default.GetString(image);
//string 转换为byte[ ]
//string ss;
//byte[] b = Encoding.Default.GetBytes(ss);
//
namespace DataBase
{
    public class TradeSDK
    {
        static List<TradeSDK> _sdkList;
        static XmlDocument _xmlDoc;
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _desc;

        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }
        byte[] _sdk;

        public byte[] SDK
        {
            get { return _sdk; }
            set { _sdk = value; }
        }

        public static void Load()
        {
            _sdkList = new List<TradeSDK>();
            _xmlDoc = new XmlDocument();

            if (!File.Exists(ConfigFileName.SDKFileName))
            {
                StreamWriter sw = new StreamWriter(ConfigFileName.SDKFileName, true, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<TradeSDK>");
                sw.WriteLine("<Items>");
                sw.WriteLine("</Items>");
                sw.WriteLine("</TradeSDK>");
                sw.Close();
            }
            _xmlDoc.Load(ConfigFileName.SDKFileName);
            XmlNode root = _xmlDoc.SelectSingleNode("TradeSDK");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList SDKS = item.SelectNodes("SDK");

            foreach (XmlNode SDK in SDKS)
            {

                string name = SDK.Attributes["name"].Value;
                string desc = SDK.SelectSingleNode("Desc").FirstChild.Value;
                string codestring = SDK.SelectSingleNode("Code").InnerText;
                TradeSDK pp = new TradeSDK();
                pp._name = name;
                pp._desc = desc;
                pp._sdk = Convert.FromBase64String(codestring);
                _sdkList.Add(pp);
            }

        }

        public static string getDllName(string market)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name.ToUpper() == ("TRADEAPI" + market + ".dll").ToUpper())
                {
                    return _sdkList[i]._name;
                }
            }
            return "";
        }

        public static byte[] getSDK(string dllName)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name == dllName)
                {
                    return _sdkList[i]._sdk;
                }
            }
            return new byte[1];
        }

        public static DataTable getSDKList()
        {
            DataTable list = new DataTable("SDK");
            list.Columns.Add("dll_name");
            list.Columns.Add("dll_desc");
            for (int i = 0; i < _sdkList.Count; i++)
            {
                DataRow dr = list.NewRow();
                dr[0] = _sdkList[i]._name;
                dr[1] = _sdkList[i]._desc;
                list.Rows.Add(dr);
            }
            return list;
        }

        public static bool exists(string dllName)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name == dllName)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateDLL(string dllname, byte[] sdk, string desc)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name == dllname)
                {
                    _sdkList[i]._desc = desc;
                    _sdkList[i]._sdk = sdk;
                    UpdateXML(dllname, sdk, desc);
                    return;
                }
            }
        }

        public static void InsertDLL(string dllname, byte[] sdk, string desc)
        {
            TradeSDK pp = new TradeSDK();
            pp._name = dllname;
            pp._sdk = sdk;
            pp._desc = desc;
            _sdkList.Add(pp);
            InsertXML(dllname, sdk, desc);
        }

        private static void InsertXML(string dllname, byte[] sdk, string desc)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("TradeSDK");
            XmlNode item = root.SelectSingleNode("Items");
            XmlElement ep = _xmlDoc.CreateElement("SDK");
            ep.SetAttribute("name", dllname);
            XmlNode nd = _xmlDoc.CreateElement("Desc");
            nd.InnerText = desc;
            ep.AppendChild(nd);
            XmlNode np = _xmlDoc.CreateElement("Code");
            np.AppendChild(_xmlDoc.CreateCDataSection(Convert.ToBase64String(sdk)));
            ep.AppendChild(np);
            item.AppendChild(ep);
            _xmlDoc.Save(ConfigFileName.SDKFileName);

        }

        public static void DeleteDLL(string dllname)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name == dllname)
                {
                    _sdkList.RemoveAt(i);
                    DeleteXML(dllname);
                    return;
                }
            }
        }

        private static void DeleteXML(string dllname)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("TradeSDK");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList SDKS = item.SelectNodes("SDK");
            for (int i = 0; i < SDKS.Count; i++)
            {
                if (SDKS[i].Attributes["name"].Value == dllname)
                {
                    item.RemoveChild(SDKS[i]);
                    break;
                }
            }
            _xmlDoc.Save(ConfigFileName.SDKFileName);
        }


        private static void UpdateXML(string dllname, byte[] source, string desc)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("TradeSDK");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList SDKS = item.SelectNodes("SDK");
            foreach (XmlNode SDK in SDKS)
            {
                if (SDK.Attributes["name"].Value == dllname)
                {
                    SDK.SelectSingleNode("Desc").FirstChild.Value = desc;
                    ((XmlCDataSection)SDK.SelectSingleNode("Code").FirstChild).Value = Convert.ToBase64String(source);
                    break;
                }
            }
            _xmlDoc.Save(ConfigFileName.SDKFileName);
        }

        public static string getSDKName(string sdkname)
        {
            for (int i = 0; i < _sdkList.Count; i++)
            {
                if (_sdkList[i]._name == sdkname)
                {
                    return _sdkList[i]._desc;
                }
            }
            return string.Empty;
        }
    }
}
