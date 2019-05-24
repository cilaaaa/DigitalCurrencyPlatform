using C1.Win.C1Ribbon;
using DataBase;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace PolicyClient
{
    public partial class frm_SelectMonitor : C1RibbonForm
    {
        IniFile inifile;
        public frm_Monitor MainForm;
        public frm_SelectMonitor()
        {
            InitializeComponent();
            inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
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
                List<SecurityInfo> lists = GlobalValue.GetFutureByCode(code);
                if (lists.Count == 0)
                {
                    MessageBox.Show("找不到股票代码");
                    return;
                }
                if (lists.Count == 1)
                {
                    AddStockCode(lists[0]);
                }
                else
                {
                    uc_StockSelect uc_select = new uc_StockSelect();
                    uc_select.Top = this.com_type.Top + this.com_type.Height;
                    uc_select.Left = this.com_type.Left;
                    this.Controls.Add(uc_select);
                    uc_select.BringToFront();
                    uc_select.DataSource = lists;
                    uc_select.DataBind();
                    uc_select.Show();
                    uc_select.Focus();
                    uc_select.Stock_Selected += uc_select_Stock_Selected;
                    uc_select.Leave += uc_select_Leave;
                }
            }
        }

        void uc_select_Leave(object sender, EventArgs e)
        {
            this.Controls.Remove((uc_StockSelect)sender);
        }

        void uc_select_Stock_Selected(object sender, SecurityInfo si)
        {
            AddStockCode(si);
            this.Controls.Remove((uc_StockSelect)sender);
            this.com_type.Focus();
        }

        private void AddStockCode(SecurityInfo si, string policyname, string programName)
        {
            DateTime fromdate = this.dateTimePicker1.Value.Date;
            DateTime todate = this.dateTimePicker2.Value.Date;
            PolicyParameter pp = new PolicyParameter();
            pp.EndDate = todate;
            pp.StartDate = fromdate;
            pp.Inteval = 0;
            pp.IsReal = false;
            try
            {
                string dllname = programName.Substring(0, programName.Length - 4);
                Assembly assembly = Assembly.Load(PolicyProgram.getProgram(programName));
                Type ClassParamter = assembly.GetType(string.Format("{0}.Parameter", dllname));
                Object ObjectParameter = assembly.CreateInstance(ClassParamter.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, pp, string.Empty }, null, null);
                int i = grid_stocks.Rows.Add();
                grid_stocks.Rows[i].Cells[0].Value = si.Code;
                grid_stocks.Rows[i].Cells[1].Value = si.Name;
                grid_stocks.Rows[i].Cells[2].Value = si.Market;
                grid_stocks.Rows[i].Cells[3].Value = policyname;
                grid_stocks.Rows[i].Cells[4].Value = "删除";
                grid_stocks.Rows[i].Cells[5].Value = programName;
                grid_stocks.Rows[i].Tag = new GridRowTag(assembly, ObjectParameter);
                this.com_type.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddStockCode(SecurityInfo si)
        {
            ComboBoxItem cbi = (ComboBoxItem)this.cb_policy.SelectedItem;

            string dllname = cbi.Value;
            string policyname = cbi.Text;
            //string programname = ((ComboBoxItem)this.cb_policy.SelectedItem).Value;
            AddStockCode(si, policyname, dllname);
        }
        private void btn_stock_Click(object sender, EventArgs e)
        {
            inifile.WriteString("daterange", "fromdate", this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd"));
            inifile.WriteString("daterange", "todate", this.dateTimePicker2.Value.Date.ToString("yyyy-MM-dd"));
            List<RunningPolicy> policies = new List<RunningPolicy>();
            for (int i = 0; i < this.grid_stocks.Rows.Count; i++)
            {
                string code = this.grid_stocks.Rows[i].Cells[0].Value.ToString().Trim();
                string name = this.grid_stocks.Rows[i].Cells[1].Value.ToString().Trim();
                string market = this.grid_stocks.Rows[i].Cells[2].Value.ToString();
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, market);
                string programname = this.grid_stocks.Rows[i].Cells[5].Value.ToString();
                string dllname = programname.Substring(0, programname.Length - 4);
                Assembly assembly = ((GridRowTag)this.grid_stocks.Rows[i].Tag).assembly;
                Type ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
                Object o = ((GridRowTag)this.grid_stocks.Rows[i].Tag).parameter;
                PolicyProperties pp = new PolicyProperties();
                pp.Account = string.Empty;
                pp.IsLianDong = false;
                pp.IsSim = true;
                Object ObjectPolicy = assembly.CreateInstance(ClassPolicy.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, o, pp }, null, null);
                policies.Add((RunningPolicy)ObjectPolicy);
            }

            this.MainForm.AddStock(policies);
            this.Close();
        }
        private List<string[]> GetListFromTable()
        {
            List<string[]> stocks = new List<string[]>();
            for (int i = 0; i < this.grid_stocks.Rows.Count; i++)
            {
                string[] t = new string[2];
                t[0] = this.grid_stocks.Rows[i].Cells[0].Value.ToString().Trim();
                t[1] = this.grid_stocks.Rows[i].Cells[1].Value.ToString().Trim();
                stocks.Add(t);
            }
            return stocks;
        }
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
            if (dt.Rows.Count > 0)
            {
                this.cb_policy.SelectedIndex = 0;
            }

        }

        private void cb_policy_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void grid_stocks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 4)
                {
                    this.grid_stocks.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
        private void grid_stocks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex != 4)
                {
                    //((GridRowTag)this.grid_stocks.Rows[i].Tag).parameter;
                    this.propertyGrid1.SelectedObject = ((GridRowTag)grid_stocks.Rows[e.RowIndex].Tag).parameter;
                }
            }
        }
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

        private void bt_load_Click(object sender, EventArgs e)
        {
            string section;
            try
            {
                section = ((ComboBoxItem)this.cb_policy.SelectedItem).Value;
            }
            catch
            {
                MessageBox.Show("请选择策略");
                return;
            }
            DataTable list = null;
            //DataTable list = LocalSQL.QueryDataTable(string.Format("select policysi_code,policysi_name,policysi_market from policysi_mstr where policysi_dll = '{0}' order by policysi_code ", section));
            //string list = inifile.GetString("StockList", section, string.Empty);
            for (int x = 0; x < list.Rows.Count; x++)
            //if (list != string.Empty)
            {

                string code = list.Rows[x]["policysi_code"].ToString();
                string name = list.Rows[x]["policysi_name"].ToString();
                string market = list.Rows[x]["policysi_market"].ToString();
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, market);
                AddStockCode(si);
                //bool hasDup = false;
                //for (int j = 0; j < grid_stocks.Rows.Count; j++)
                //{
                //    if (grid_stocks.Rows[j].Cells[0].Value.ToString() == list.Rows[x]["policysi_code"].ToString() && grid_stocks.Rows[j].Cells[2].Value.ToString() == list.Rows[x]["policysi_market"].ToString())
                //    {
                //        //弹出提示框
                //        //MessageBox.Show("有相同的数据，请重新选择");
                //        hasDup = true;
                //        break;
                //        //如果有相同的数据，就返回
                //        //return;
                //    }
                //}
                //if (!hasDup)
                //{
                //    //在查找所有的数据都没有与之匹配的数据，就添加一条数据
                //    //cbi = (ComboBoxItem)com_type.SelectedItem;
                //    int i = grid_stocks.Rows.Add();
                //    //添加code
                //    grid_stocks.Rows[i].Cells[0].Value = list.Rows[x]["policysi_code"];
                //    //添加名称
                //    grid_stocks.Rows[i].Cells[1].Value = list.Rows[x]["policysi_name"];
                //    //添加市场
                //    grid_stocks.Rows[i].Cells[2].Value = list.Rows[x]["policysi_market"];
                //    //添加完之后清空com_type中的股票代码
                //    grid_stocks.Rows[i].Cells[3].Value = "删除";
                //    this.com_type.Text = string.Empty;
                //}
            }

        }

        public class GridRowTag
        {
            public Assembly assembly;
            public Object parameter;
            public GridRowTag(Assembly a, Object p)
            {
                this.assembly = a;
                this.parameter = p;
            }
        }

        private void frm_SelectMonitor_Load(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = System.Convert.ToDateTime(inifile.GetString("daterange", "fromdate", "2015-01-01")).Date;
            this.dateTimePicker2.Value = System.Convert.ToDateTime(inifile.GetString("daterange", "todate", System.DateTime.Now.ToString("yyyy-MM-dd")));
        }

        private void bt_import_Click(object sender, EventArgs e)
        {
            string filename = string.Format("{0}\\parameter.xml", Application.StartupPath);
            {
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
                    root = doc.SelectSingleNode("PolicyPool");
                }
                catch
                {
                    MessageBox.Show("找不到PolicyPool");
                    return;
                }
                XmlNodeList accounts;
                try
                {
                    accounts = root.SelectNodes("Account");
                }
                catch
                {
                    MessageBox.Show("无法找到账户信息");
                    return;
                }
                foreach (XmlNode account in accounts)
                {
                    string acctname;
                    try
                    {
                        acctname = account.Attributes["name"].Value;
                        //MessageBox.Show(acctname);
                    }
                    catch
                    {
                        MessageBox.Show("无法找到账户名称");
                        return;
                    }
                    if (acctname == string.Empty)
                    {
                        XmlNodeList policyitems;
                        try
                        {
                            policyitems = account.SelectNodes("PolicyItem");
                        }
                        catch
                        {
                            MessageBox.Show("找不到账号对应的策略信息");
                            return;
                        }
                        foreach (XmlNode policyitem in policyitems)
                        {
                            string programName;
                            try
                            {
                                programName = policyitem.Attributes["programName"].Value;
                            }
                            catch
                            {
                                MessageBox.Show("找不到策略名称");
                                return;
                            }
                            XmlNodeList sinodes;

                            try
                            {
                                sinodes = policyitem.SelectNodes("SecurityInfo");
                            }
                            catch
                            {
                                MessageBox.Show("找不到股票信息");
                                return;
                            }
                            foreach (XmlNode sinode in sinodes)
                            {
                                string code = sinode.Attributes["code"].Value;
                                byte market = System.Convert.ToByte(sinode.Attributes["market"].Value);
                                string siname = sinode.Attributes["name"].Value;
                                string policyname = PolicyProgram.getPolicyName(programName);
                                if (policyname == string.Empty)
                                {
                                    MessageBox.Show(string.Format("找不到{0}对应的策略", programName));
                                    return;
                                }
                                //this.AddStockCode(GlobalValue.GetFutureByCode(code), policyname, programName);
                            }
                        }
                    }
                }
            }
        }


    }

}
