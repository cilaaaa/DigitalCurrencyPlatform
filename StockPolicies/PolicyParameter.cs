using StockData;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace StockPolicies
{
    //创建一个策略参数的类
    public class PolicyParameter
    {
        protected int inteval;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("Tick间隔")]
        [Description("策略执行Tick间隔")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int Inteval
        {
            get { return inteval; }
            set { inteval = value; }
        }
        protected DateTime startDate;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("开始日期")]
        [Description("策略执行开始日期")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        protected DateTime endDate;

        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("结束日期")]
        [Description("策略执行结束日期")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        protected bool isReal;
        [Browsable(false)]
        [Category("1-环境参数")]
        [DisplayName("是否实盘")]
        [Description("是否使用实盘验证或是回测验证，设为实盘的话将忽略日期设置")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool IsReal
        {
            get { return isReal; }
            set { isReal = value; }
        }

        private bool canStart;
        [Browsable(true)]
        [Category("1-环境参数")]
        [DisplayName("是否启动策略")]
        [Description("启动策略/暂停策略")]
        public bool CanStart
        {
            get { return canStart; }
            set { canStart = value; }
        }
        //private bool dynamicOpen;
        //[Browsable(true)]
        //[Category("2-入场参数")]
        //[DisplayName("动态开仓")]
        //[Description("是否动态开仓")]
        //[EditorBrowsable(EditorBrowsableState.Always)]
        //public bool DynamicOpen
        //{
        //    get { return dynamicOpen; }
        //    set { dynamicOpen = value; }
        //}

        public double qty = 100;
        [Browsable(true)]
        [Category("X-交易参数")]
        [DisplayName("单次交易股数")]
        [Description("单次交易股数")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        public double fee = 0.0003;
        [Browsable(true)]
        [Category("X-交易参数")]
        [DisplayName("交易手续费")]
        [Description("单向交易手续费")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public double Fee
        {
            get { return fee; }
            set { fee = value; }
        }
        private TradeSendOrderPriceType enterOrderPriceType;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("入场交易类型")]
        [Description("入场交易类型")]
        public TradeSendOrderPriceType EnterOrderPriceType
        {
            get { return enterOrderPriceType; }
            set { enterOrderPriceType = value; }
        }

        private int enterOrderWaitSecond;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("入场挂单等待秒数")]
        [Description("入场挂单等待秒数")]
        public int EnterOrderWaitSecond
        {
            get { return enterOrderWaitSecond; }
            set { enterOrderWaitSecond = value; }
        }

        private double reEnterPercent;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("入场追单百分比(%)")]
        [Description("入场追单百分比(%) -1 表示不追单")]
        public double ReEnterPercent
        {
            get { return reEnterPercent; }
            set { reEnterPercent = value; }
        }


        private TradeSendOrderPriceType closeorderPriceType;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("出场交易类型")]
        [Description("出场交易类型")]
        public TradeSendOrderPriceType CloseOrderPriceType
        {
            get { return closeorderPriceType; }
            set { closeorderPriceType = value; }
        }

        private int closeOrderWaitSecond;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("出场挂单等待秒数")]
        [Description("出场挂单等待秒数")]
        public int CloseOrderWaitSecond
        {
            get { return closeOrderWaitSecond; }
            set { closeOrderWaitSecond = value; }
        }


        private TradeSendOrderPriceType reCloseorderPriceType;
        [Browsable(true)]
        [Category("Y-交易参数")]
        [DisplayName("再出场交易类型")]
        [Description("再出场交易类型")]
        public TradeSendOrderPriceType ReCloseOrderPriceType
        {
            get { return reCloseorderPriceType; }
            set { reCloseorderPriceType = value; }
        }

        


        private string filename;
        [Browsable(false)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private SecurityInfo si;
        [Browsable(false)]
        public SecurityInfo SI
        {
            get { return si; }
            set { si = value; }
        }

        private string programName;
        [Browsable(false)]
        public string ProgramName
        {
            get { return programName; }
            set { programName = value; }
        }
        private string account;
        [Browsable(false)]
        public string Account
        {
            get { return account; }
            set { account = value; }
        }

        protected double getDouble(string parameter)
        {
            return ParameterFile.getDouble(this.filename, this.programName, this.si, this.account, parameter);
        }

        protected int getInt(string parameter)
        {
            return ParameterFile.getInt(this.filename, this.programName, this.si, this.account, parameter);
        }
        protected string getString(string parameter)
        {
            return ParameterFile.getString(this.filename, this.programName, this.si, this.account, parameter);
        }
        protected bool getBool(string parameter)
        {
            return ParameterFile.getBool(this.filename, this.programName, this.si, this.account, parameter);
        }

        protected void GetGeneralValue()
        {
            try
            {
                this.canStart = getBool("CanStart");
            }
            catch
            {
                this.canStart = true;
            }
            try
            {
                this.Fee = getDouble("Fee");
            }
            catch
            {
                this.Fee = 0.0003;
            }
            try
            {
                this.Qty = getInt("Qty");
            }
            catch
            {
                this.Qty = 100;
            }
            //try
            //{
            //    this.dynamicOpen = getBool("DynamicOpen");
            //}
            //catch
            //{
            //    this.dynamicOpen = false;
            //}
            TradeSendOrderPriceType entertype;

            if (Enum.TryParse(getString("EnterOrderPriceType"), out entertype))
            {
                this.EnterOrderPriceType = entertype;
            }
            else
            {
                this.EnterOrderPriceType = TradeSendOrderPriceType.Limit;
            }

            TradeSendOrderPriceType closetype;
            if (Enum.TryParse(getString("CloseOrderPriceType"), out closetype))
            {
                this.CloseOrderPriceType = closetype;
            }
            else
            {
                this.CloseOrderPriceType = TradeSendOrderPriceType.Limit;
            }
            TradeSendOrderPriceType reclosetype;
            if (Enum.TryParse(getString("ReCloseOrderPriceType"), out reclosetype))
            {
                this.ReCloseOrderPriceType = reclosetype;
            }
            else
            {
                this.ReCloseOrderPriceType = TradeSendOrderPriceType.Limit;
            }

            try
            {
                this.EnterOrderWaitSecond = getInt("EnterOrderWaitSecond");
            }
            catch
            {
                this.EnterOrderWaitSecond = 6;
            }

            try
            {
                this.CloseOrderWaitSecond = getInt("CloseOrderWaitSecond");
            }
            catch
            {
                this.CloseOrderWaitSecond = 6;
            }

            try
            {
                this.ReEnterPercent = getDouble("ReEnterPercent");
            }
            catch
            {
                this.ReEnterPercent = 0.2;
            }

        }

        protected bool SaveFile(List<string[]> values)
        {
            values.Add(new string[] { "CanStart", System.Convert.ToInt32(this.canStart).ToString() });
            values.Add(new string[] { "IsReal", System.Convert.ToInt32(this.isReal).ToString() });
            values.Add(new string[] { "Qty", this.Qty.ToString() });
            values.Add(new string[] { "Fee", this.Fee.ToString() });
            values.Add(new string[] { "EnterOrderPriceType", Enum.GetName(typeof(TradeSendOrderPriceType), this.EnterOrderPriceType) });
            values.Add(new string[] { "EnterOrderWaitSecond", this.EnterOrderWaitSecond.ToString() });
            values.Add(new string[] { "ReEnterPercent", this.ReEnterPercent.ToString() });
            values.Add(new string[] { "CloseOrderPriceType", Enum.GetName(typeof(TradeSendOrderPriceType), this.CloseOrderPriceType) });
            values.Add(new string[] { "CloseOrderWaitSecond", this.CloseOrderWaitSecond.ToString() });
            values.Add(new string[] { "ReCloseOrderPriceType", Enum.GetName(typeof(TradeSendOrderPriceType), this.ReCloseOrderPriceType) });
            //values.Add(new string[] { "DynamicOpen", System.Convert.ToInt32(this.dynamicOpen) .ToString()});
            XmlDocument file = new XmlDocument();
            file.Load(this.Filename);
            XmlNode root = file.SelectSingleNode("PolicyPool");
            XmlNodeList accounts = root.SelectNodes("Account");
            bool findaccount = false;
            XmlNode parameterNode = file.CreateElement("Parameter");
            foreach (XmlNode accountNode in accounts)
            {
                if (accountNode.Attributes["name"].Value == this.Account)
                {
                    findaccount = true;
                    bool findpolicyitem = false;
                    XmlNodeList policyitems = accountNode.SelectNodes("PolicyItem");
                    foreach (XmlNode policyitem in policyitems)
                    {
                        if (policyitem.Attributes["programName"].Value.ToUpper() == this.ProgramName.ToUpper())
                        {
                            findpolicyitem = true;
                            bool findSecurityInfo = false;
                            XmlNodeList securityList = policyitem.SelectNodes("SecurityInfo");
                            foreach (XmlNode securityCode in securityList)
                            {
                                if (securityCode.Attributes["code"].Value == this.SI.Code && securityCode.Attributes["market"].Value == this.SI.Market.ToString())
                                {
                                    if (securityCode.Attributes["enabled"] != null)
                                    {
                                        securityCode.Attributes["enabled"].Value = "1";
                                    }else
                                    {
                                        XmlNode t = file.CreateNode(XmlNodeType.Attribute, "enabled", string.Empty);
                                        t.Value = "1";
                                        securityCode.Attributes.SetNamedItem(t);
                                    }
                                    findSecurityInfo = true;
                                    parameterNode = securityCode.SelectSingleNode("Parameter");
                                    if (object.Equals(null, parameterNode))
                                    {

                                        parameterNode = securityCode.AppendChild(file.CreateElement("Parameter"));
                                    }
                                    break;
                                }
                            }
                            if (!findSecurityInfo)
                            {
                                XmlElement sicode = file.CreateElement("SecurityInfo");
                                sicode.SetAttribute("code", this.SI.Code);
                                sicode.SetAttribute("market", this.SI.Market.ToString());
                                sicode.SetAttribute("name", this.SI.Name);
                                sicode.SetAttribute("enabled", "1");
                                parameterNode = sicode.AppendChild(file.CreateElement("Parameter"));
                                policyitem.AppendChild(sicode);
                            }
                            break;
                        }
                    }
                    if (!findpolicyitem)
                    {
                        XmlElement pitem = file.CreateElement("PolicyItem");
                        pitem.SetAttribute("programName", this.ProgramName);
                        XmlElement sicode = file.CreateElement("SecurityInfo");
                        sicode.SetAttribute("code", this.SI.Code);
                        sicode.SetAttribute("market", this.SI.Market.ToString());
                        sicode.SetAttribute("name", this.SI.Name);
                        sicode.SetAttribute("enabled", "1");
                        parameterNode = sicode.AppendChild(file.CreateElement("Parameter"));
                        pitem.AppendChild(sicode);
                        accountNode.AppendChild(pitem);
                    }
                    break;
                }
            }
            if (!findaccount)
            {
                XmlElement acc = file.CreateElement("Account");
                acc.SetAttribute("name", this.Account);
                XmlElement pitem = file.CreateElement("PolicyItem");
                pitem.SetAttribute("programName", this.ProgramName);
                XmlElement sicode = file.CreateElement("SecurityInfo");
                sicode.SetAttribute("code", this.SI.Code);
                sicode.SetAttribute("market", this.SI.Market.ToString());
                sicode.SetAttribute("name", this.SI.Name);
                sicode.SetAttribute("enabled", "1");
                parameterNode = sicode.AppendChild(file.CreateElement("Parameter"));
                pitem.AppendChild(sicode);
                acc.AppendChild(pitem);
                root.AppendChild(acc);
            }

            for(int i=0;i<values.Count;i++)
            {
                XmlNode node = parameterNode.SelectSingleNode(values[i][0]);
                if (object.Equals(null, node))
                {
                    XmlNode d = file.CreateElement(values[i][0]);
                    d.InnerText = values[i][1];
                    //d.FirstChild.Value = this.dealVolumn.ToString();
                    parameterNode.AppendChild(d);

                }
                else
                {
                    node.InnerText = values[i][1];
                }
            }

            file.Save(this.Filename);
            return true;
        }
    }
}
