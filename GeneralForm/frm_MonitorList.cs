using C1.Win.C1Ribbon;
using DataBase;
using StockData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace GeneralForm
{
    public delegate void AddStock(object sender,MonitorStockEventArgs args);
    public delegate void RemoveStock(object sender,MonitorStockEventArgs args);



    public partial class frm_MonitorList : C1RibbonForm
    {
        public static bool isRunning = false;
        public static bool isMdiStyle = true;
        //public frm_MainX mutilForm;
        //public frm_MainNew newForm;

        public event AddStock onAddStock;
        public event RemoveStock onRemoveStock;
        public frm_MonitorList()
        {
            InitializeComponent();
            GUITools.DoubleBuffer(this.grid_stocklist, true);
        }

        public void RaiseAddStock(SecurityInfo si)
        {
            if(onAddStock!=null)
            {
                onAddStock(this, new MonitorStockEventArgs(si));
            }
        }

        public void RaiseRemoveStock(SecurityInfo si)
        {
            if(onRemoveStock != null)
            {
                onRemoveStock(this,new MonitorStockEventArgs(si));
            }
        }
        private void frm_MonitorList_Load(object sender, EventArgs e)
        {
            getMonitorList();
        }
        private void getMonitorList()
        {
            try
            {
                this.grid_stocklist.Rows.Clear();

            }
            catch { }
            List<SecurityInfo> sis = GlobalValue.SecurityList;
            for (int i = 0; i < sis.Count; i++)
            {
                int index = this.grid_stocklist.Rows.Add();
                this.grid_stocklist.Rows[index].Cells[0].Value = string.Format("{0}", index + 1);
                this.grid_stocklist.Rows[index].Cells[1].Value = sis[i].Code.ToString();
                this.grid_stocklist.Rows[index].Cells[2].Value = sis[i].Name.ToString();
                this.grid_stocklist.Rows[index].Cells[3].Value = sis[i].Market.ToString();
                this.grid_stocklist.Rows[index].Cells[4].Value = "删除";
            }
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            AddStock();
        }

        private void AddStock()
        {
            string code = this.tx_code.Text.Trim();
            if (code == string.Empty)
            {
                MessageBox.Show("请输入股票代码");
                return;
            }
            List<SecurityInfo> lists = GlobalValue.GetFutureByCode(code);
            if (lists.Count == 0)
            {
                MessageBox.Show("找不到股票代码");
                return;
            }

            //SecurityInfo si = new SecurityInfo();
            if (lists.Count == 1)
            {
                //si = lists[0];
                AddStock(lists[0]);
            }
            else
            {
                uc_StockSelect uc_select = new uc_StockSelect();
                uc_select.Top = this.tx_code.Top + this.panel1.Height;
                uc_select.Left = this.tx_code.Left;
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

        void uc_select_Leave(object sender, EventArgs e)
        {
            this.Controls.Remove((uc_StockSelect)sender);
        }

        void uc_select_Stock_Selected(object sender, SecurityInfo si)
        {
            this.AddStock(si);
            this.Controls.Remove((uc_StockSelect)sender);
            this.tx_code.Focus();
        }

        private void AddStock(SecurityInfo si)
        {
            GlobalValue.SecurityList.Add(si);
            SaveXml();
            int index = this.grid_stocklist.Rows.Add();
            grid_stocklist.Rows[index].Cells[0].Value = index + 1;
            grid_stocklist.Rows[index].Cells[1].Value = si.Code;
            grid_stocklist.Rows[index].Cells[2].Value = si.Name;
            grid_stocklist.Rows[index].Cells[3].Value = si.Market;
            this.grid_stocklist.Rows[index].Cells[4].Value = "删除";
            RaiseAddStock(si);
            this.tx_code.Text = string.Empty;
        }

        private void tx_code_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                AddStock();
            }
        }

        private void grid_stocklist_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 4)
            {
                string code = grid_stocklist.Rows[e.RowIndex].Cells[1].Value.ToString();
                string market = grid_stocklist.Rows[e.RowIndex].Cells[3].Value.ToString();
                if (MessageBox.Show(string.Format("是否要停止接收{0}", code), "确认", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    for (int i = 0; i < GlobalValue.SecurityList.Count; i++)
                    {
                        if (GlobalValue.SecurityList[i].Code == code && GlobalValue.SecurityList[i].Market == market)
                        {
                            GlobalValue.SecurityList.RemoveAt(i);
                            getMonitorList();
                            SaveXml();
                            RaiseRemoveStock(GlobalValue.GetFutureByCodeAndMarket(code,market));
                        }
                    }
                            

                }
            }
        }

        private void SaveXml(){
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigFileName.ReceiveCodeFileName);
            XmlNode root = doc.SelectSingleNode("ReceiveCode");
            XmlNodeList items = root.SelectNodes("Item");
            if (!object.Equals(null, items))
            {
                root.RemoveAll();
            }
            for (int i = 0; i < GlobalValue.SecurityList.Count; i++)
            {
                XmlElement itemnode = doc.CreateElement("Item");
                itemnode.SetAttribute("code", GlobalValue.SecurityList[i].Code.ToString());
                itemnode.SetAttribute("name", GlobalValue.SecurityList[i].Name.ToString());
                itemnode.SetAttribute("market", GlobalValue.SecurityList[i].Market.ToString());
                itemnode.SetAttribute("minqty", GlobalValue.SecurityList[i].MinQty.ToString());
                itemnode.SetAttribute("jingdu", GlobalValue.SecurityList[i].JingDu.ToString());
                itemnode.SetAttribute("pricejingdu", GlobalValue.SecurityList[i].PriceJingDu.ToString());
                itemnode.SetAttribute("type", GlobalValue.SecurityList[i].Type.ToString());
                itemnode.SetAttribute("contractval", GlobalValue.SecurityList[i].ContractVal.ToString());
                XmlElement codenode = doc.CreateElement("Code");

                root.AppendChild(itemnode);
            }
            doc.Save(ConfigFileName.ReceiveCodeFileName);
        }

        private void frm_MonitorList_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRunning = false;
        }
    }
}
