using C1.Win.C1FlexGrid;
using C1.Win.C1Ribbon;
using DataAPI;
using DataBase;
using GeneralForm;
using StockData;
using StockPolicies;
using StockTradeAPI;
using StockTrade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;

namespace EntropyRiseStockTP0
{
    public partial class frm_TradeMonitor : C1RibbonForm
    {
        //int ClientID = -1;


        Trader _trader;
        SecurityInfo _tradeSI;


        CellStyle cs_Clickable;

        public void setShowView(bool sw)
        {
            //this._trader._showView = sw;
        }

        public frm_TradeMonitor(Trader trader)
        {
            this._trader = trader;
            InitializeComponent();
            InitializeZiJinGrid();
            InitializeWeiTuoGrid();
            InitializeChenJiaoGrid();
            InitializeChiCangGrid();
            InitializeCeLueGrid();
            InitializeKeCheGrid();

            trader.Account_Connected += trader_Account_Connected;
            trader.ChenJiao_Arrival += trader_ChenJiao_Arrival;
            trader.KeChe_Change += trader_KeChe_Change;
            trader.ChiCang_Change += trader_ChiCang_Change;
            trader.Message_Arrival += trader_Message_Arrival;
            trader.WeiTuo_Change += trader_WeiTuo_Change;
            trader.ZiJing_Update += trader_ZiJing_Update;
            trader.TradeDetail_Update += trader_TradeDetail_Update;
            trader.onHandDetailChange += trade_onHandDetailChange;
            trader.TA.Bta.tradeQueryArrival += trader_Quote5_Arrival;
            this.Text = string.Format("交易监控-{1}-{0}", trader.AccountID, trader.TA.Bta.market);
            _tradeSI = new SecurityInfo(string.Empty, string.Empty, "0", 0, 0, 0, "", "");
            cs_Clickable = grid_celueX.Styles.Add("cs_Clickable");
            cs_Clickable.ForeColor = Color.Blue;


        }

        private void trade_onHandDetailChange(object sender, OnHandDetailChangeEventArgs args)
        {
            UpdateOnHandDetail(args);
        }

