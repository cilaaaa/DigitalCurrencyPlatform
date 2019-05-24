using C1.Win.C1Ribbon;
using DataBase;
using GeneralForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EntropyRiseStockTP0
{
    public partial class frm_Account : C1RibbonForm
    {
        public frm_Account()
        {
            InitializeComponent();
            this.c1FlexGrid1.MouseClick += c1FlexGrid1_MouseClick;
        }



        private void frm_Account_Load(object sender, EventArgs e)
        {

            RefreshTraderCombo();
            refreshList();
            resetControl();
        }

        private void refreshList()
        {
            try
            {
                this.c1FlexGrid1.Rows.RemoveRange(1, this.c1FlexGrid1.Rows.Count - 1);
            }
            catch { }
            if (File.Exists(ConfigFileName.AccountFileName))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigFileName.AccountFileName);
                    XmlNode root = doc.SelectSingleNode("Accounts");
                    if (!object.Equals(null, root))
                    {
                        XmlNodeList accounts = root.SelectNodes("Account");
                        foreach (XmlNode account in accounts)
                        {
                            string accountId = account.SelectSingleNode("accountId").InnerText;
                            string apikey = account.SelectSingleNode("api_key").InnerText;
                            string secretkey = account.SelectSingleNode("secret_key").InnerText;
                            string environment = account.SelectSingleNode("environment").InnerText;
                            bool use = System.Convert.ToBoolean(account.SelectSingleNode("use").InnerText);
                            this.c1FlexGrid1.Rows.Add();
                            int index = this.c1FlexGrid1.Rows.Count - 1;
                            this.c1FlexGrid1.Rows[index][1] = accountId;
                            this.c1FlexGrid1.Rows[index][2] = environment;
                            this.c1FlexGrid1.Rows[index][3] = apikey;
                            this.c1FlexGrid1.Rows[index][4] = secretkey;
                            this.c1FlexGrid1.Rows[index][5] = use;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("配置文件读取失败");
                }
            }
        }

        private void RefreshTraderCombo()
        {
            ComboBoxItem cbi = new ComboBoxItem("模拟", "test");
            this.environment.Items.Add(cbi);
            ComboBoxItem cbi2 = new ComboBoxItem("实盘", "real");
            this.environment.Items.Add(cbi2);
        }

        private void resetControl()
        {
            this.account.Enabled = true;
            this.account.Text = string.Empty;
            this.account.Enabled = false;
            this.api_key.Text = string.Empty;
            this.secret_key.Text = string.Empty;
            this.environment.Text = string.Empty;
            this.cck_r.Checked = false;

        }

        private void c1FlexGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                string accountid = this.c1FlexGrid1.Rows[c1FlexGrid1.Row][1].ToString();
                string environment = this.c1FlexGrid1.Rows[c1FlexGrid1.Row][2].ToString();
                string apikey = this.c1FlexGrid1.Rows[c1FlexGrid1.Row][3].ToString();
                string secretkey = this.c1FlexGrid1.Rows[c1FlexGrid1.Row][4].ToString();
                bool use = System.Convert.ToBoolean(this.c1FlexGrid1.Rows[c1FlexGrid1.Row][5].ToString());
                this.account.Enabled = true;
                this.account.Text = accountid;
                this.account.Enabled = false;
                this.api_key.Text = apikey;
                this.secret_key.Text = secretkey;
                this.cck_r.Checked = use;
                SetEnvironmentComboValue(environment);
            }
        }

        private void SetEnvironmentComboValue(string environmentname)
        {
            for (int i = 0; i < environment.Items.Count; i++)
            {
                if (((ComboBoxItem)environment.Items[i]).Value == environmentname)
                {
                    this.environment.SelectedIndex = i;
                    break;
                }
            }
        }

        private void cb_trader_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(environment.SelectedItem.ToString());
        }

        private void cbt_delete_Click(object sender, EventArgs e)
        {
            if (!this.account.Enabled)
            {
                string account = this.account.Text;
                if (account.Trim() != string.Empty)
                {
                    if (MessageBox.Show(string.Format("确认要删除账号:{0}", account), "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK)
                    {
                        //LocalSQL.ExecuteSql(string.Format("delete from account_mstr where account_id = '{0}'", account));
                        refreshList();
                        resetControl();
                    }
                }
                else
                {
                    MessageBox.Show("请选择要删除的账号");
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的账号");
            }
        }

        private void ccb_new_Click(object sender, EventArgs e)
        {
            resetControl();
            this.account.Enabled = true;
        }

        private void cbt_save_Click(object sender, EventArgs e)
        {
            //string account = this.account.Text;
            //if (account.Trim() == string.Empty)
            //{
            //    MessageBox.Show("请选择要修改的账号或者输入新账号");
            //    return;
            //}
            //string pwd = this.ctx_pwd.Text.Trim();
            //string conpwd = this.ctx_conpwd.Text.Trim();
            //string yybid = this.ctx_yybid.Text.Trim();
            //try
            //{
            //    int.Parse(yybid);

            //}
            //catch
            //{
            //    MessageBox.Show("对不起，营业部编号必须为数字");
            //    return;
            //}
            //string trader = string.Empty;
            //try
            //{
            //    trader = ((ComboBoxItem)this.cb_trader.SelectedItem).Value;
            //}
            //catch { }
            //if (trader == string.Empty)
            //{
            //    MessageBox.Show("对不起，请选择券商");
            //    return;
            //}
            //string zjaccount = this.ctx_zjaccount.Text.Trim();
            //bool r = this.cck_r.Checked;
            //if (this.account.Enabled)
            //{
            //    string strSql = string.Format("insert into account_mstr (account_id,account_trader,account_pwd,account_conpwd,account_yybid,account_enabled,account_r,account_zjaccount) values ('{0}','{1}','{2}','{3}',{4},{5},{6},'{7}')", account, trader, pwd, conpwd, yybid, System.Convert.ToInt32(enable), System.Convert.ToInt32(r), zjaccount);
            //    try
            //    {
            //        LocalSQL.ExecuteSql(strSql);
            //        resetControl();
            //        refreshList();
            //        MessageBox.Show("账号添加成功");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(string.Format("账号添加失败:{0}", ex.Message));
            //    }
            //}
            //else
            //{
            //    string strSql = String.Format("update account_mstr set account_trader = '{1}',account_pwd = '{2}',account_conpwd = '{3}',account_yybid = {4},account_enabled = {5},account_r = {6},account_zjaccount = '{7}' where account_id = '{0}'", account, trader, pwd, conpwd, yybid, System.Convert.ToInt32(enable), System.Convert.ToInt32(r), zjaccount);
            //    try
            //    {
            //        LocalSQL.ExecuteSql(strSql);
            //        resetControl();
            //        refreshList();
            //        MessageBox.Show("账号修改成功");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(string.Format("账号修改失败:{0}", ex.Message));
            //    }
            //}
        }
    }
}
