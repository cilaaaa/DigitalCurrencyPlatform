using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StockPolicies;

namespace StockPolicyContorl
{
    public partial class uc_StockMonitor : UserControl
    {
        private RunningPolicy policy;

        public RunningPolicy Policy
        {
            get { return policy; }
        }
        System.Timers.Timer resetBackGroundTimer;
        public delegate void PolicyResultReceiveEventHandler(RunningPolicy policy,PolicyResultEventArgs args);
        public event PolicyResultReceiveEventHandler StockMonitor_ResultArrival;
        public delegate void PolicyRemoveEventHandler(uc_StockMonitor ucs);
        public event PolicyRemoveEventHandler Policy_Remove;

        public delegate void PolicyMessageArrivalEventHandler(RunningPolicy policy,PolicyMessageEventArgs args);
        public event PolicyMessageArrivalEventHandler StockMonitor_MessageArrival;

        public uc_StockMonitor()
        {
            InitializeComponent();
            //设置时间间隔
            resetBackGroundTimer = new System.Timers.Timer();
            resetBackGroundTimer.Interval = 1000;
            resetBackGroundTimer.Elapsed += resetBackGroundTimer_Elapsed;
        }


        private void Remove()
        {
            if (Policy_Remove != null)
            {
                this.policy.Stop();
                Policy_Remove(this);
            }
        }

        private void resetBackGroundTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.lb_stockprice.BackColor = System.Drawing.SystemColors.Control;
            resetBackGroundTimer.Enabled = false;
        }
        public void InitialStock(RunningPolicy _policy)
        {
            this.policy = _policy;
            this.lb_stockcode.Text = _policy.SecInfo.Code;
            this.lb_stockname.Text = _policy.SecInfo.Name;
            this.lb_stockprice.Text = string.Empty;
            this.lb_stockpercent.Text = string.Empty;
            this.lb_policyname.Text = _policy.PolicyName;
            policy.LiveData_Arrival += runningPolicy_LiveData_Arrival;
            policy.PolicyResult_Arrival += policy_PolicyResult_Arrival;
            policy.PolicyMessage_Arrival += policy_PolicyMessage_Arrival;
        }

        void policy_PolicyMessage_Arrival(object sender, PolicyMessageEventArgs args)
        {
            if(StockMonitor_MessageArrival != null)
            {
                StockMonitor_MessageArrival((RunningPolicy)sender,args);
            }
        }
        //private void RaiseResult(RunningPolicy policy,PolicyResultEventArgs args)
        //{
        //    if (StockMonitor_ResultArrival != null)
        //    {
        //        StockMonitor_ResultArrival(policy,args);
        //    }
        //}
        void policy_PolicyResult_Arrival(object sender, PolicyResultEventArgs args)
        {
            if (StockMonitor_ResultArrival != null)
            {
                StockMonitor_ResultArrival((RunningPolicy)sender, args);
            }
        }

        void runningPolicy_LiveData_Arrival(object sender, LiveDataEventArgs args)
        {
            UpdateStockData(args);

        }
        delegate void UpdateStockDataDelegate(LiveDataEventArgs args);
        private void UpdateStockData(LiveDataEventArgs args)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdateStockDataDelegate(UpdateStockData), new object[] { args });
                }
                catch { }
            }
            else
            {
                try
                {
                    //if(true)
                    if (args.IsReal)
                    {
                        if (this.lb_stockprice.Text != args.CurrentPrice.ToString("0.00"))
                        {
                            this.lb_stockprice.Text = args.CurrentPrice.ToString("0.00");
                            double percent = (args.CurrentPrice - args.PreClose) / args.PreClose * 100;
                            this.lb_stockpercent.Text = string.Format("{0}", percent.ToString("0.00"));
                            if (args.CurrentPrice < args.PreClose)
                            {
                                //背景变绿色
                                this.lb_stockprice.ForeColor = Color.Green;
                                this.lb_stockpercent.ForeColor = Color.Green;

                            }
                            else if (args.CurrentPrice == args.PreClose)
                            {
                                //背景变黑色
                                this.lb_stockpercent.ForeColor = Color.Black;
                                this.lb_stockprice.ForeColor = Color.Black;
                            }
                            else
                            {
                                //背景变红色
                                this.lb_stockpercent.ForeColor = Color.Red;
                                this.lb_stockprice.ForeColor = Color.Red;
                            }
                            //背景变蓝色

                            this.lb_stockprice.BackColor = Color.Blue;
                            resetBackGroundTimer.Enabled = true;
                        }
                    }
                    else
                    {
                        if (args.Stockcode == string.Empty)
                        {
                            this.lb_stockprice.Text = "结束";
                        }
                        else
                        {

                            this.lb_stockprice.Text = "模拟中...";
                        }
                    }
                }
                catch { }
            }
        }
        //定义一个方法，传递参数
        public void UpdatePrice(double price, double percent)
        {
            //添加的控件等于参数
            this.lb_stockprice.Text = price.ToString();
            //添加的控件等于参数
            this.lb_stockpercent.Text = percent.ToString();
        }
        //定义一个方法，显示正常情况下的图片
        //public void ErrorImage()
        //{
        //    this.pictureBox1.Image = global::PolicyMonitor1.Properties.Resources.t1;        
        //}
        ////定义一个方法，显示异常情况下的图片
        //public void NormalImage()
        //{
        //    this.pictureBox1.Image = global::PolicyMonitor1.Properties.Resources.t2;
        //}

        private void lb_policyname_Click(object sender, EventArgs e)
        {
#if !READONLY
            Screen screen = Screen.FromControl(this);
            policy.showParameter(this.ParentForm,screen);
#endif
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要删除该策略", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Remove();
            }
        }

        private void bt_chart_Click(object sender, EventArgs e)
        {
            Screen screen = Screen.FromControl(this);
            policy.showMonitor(screen);
        }
    }
}