        delegate void UpdateOnHandDetailDelegate(OnHandDetailChangeEventArgs args);
        private void UpdateOnHandDetail(OnHandDetailChangeEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateOnHandDetailDelegate(UpdateOnHandDetail), new object[] { args });
            }
            else
            {
                for (int i = 1; i < grid_chicangX.Rows.Count; i++)
                {
                    if (grid_chicangX.Rows[i][0].ToString() == args.OnHandStock.Si.Code)
                    {
                        this.grid_chicangX.Rows[i][5] = args.OnHandStock.StockAvailable;
                    }
                }
            }
        }

        void trader_TradeDetail_Update(object sender, TradeDetailUpdateEventArgs args)
        {
            UpdatePolicy(args.TDetail);
        }

        public void Quote5_Arrival(TickData td)
        {
            UpdateRealQuote(td);
        }

        void trader_ZiJing_Update(object sender, ZiJingEventArgs args)
        {
            UpdateZiJingGrid(args);
        }

        delegate void UpdateZiJingGridDelegate(ZiJingEventArgs args);
        private void UpdateZiJingGrid(ZiJingEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateZiJingGridDelegate(UpdateZiJingGrid), new object[] { args });
            }
            else
            {
                if (args.ChangeType == DataChangeType.Add)
                {
                    int index;
                    this.grid_zijingX.Rows.Add();
                    index = this.grid_zijingX.Rows.Count - 1;
                    this.grid_zijingX.Rows[index][0] = args.Zj.Instrument_id;
                    this.grid_zijingX.Rows[index][1] = args.Zj.Equity;
                    this.grid_zijingX.Rows[index][2] = args.Zj.Frozen;
                    this.grid_zijingX.Rows[index][3] = args.Zj.Total_avail_balance;
                    this.grid_zijingX.Rows[index][4] = args.Zj.Margin_ratio;
                    this.grid_zijingX.Rows[index][5] = args.Zj.Realized_pnl;
                    this.grid_zijingX.Rows[index][6] = args.Zj.Timestamp;
                    this.grid_zijingX.Rows[index][7] = args.Zj.Margin;
                    this.grid_zijingX.Rows[index][8] = args.Zj.Unrealized_pnl;
                    this.grid_zijingX.Rows[index][9] = args.Zj.Fixed_balance;
                    this.grid_zijingX.Rows[index][10] = args.Zj.Mode;
                }
                else
                {
                    for (int i = grid_zijingX.Rows.Fixed; i < grid_zijingX.Rows.Count; i++)
                    {
                        if (grid_zijingX.Rows[i][0].ToString() == args.Zj.Instrument_id)
                        {
                            this.grid_zijingX.Rows[i][1] = args.Zj.Equity;
                            this.grid_zijingX.Rows[i][2] = args.Zj.Frozen;
                            this.grid_zijingX.Rows[i][3] = args.Zj.Total_avail_balance;
                            this.grid_zijingX.Rows[i][4] = args.Zj.Margin_ratio;
                            this.grid_zijingX.Rows[i][5] = args.Zj.Realized_pnl;
                            this.grid_zijingX.Rows[i][6] = args.Zj.Timestamp;
                            this.grid_zijingX.Rows[i][7] = args.Zj.Margin;
                            this.grid_zijingX.Rows[i][8] = args.Zj.Unrealized_pnl;
                            this.grid_zijingX.Rows[i][9] = args.Zj.Fixed_balance;
                            this.grid_zijingX.Rows[i][10] = args.Zj.Mode;
                        }
                    }
                }
            }

        }

        void trader_WeiTuo_Change(object sender, WeiTuoEventArgs args)
        {
            UpdateWeiTuoGrid(args);
        }

        delegate void UpdateWeiTuoGridDelegate(WeiTuoEventArgs args);
        private void UpdateWeiTuoGrid(WeiTuoEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateWeiTuoGridDelegate(UpdateWeiTuoGrid), new object[] { args });
            }
            else
            {
                if (args.ChangeType == DataChangeType.Add)
                {

                    int index;
                    this.grid_weituoX.Rows.Add();
                    index = this.grid_weituoX.Rows.Count - 1;
                    grid_weituoX.Rows[index][0] = args.Wt.Time;
                    grid_weituoX.Rows[index][2] = args.Wt.Code;
                    grid_weituoX.Rows[index][3] = args.Wt.Name;
                    grid_weituoX.Rows[index][4] = args.Wt.Title1;
                    grid_weituoX.Rows[index][5] = args.Wt.Title2;
                    grid_weituoX.Rows[index][6] = args.Wt.Price;
                    grid_weituoX.Rows[index][7] = args.Wt.Qty;
                    grid_weituoX.Rows[index][8] = args.Wt.Price_deal;
                    grid_weituoX.Rows[index][9] = args.Wt.Qty_deal;
                    grid_weituoX.Rows[index][10] = args.Wt.Fee;
                    grid_weituoX.Rows[index][11] = args.Wt.CancelTime;
                    grid_weituoX.Rows[index][12] = args.Wt.Qty_cancel;
                    grid_weituoX.Rows[index][13] = args.Wt.WTnbr;
                    grid_weituoX.Rows[index][14] = args.Wt.CWTnbr;
                    grid_weituoX.Rows[index][15] = args.Wt.WeiTuo_Type;
                    grid_weituoX.Rows[index][16] = args.Wt.Status;

                }
                else if (args.ChangeType == DataChangeType.Change)
                {
                    for (int i = grid_weituoX.Rows.Fixed; i < grid_weituoX.Rows.Count; i++)
                    {
                        if (grid_weituoX.Rows[i][14].ToString() == args.Wt.CWTnbr)
                        {
                            grid_weituoX.Rows[i][0] = args.Wt.Time;
                            grid_weituoX.Rows[i][2] = args.Wt.Code;
                            grid_weituoX.Rows[i][3] = args.Wt.Name;
                            grid_weituoX.Rows[i][4] = args.Wt.Title1;
                            grid_weituoX.Rows[i][5] = args.Wt.Title2;
                            grid_weituoX.Rows[i][6] = args.Wt.Price;
                            grid_weituoX.Rows[i][7] = args.Wt.Qty;
                            grid_weituoX.Rows[i][8] = args.Wt.Price_deal;
                            grid_weituoX.Rows[i][9] = args.Wt.Qty_deal;
                            grid_weituoX.Rows[i][10] = args.Wt.Fee;
                            grid_weituoX.Rows[i][11] = args.Wt.CancelTime;
                            grid_weituoX.Rows[i][12] = args.Wt.Qty_cancel;
                            grid_weituoX.Rows[i][13] = args.Wt.WTnbr;
                            grid_weituoX.Rows[i][14] = args.Wt.CWTnbr;
                            grid_weituoX.Rows[i][15] = args.Wt.WeiTuo_Type;
                            grid_weituoX.Rows[i][16] = args.Wt.Status;
                        }
                    }
                }
            }
        }

        void trader_Message_Arrival(object sender, MessageEventArgs args)
        {
            TradeLog.Log(args.Message);
        }

        void trader_KeChe_Change(object sender, KeCheEventArgs args)
        {
            UpdateKeCheGrid(args);
        }
        delegate void UpdateKeCheGridDelegate(KeCheEventArgs args);
        private void UpdateKeCheGrid(KeCheEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateKeCheGridDelegate(UpdateKeCheGrid), new object[] { args });
            }
            else
            {
                if (args.KeCheList.Count == 0)
                {
                    if (this.grid_kecheX.Rows.Count > this.grid_kecheX.Rows.Fixed)
                    {
                        this.grid_kecheX.Rows.RemoveRange(this.grid_kecheX.Rows.Fixed, this.grid_kecheX.Rows.Count - this.grid_kecheX.Rows.Fixed);
                    }
                }
                else
                {
                    List<string> keys = args.KeCheList.Keys.ToList();
                    for (int i = this.grid_kecheX.Rows.Count - 1; i >= this.grid_kecheX.Rows.Fixed; i--)
                    {
                        string gwtbh = grid_kecheX.Rows[i][12].ToString().Trim();
                        bool exists = false;
                        for (int j = 0; j < keys.Count; j++)
                        {
                            if (args.KeCheList.ContainsKey(keys[j]))
                            {
                                StockWeiTuo skc = args.KeCheList[keys[j]];
                                if (gwtbh == skc.WTnbr)
                                {
                                    grid_kecheX.Rows[i][1] = skc.Time;
                                    grid_kecheX.Rows[i][2] = skc.Code;
                                    grid_kecheX.Rows[i][3] = skc.Name;
                                    grid_kecheX.Rows[i][4] = skc.Title1;
                                    grid_kecheX.Rows[i][5] = skc.Title2;
                                    grid_kecheX.Rows[i][6] = skc.Price;
                                    grid_kecheX.Rows[i][7] = skc.Qty;
                                    grid_kecheX.Rows[i][8] = skc.Price_deal;
                                    grid_kecheX.Rows[i][9] = skc.Qty_deal;
                                    grid_kecheX.Rows[i][10] = skc.Fee;
                                    grid_kecheX.Rows[i][11] = skc.Qty_cancel;
                                    grid_kecheX.Rows[i][12] = skc.WTnbr;
                                    grid_kecheX.Rows[i][13] = skc.CWTnbr;
                                    grid_kecheX.Rows[i][14] = skc.WeiTuo_Type;
                                    grid_kecheX.Rows[i][15] = skc.Status;
                                    skc.Exists = true;
                                    exists = true;
                                    break;
                                }
                            }
                        }
                        if (!exists)
                        {
                            this.grid_kecheX.Rows.Remove(i);
                        }
                    }
                    for (int j = 0; j < keys.Count; j++)
                    {
                        if (args.KeCheList.ContainsKey(keys[j]))
                        {
                            StockWeiTuo skc = args.KeCheList[keys[j]];
                            if (!skc.Exists)
                            {
                                this.grid_kecheX.Rows.Add();
                                int index = grid_kecheX.Rows.Count - 1;
                                this.grid_kecheX.Rows[index][0] = false;
                                grid_kecheX.Rows[index][1] = skc.Time;
                                grid_kecheX.Rows[index][2] = skc.Code;
                                grid_kecheX.Rows[index][3] = skc.Name;
                                grid_kecheX.Rows[index][4] = skc.Title1;
                                grid_kecheX.Rows[index][5] = skc.Title2;
                                grid_kecheX.Rows[index][6] = skc.Price;
                                grid_kecheX.Rows[index][7] = skc.Qty;
                                grid_kecheX.Rows[index][8] = skc.Price_deal;
                                grid_kecheX.Rows[index][9] = skc.Qty_deal;
                                grid_kecheX.Rows[index][10] = skc.Fee;
                                grid_kecheX.Rows[index][11] = skc.Qty_cancel;
                                grid_kecheX.Rows[index][12] = skc.WTnbr;
                                grid_kecheX.Rows[index][13] = skc.CWTnbr;
                                grid_kecheX.Rows[index][14] = skc.WeiTuo_Type;
                                grid_kecheX.Rows[index][15] = skc.Status;
                            }
                        }
                    }
                }
            }
        }

        void trader_ChiCang_Change(object sender, ChiCangEventArgs args)
        {
            UpdateChiCangGrid(args);
        }

        delegate void UpdateChiCangGirdDelegate(ChiCangEventArgs args);
        private void UpdateChiCangGrid(ChiCangEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateChiCangGirdDelegate(UpdateChiCangGrid), new object[] { args });
            }
            else
            {
                for (int i = this.grid_chicangX.Rows.Count - 1; i >= this.grid_chicangX.Rows.Fixed; i--)
                {
                    string code = grid_chicangX.Rows[i][0].ToString();
                    foreach (StockChiCang scc in args.Chicangs)
                    {
                        if (code == scc.Instrument_id)
                        {
                            grid_chicangX.Rows[i][1] = scc.Margin_mode;
                            grid_chicangX.Rows[i][2] = scc.Liquidation_price;
                            grid_chicangX.Rows[i][3] = scc.Long_qty;
                            grid_chicangX.Rows[i][4] = scc.Long_avail_qty;
                            grid_chicangX.Rows[i][5] = scc.Long_avg_cost;
                            grid_chicangX.Rows[i][6] = scc.Long_settlement_price;
                            grid_chicangX.Rows[i][7] = scc.Realized_pnl;
                            grid_chicangX.Rows[i][8] = scc.Leverage;
                            grid_chicangX.Rows[i][9] = scc.Short_qty;
                            grid_chicangX.Rows[i][10] = scc.Short_avail_qty;
                            grid_chicangX.Rows[i][11] = scc.Short_avg_cost;
                            grid_chicangX.Rows[i][12] = scc.Short_settlement_price;
                            grid_chicangX.Rows[i][13] = scc.Created_at;
                            grid_chicangX.Rows[i][14] = scc.Updated_at;
                            scc.Exist = true;
                            break;
                        }
                    }
                }
                foreach (StockChiCang scc in args.Chicangs)
                {
                    if (!scc.Exist)
                    {
                        this.grid_chicangX.Rows.Add();
                        int i = grid_chicangX.Rows.Count - 1;
                        grid_chicangX.Rows[i][0] = scc.Instrument_id;
                        grid_chicangX.Rows[i][1] = scc.Margin_mode;
                        grid_chicangX.Rows[i][2] = scc.Liquidation_price;
                        grid_chicangX.Rows[i][3] = scc.Long_qty;
                        grid_chicangX.Rows[i][4] = scc.Long_avail_qty;
                        grid_chicangX.Rows[i][5] = scc.Long_avg_cost;
                        grid_chicangX.Rows[i][6] = scc.Long_settlement_price;
                        grid_chicangX.Rows[i][7] = scc.Realized_pnl;
                        grid_chicangX.Rows[i][8] = scc.Leverage;
                        grid_chicangX.Rows[i][9] = scc.Short_qty;
                        grid_chicangX.Rows[i][10] = scc.Short_avail_qty;
                        grid_chicangX.Rows[i][11] = scc.Short_avg_cost;
                        grid_chicangX.Rows[i][12] = scc.Short_settlement_price;
                        grid_chicangX.Rows[i][13] = scc.Created_at;
                        grid_chicangX.Rows[i][14] = scc.Updated_at;
                    }
                }
            }
        }

        void trader_ChenJiao_Arrival(object sender, ChengJiaoEventArgs args)
        {
            UpdateChengJiaoGrid(args);
        }

        delegate void UpdateChengJiaoGridDelegate(ChengJiaoEventArgs args);
        private void UpdateChengJiaoGrid(ChengJiaoEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateChengJiaoGridDelegate(UpdateChengJiaoGrid), new object[] { args });
            }
            else
            {
                this.grid_chenjiaoX.Rows.Add();
                int index = grid_chenjiaoX.Rows.Count - 1;
                this.grid_chenjiaoX.Rows[index][0] = args.Cj.Time;
                this.grid_chenjiaoX.Rows[index][1] = args.Cj.Code;
                this.grid_chenjiaoX.Rows[index][2] = args.Cj.Name;
                this.grid_chenjiaoX.Rows[index][3] = args.Cj.Title1;
                this.grid_chenjiaoX.Rows[index][4] = args.Cj.Title2;
                this.grid_chenjiaoX.Rows[index][5] = args.Cj.Price;
                this.grid_chenjiaoX.Rows[index][6] = args.Cj.Qty;
                this.grid_chenjiaoX.Rows[index][7] = args.Cj.Fee;
                this.grid_chenjiaoX.Rows[index][8] = args.Cj.Amount;
                this.grid_chenjiaoX.Rows[index][9] = args.Cj.CJnbr;
                this.grid_chenjiaoX.Rows[index][10] = args.Cj.WTnbr;
            }
        }

        void trader_Account_Connected(object sender, EventArgs args)
        {
            SwitchButton();
        }

        void trader_Quote5_Arrival(object sender, TickData args)
        {
            UpdateRealQuote(args);
        }

        delegate void UpdateRealQuoteDelegate(TickData tickdata);
        void UpdateRealQuote(TickData tickdata)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateRealQuoteDelegate(UpdateRealQuote), new object[] { tickdata });
            }
            else
            {
                if (tickdata.Code != string.Empty)
                {
                    double high = Math.Round(tickdata.Preclose * 1.1, 3, MidpointRounding.AwayFromZero);
                    double low = Math.Round(tickdata.Preclose * 0.9, 3, MidpointRounding.AwayFromZero);
                    this.lb_zhangting.Text = high.ToString("0.000");
                    this.lb_dieting.Text = low.ToString("0.000");

                    //if (this.nu_price.Value == 0)
                    //{
                    //    this.nu_price.Value = (decimal)tickdata.Last;
                    //}
                    //this.nu_price.Maximum = (decimal)high;
                    //this.nu_price.Minimum = (decimal)low;



                    this.lb_preclose.Text = tickdata.Preclose.ToString("0.00000000");
                    this.lb_last.Text = tickdata.Last.ToString("0.00000000");
                    this.lb_a1.Text = tickdata.Asks[0].ToString("0.00000000");
                    this.lb_a2.Text = tickdata.Asks[1].ToString("0.00000000");
                    this.lb_a3.Text = tickdata.Asks[2].ToString("0.00000000");
                    this.lb_a4.Text = tickdata.Asks[3].ToString("0.00000000");
                    this.lb_a5.Text = tickdata.Asks[4].ToString("0.00000000");
                    this.lb_as1.Text = tickdata.Asksizes[0].ToString();
                    this.lb_as2.Text = tickdata.Asksizes[1].ToString();
                    this.lb_as3.Text = tickdata.Asksizes[2].ToString();
                    this.lb_as4.Text = tickdata.Asksizes[3].ToString();
                    this.lb_as5.Text = tickdata.Asksizes[4].ToString();
                    this.lb_b1.Text = tickdata.Bids[0].ToString("0.00000000");
                    this.lb_bs1.Text = tickdata.Bidsizes[0].ToString();
                    this.lb_b2.Text = tickdata.Bids[1].ToString("0.00000000");
                    this.lb_bs2.Text = tickdata.Bidsizes[1].ToString();
                    this.lb_b3.Text = tickdata.Bids[2].ToString("0.00000000");
                    this.lb_bs3.Text = tickdata.Bidsizes[2].ToString();
                    this.lb_b4.Text = tickdata.Bids[3].ToString("0.00000000");
                    this.lb_bs4.Text = tickdata.Bidsizes[3].ToString();
                    this.lb_b5.Text = tickdata.Bids[4].ToString("0.00000000");
                    this.lb_bs5.Text = tickdata.Bidsizes[4].ToString();
                }
                else
                {

                }
            }
        }


        void stockAccount_Message_Arrival(object sender, MessageEventArgs args)
        {
            TradeLog.Log(args.Message);
        }

        private void InitializeChiCangGrid()
        {
            this.grid_chicangX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "合约名称", 100, string.Empty });
            titles.Add(new object[3] { "账户类型", 80, string.Empty });
            titles.Add(new object[3] { "预估爆仓价", 80, string.Empty });
            titles.Add(new object[3] { "多仓数量", 100, string.Empty });
            titles.Add(new object[3] { "多仓可平仓数量", 100, string.Empty });
            titles.Add(new object[3] { "多仓平均价", 100, string.Empty });
            titles.Add(new object[3] { "多仓结算基准价", 100, string.Empty });
            titles.Add(new object[3] { "已实现盈余", 100, string.Empty });
            titles.Add(new object[3] { "杠杆倍数", 100, string.Empty });
            titles.Add(new object[3] { "空仓数量", 100, string.Empty });
            titles.Add(new object[3] { "空仓可平仓数量", 100, string.Empty });
            titles.Add(new object[3] { "空仓平均价", 100, string.Empty });
            titles.Add(new object[3] { "空仓结算基准价", 100, string.Empty });
            titles.Add(new object[3] { "创建时间", 130, string.Empty });
            titles.Add(new object[3] { "更新时间", 130, string.Empty });



            this.grid_chicangX.Cols.Count = 15;
            this.grid_chicangX.Cols.Fixed = 0;
            this.grid_chicangX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_chicangX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_chicangX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_chicangX.AllowEditing = true;
            for (int i = 0; i < 15; i++)
            {
                this.grid_chicangX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_chicangX.Cols[i].Width = (int)titles[i][1];
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_celueX.Cols[i].Format = System.Convert.ToString(titles[i][2]);
                }
            }
        }

        private void InitializeKeCheGrid()
        {
            this.grid_kecheX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "", 40, string.Empty });
            titles.Add(new object[3] { "委托时间", 160, string.Empty });
            titles.Add(new object[3] { "证券代码", 100, string.Empty });
            titles.Add(new object[3] { "证券名称", 100, string.Empty });
            titles.Add(new object[3] { "买卖标志1", 60, string.Empty });
            titles.Add(new object[3] { "买卖标志2", 60, string.Empty });
            titles.Add(new object[3] { "委托价格", 100, "N8" });
            titles.Add(new object[3] { "委托数量", 80, string.Empty });
            titles.Add(new object[3] { "成交价格", 100, "N8" });
            titles.Add(new object[3] { "成交数量", 80, string.Empty });
            titles.Add(new object[3] { "手续费（买入为币，卖出为钱）", 160, string.Empty });
            titles.Add(new object[3] { "撤单数量", 80, string.Empty });
            titles.Add(new object[3] { "委托编号", 110, string.Empty });
            titles.Add(new object[3] { "内部委托编号", 190, string.Empty });
            titles.Add(new object[3] { "报价方式", 80, string.Empty });
            titles.Add(new object[3] { "状态说明", 120, string.Empty });




            this.grid_kecheX.Cols.Count = 16;
            this.grid_kecheX.Cols.Fixed = 0;
            this.grid_kecheX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_kecheX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_kecheX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_kecheX.AllowEditing = true;
            for (int i = 0; i < 16; i++)
            {
                this.grid_kecheX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_kecheX.Cols[i].Width = (int)titles[i][1];
                if (i > 0)
                    this.grid_kecheX.Cols[i].AllowEditing = false;
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_kecheX.Cols[i].Format = titles[i][2].ToString();
                }
            }

            this.grid_kecheX.Cols[0].AllowEditing = true;
            this.grid_kecheX.Cols[0].DataType = typeof(bool);
        }

        private void InitializeCeLueGrid()
        {
            this.grid_celueX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "GUID", 0, string.Empty });
            titles.Add(new object[3] { "策略名称", 220, string.Empty });
            titles.Add(new object[3] { "证券代码", 100, string.Empty });
            titles.Add(new object[3] { "证券名称", 100, string.Empty });
            titles.Add(new object[3] { "策略持仓", 60, string.Empty });
            titles.Add(new object[3] { "", 50, string.Empty });
            titles.Add(new object[3] { "策略盈利", 160, string.Empty });
            titles.Add(new object[3] { "实际盈利", 160, string.Empty });
            titles.Add(new object[3] { "入场委托时间", 160, string.Empty });
            titles.Add(new object[3] { "入场委托价格", 100, "N8" });
            titles.Add(new object[3] { "入场委托数量", 120, string.Empty });
            titles.Add(new object[3] { "入场委托方向", 80, string.Empty });
            titles.Add(new object[3] { "入场成交价格", 100, "N8" });
            titles.Add(new object[3] { "入场成交数量", 120, string.Empty });
            titles.Add(new object[3] { "入场手续费", 80, string.Empty });
            titles.Add(new object[3] { "出场委托时间", 160, string.Empty });
            titles.Add(new object[3] { "出场委托价格", 100, "N8" });
            titles.Add(new object[3] { "出场委托数量", 120, string.Empty });
            titles.Add(new object[3] { "出场委托方向", 80, string.Empty });
            titles.Add(new object[3] { "出场成交价格", 120, "N8" });
            titles.Add(new object[3] { "出场成交数量", 80, string.Empty });
            titles.Add(new object[3] { "出场手续费", 80, string.Empty });
            titles.Add(new object[3] { "完成", 0, string.Empty });
            titles.Add(new object[3] { "备注", 150, string.Empty });



            this.grid_celueX.Cols.Count = 24;
            this.grid_celueX.Cols.Fixed = 0;
            this.grid_celueX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_celueX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_celueX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_celueX.AllowEditing = false;
            for (int i = 0; i < 24; i++)
            {
                this.grid_celueX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_celueX.Cols[i].Width = (int)titles[i][1];
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_celueX.Cols[i].Format = System.Convert.ToString(titles[i][2]);
                }
            }
        }

        //private void InitializeChiCangGrid()
        //{
        //    this.grid_chicangX.Font = new Font("Microsoft YaHei", 8.25f);
        //    List<object[]> titles = new List<object[]>();
        //    titles.Add(new object[3] { "证券代码", 80 ,string.Empty});
        //    titles.Add(new object[3] { "证券名称", 80, string.Empty });
        //    titles.Add(new object[3] { "当前价", 80, "N3" });
        //    titles.Add(new object[3] { "股票余额", 100, string.Empty });
        //    titles.Add(new object[3] { "可用余额", 100, string.Empty });
        //    titles.Add(new object[3] { "T加0可用", 100, string.Empty });
        //    titles.Add(new object[3] { "冻结数量", 100, string.Empty });
        //    titles.Add(new object[3] { "最新市值", 100, "N3" });
        //    titles.Add(new object[3] { "买入成本", 60, "N3" });
        //    titles.Add(new object[3] { "参考盈亏", 100, "N3" });
        //    titles.Add(new object[3] { "保本价", 0, "N3" });
        //    titles.Add(new object[3] { "盈亏比例", 60, string.Empty });
        //    titles.Add(new object[3] { "市场", 60, string.Empty });

        //    this.grid_chicangX.Cols.Count = 13;
        //    this.grid_chicangX.Cols.Fixed = 0;
        //    this.grid_chicangX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
        //    this.grid_chicangX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
        //    this.grid_chicangX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.None;
        //    //this.grid_chicangX.AllowEditing = false;
        //    this.grid_chicangX.AllowEditing = true;
        //    for (int i = 0; i < 13; i++)
        //    {
        //        if (i != 5)
        //        {
        //            this.grid_chicangX.Cols[i].AllowEditing = false;
        //        }
        //        else
        //        {
        //            this.grid_chicangX.Cols[i].DataType = typeof(Int32);
        //        }
        //        this.grid_chicangX.Cols[i].Caption = titles[i][0].ToString();
        //        this.grid_chicangX.Cols[i].Width = (int)titles[i][1];
        //        if (titles[i][2].ToString() != string.Empty)
        //        {
        //            this.grid_chicangX.Cols[i].Format = titles[i][2].ToString();
        //        }
        //    }
        //    this.grid_chicangX.AfterEdit += grid_chicangX_AfterEdit;
        //    //this.grid_chicangX.Cols[2].Format = "N2";
        //}

        //private void grid_chicangX_AfterEdit(object sender, RowColEventArgs e)
        //{
        //    if (e.Col == 5)
        //    {
        //        int qty = System.Convert.ToInt32(this.grid_chicangX.Rows[e.Row][5]);
        //        string code = grid_chicangX.Rows[e.Row][0].ToString();
        //        byte market;
        //        if (code.StartsWith("6") || code.StartsWith("2"))
        //        {
        //            market = 1;
        //        }
        //        else
        //        {
        //            market = 0;
        //        }
        //        this._trader.updateOnHandStock(new SecurityInfo(code, string.Empty, market), qty);
        //    }
        //}

        private void InitializeChenJiaoGrid()
        {
            this.grid_chenjiaoX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "成交时间", 160, string.Empty });
            titles.Add(new object[3] { "证券代码", 100, string.Empty });
            titles.Add(new object[3] { "证券名称", 100, string.Empty });
            titles.Add(new object[3] { "买卖标志1", 50, string.Empty });
            titles.Add(new object[3] { "买卖标志2", 50, string.Empty });
            titles.Add(new object[3] { "成交价格", 100, string.Empty });
            titles.Add(new object[3] { "成交数量", 100, string.Empty });
            titles.Add(new object[3] { "手续费（买入为币，卖出为钱）", 160, string.Empty });
            titles.Add(new object[3] { "发生金额", 100, string.Empty });
            titles.Add(new object[3] { "成交编号", 300, string.Empty });
            titles.Add(new object[3] { "委托编号", 300, string.Empty });
            //titles.Add(new object[2] { "撤单标志", 100 });
            //titles.Add(new object[2] { "申报编号", 100 });
            this.grid_chenjiaoX.Cols.Count = 11;
            this.grid_chenjiaoX.Cols.Fixed = 0;
            this.grid_chenjiaoX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_chenjiaoX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_chenjiaoX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_chenjiaoX.AllowEditing = false;
            for (int i = 0; i < 11; i++)
            {
                this.grid_chenjiaoX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_chenjiaoX.Cols[i].Width = (int)titles[i][1];
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_chenjiaoX.Cols[i].Format = titles[i][2].ToString();
                }
            }
        }

        private void InitializeWeiTuoGrid()
        {
            this.grid_weituoX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "委托时间", 160, string.Empty });
            titles.Add(new object[3] { "股东代码", 0, string.Empty });
            titles.Add(new object[3] { "证券代码", 100, string.Empty });
            titles.Add(new object[3] { "证券名称", 100, string.Empty });
            titles.Add(new object[3] { "买卖标志1", 50, string.Empty });
            titles.Add(new object[3] { "买卖标志2", 50, string.Empty });
            titles.Add(new object[3] { "委托价格", 100, "N8" });
            titles.Add(new object[3] { "委托数量", 60, string.Empty });
            titles.Add(new object[3] { "成交价格", 100, "N8" });
            titles.Add(new object[3] { "成交数量", 60, string.Empty });
            titles.Add(new object[3] { "手续费（买入为币，卖出为钱）", 160, string.Empty });
            titles.Add(new object[3] { "撤单申请时间", 0, string.Empty });
            titles.Add(new object[3] { "撤单数量", 60, string.Empty });
            titles.Add(new object[3] { "委托编号", 110, string.Empty });
            titles.Add(new object[3] { "自定义委托编号", 190, string.Empty });
            titles.Add(new object[3] { "报价方式", 100, string.Empty });
            titles.Add(new object[3] { "状态说明", 100, string.Empty });
            this.grid_weituoX.Cols.Count = 17;
            this.grid_weituoX.Cols.Fixed = 0;
            this.grid_weituoX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_weituoX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_weituoX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_weituoX.AllowEditing = false;
            for (int i = 0; i < 17; i++)
            {
                this.grid_weituoX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_weituoX.Cols[i].Width = (int)titles[i][1];
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_weituoX.Cols[i].Format = titles[i][2].ToString();
                }
            }
        }

        private void InitializeZiJinGrid()
        {
            this.grid_zijingX.Font = new Font("Microsoft YaHei", 8.25f);
            List<object[]> titles = new List<object[]>();
            titles.Add(new object[3] { "币种", 120, string.Empty });
            titles.Add(new object[3] { "可用余额", 120, string.Empty });
            titles.Add(new object[3] { "冻结余额", 120, string.Empty });
            titles.Add(new object[3] { "账户余额", 120, string.Empty });
            titles.Add(new object[3] { "保证金率", 120, string.Empty });
            titles.Add(new object[3] { "已实现盈亏", 120, string.Empty });
            titles.Add(new object[3] { "创建时间", 130, string.Empty });
            titles.Add(new object[3] { "已用保证金", 120, string.Empty });
            titles.Add(new object[3] { "未实现盈亏", 120, string.Empty });
            titles.Add(new object[3] { "逐仓账户余额", 120, string.Empty });
            titles.Add(new object[3] { "账户类型：逐仓fixed 全仓crossed", 200, string.Empty });

            this.grid_zijingX.Cols.Count = 11;
            this.grid_zijingX.Cols.Fixed = 0;
            this.grid_zijingX.AllowSorting = C1.Win.C1FlexGrid.AllowSortingEnum.None;
            this.grid_zijingX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_zijingX.AllowResizing = C1.Win.C1FlexGrid.AllowResizingEnum.Columns;
            this.grid_zijingX.AllowEditing = false;
            for (int i = 0; i < 11; i++)
            {
                this.grid_zijingX.Cols[i].Caption = titles[i][0].ToString();
                this.grid_zijingX.Cols[i].Width = (int)titles[i][1];
                if (titles[i][2].ToString() != string.Empty)
                {
                    this.grid_zijingX.Cols[i].Format = titles[i][2].ToString();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {

            const int WM_SYSCOMMAND = 0x0112;


            const int SC_CLOSE = 0xF060;


            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                // 屏蔽传入的消息事件
                //this.WindowState = FormWindowState.Minimized;
                return;
            }
            base.WndProc(ref m);
        }

        private void bt_connect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        public delegate void MessageArrival(object sender, MessageEventArgs args);
        public event MessageArrival Message_Arrival;
        private void ExportMessage(MessageEventArgs args)
        {
            if (Message_Arrival != null)
            {
                Message_Arrival(this, args);
            }
        }

        private void Connect()
        {
            this.cbt_connect.Enabled = false;
            this.cbt_disconnect.Enabled = true;
            _trader.TryConnect();

        }
        delegate void SwitchButtonDelegate();
        private void SwitchButton()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SwitchButtonDelegate(SwitchButton));
            }
            else
            {
                this.lb_status.ForeColor = Color.Green;
                this.lb_status.Text = "已连接";
                this.cbt_connect.Enabled = false;

                this.cbt_disconnect.Enabled = true;
            }
        }


        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void bt_chedan_Click(object sender, EventArgs e)
        {
            CheDan();
        }

        private void CheDan()
        {
            List<Dictionary<string, string>> weituo = new List<Dictionary<string, string>>();
            for (int i = 1; i < this.grid_kecheX.Rows.Count; i++)
            {
                if (System.Convert.ToBoolean(grid_kecheX.Rows[i][0]))
                {
                    string weituobianhao = grid_kecheX.Rows[i][12].ToString().Trim();
                    string code = grid_kecheX.Rows[i][2].ToString().Trim();
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param["code"] = code;
                    param["orderId"] = weituobianhao;
                    weituo.Add(param);
                }
            }
            if (weituo.Count > 0)
            {
                _trader.TA.Bta.DeleteOrders(weituo);
            }
        }

        delegate void UpdatePolicyDelegate(TradeDetail td);
        internal void UpdatePolicy(TradeDetail td)
        {
            if (this.grid_celueX.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdatePolicyDelegate(UpdatePolicy), new object[] { td });
                }
                catch { }
            }
            else
            {
                for (int i = 1; i < this.grid_celueX.Rows.Count; i++)
                {
                    Guid g = Guid.Parse(grid_celueX.Rows[i][0].ToString());
                    if (g == td.Tradeid)
                    {
                        this.grid_celueX.Rows[i][1] = td.PolicyName;
                        this.grid_celueX.Rows[i][2] = td.TradeSi.Code;
                        this.grid_celueX.Rows[i][3] = td.TradeSi.Name;
                        this.grid_celueX.Rows[i][4] = td.TradeOHQty;
                        if (td.TradeOHQty != 0)
                        {
                            this.grid_celueX.Rows[i][5] = "平仓";
                            this.grid_celueX.SetCellStyle(i, 5, cs_Clickable);
                        }
                        else
                        {
                            this.grid_celueX.Rows[i][5] = string.Empty;
                        }
                        this.grid_celueX.Rows[i][8] = td.TradeOpenOrderTime; //入场委托时间
                        this.grid_celueX.Rows[i][9] = td.TradeOpenOrderPrice; //入场委托价格
                        this.grid_celueX.Rows[i][10] = td.TradeOpenOrderQty;//入场委托数量
                        this.grid_celueX.Rows[i][11] = Enum.GetName(typeof(OpenType), td.TradeOpenOrderType); //入场委托方向
                        this.grid_celueX.Rows[i][12] = td.TradeOpenDealPrice; //入场成交价格
                        this.grid_celueX.Rows[i][13] = td.TradeOpenDealQty; //入场成交数量
                        this.grid_celueX.Rows[i][14] = td.TradeOpenFee; //入场成交手续费
                        this.grid_celueX.Rows[i][15] = td.TradeCloseOrderTime; //出场委托时间
                        this.grid_celueX.Rows[i][16] = td.TradeCloseOrderPrice; //出场委托价格
                        this.grid_celueX.Rows[i][17] = td.TradeCloseOrderQty; //出场委托数量
                        this.grid_celueX.Rows[i][18] = Enum.GetName(typeof(OpenType), td.TradeCloseOrderType); //出场委托方向
                        this.grid_celueX.Rows[i][19] = td.TradeCloseDealPrice; //出场成交价格
                        this.grid_celueX.Rows[i][20] = td.TradeCloseDealQty; //出场成交数量
                        this.grid_celueX.Rows[i][21] = td.TradeCloseFee; //出场成交手续费
                        if (td.TradeCloseOrderQty != 0)
                        {
                            double gainpolicy = 0;
                            double gainacture = 0;
                            if (td.PolicyOpenPoint.OpenType == OpenType.KaiDuo)
                            {
                                gainpolicy = (1 / td.PolicyOpenPoint.OpenPrice - 1 / td.PolicyClosePoint.OpenPrice) * td.TradeOpenOrderQty * System.Convert.ToInt16(td.TradeSi.ContractVal);
                                if (td.TradeCloseDealQty != 0 && td.TradeOpenDealQty != 0)
                                {
                                    gainacture = (1 / td.TradeOpenDealPrice - 1 / td.TradeCloseDealPrice) * td.TradeCloseDealQty * System.Convert.ToInt16(td.TradeSi.ContractVal);
                                }
                            }
                            else if (td.PolicyOpenPoint.OpenType == OpenType.KaiKong)
                            {
                                gainpolicy = (1 / td.PolicyClosePoint.OpenPrice - 1 / td.PolicyOpenPoint.OpenPrice) * td.TradeOpenOrderQty * System.Convert.ToInt16(td.TradeSi.ContractVal);
                                if (td.TradeCloseDealQty != 0 && td.TradeOpenDealQty != 0)
                                {
                                    gainacture = (1 / td.TradeCloseDealPrice - 1 / td.TradeOpenDealPrice) * td.TradeCloseDealQty * System.Convert.ToInt16(td.TradeSi.ContractVal);
                                }
                            }
                            else if (td.PolicyOpenPoint.OpenType == OpenType.Buy)
                            {
                                gainpolicy = (td.PolicyOpenPoint.OpenPrice - td.PolicyClosePoint.OpenPrice) * td.TradeOpenOrderQty;
                                if (td.TradeCloseDealQty != 0 && td.TradeOpenDealQty != 0)
                                {
                                    gainacture = (td.TradeOpenDealPrice - td.TradeCloseDealPrice) * td.TradeCloseDealQty;
                                }
                            }
                            else if (td.PolicyOpenPoint.OpenType == OpenType.Sell)
                            {
                                gainpolicy = (td.PolicyOpenPoint.OpenPrice - td.PolicyClosePoint.OpenPrice) * td.TradeOpenOrderQty;
                                if (td.TradeCloseDealQty != 0 && td.TradeOpenDealQty != 0)
                                {
                                    gainacture = (td.TradeOpenDealPrice - td.TradeCloseDealPrice) * td.TradeCloseDealQty;

                                }
                            }
                            gainacture += (td.TradeCloseFee + td.TradeOpenFee);
                            this.grid_celueX.Rows[i][6] = Math.Round(gainpolicy, 8, MidpointRounding.AwayFromZero);
                            this.grid_celueX.Rows[i][7] = Math.Round(gainacture, 8, MidpointRounding.AwayFromZero);
                            td.GainActure = gainacture;
                            td.GainPolicy = gainpolicy;
                        }
                        this.grid_celueX.Rows[i][22] = td.Closed;
                        this.grid_celueX.Rows[i][23] = td.Remark;
                        UpdateTradeLog(td);
                        CalculateStatus();
                        return;
                    }

                }//end for

                this.grid_celueX.Rows.Insert(1);
                int index = 1;
                this.grid_celueX.Rows[index][0] = td.Tradeid;
                this.grid_celueX.Rows[index][1] = td.PolicyName;
                this.grid_celueX.Rows[index][2] = td.TradeSi.Code;
                this.grid_celueX.Rows[index][3] = td.TradeSi.Name;
                this.grid_celueX.Rows[index][4] = td.TradeOHQty;
                if (td.TradeOHQty != 0)
                {
                    this.grid_celueX.Rows[index][5] = "平仓";
                    this.grid_celueX.SetCellStyle(index, 5, cs_Clickable);
                }
                else
                {
                    this.grid_celueX.Rows[index][5] = string.Empty;
                }
                this.grid_celueX.Rows[index][8] = td.TradeOpenOrderTime; //入场委托时间
                this.grid_celueX.Rows[index][9] = td.TradeOpenOrderPrice; //入场委托价格
                this.grid_celueX.Rows[index][10] = td.TradeOpenOrderQty;//入场委托数量
                this.grid_celueX.Rows[index][11] = Enum.GetName(typeof(OpenType), td.TradeOpenOrderType); //入场委托方向
                this.grid_celueX.Rows[index][12] = td.TradeOpenDealPrice; //入场成交价格
                this.grid_celueX.Rows[index][13] = td.TradeOpenDealQty; //入场成交数量
                this.grid_celueX.Rows[index][14] = td.TradeOpenFee; //入场成交手续费
                this.grid_celueX.Rows[index][23] = td.Remark;
                InsertTradeLog(td);
            }
            CalculateStatus();
        }

        private void CalculateStatus()
        {
            this.lb_times.Text = string.Format("{0}次", this.grid_celueX.Rows.Count - 1);
            double profit = 0;
            for (int i = 1; i < this.grid_celueX.Rows.Count; i++)
            {
                try
                {

                    if (System.Convert.ToDouble(this.grid_celueX.Rows[i][4]) == 0)
                    {
                        profit += System.Convert.ToDouble(this.grid_celueX.Rows[i][7]);
                    }

                }
                catch { }
            }
            this.lb_profit.Text = string.Format("{0}", profit.ToString("0.00000000"));

        }
        private void InsertTradeLog(TradeDetail tradeDetail)
        {
            XmlDocument doc = new XmlDocument();
            string trade_day = tradeDetail.CreateTime.ToString("yyyyMMdd");
            string tradeLogFile = string.Format("{0}\\{3}_trade_log_{2}_{1}.xml", ConfigFileName.TradeLogDriectory, trade_day, _trader.AccountID, _trader.TA.Bta.market);
            if (!File.Exists(tradeLogFile))
            {
                StreamWriter sw = new StreamWriter(tradeLogFile, true, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<TradeHistory>");
                sw.WriteLine("</TradeHistory>");
                sw.Close();
            }
            doc.Load(tradeLogFile);
            XmlNode root = doc.SelectSingleNode("TradeHistory");
            XmlElement ntd = doc.CreateElement("TradeDetail");
            ntd.SetAttribute("Guid", tradeDetail.Tradeid.ToString());

            XmlNode xn = doc.CreateElement("PolicyName");
            xn.InnerText = tradeDetail.PolicyName;
            ntd.AppendChild(xn);

            xn = doc.CreateElement("Code");
            xn.InnerText = tradeDetail.TradeSi.Code;
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OHQty");
            xn.InnerText = tradeDetail.TradeOHQty.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("GainPolicy");
            xn.InnerText = tradeDetail.GainPolicy.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("GainActure");
            xn.InnerText = tradeDetail.GainActure.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenOrderType");
            xn.InnerText = Enum.GetName(typeof(OpenType), tradeDetail.TradeOpenOrderType);
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenOrderTime");
            xn.InnerText = tradeDetail.TradeOpenOrderTime;
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenOrderPrice");
            xn.InnerText = tradeDetail.TradeOpenOrderPrice.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenOrderQty");
            xn.InnerText = tradeDetail.TradeOpenOrderQty.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenDealPrice");
            xn.InnerText = tradeDetail.TradeOpenDealPrice.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenDealQty");
            xn.InnerText = tradeDetail.TradeOpenDealQty.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("OpenFee");
            xn.InnerText = tradeDetail.TradeOpenFee.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseOrderType");
            xn.InnerText = Enum.GetName(typeof(OpenType), tradeDetail.TradeCloseOrderType);
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseOrderTime");
            xn.InnerText = tradeDetail.TradeCloseOrderTime;
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseOrderPrice");
            xn.InnerText = tradeDetail.TradeCloseOrderPrice.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseOrderQty");
            xn.InnerText = tradeDetail.TradeCloseOrderQty.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseDealPrice");
            xn.InnerText = tradeDetail.TradeCloseDealPrice.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseDealQty");
            xn.InnerText = tradeDetail.TradeCloseDealQty.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("CloseFee");
            xn.InnerText = tradeDetail.TradeCloseFee.ToString();
            ntd.AppendChild(xn);

            xn = doc.CreateElement("Account");
            xn.InnerText = _trader.AccountID;
            ntd.AppendChild(xn);

            xn = doc.CreateElement("Remark");
            xn.InnerText = tradeDetail.Remark;
            ntd.AppendChild(xn);

            root.AppendChild(ntd);
            doc.Save(tradeLogFile);
        }

        private void UpdateTradeLog(TradeDetail tradeDetail)
        {
            XmlDocument doc = new XmlDocument();
            string trade_day = tradeDetail.CreateTime.ToString("yyyyMMdd");
            string tradeLogFile = string.Format("{0}\\{3}_trade_log_{2}_{1}.xml", ConfigFileName.TradeLogDriectory, trade_day, _trader.AccountID, _trader.TA.Bta.market);
            doc.Load(tradeLogFile);
            XmlNode root = doc.SelectSingleNode("TradeHistory");
            XmlNodeList tradeDetails = root.SelectNodes("TradeDetail");
            if (!object.Equals(null, tradeDetails))
            {
                foreach (XmlNode td in tradeDetails)
                {
                    if (td.Attributes["Guid"].Value == tradeDetail.Tradeid.ToString())
                    {
                        td.SelectSingleNode("OHQty").InnerText = tradeDetail.TradeOHQty.ToString();

                        td.SelectSingleNode("GainPolicy").InnerText = tradeDetail.GainPolicy.ToString();

                        td.SelectSingleNode("GainActure").InnerText = tradeDetail.GainActure.ToString();

                        td.SelectSingleNode("OpenOrderType").InnerText = Enum.GetName(typeof(OpenType), tradeDetail.TradeOpenOrderType);

                        td.SelectSingleNode("OpenOrderTime").InnerText = tradeDetail.TradeOpenOrderTime;

                        td.SelectSingleNode("OpenOrderPrice").InnerText = tradeDetail.TradeOpenOrderPrice.ToString();

                        td.SelectSingleNode("OpenOrderQty").InnerText = tradeDetail.TradeOpenOrderQty.ToString();

                        td.SelectSingleNode("OpenDealPrice").InnerText = tradeDetail.TradeOpenDealPrice.ToString();

                        td.SelectSingleNode("OpenDealQty").InnerText = tradeDetail.TradeOpenDealQty.ToString();

                        td.SelectSingleNode("OpenFee").InnerText = tradeDetail.TradeOpenFee.ToString();

                        td.SelectSingleNode("CloseOrderType").InnerText = Enum.GetName(typeof(OpenType), tradeDetail.TradeCloseOrderType);

                        td.SelectSingleNode("CloseOrderTime").InnerText = tradeDetail.TradeCloseOrderTime;

                        td.SelectSingleNode("CloseOrderPrice").InnerText = tradeDetail.TradeCloseOrderPrice.ToString();

                        td.SelectSingleNode("CloseOrderQty").InnerText = tradeDetail.TradeCloseOrderQty.ToString();

                        td.SelectSingleNode("CloseDealPrice").InnerText = tradeDetail.TradeCloseDealPrice.ToString();

                        td.SelectSingleNode("CloseDealQty").InnerText = tradeDetail.TradeCloseDealQty.ToString();

                        td.SelectSingleNode("CloseFee").InnerText = tradeDetail.TradeCloseFee.ToString();

                        td.SelectSingleNode("Remark").InnerText = tradeDetail.Remark;

                        doc.Save(tradeLogFile);
                        return;
                    }
                }
            }
        }

        private void bt_disconnect_Click(object sender, EventArgs e)
        {
            if (_trader.TryDisConnect())
            {
                this.lb_status.ForeColor = Color.Red;
                this.lb_status.Text = "未连接";
                this.cbt_connect.Enabled = true;
                this.cbt_disconnect.Enabled = false;
            }
            else
            {
                this.lb_status.ForeColor = Color.Red;
                this.lb_status.Text = "未连接";
                this.cbt_connect.Enabled = true;
                this.cbt_disconnect.Enabled = false;
            }
        }

        private void frm_TradeMonitor_Shown(object sender, EventArgs e)
        {
            LoadTradeHistory();
            resetRealQuote();
        }


        delegate void ConnectTradeDelegate();
        public void ConnectTrade()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ConnectTradeDelegate(ConnectTrade));

            }
            else
            {
                this.cbt_connect.PerformClick();
            }
        }



        private void grid_celueX_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.cms_tarde.Show(this.grid_celueX, e.Location);
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.grid_celueX.Col == 5)
                {
                    if (this.grid_celueX.Rows[grid_celueX.Row][5].ToString().Trim() == "平仓")
                    {
                        Guid g = Guid.Parse(grid_celueX.Rows[grid_celueX.Row][0].ToString());
                        TradeDetail td = new TradeDetail();
                        if (this._trader.TradeHistory.getTradeDetail(g, ref td))
                        {
                            ((RunningPolicy)td.PolicyObject).ManualClose(td.Tradeid);
                        }
                    }
                }
            }
        }

        private void tsm_trade_save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl工作表(*.xls)|*.xls";
            saveFileDialog.FileName = string.Format("{1}-实盘结果-{0}", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), _trader.AccountID);
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "另存信息";
            saveFileDialog.ShowDialog();
            string filename = saveFileDialog.FileName;
            if (filename != string.Empty)
            {
                try
                {
                    this.grid_celueX.SaveExcel(filename, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                }
                catch
                {
                    MessageBox.Show("保存文件错误");
                }
            }
        }

        private void LoadTradeHistory()
        {
            XmlDocument doc = new XmlDocument();
            string trade_day = DateTime.Now.ToString("yyyyMMdd");
            string tradeLogFile = string.Format("{0}\\{3}_trade_log_{2}_{1}.xml", ConfigFileName.TradeLogDriectory, trade_day, _trader.AccountID, _trader.TA.Bta.market);
            if (!File.Exists(tradeLogFile))
            {
                StreamWriter sw = new StreamWriter(tradeLogFile, true, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<TradeHistory>");
                sw.WriteLine("</TradeHistory>");
                sw.Close();
            }
            else
            {
                doc.Load(tradeLogFile);
                XmlNode root = doc.SelectSingleNode("TradeHistory");
                XmlNodeList tradeDetails = root.SelectNodes("TradeDetail");

                try
                {
                    this.grid_celueX.Rows.RemoveRange(this.grid_celueX.Rows.Fixed, this.grid_celueX.Rows.Count - 1);

                }
                catch { }
                if (!object.Equals(null, tradeDetails))
                {
                    for (int j = tradeDetails.Count -1 ;j >= 0; j--)
                    {
                        XmlNode td = tradeDetails[j];
                        this.grid_celueX.Rows.Add();
                        int index = grid_celueX.Rows.Count - 1;
                        this.grid_celueX.Rows[index][0] = Guid.Parse(td.Attributes["Guid"].Value);
                        this.grid_celueX.Rows[index][1] = td.SelectSingleNode("PolicyName").InnerText;
                        this.grid_celueX.Rows[index][2] = td.SelectSingleNode("Code").InnerText;
                        this.grid_celueX.Rows[index][3] = td.SelectSingleNode("Code").InnerText;
                        this.grid_celueX.Rows[index][4] = td.SelectSingleNode("OHQty").InnerText;
                        this.grid_celueX.Rows[index][5] = string.Empty;
                        this.grid_celueX.Rows[index][6] = td.SelectSingleNode("GainPolicy").InnerText;
                        this.grid_celueX.Rows[index][7] = td.SelectSingleNode("GainActure").InnerText;
                        this.grid_celueX.Rows[index][8] = td.SelectSingleNode("OpenOrderTime").InnerText;
                        this.grid_celueX.Rows[index][9] = td.SelectSingleNode("OpenOrderPrice").InnerText;
                        this.grid_celueX.Rows[index][10] = td.SelectSingleNode("OpenOrderQty").InnerText;
                        this.grid_celueX.Rows[index][11] = td.SelectSingleNode("OpenOrderType").InnerText;
                        this.grid_celueX.Rows[index][12] = td.SelectSingleNode("OpenDealPrice").InnerText;
                        this.grid_celueX.Rows[index][13] = td.SelectSingleNode("OpenDealQty").InnerText;
                        this.grid_celueX.Rows[index][14] = td.SelectSingleNode("OpenFee").InnerText;
                        this.grid_celueX.Rows[index][15] = td.SelectSingleNode("CloseOrderTime").InnerText;
                        this.grid_celueX.Rows[index][16] = td.SelectSingleNode("CloseOrderPrice").InnerText;
                        this.grid_celueX.Rows[index][17] = td.SelectSingleNode("CloseOrderQty").InnerText;
                        this.grid_celueX.Rows[index][18] = td.SelectSingleNode("CloseOrderType").InnerText;
                        this.grid_celueX.Rows[index][19] = td.SelectSingleNode("CloseDealPrice").InnerText;
                        this.grid_celueX.Rows[index][20] = td.SelectSingleNode("CloseDealQty").InnerText;
                        this.grid_celueX.Rows[index][21] = td.SelectSingleNode("CloseFee").InnerText;
                        this.grid_celueX.Rows[index][23] = td.SelectSingleNode("Remark").InnerText;
                    }
                }
            }
            CalculateStatus();

        }
        private void tsm_load_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("载入时将清空目前数据，是否继续?", "载入", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK)
            {
                LoadTradeHistory();
            }
        }

        private void cbt_selall_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < this.grid_kecheX.Rows.Count; i++)
            {
                this.grid_kecheX.Rows[i][0] = true;
            }
        }

        private void tsm_empty_Click(object sender, EventArgs e)
        {
            try
            {
                this.grid_celueX.Rows.RemoveRange(this.grid_celueX.Rows.Fixed, this.grid_celueX.Rows.Count - 1);
            }
            catch { }
            this.lb_times.Text = string.Empty;
            this.lb_profit.Text = string.Empty;
        }

        private void cch_policy_CommandClick(object sender, C1.Win.C1Command.CommandClickEventArgs e)
        {
            if (e.Command.Name == "ccd_policy")
            {
                frm_RunPolicy frm = new frm_RunPolicy(_trader, false);
                frm.frmPolicyResult_Arrival += frmPolicyResultArrival;
                frm.frmFunPolicyResult_Arrival += frmFunPolicyResultArrival;
                frm.Show();
            }
            else if (e.Command.Name == "ccd_liandong")
            {
                MessageBox.Show("此功能未开放");
            }
        }

        public delegate void TradePolicyResultArrival(object sender, PolicyResultEventArgs args);
        public event TradePolicyResultArrival tradePolicyResult_Arrival;
        public void tradeRaiseResult(object sender, PolicyResultEventArgs args)
        {
            if (tradePolicyResult_Arrival != null)
            {
                tradePolicyResult_Arrival(sender, args);
            }
        }
        private void frmPolicyResultArrival(object sender, PolicyResultEventArgs args)
        {
            tradeRaiseResult(sender, args);
        }

        public delegate void TradeFunPolicyResultArrival(object sender, PolicyFunCGetEventArgs args);
        public event TradeFunPolicyResultArrival tradeFunPolicyResult_Arrival;
        public void tradeFunRaiseResult(object sender, PolicyFunCGetEventArgs args)
        {
            if (tradeFunPolicyResult_Arrival != null)
            {
                tradeFunPolicyResult_Arrival(sender, args);
            }
        }
        private void frmFunPolicyResultArrival(object sender, PolicyFunCGetEventArgs args)
        {
            tradeFunRaiseResult(sender, args);
        }

        private void tx_code_TextChanged(object sender, EventArgs e)
        {
            string code = this.tx_code.Text.Trim();
            if (code.Length > 0)
            {
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, _trader.TA.Bta.market);
                if (si != null)
                {
                    this.cb_ordertype.Items.Clear();
                    if (si.Type == "futures")
                    {
                        this.cb_ganggantype.Items.AddRange(new object[] { "10", "20" });
                    }
                    else
                    {
                        this.cb_ganggantype.Items.Clear();
                    }
                    if (si.Type == "spot")
                    {
                        this.cb_ordertype.Items.AddRange(new object[] { "买入", "卖出" });
                    }
                    else
                    {
                        this.cb_ordertype.Items.AddRange(new object[] { "开多", "开空", "平多", "平空" });
                    }
                    _trader.setQueryStock(si);
                    this.tx_name.Text = si.Name;
                }
                else
                {
                    resetRealQuote();
                }
            }
            else
            {
                _trader.clearQueryStock();
                resetRealQuote();
            }
        }



        private void resetRealQuote()
        {
            this.tx_name.Text = string.Empty;
            this.lb_calqty.Text = string.Empty;
            this.lb_dieting.Text = string.Empty;
            this.lb_zhangting.Text = string.Empty;
            this.lb_b1.Text = "-";
            this.lb_b2.Text = "-";
            this.lb_b3.Text = "-";
            this.lb_b4.Text = "-";
            this.lb_b5.Text = "-";
            this.lb_bs1.Text = "-";
            this.lb_a1.Text = "-";
            this.lb_as1.Text = "-";
            this.lb_bs2.Text = "-";
            this.lb_a2.Text = "-";
            this.lb_as2.Text = "-";
            this.lb_bs3.Text = "-";
            this.lb_a3.Text = "-";
            this.lb_as3.Text = "-";
            this.lb_bs4.Text = "-";
            this.lb_a4.Text = "-";
            this.lb_as4.Text = "-";
            this.lb_bs5.Text = "-";
            this.lb_a5.Text = "-";
            this.lb_as5.Text = "-";
            this.lb_last.Text = "-";
            this.lb_preclose.Text = "-";
            this.nu_price.Maximum = 100000;
            this.nu_price.Minimum = 0;
            this.nu_price.Value = 0;

        }

        private void cbt_manualTrade_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = !this.panel1.Visible;
            _trader.clearQueryStock();
            //this.stockAccount.TradeSi = new SecurityInfo();
        }

        private void cbt_xiadan_Click(object sender, EventArgs e)
        {
            string tradeCategoryindex = this.cb_ordertype.Text;
            if (tradeCategoryindex == "")
            {
                MessageBox.Show("请选择委托种类");
                return;
            }
            string tradeCategory = "0";
            switch (tradeCategoryindex)
            {
                case "开多":
                    tradeCategory = TradeSendOrderCategory.KaiDuo;
                    break;
                case "开空":
                    tradeCategory = TradeSendOrderCategory.KaiKong;
                    break;
                case "平多":
                    tradeCategory = TradeSendOrderCategory.PingDuo;
                    break;
                case "平空":
                    tradeCategory = TradeSendOrderCategory.PingKong;
                    break;
                case "买入":
                    tradeCategory = TradeSendOrderCategory.Buy;
                    break;
                case "卖出":
                    tradeCategory = TradeSendOrderCategory.Sell;
                    break;
            }
            string code = this.tx_code.Text.Trim();

            float price = (float)this.nu_price.Value;
            if (price == 0)
            {
                MessageBox.Show("请输入交易价格");
                return;
            }
            double qty = (double)this.nu_orderqty.Value;
            if (qty <= 0)
            {
                MessageBox.Show("请输入交易数量");
                return;
            }
            int priceTypeindex = this.cb_pricetype.SelectedIndex;

            if (priceTypeindex < 0)
            {
                MessageBox.Show("请选择报价方式");
                return;
            }
            string priceType;
            if (priceTypeindex == 0)
            {
                priceType = TradeSendOrderPriceType.Limit.ToString();
            }
            else
            {
                priceType = TradeSendOrderPriceType.Market.ToString();
            }
            string leverage = this.cb_ganggantype.Text;

            if (MessageBox.Show("确认下单", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.OK)
            {
                string weituobianhao = string.Empty;
                string errorinfo = string.Empty;
                SecurityInfo si = GlobalValue.GetFutureByCodeAndMarket(code, _trader.TA.Bta.market);
                bool maker = this.makerCheck.Checked;
                if (_trader.ManualXiaDan(tradeCategory, priceType, code, price, qty, ref weituobianhao, ref errorinfo, si.Type, maker, leverage))
                {
                    MessageBox.Show(string.Format("下单成功,委托编号{0}", weituobianhao));
                }
                else
                {
                    MessageBox.Show(string.Format("下单失败,错误{0}", errorinfo));
                }
            }
        }

        private void ccd_savechengjiao_Click(object sender, C1.Win.C1Command.ClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl工作表(*.xls)|*.xls";
            saveFileDialog.FileName = string.Format("{1}-成交记录-{0}", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), _trader.AccountID);
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "另存信息";
            saveFileDialog.ShowDialog();
            string filename = saveFileDialog.FileName;
            if (filename != string.Empty)
            {
                try
                {
                    this.grid_chenjiaoX.SaveExcel(filename, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                }
                catch
                {
                    MessageBox.Show("保存文件错误");
                }
            }
        }

        private void grid_chenjiaoX_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.ccm_chengjiao.ShowContextMenu(this.grid_chenjiaoX, e.Location);
            }
        }

        private void grid_weituoX_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.ccm_weituo.ShowContextMenu(this.grid_weituoX, e.Location);
            }
        }

        private void ccd_saveweituo_Click(object sender, C1.Win.C1Command.ClickEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl工作表(*.xls)|*.xls";
            saveFileDialog.FileName = string.Format("{1}-委托记录-{0}", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), _trader.AccountID);
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.Title = "另存信息";
            saveFileDialog.ShowDialog();
            string filename = saveFileDialog.FileName;
            if (filename != string.Empty)
            {
                try
                {
                    this.grid_weituoX.SaveExcel(filename, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                }
                catch
                {
                    MessageBox.Show("保存文件错误");
                }
            }
        }

    }
}
