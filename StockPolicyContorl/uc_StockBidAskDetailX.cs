using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StockData;
using System.Threading;
using System.Diagnostics;

namespace StockPolicyContorl
{
    public partial class uc_StockBidAskDetailX : UserControl
    {
        string[] chineseCount = new string[10] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        DataGridViewCellStyle EqualStyle;
        DataGridViewCellStyle AboveStyle;
        DataGridViewCellStyle LowStyle;
        public uc_StockBidAskDetailX()
        {
            InitializeComponent();
            //this.tlp_ask.CellPaint += tlp_ask_CellPaint;
            initializeDataGridView();
            GUITools.DoubleBuffer(this.grid_ask, true);
            GUITools.DoubleBuffer(this.grid_bid, true);
            EqualStyle = CreateStyle(Color.White, Color.Black);
            AboveStyle = CreateStyle(Color.White, Color.Red);
            LowStyle = CreateStyle(Color.White, Color.Green);
        }

        private DataGridViewCellStyle CreateStyle(Color color1, Color color2)
        {
            DataGridViewCellStyle dvcs = new DataGridViewCellStyle();
            dvcs.BackColor = color1;
            dvcs.ForeColor = color2;
            dvcs.SelectionBackColor = color1;
            dvcs.SelectionForeColor = color2;
            dvcs.Alignment = DataGridViewContentAlignment.BottomRight;
            
            return dvcs;
        }

        private void initializeDataGridView()
        {
            
            for(int i=0;i<10;i++)
            {
                this.grid_ask.Rows.Insert(0, 1);
                this.grid_ask.Rows[0].Cells[0].Value = string.Format("卖{0}", chineseCount[i]);
                int index = this.grid_bid.Rows.Add();
                this.grid_bid.Rows[index].Cells[0].Value = string.Format("买{0}",chineseCount[i]);
                
            }
        }
        public void setStockName(string code,string name)
        {
            this.lb_stockcode.Text = code;
            this.lb_stockname.Text = name;
        }

        public void Clear()
        {
            this.lb_stockcode.Text = string.Empty;
            this.lb_stockname.Text = string.Empty;
            
        }

        public void Update(StockData.TickData td)
        {
            //ParameterizedThreadStart pts = new ParameterizedThreadStart(UpdateLabel);
            //Thread th = new Thread(pts);
            //th.Start(td);
            UpdateLabel(td);
        }
        delegate void UpdateDelegate(object td);
        public void UpdateLabel(object otd)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateDelegate(UpdateLabel),new object[]{otd});
            }
            else
            {

                this.SuspendLayout();
                TickData td = (TickData)otd;
                for(int i=0;i<10;i++)
                {
                    if(td.Asks[i] > td.Preclose)
                    {
                        grid_ask.Rows[9 - i].Cells[1].Style = AboveStyle;
                        //grid_ask.Rows[9 - i].Cells[2].Style = AboveStyle;
                    }else if(td.Asks[i] < td.Preclose)
                    {
                        grid_ask.Rows[9 - i].Cells[1].Style = LowStyle;
                        //grid_ask.Rows[9 - i].Cells[2].Style = LowStyle;
                    }
                    else
                    {
                        grid_ask.Rows[9 - i].Cells[1].Style = EqualStyle;
                        //grid_ask.Rows[9 - i].Cells[2].Style = EqualStyle;
                    }
                    if(td.Bids[i] > td.Preclose)
                    {
                        grid_bid.Rows[i].Cells[1].Style = AboveStyle;
                        //grid_bid.Rows[i].Cells[2].Style = AboveStyle;
                
                    }else if(td.Bids[i] < td.Preclose)
                    {
                        grid_bid.Rows[i].Cells[1].Style = LowStyle;
                        //grid_bid.Rows[i].Cells[2].Style = LowStyle;
                    }
                    else
                    {
                        grid_bid.Rows[i].Cells[1].Style = EqualStyle;
                        //grid_bid.Rows[i].Cells[2].Style = EqualStyle;

                    }
                    grid_ask.Rows[9-i].Cells[1].Value = td.Asks[i].ToString("0.00");
                    grid_ask.Rows[9-i].Cells[2].Value = td.Asksizes[i];
                    grid_bid.Rows[i].Cells[1].Value = td.Bids[i].ToString("0.00");
                    grid_bid.Rows[i].Cells[2].Value = td.Bidsizes[i];
                }
                this.lb_time.Text = string.Format("价格:{0},时间{1},量{2}", td.Last.ToString("0.00"), td.Time.TimeOfDay.ToString(),td.Volume);
                this.ResumeLayout();
            }
            
        }

        void tlp_ask_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            Debug.Print(string.Format("{0},{1}", e.Row, e.Column));
        }

        private void grid_SizeChanged(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            dgv.Columns[0].Width = System.Convert.ToInt32(dgv.Width * 0.2);
            dgv.Columns[1].Width = System.Convert.ToInt32(dgv.Width * 0.3);
            dgv.Columns[2].Width = System.Convert.ToInt32(dgv.Width * 0.4);
            dgv.Columns[3].Width = System.Convert.ToInt32(dgv.Width * 0.1);
            foreach(DataGridViewRow  dr in dgv.Rows)
            {
                dr.Height = dgv.Height / dgv.Rows.Count;
            }
        }
    }
}
