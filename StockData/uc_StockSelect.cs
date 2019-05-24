using System.Collections.Generic;
using System.Windows.Forms;

namespace StockData
{
    public partial class uc_StockSelect : UserControl
    {
        public uc_StockSelect()
        {
            InitializeComponent();
        }
        List<SecurityInfo> stockList;
        public List<SecurityInfo> DataSource
        {
            get
            {
                return stockList;
            }
            set
            {
                this.stockList = value;
            }
        }
        public void DataBind()
        {
            this.grid_list.Rows.Clear();
            foreach (SecurityInfo si in stockList)
            {
                int i = this.grid_list.Rows.Add();
                grid_list.Rows[i].Cells[0].Value = si.Code;
                grid_list.Rows[i].Cells[1].Value = si.Name;
                grid_list.Rows[i].Cells[2].Value = si.Market;
            }
        }
        public delegate void StockSelected(object sender, SecurityInfo si);
        public event StockSelected Stock_Selected;
        private void grid_list_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                DataGridViewRow dr = this.grid_list.SelectedRows[0];

                string code = dr.Cells[0].Value.ToString();
                string name = dr.Cells[1].Value.ToString();
                string market = dr.Cells[2].Value.ToString();
                double minqty = System.Convert.ToDouble(dr.Cells[3].Value);
                int jingdu = System.Convert.ToInt16(dr.Cells[4].Value);
                int pricejingdu = System.Convert.ToInt16(dr.Cells[5].Value);
                string type = dr.Cells[6].Value.ToString();
                string contractVal = dr.Cells[7].Value.ToString();
                SecurityInfo si = new SecurityInfo(code, name, market, minqty, jingdu, pricejingdu, type, contractVal);
                if (Stock_Selected != null)
                {
                    Stock_Selected(this, si);
                }
                e.Handled = true;
            }
        }

        private void grid_list_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow dr = this.grid_list.Rows[e.RowIndex];
                string code = dr.Cells[0].Value.ToString();
                string name = dr.Cells[1].Value.ToString();
                string market = dr.Cells[2].Value.ToString();
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, market);
                if (Stock_Selected != null)
                {
                    Stock_Selected(this, si);
                }
            }
        }


    }
}
