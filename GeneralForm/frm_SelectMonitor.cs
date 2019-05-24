using C1.Win.C1Ribbon;
using DataBase;
using StockData;
using StockPolicies;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GeneralForm
{
    public delegate void AddPolicies(object sender,SelectMonitorEventArgs args);
    public partial class frm_SelectMonitor : C1RibbonForm
    {
        //public frm_RunPolicy MainForm;
        bool canadd = false;
        private bool isSim;

        private string stockAccount;

        public event AddPolicies onAddPolicies;

        string market;


        public frm_SelectMonitor(bool isSim,BaseTradeAPI bta)
        {
            // TODO: Complete member initialization
            this.isSim = isSim;
            this.stockAccount = bta.name;
            this.market = bta.market;
            InitializeComponent();
#if READONLY
            this.propertyGrid1.Visible = false;
#endif
        }

        private void btn_sure_Click(object sender, EventArgs e)
        {
            AddStockCode();
        }
        private void AddStockCode()
        {
            string code = com_type.Text.Trim();
            if (code == string.Empty)
            {
                MessageBox.Show("请输入股票代码");
            }
            else
            {
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, market);
                if (si == null)
                {
                    MessageBox.Show("找不到股票代码");
                    return;
                }
                AddStockCode(si);
            }
        }

        private void AddStockCode(SecurityInfo si)
        {
            ComboBoxItem cbi = (ComboBoxItem)this.cb_policy.SelectedItem;

            //string dllname = cbi.Value;
            string policyname = cbi.Text;
            string programname = ((ComboBoxItem)this.cb_policy.SelectedItem).Value;

            AddStockCode(si, policyname, programname,string.Format("{0}\\parameter.xml",Application.StartupPath));
        }

        private void AddStockCode(SecurityInfo si, string policyname, string programname,string filename)
        {
            try
            {
                string dllname = programname.Substring(0, programname.Length - 4);
                Assembly assembly = Assembly.Load(PolicyProgram.getProgram(programname));
                Type ClassParamter = assembly.GetType(string.Format("{0}.Parameter", dllname));
                Object ObjectParameter = assembly.CreateInstance(ClassParamter.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, stockAccount,filename }, null, null);
                //int i = grid_stocks.Rows.Add();
                //grid_stocks.Rows[i].Cells[0].Value = si.Code;
                //grid_stocks.Rows[i].Cells[1].Value = si.Name;
                //grid_stocks.Rows[i].Cells[2].Value = si.Market1;
                //grid_stocks.Rows[i].Cells[3].Value = policyname;
                //grid_stocks.Rows[i].Cells[4].Value = "删除";
                //grid_stocks.Rows[i].Cells[5].Value = programname;
                //grid_stocks.Rows[i].Tag = new GridRowTag(assembly,ObjectParameter);

                grid_stockX.Rows.Add();
                int row = grid_stockX.Rows.Count - 1;
                grid_stockX.Rows[row][0] = si.Code;
                grid_stockX.Rows[row][1] = si.Name;
                grid_stockX.Rows[row][2] = si.Market;
                grid_stockX.Rows[row][3] = policyname;
                grid_stockX.Rows[row][5] = programname;
                grid_stockX.Rows[row].UserData = new GridRowTag(assembly, ObjectParameter);
                grid_stockX.SetCellImage(row, 4, global::GeneralForm.Properties.Resources.Ext_Net_Build_Ext_Net_icons_cancel);

                this.com_type.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //private void btn_stock_Click(object sender, EventArgs e)
        //{

        //    List<RunningPolicy> policies = new List<RunningPolicy>();
        //    for (int i = 0; i < this.grid_stocks.Rows.Count; i++)
        //    {
        //        string code = this.grid_stocks.Rows[i].Cells[0].Value.ToString().Trim();
        //        string name = this.grid_stocks.Rows[i].Cells[1].Value.ToString().Trim();
        //        byte market = System.Convert.ToByte(this.grid_stocks.Rows[i].Cells[2].Value);
        //        SecurityInfo si = new SecurityInfo(code, name, market);
        //        string programname = this.grid_stocks.Rows[i].Cells[5].Value.ToString();
        //        string dllname = programname.Substring(0, programname.Length - 4);
        //        Assembly assembly = ((GridRowTag)this.grid_stocks.Rows[i].Tag).assembly;
        //        Type ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
        //        Object o = ((GridRowTag)this.grid_stocks.Rows[i].Tag).parameter;
        //        PolicyProperties pp = new PolicyProperties();
        //        pp.IsLianDong = false;
        //        pp.IsSim = isSim;
        //        pp.Account = stockAccount;
        //        Object ObjectPolicy = assembly.CreateInstance(ClassPolicy.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, o, pp }, null, null);
        //        policies.Add((RunningPolicy)ObjectPolicy);
        //    }

        //    this.MainForm.AddStock(policies);
        //    this.Close();
        //}
        //private List<string[]> GetListFromTable()
        //{
        //    List<string[]> stocks = new List<string[]>();
        //    for (int i = 0; i < this.grid_stocks.Rows.Count; i++)
        //    {
        //        string[] t = new string[2];
        //        t[0] = this.grid_stocks.Rows[i].Cells[0].Value.ToString().Trim();
        //        t[1] = this.grid_stocks.Rows[i].Cells[1].Value.ToString().Trim();
        //        stocks.Add(t);
        //    }
        //    return stocks;
        //}
        private void btn_close_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要关闭吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
        private void com_type_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                if(canadd)
                AddStockCode();
            }
        }

        private void frm_Statrtstrategy_Shown(object sender, EventArgs e)
        {
            this.com_type.Focus();
            InitialPolicyCombo();
        }
        private void InitialPolicyCombo()
        {
            DataTable dt = PolicyProgram.getProgramList();
            foreach (DataRow dr in dt.Rows)
            {
                ComboBoxItem cbi = new ComboBoxItem(dr[1].ToString().Trim(), dr[0].ToString().Trim());
                this.cb_policy.Items.Add(cbi);

            }
            //this.cb_policy.Items.Add(new ComboBoxItem("预埋组合策略", "yumai"));
            //this.cb_policy.Items.Add(new ComboBoxItem("起量组合策略", "qiliang"));
            this.cb_policy.SelectedIndex = 0;
        }

        private void cb_policy_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(this.cb_policy.SelectedIndex >= 2)
            //{
                //canadd = false;
                //this.btn_sure.Enabled = false;
            //}
            //else
            //{
                canadd = true;
                //this.btn_sure.Enabled = true;
            //}
        }

        //private void grid_stocks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0)
        //    {
        //        if (e.ColumnIndex == 4)
        //        {
        //            this.grid_stocks.Rows.RemoveAt(e.RowIndex);
        //        }
        //    }
        //}
        //private void grid_stocks_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if(e.RowIndex >=0)
        //    {
        //        if(e.ColumnIndex != 4)
        //        {
        //            //((GridRowTag)this.grid_stocks.Rows[i].Tag).parameter;
        //            this.propertyGrid1.SelectedObject = ((GridRowTag)grid_stocks.Rows[e.RowIndex].Tag).parameter;
        //        }
        //    }
        //}
        private void bt_save_Click(object sender, EventArgs e)
        {
            //string section;
            //try
            //{
            //    section = ClassPolicy.FullName;
            //}
            //catch
            //{
            //    MessageBox.Show("请选择策略");
            //    return;
            //}
            //StringBuilder stb = new StringBuilder();
            //foreach (DataGridViewRow row in this.grid_stocks.Rows)
            //{
            //    stb.Append(string.Format("{0}|{1}|{2},", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value));
            //}
            //try
            //{
            //    inifile.WriteString("StockList", section, stb.ToString());
            //    MessageBox.Show("品种保存成功");
            //}
            //catch
            //{
            //    MessageBox.Show("有错误发生，请稍后再试");
            //}
        }


        public class GridRowTag
        {
            public Assembly assembly;
            public Object parameter;
            public GridRowTag(Assembly a,Object p)
            {
                this.assembly = a;
                this.parameter = p;
            }
        }

        private void cbt_ok_Click(object sender, EventArgs e)
        {
            List<RunningPolicy> policies = new List<RunningPolicy>();
            //for (int i = 0; i < this.grid_stocks.Rows.Count; i++)
            //{
            //    string code = this.grid_stocks.Rows[i].Cells[0].Value.ToString().Trim();
            //    string name = this.grid_stocks.Rows[i].Cells[1].Value.ToString().Trim();
            //    byte market = System.Convert.ToByte(this.grid_stocks.Rows[i].Cells[2].Value);
            //    SecurityInfo si = new SecurityInfo(code, name, market);
            //    string programname = this.grid_stocks.Rows[i].Cells[5].Value.ToString();
            //    string dllname = programname.Substring(0, programname.Length - 4);
            //    Assembly assembly = ((GridRowTag)this.grid_stocks.Rows[i].Tag).assembly;
            //    Type ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
            //    Object o = ((GridRowTag)this.grid_stocks.Rows[i].Tag).parameter;
            //    PolicyProperties pp = new PolicyProperties();
            //    pp.IsLianDong = false;
            //    pp.IsSim = isSim;
            //    pp.Account = stockAccount;
            //    Object ObjectPolicy = assembly.CreateInstance(ClassPolicy.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, o, pp }, null, null);
            //    policies.Add((RunningPolicy)ObjectPolicy);
            //}

            for (int i = grid_stockX.Rows.Fixed; i < this.grid_stockX.Rows.Count;i++)
            {
                string code = this.grid_stockX.Rows[i][0].ToString();
                string name = this.grid_stockX.Rows[i][1].ToString();
                string market = this.grid_stockX.Rows[i][2].ToString();
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code,market);
                string programname = this.grid_stockX.Rows[i][5].ToString();
                string dllname = programname.Substring(0, programname.Length - 4);
                Assembly assembly = ((GridRowTag)this.grid_stockX.Rows[i].UserData).assembly;
                Type ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
                object o = ((GridRowTag)this.grid_stockX.Rows[i].UserData).parameter;
                PolicyProperties pp = new PolicyProperties();
                pp.IsLianDong = false;
                pp.IsSim = isSim;
                pp.Account = stockAccount;
                Object ObjectPolicy = assembly.CreateInstance(ClassPolicy.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, o, pp }, null, null);
                policies.Add((RunningPolicy)ObjectPolicy);

            }




            AddPolicies(policies);
                //this.MainForm.AddStock(policies);
            this.Close();
        }

        private void AddPolicies(List<RunningPolicy> policies)
        {
            if (onAddPolicies != null)
            {
                onAddPolicies(this, new SelectMonitorEventArgs(policies));
            }
        }

        private void cbt_close_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("确定要关闭吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void cbt_add_Click(object sender, EventArgs e)
        {
            AddStockCode();
        }

        private void grid_stockX_MouseClick(object sender, MouseEventArgs e)
        {
            int col = grid_stockX.Col;
            if (grid_stockX.Row >= grid_stockX.Rows.Fixed)
            {
                if (col == 4)
                {
                    grid_stockX.Rows.Remove(grid_stockX.Row);
                }
                else
                {
                    this.propertyGrid1.SelectedObject = ((GridRowTag)grid_stockX.Rows[grid_stockX.Row].UserData).parameter;
                }
            }
        }

        private void bt_importfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.ShowDialog();
            StringBuilder strb = new StringBuilder();
            string filename = ofd.FileName;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filename);
            }
            catch
            {
                MessageBox.Show("对不起，无法加载参数文件");
                return;
            }
            XmlNode root;
            try
            {
                root = doc.SelectSingleNode("Parameters");
            }
            catch
            {
                MessageBox.Show("找不到Parameters");
                return;
            }
            XmlNodeList parameters;
            try
            {
                parameters = root.SelectNodes("PolicyItem");
            }
            catch
            {
                MessageBox.Show("无法找到参数信息");
                return;
            }
            foreach (XmlNode parameter in parameters)
            {
                string policyname = parameter.Attributes["policyName"].Value;
                XmlNode siNode = parameter.SelectSingleNode("SecurityInfo");
                string code = siNode.Attributes["code"].Value;
                string market = siNode.Attributes["market"].Value;
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, market);
                XmlNode pNode = siNode.SelectSingleNode("Parameter");
                Dictionary<string, string> pDic = new Dictionary<string, string>();
                
                foreach (XmlNode item in pNode.ChildNodes)
                {
                    pDic.Add(item.Name, item.InnerText);
                }
                AddStockCodeByFile(si, policyname, pDic);
            }
        }

        private void AddStockCodeByFile(SecurityInfo si, string policyname, Dictionary<string,string> pDic)
        {
            try
            {
                string programname = pDic["ProgramName"];
                string dllname = programname.Substring(0, programname.Length - 4);
                Assembly assembly = Assembly.Load(PolicyProgram.getProgram(programname));
                Type ClassParamter = assembly.GetType(string.Format("{0}.Parameter", dllname));
                Object ObjectParameter = assembly.CreateInstance(ClassParamter.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, stockAccount, "" }, null, null);
                foreach (System.Reflection.PropertyInfo p in ObjectParameter.GetType().GetProperties())
                {
                    foreach(var item in pDic)
                    {
                        if(p.Name == item.Key)
                        {
                            if (p.PropertyType == typeof(TradeSendOrderPriceType))
                            {
                                foreach (TradeSendOrderPriceType tsopt in Enum.GetValues(typeof(TradeSendOrderPriceType)))
                                {
                                    if (tsopt.ToString() == item.Value)
                                    {
                                        p.SetValue(ObjectParameter, tsopt);
                                        break;
                                    }
                                }
                            }
                            else if (p.PropertyType == typeof(SecurityInfo))
                            {
                                p.SetValue(ObjectParameter, si);
                            }
                            else
                            {
                                p.SetValue(ObjectParameter, Convert.ChangeType(item.Value, p.PropertyType));
                            }
                            break;
                        }
                    }
                }
                grid_stockX.Rows.Add();
                int row = grid_stockX.Rows.Count - 1;
                grid_stockX.Rows[row][0] = si.Code;
                grid_stockX.Rows[row][1] = si.Name;
                grid_stockX.Rows[row][2] = si.Market;
                grid_stockX.Rows[row][3] = policyname;
                grid_stockX.Rows[row][5] = programname;
                grid_stockX.Rows[row].UserData = new GridRowTag(assembly, ObjectParameter);
                grid_stockX.SetCellImage(row, 4, global::GeneralForm.Properties.Resources.Ext_Net_Build_Ext_Net_icons_cancel);

                this.com_type.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cbt_save_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML文件|*.xml";
            saveFileDialog.FileName = string.Format("{0}-参数列表.xml", System.DateTime.Now.ToString("yyyy-MM-dd"));
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "另存信息";
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                if (filename != string.Empty)
                {
                    try
                    {
                        XmlDocument _xmlDoc = new XmlDocument();
                        if (!File.Exists(filename))
                        {
                            StreamWriter sw = new StreamWriter(filename, true, Encoding.UTF8);
                            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                            sw.WriteLine("<Parameters>");
                            sw.WriteLine("</Parameters>");
                            sw.Close();
                        }
                        _xmlDoc.Load(filename);
                        XmlNode root = _xmlDoc.SelectSingleNode("Parameters");
                        root.RemoveAll();
                        for (int i = grid_stockX.Rows.Fixed; i < this.grid_stockX.Rows.Count; i++)
                        {
                            string code = this.grid_stockX.Rows[i][0].ToString();
                            string market = this.grid_stockX.Rows[i][2].ToString();
                            string programname = this.grid_stockX.Rows[i][5].ToString();
                            string policyname = programname.Substring(0, programname.Length - 4);
                            object o = ((GridRowTag)this.grid_stockX.Rows[i].UserData).parameter;

                            XmlElement ep = _xmlDoc.CreateElement("PolicyItem");
                            ep.SetAttribute("policyName", policyname);
                            XmlElement ep2 = _xmlDoc.CreateElement("SecurityInfo");
                            ep2.SetAttribute("code", code);
                            ep2.SetAttribute("market", market);
                            XmlElement ep3 = _xmlDoc.CreateElement("Parameter");
                            foreach (System.Reflection.PropertyInfo p in o.GetType().GetProperties())
                            {
                                XmlNode node = _xmlDoc.CreateElement(p.Name);
                                node.InnerText = p.GetValue(o).ToString();
                                ep3.AppendChild(node);
                            }
                            ep2.AppendChild(ep3);
                            ep.AppendChild(ep2);
                            root.AppendChild(ep);
                        }
                        _xmlDoc.Save(filename);
                    }
                    catch
                    {
                        MessageBox.Show("保存文件错误");
                    }
                }
            }
        }
    }

}
