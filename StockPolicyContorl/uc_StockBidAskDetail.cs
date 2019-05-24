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

namespace StockPolicyContorl
{
    public partial class uc_StockBidAskDetail : UserControl
    {
        Label[] lb_AskPrice = new Label[10];
        Label[] lb_BidPrice = new Label[10];
        Label[] lb_AskSize = new Label[10];
        Label[] lb_BidSize = new Label[10];
        public uc_StockBidAskDetail()
        {
            InitializeComponent();
            for(int i=0;i<lb_AskPrice.Length;i++)
            {
                lb_AskPrice[i] = (Label)this.tlp_ask.Controls.Find(string.Format("lb_ask{0}", i + 1),true)[0];
                lb_AskSize[i] = (Label)this.tlp_ask.Controls.Find(string.Format("lb_asks{0}", i + 1), true)[0];
                lb_BidPrice[i] = (Label)this.tlp_bid.Controls.Find(string.Format("lb_bid{0}", i + 1), true)[0];
                lb_BidSize[i] = (Label)this.tlp_bid.Controls.Find(string.Format("lb_bids{0}", i + 1), true)[0];
            }
        }
        //创建一个带有参数的方法
        public void setStockName(string code,string name)
        {
            this.lb_stockcode.Text = code;
            this.lb_stockname.Text = name;
        }
        //清空数据
        public  void Clear()
        {
            this.lb_stockcode.Text = string.Empty;
            this.lb_stockname.Text = string.Empty;
            for(int i=0;i<lb_AskPrice.Length;i++)
            {
                //清除为空
                lb_AskPrice[i].Text = string.Empty;
                lb_BidPrice[i].Text = string.Empty;
                lb_AskSize[i].Text = string.Empty;
                lb_BidSize[i].Text = string.Empty;
            }
        }
        //修改
        public void Update(StockData.TickData td)
        {
            //ParameterizedThreadStart pts = new ParameterizedThreadStart(UpdateLabel);
            //Thread th = new Thread(pts);
            //th.Start(td);
            UpdateLabel(td);
        }
        //创建一个委托
        delegate void UpdateDelegate(object td);
        //创建一个方法修改label中的值
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
                for (int i = 0; i < this.lb_AskPrice.Length; i++)
                {
                    lb_AskPrice[i].Text = td.Asks[i].ToString();
                    lb_BidPrice[i].Text = td.Bids[i].ToString();
                    lb_AskSize[i].Text = td.Asksizes[i].ToString();
                    lb_BidSize[i].Text = td.Bidsizes[i].ToString();
                }
                this.ResumeLayout();
            }
            
        }
    }
}
