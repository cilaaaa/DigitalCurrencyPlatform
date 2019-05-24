using StockData;
using System.Xml;

//policybidvolumnC
//PolicyBidVolumnSellC
//PolicyDealVolumnEE
//PolicyNewLows15
//PolicyReturnUp





namespace StockPolicies
{
    public class ParameterFile
    {
        public static double getDouble(string filename,string programname,SecurityInfo si,string account,string parameter)
        {
            try
            {
                return System.Convert.ToDouble(getString(filename, programname, si, account, parameter));
            }
            catch
            {
                return 0;
            }
        }

        public static int getInt(string filename, string programname, SecurityInfo si, string account, string parameter)
        {
            try
            {
                return System.Convert.ToInt32(getString( filename, programname, si, account, parameter));
            }
            catch
            {
                return 0;
            }
        }
        internal static bool getBool(string filename, string programname, SecurityInfo si, string account, string parameter)
        {
            try
            {
                return System.Convert.ToBoolean(getInt(filename, programname, si, account, parameter));
            }
            catch
            {
                return false;
            }
        }
        public static string getString(string filename, string programname, SecurityInfo si, string account, string parameter)
        {
            try
            {
                //string returnValue;
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode root = doc.SelectSingleNode("PolicyPool");
                XmlNodeList accounts = root.SelectNodes("Account");
                foreach(XmlNode accountNode in accounts)
                {
                    if (accountNode.Attributes["name"].Value == account)
                    {
                        XmlNodeList policyitems = accountNode.SelectNodes("PolicyItem");
                        foreach (XmlNode policyitem in policyitems)
                        {
                            if(policyitem.Attributes["programName"].Value.ToUpper() == programname.ToUpper())
                            {
                                XmlNodeList securityList = policyitem.SelectNodes("SecurityInfo");
                                foreach(XmlNode securityCode in securityList)
                                {
                                    if(securityCode.Attributes["code"].Value == si.Code && securityCode.Attributes["market"].Value == si.Market.ToString())
                                    {
                                        XmlNode parameterNode = securityCode.SelectSingleNode("Parameter");
                                        string x = parameterNode.SelectSingleNode(parameter).FirstChild.Value;
                                        return x;
                                    }
                                }
                            }
                        }
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        
    }
}
