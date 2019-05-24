using C1.Win.C1Ribbon;
using DataBase;
using StockData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace PolicyClient
{
    public partial class frm_backtest : C1RibbonForm
    {
        SecurityInfo si;
        Assembly assembly;
        Type ClassParamter;

        //Type ClassPolicy;
        public frm_backtest()
        {
            InitializeComponent();
        }

        private void frm_backtest_Load(object sender, EventArgs e)
        {
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
        }

        private void startTest()
        {
            frm_BackTestResult frm = new frm_BackTestResult(si, assembly, this.propertyGrid1.SelectedObject);
            frm.Text = string.Format("{0}-{1}-{2}", si.Code, si.Name, this.cb_policy.SelectedItem.ToString());
            //frm.MdiParent = this.MdiParent;
            frm.Show();
            this.Close();
        }

        void uc_select_Leave(object sender, EventArgs e)
        {
            this.uc_StockSelect1.Visible = false;
        }

        void uc_select_Stock_Selected(object sender, SecurityInfo si)
        {
            this.si = si;
            this.Controls.Remove((uc_StockSelect)sender);
            startTest();
        }

        private void cb_policy_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string programname = ((ComboBoxItem)this.cb_policy.SelectedItem).Value;
                string dllname = programname.Substring(0, programname.Length - 4);
                //加载程序集(dll文件地址)，使用Assembly类   
                assembly = Assembly.Load(PolicyProgram.getProgram(programname));
                //assembly = Assembly.LoadFile(string.Format(@"{0}\RunningPolicy1.dll", Application.StartupPath));
                //获取类型，参数（名称空间+类）   
                ClassParamter = assembly.GetType(string.Format("{0}.BackTestParameter", dllname));
                //ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
                //创建该对象的实例，object类型，参数（名称空间+类）   
                Object ObjectParameter = assembly.CreateInstance(ClassParamter.FullName);
                //可视属性窗体
                this.propertyGrid1.SelectedObject = ObjectParameter;
            }
            catch (Exception ex)
            {
                //弹出错误信息
                MessageBox.Show(ex.Message);
            }
        }

        private void bt_add_Click(object sender, EventArgs e)
        {
            string policyname;
            try
            {
                policyname = this.cb_policy.SelectedItem.ToString();
            }
            catch
            {
                policyname = string.Empty;
            }
            if (policyname == string.Empty)
            {
                MessageBox.Show("请选择策略名称", "错误");
                return;
            }
            string code = string.Empty;
            code = this.comboBox2.Text.Trim();

            if (code == string.Empty)
            {
                MessageBox.Show(this, "请输入股票代码", "错误");
            }
            List<SecurityInfo> lists = GlobalValue.GetFutureByCode(code);
            if (lists.Count == 0)
            {
                MessageBox.Show("找不到股票代码");
                return;
            }

            if (lists.Count == 1)
            {
                si = lists[0];
            }
            else
            {
                uc_StockSelect uc_select = new uc_StockSelect();
                uc_select.Top = this.comboBox2.Top + this.comboBox2.Height;
                uc_select.Left = this.comboBox2.Left;
                this.Controls.Add(uc_select);
                uc_select.BringToFront();
                uc_select.DataSource = lists;
                uc_select.DataBind();
                uc_select.Show();
                uc_select.Focus();
                uc_select.Stock_Selected += uc_select_Stock_Selected;
                uc_select.Leave += uc_select_Leave;
            }
            GridRowTag grt = new GridRowTag(assembly, this.propertyGrid1.SelectedObject, si);
            int index = this.grid_stocks.Rows.Add();
            this.grid_stocks.Rows[index].Cells[0].Value = si.Code;
            this.grid_stocks.Rows[index].Cells[1].Value = si.Name;
            this.grid_stocks.Rows[index].Cells[2].Value = si.Market;
            this.grid_stocks.Rows[index].Cells[3].Value = policyname;
            this.grid_stocks.Rows[index].Cells[4].Value = "删除";
            this.grid_stocks.Rows[index].Tag = grt;
            cb_policy_SelectedIndexChanged(this, new EventArgs());
        }

        private void grid_stocks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 3)
                {
                    frm_PolicyParameter frm = new frm_PolicyParameter(((GridRowTag)grid_stocks.Rows[e.RowIndex].Tag).parameter);
                    frm.ShowDialog();
                    //this.propertyGrid1.SelectedObject = ((GridRowTag)grid_stocks.Rows[e.RowIndex].Tag).parameter;

                }
                else if (e.ColumnIndex == 4)
                {

                    this.grid_stocks.Rows.RemoveAt(e.RowIndex);

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        int totalBackTestCount;
        int currentBackTestCount;
        private void bt_startbacktest_Click(object sender, EventArgs e)
        {
            this.bt_add.Enabled = false;
            totalBackTestCount = this.grid_stocks.Rows.Count;
            currentBackTestCount = 0;
            ContinueBackTest();
        }

        delegate void ContinueBackTestDelegate();
        private void ContinueBackTest()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ContinueBackTestDelegate(ContinueBackTest));
            }
            else
            {

                GridRowTag grt = (GridRowTag)grid_stocks.Rows[currentBackTestCount].Tag;
                string name = this.grid_stocks.Rows[currentBackTestCount].Cells[3].Value.ToString();
                frm_BackTestResult frm = new frm_BackTestResult(grt.si, grt.assembly, grt.parameter);
                frm.Test_Finished += frm_Test_Finished;
                frm.Text = string.Format("{0}-{1}-{2}", grt.si.Code, grt.si.Name, name);
                frm.Show();
            }
        }

        delegate void StartBackTestDelegate(int count);
        private void StartBackTest(int count)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new StartBackTestDelegate(StartBackTest), new object[] { count });
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < totalBackTestCount)
                    {
                        GridRowTag grt = (GridRowTag)grid_stocks.Rows[i].Tag;
                        string name = this.grid_stocks.Rows[i].Cells[3].Value.ToString();
                        frm_BackTestResult frm = new frm_BackTestResult(grt.si, grt.assembly, grt.parameter);
                        frm.Test_Finished += frm_Test_Finished;
                        frm.Text = string.Format("{0}-{1}-{2}", grt.si.Code, grt.si.Name, name);
                        frm.Show();
                    }
                }
            }
        }

        void frm_Test_Finished(object sender, EventArgs e)
        {
            currentBackTestCount++;
            if (currentBackTestCount < totalBackTestCount)
            {
                ContinueBackTest();
            }
            else
            {
                this.bt_add.Enabled = true;
            }
        }
    }
    public class ComboBoxItem
    {
        public string Text;
        public string Value;
        public ComboBoxItem(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
        public override string ToString()
        {
            return this.Text;
        }
    }

    public class GridRowTag
    {
        public Assembly assembly;
        public Object parameter;
        public SecurityInfo si;
        public GridRowTag(Assembly a, Object p, SecurityInfo si)
        {
            this.assembly = a;
            this.parameter = p;
            this.si = si;
        }
    }
}
