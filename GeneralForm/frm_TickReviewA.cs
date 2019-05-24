using C1.Win.C1Ribbon;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GeneralForm
{
    public partial class frm_TickReviewA : C1RibbonForm
    {
        public static List<TickHistory> tickHistorys = new List<TickHistory>();
        TickHistory th;
        bool doubleClicked = false;
        bool dataget = false;
        int lastindex = 0;
        RiscPoints _riscPoints;
        SecurityInfo _si;
        public frm_TickReviewA()
        {
            InitializeComponent();
            GUITools.DoubleBuffer(this.chart_tick, true);
            GUITools.DoubleBuffer(this.uc_stockdetail, true);
            this.chart_15s.Top = 0;
            this.chart_15s.Left = 0;
            _riscPoints = new RiscPoints();
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            //调用方法，显示查到的数据
            ShowChart();
            
        }
        //添加一个方法
        private void ShowChart()
        {
            _riscPoints = new RiscPoints();
            dataget = false;
            this.lb_select.Text = string.Empty;
            this.chart_tick.Series.Clear();
            //清除缓存
            this.uc_stockdetail.Clear();
            //定义一个变量，保存
            string code = this.tx_code.Text.Trim();
            //定义一个当前时间
            DateTime datetime = this.dt_date.Value.Date;
            //判断股票代码是否为空
            if (code == string.Empty)
            {
                return;
            }
            List<SecurityInfo> lists = GlobalValue.GetFutureByCode(code);
            if (lists.Count == 0)
            {
                MessageBox.Show("找不到股票代码");
                return;
            }
            if (lists.Count == 1)
            {
                this._si = lists[0];
            }
            else
            {
                uc_StockSelect uc_select = new uc_StockSelect();
                uc_select.Top = this.tx_code.Top + this.tx_code.Height;
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
            ShowCharts(datetime, _si);
        }

        void uc_select_Leave(object sender, EventArgs e)
        {
            this.Controls.Remove((uc_StockSelect)sender);
        }

        void uc_select_Stock_Selected(object sender, SecurityInfo si)
        {
            this._si = si;
            this.Controls.Remove((uc_StockSelect)sender);
            this.tx_code.Focus();
        }

        private void ShowCharts(DateTime datetime, SecurityInfo si)
        {
            //bool hasHistory = false;
            //foreach (var th in tickHistorys)
            //{
            //    if (th.code == si.Code && th.datetime == datetime)
            //    {
            //        this.th = th;
            //        hasHistory = true;
            //        break;
            //    }
            //} 
            //StockDataAnalisys dataAnalisys = new StockDataAnalisys();
            //MarketTimeRange marketTimeRange = MarketTimeRange.getTimeRange(si.Market1);
            List<int> intevals = new List<int>();
            intevals.Add(15);
            LiveDataProcessor ldpa = new LiveDataProcessor(intevals,si);
            ldpa.OnLiveBarArrival += ldpa_OnLiveBarArrival;
            if (true)
            {

                DateTime startTime = datetime.Add(si.TimeRange.StartTime);
                DateTime endTime = datetime.Add(si.TimeRange.EndTime);
                DataTable dt = null;
                //DataTable dt = LocalSQL.QueryDataTable(string.Format("select * from ideal_tick_mstr where tick_code = '{0}' and tick_time >= '{1}' and tick_time <= '{2}' order by tick_time", si.Code, startTime, endTime));
                if (dt.Rows.Count == 0)
                {
                    //提示
                    MessageBox.Show("无历史纪录");
                    //返回
                    return;
                }
                else
                {
                    DataRow dr = dt.Rows[dt.Rows.Count - 1];
                    //传递数据
                    this.th = new TickHistory();
                    th.code = si.Code;
                    th.datetime = datetime;
                    th.lists = dt;
                    th.lastclose = System.Convert.ToDouble(dr["tick_last"]);
                    th.high = System.Convert.ToDouble(dr["tick_high"]);
                    th.low = System.Convert.ToDouble(dr["tick_low"]);
                     

                    if (Math.Abs(th.lastclose - th.high) > Math.Abs(th.lastclose - th.low))
                    {
                        th.low = th.lastclose - (th.high - th.lastclose);
                    }
                    else
                    {
                        th.high = th.lastclose + (th.lastclose - th.low);
                    }
                    th.ticks = new SortedDictionary<TimeSpan, TickData>();

                    foreach (DataRow row in dt.Rows)
                    {
                        TickData td = TickData.ConvertFromDataRow(row);
                        if (si.isLive(td.Time.TimeOfDay))// SecurityMarket.isLive(td.Time.TimeOfDay))
                        {
                            ldpa.ReceiveTick(td);
                            //dataAnalisys.ReceiveTick(td);
                        }
                        try
                        { 
                            th.ticks.Add(td.Time.TimeOfDay, td); }
                        catch { }
                    }
                    tickHistorys.Add(th);

                    //updateChartTickX(dt);
                }
            }
            this.uc_stockdetail.setStockName(si.Code, si.Name);
            UpdateChart(ldpa);
            
            Series seriesColumn = new Series();
            seriesColumn.Name = "PointSerial";
            //Series preCloseSeries = new Series();
            //Series[] h = new Series[7];
            //Series[] l = new Series[7];

            //for (int i = 0; i < 7; i++)
            //{
            //    h[i] = new Series();
            //    h[i].IsVisibleInLegend = false;
            //    h[i].ChartType = SeriesChartType.Line;
            //    h[i].Color = Color.Red;
            //    l[i] = new Series();
            //    l[i].IsVisibleInLegend = false;
            //    l[i].ChartType = SeriesChartType.Line;
            //    l[i].Color = Color.Red;
            //}

            //double[] hvalue = new double[7];
            //double[] lvalue = new double[7];

            //for (int i = 0; i < 6; i++)
            //{
            //    hvalue[i] = th.lastclose + (th.lastclose - th.low) / 7 * (i + 1);
            //    lvalue[i] = th.lastclose - (th.lastclose - th.low) / 7 * (i + 1);
            //}
            //hvalue[6] = th.high;
            //lvalue[6] = th.low;

            //不显示chart控件中的右上角提示的内容
            seriesColumn.IsVisibleInLegend = false;
            //preCloseSeries.IsVisibleInLegend = false;
            foreach (var x in th.ticks)
            {
                DataPoint dp = new DataPoint();
                dp.SetValueXY(x.Key.ToString(), x.Value.Last);
                //dp.SetValueY(x.Value.Last);
                
                //dp.Label = x.Key.ToString();
                seriesColumn.Points.Add(dp);
                //dp = new DataPoint();
                //dp.SetValueXY(x.Key.ToString(), x.Value.Preclose);
                //preCloseSeries.Points.Add(dp);
                //for(int i=0;i<7;i++)
                //{
                //    dp = new DataPoint();
                //    dp.SetValueXY(x.Key.ToString(), hvalue[i]);
                //    h[i].Points.Add(dp);
                //    dp = new DataPoint();
                //    dp.SetValueXY(x.Key.ToString(), lvalue[i]);
                //    l[i].Points.Add(dp);
                //}
            }
            //绘画一个折线图
            seriesColumn.ChartType = SeriesChartType.Line;
            //seriesColumn.Color = Color.White;
            //preCloseSeries.ChartType = SeriesChartType.Line;
            //preCloseSeries.Color = Color.Red;

            //preCloseSeries.BorderWidth = 2;
            //添加
            chart_tick.Series.Add(seriesColumn);
            //chart1.Series.Add(preCloseSeries);
            //for(int i=0;i<7;i++)
            //{
            //    chart1.Series.Add(h[i]);
            //    chart1.Series.Add(l[i]);
            //}
            System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
            //设置Y轴的最大值
            y.Maximum = th.high;
            //设置Y轴的最小值
            y.Minimum = th.low;
            //显示网格线
            y.MajorGrid.Enabled = true;
            //设置网格之前的间隔
            y.MajorGrid.Interval = th.lastclose - th.low;
            //设置网格线的颜色
            y.MajorGrid.LineColor = Color.Red;
            //设置网格线的宽度
            y.MajorGrid.LineWidth = 2;
            //显示网格线
            y.MinorGrid.Enabled = true;
            //把chart中划分为7个区域
            y.MinorGrid.Interval = (th.lastclose - th.low) / 7;
            //设置网格线的颜色
            y.MinorGrid.LineColor = Color.Red;
            y.MajorTickMark.Enabled = false;
            y.MinorGrid.LineWidth = 1;
            y.MinorGrid.Enabled = true;
            y.Interval = y.MinorGrid.Interval;
            y.LabelStyle.Format = "0.00";
            y.LabelStyle.Font = new Font("system", 11);
            System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
            y2.Maximum = Math.Round((th.high - th.lastclose) / th.lastclose, 4);
            y2.Minimum = -y2.Maximum;
            y2.Interval = y2.Maximum / 7;
            //不设置Y2轴的网格线属性
            y2.MajorGrid.Enabled = false;
            //不显示Y2轴的网格线特征
            y2.MinorGrid.Enabled = false;
            y2.LabelStyle.Format = "0.00%";
            y2.MajorTickMark.Enabled = false;
            y2.MinorTickMark.Enabled = false;
            y2.LabelStyle.Font = new Font("system", 11);
            chart_tick.ChartAreas[0].AxisY = y;
            chart_tick.ChartAreas[0].AxisY2 = y2;
            //显示Y轴的辅助线
            chart_tick.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            //不显示X轴的网格线属性
            chart_tick.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //不显示X轴的网格线特征
            chart_tick.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            //不设置X轴的边距设置
            chart_tick.ChartAreas[0].AxisX.IsMarginVisible = false;
            //chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            
            //chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart_tick.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart_tick.ChartAreas[0].CursorX.LineColor = Color.Blue;
            chart_tick.ChartAreas[0].CursorY.LineColor = Color.Blue;
            //chart1.ChartAreas[0].BackColor = Color.Black;
            
            //Series timeSeries = new Series();
            //TimeSpan start = SecurityMarket.MorningOpenTime;
            //while(start <= SecurityMarket.MorningCloseTime)
            //{
            //    DataPoint dp = new DataPoint();
            //    dp.SetValueXY(start.ToString(), 22);
            //    timeSeries.Points.Add(dp);
            //    start = start.Add(new TimeSpan(0, 1, 0));
            //}
            //timeSeries.ChartType = SeriesChartType.Line;
            //chart1.Series.Add(timeSeries);

            //chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            //chart1.ChartAreas[0].CursorY.Interval = 0.01;
            //chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            dataget = true;
        }

        void ldpa_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int intevals = ((LiveBars)sender).Inteval;
            if(intevals == 60)
            {
                _riscPoints.InsertBar(args.Bar);
            }
        }

        private void updateChartTickX(DataTable dt)
        {
            //this.c1chat_tick.DataSource = dt;
            //ChartDataSeriesCollection sc = this.c1chat_tick.ChartGroups[0].ChartData.SeriesList;
            //sc.RemoveAll();
            //ChartDataSeries s = sc.AddNewSeries();
            //s.Label = "Tick";
            //s.X.DataField = "tick_time";
            //s.Y.DataField = "tick_last";
            
            //c1chat_tick.ChartArea.AxisX.AnnoFormat = FormatEnum.DateShort;
            //c1chat_tick.ChartArea.AxisY.AnnoFormat = FormatEnum.NumericCurrency;
            //c1chat_tick.ChartArea.AxisY.Min = 20;
            //c1chat_tick.ChartArea.AxisX.Min = new TimeSpan(9, 30, 0);
            //c1chat_tick.ChartArea.AxisX.Max = new TimeSpan(15, 0, 0);
            //c1chat_tick.ChartGroups[0].ChartType = Chart2DTypeEnum.XYPlot;
            //c1chat_tick.ChartGroups[0].ShowOutline = false;
            //c1chat_tick.Legend.Compass = CompassEnum.North;
            //c1chat_tick.Legend.Orientation = LegendOrientationEnum.Horizontal;
            //c1chat_tick.Legend.Visible = true; c1chat_tick.Legend.Visible = true;
        }
        delegate void UpdateDelegate(LiveDataProcessor ldpa);
        internal void UpdateChart(LiveDataProcessor ldpa)
        {
            //画四条水平的线，
            //openLine.basePrice;
            //openLine.highPrice;
            //openLine.lowPrice;
            //openLine.volatilePrice;
            //[Database]
            //ConnectStr="Data Source=localhost;Initial Catalog=stockpolicy;Integrated Security=false;User ID=sa;Password=sa123$%^"

            //等待异步
            if (this.InvokeRequired)
            {
                //委托
                this.Invoke(new UpdateDelegate(UpdateChart), new object[] { ldpa });
            }
            else
            {
                LiveBars bar1M = ldpa.Bar1M;
                LiveBars bar15s = ldpa.Bar15S;
                #region 分时
                try
                {
                    //清除chart1中数据
                    this.chart_fengshi.Series.Clear();



                    //构建图表数据对象
                    //chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                    //chart1.ChartAreas[0].CursorY.IsUserEnabled = true; 
                    Series seriesColumn = new Series();
                    //十字线

                    //允许网格线存在
                    //chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                    //chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                    //chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                    //chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
                    //取消chart控件中右上角的标志
                    seriesColumn.IsVisibleInLegend = false;

                    //定义一个双精度浮点型最大值为0
                    double max = 0;
                    //定义一个双精度浮点型最小值为1000
                    double min = 10000;
                    double last = 0;
                    int lastindex = -1;
                    
                    List<TimeSpan> ts = new List<TimeSpan>(bar1M.Bars.Keys);
                    for (int i = ts.Count - 1; i >= 0; i--)
                    {
                        double close = bar1M.Bars[ts[i]].Close;
                        if (close != 0)
                        {
                            lastindex = i;
                            break;
                        }
                    }
                    if (lastindex == -1)
                        return;
                    int line = 0;
                    foreach (var bar in bar1M.Bars)
                    {

                        if (line >= lastindex)
                            break;
                        double close = bar.Value.Close;
                        if (close == 0 && last == 0)
                        {
                            close = ldpa.LastClose;
                        }
                        else if (close == 0 && last != 0)
                        {
                            close = last;
                        }
                        else
                        {
                            last = close;
                        }

                        string time = bar.Key.ToString().Substring(0, 5);
                        DataPoint dp = new DataPoint();
                        dp.SetValueXY(line, close);
                        dp.AxisLabel = time;
                        seriesColumn.Points.Add(dp);
                        if (max < close)
                            max = close;
                        if (min > close)
                            min = close;
                        line++;
                    }
                    //绘画出一个折线图
                    seriesColumn.ChartType = SeriesChartType.Line;
                    seriesColumn.Color = Color.Blue;
                    chart_fengshi.Series.Add(seriesColumn);
                    //获取或设置数据的名称
                    chart_fengshi.Series[0].Name = string.Empty;
                    //new一个y轴的方法
                    double lastclose = ldpa.LastClose;
                    //if (max < openLine.highPrice)
                    //{
                    //    max = openLine.highPrice;
                    //}
                    //if (min > sellLine.highPrice)
                    //{
                    //    min = sellLine.highPrice;
                    //}

                    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                    {
                        min = lastclose - (max - lastclose);
                    }
                    else
                    {
                        max = lastclose + (lastclose - min);
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y.Maximum = max;
                    y.Minimum = min;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - min;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - min) / 7;
                    y.MinorGrid.LineColor = Color.Red;
                    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";
                    //y轴上的值
                    chart_fengshi.ChartAreas[0].AxisY = y;


                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((max - lastclose) / lastclose, 4);
                    y2.Minimum = -y2.Maximum;
                    y2.Interval = y2.Maximum / 7;
                    y2.MajorGrid.Enabled = false;
                    y2.MinorGrid.Enabled = false;
                    y2.LabelStyle.Format = "0.00%";
                    y2.MajorTickMark.Enabled = false;
                    y2.MinorTickMark.Enabled = false;
                    y2.LineColor = Color.Red;
                    y2.LabelStyle.ForeColor = Color.Red;

                    chart_fengshi.ChartAreas[0].AxisY2 = y2;
                    chart_fengshi.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;

                    //new一个x轴的方法
                    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    //设置x的最小值为0
                    x.Minimum = 0;
                    //设置x的最大值为240
                    x.Maximum = bar1M.Bars.Count -1;
                    //表示取消x轴的网格线
                    x.IsMarginVisible = false;
                    x.LineColor = Color.Red;
                    x.MajorGrid.Interval = 120;
                    x.MajorGrid.LineColor = Color.Red;
                    x.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.LabelStyle.ForeColor = Color.Red;
                    x.MinorGrid.Interval = 30;
                    x.MinorGrid.LineColor = Color.Red;
                    x.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.Interval = x.MinorGrid.Interval;
                    x.MinorGrid.Enabled = true;

                    chart_fengshi.ChartAreas[0].AxisX = x;

                    //显示出8条直线（辅助线）
                    //Series se = new Series();

                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线

                    //se.Points.AddXY(0, openLine.basePrice);
                    //se.Points.AddXY(239, openLine.basePrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart_fengshi.Series.Add(se);
                    //se = new Series();
                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线
                    //se.Points.AddXY(0, openLine.highPrice);
                    //se.Points.AddXY(239, openLine.highPrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart_fengshi.Series.Add(se);
                    //se = new Series();
                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线
                    //se.Points.AddXY(0, openLine.lowPrice);
                    //se.Points.AddXY(239, openLine.lowPrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart_fengshi.Series.Add(se);
                    ////se = new Series();
                    //////设置图形的类型为折线图
                    ////se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{ 
                    ////    //添加一条辅助线
                    ////    se.Points.AddY(openLine.volatilePrice);
                    ////}
                    //////取消chart控件中右上角的标志
                    ////se.IsVisibleInLegend = false;
                    //////添加
                    ////chart1.Series.Add(se);
                    //se = new Series();
                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线
                    //se.Points.AddXY(0, sellLine.basePrice);
                    //se.Points.AddXY(239, sellLine.basePrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart_fengshi.Series.Add(se);
                    //se = new Series();
                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线
                    //se.Points.AddXY(0, sellLine.highPrice);
                    //se.Points.AddXY(239, sellLine.highPrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    //chart_fengshi.Series.Add(se);
                    ////添加
                    //se = new Series();
                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    ////for (int i = 0; i < 240; i++)
                    ////{
                    ////添加一条辅助线
                    //se.Points.AddXY(0, sellLine.lowPrice);
                    //se.Points.AddXY(239, sellLine.lowPrice);
                    ////}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart_fengshi.Series.Add(se);
                    //se = new Series();

                    ////设置图形的类型为折线图
                    //se.ChartType = SeriesChartType.Line;
                    //for (int i = 0; i < 240; i++)
                    //{
                    //    //添加一条辅助线
                    //    se.Points.AddY(sellLine.volatilePrice);
                    //}
                    ////取消chart控件中右上角的标志
                    //se.IsVisibleInLegend = false;
                    ////添加
                    //chart1.Series.Add(se);  


                    //Series Myseries = new Series();
                    ////设置图形为散点图
                    //Myseries.ChartType = SeriesChartType.Point;
                    ////设置散点图的颜色
                    //Myseries.Color = Color.Green;
                    ////设置不显示右上角的提示
                    //Myseries.IsVisibleInLegend = false;
                    ////设置散点图的数据点的样式
                    //Myseries.MarkerStyle = MarkerStyle.Cross;
                    ////遍历出openpoints中元素的个数
                    //for (int i = 0; i < openPoints.Count; i++)
                    //{
                    //    DataPoint datapoint = new DataPoint();
                    //    //设置标记的大小
                    //    datapoint.MarkerSize = 10;
                    //    //提示信息（买卖的类型（buy，sell）和价格）
                    //    datapoint.ToolTip = string.Format("{0}-{1}", Enum.GetName(typeof(OpenType), openPoints[i].OpenType), openPoints[i].OpenPrice);
                    //    //DateTime a = openPoints[i].OpenTime;
                    //    //TimeSpan t = a.TimeOfDay;
                    //    double t1 = (openPoints[i].OpenTime.TimeOfDay - SecurityMarket.MorningOpenTime).TotalMinutes - 1;
                    //    if (t1 > 119)
                    //    {
                    //        t1 = t1 - 90;
                    //    }
                    //    //double dou = openPoints[i].OpenPrice;
                    //    datapoint.SetValueXY(t1, openPoints[i].OpenPrice);
                    //    Myseries.Points.Add(datapoint);
                    //}
                    //this.chart_fengshi.Series.Add(Myseries);
                    //背景颜色
                    chart_fengshi.ChartAreas[0].BackColor = Color.White;
                    //Y轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorY.Interval = 0.001;
                    //X轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorX.Interval = 0.001;
                }
                catch { }
                #endregion
                #region 15秒K
                try
                {
                    this.chart_15s.Series.Clear();
                    Series SeriesColumn = new Series("Bar");
                    SeriesColumn.IsVisibleInLegend = false;
                    chart_15s.Series.Add(SeriesColumn);
                    int line = 0;
                    double max = 0;
                    double min = 10000;
                    
                    List<TimeSpan> ts = new List<TimeSpan>(bar15s.Bars.Keys);
                    SeriesColumn.ChartType = SeriesChartType.Candlestick;
                    SeriesColumn["OpenCloseStyle"] = "Triangle";
                    SeriesColumn["ShowOpenClose"] = "Both";
                    SeriesColumn["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    SeriesColumn["PriceUpColor"] = "Red";
                    SeriesColumn["PriceDownColor"] = "Green";

                    Series sma5 = new Series("Ma5");
                    chart_15s.Series.Add(sma5);
                    sma5.ChartType = SeriesChartType.Spline;
                    sma5.Color = Color.Black;
                    Series sma20 = new Series("Ma20");
                    chart_15s.Series.Add(sma20);
                    sma20.ChartType = SeriesChartType.Spline;
                    sma20.Color = Color.OrangeRed;

                    foreach(var bar in bar15s.Bars)
                    {
                        if (bar.Value.Open != 0)
                        {
                            string time = bar.Key.ToString();
                            SeriesColumn.Points.AddXY(line, bar.Value.High);
                            SeriesColumn.Points[line].YValues[1] = bar.Value.Low;
                            SeriesColumn.Points[line].YValues[2] = bar.Value.Open;
                            SeriesColumn.Points[line].YValues[3] = bar.Value.Close;
                            if (bar.Value.Close > bar.Value.Open)
                            {
                                SeriesColumn.Points[line].Color = Color.Red;
                            }
                            else if (bar.Value.Close < bar.Value.Open)
                            {
                                SeriesColumn.Points[line].Color = Color.Green;//.Blue;//.FromName("#54FFFF");
                            }
                            else
                            {
                                SeriesColumn.Points[line].Color = Color.Black;
                            }
                            SeriesColumn.Points[line].AxisLabel = time;
                            //SeriesColumn.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\nMA2:{5}\nMA3:{6}\nMA5:{7}\nLMA2{8}\nLMA3:{9}\nLMA5:{10}", time, bar.Value.High, bar.Value.Open, bar.Value.Low, bar.Value.Close, bar.Value.MA.MA2, bar.Value.MA.MA3, bar.Value.MA.MA5, bar.Value.LastMA.MA2, bar.Value.LastMA.MA3, bar.Value.LastMA.MA5);
                            SeriesColumn.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\nMA5:{5}\nMA20:{6}", time, bar.Value.High, bar.Value.Open, bar.Value.Low, bar.Value.Close, bar.Value.MA.MA5,bar.Value.MA.MA20);

                            if (bar.Value.High > max)
                            {
                                max = bar.Value.High;
                            }
                            if (bar.Value.Low < min)
                            {
                                min = bar.Value.Low;
                            }

                            if (line > 5)
                            {
                                sma5.Points.AddXY(line, bar.Value.MA.MA5);
                            }
                            if (line > 20)
                            {
                                sma20.Points.AddXY(line, bar.Value.MA.MA20);

                            }
                            line++;
                        }
                        

                    }

                    double lastclose = ldpa.LastClose;
                    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                    {
                        min = lastclose - (max - lastclose);
                    }
                    else
                    {
                        max = lastclose + (lastclose - min);
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    x.Minimum = 0;

                    x.Maximum = bar15s.Bars.Count -1;
                    x.IsMarginVisible = false;
                    x.LineColor = Color.Red;
                    x.MajorGrid.Interval = 120;
                    x.MajorGrid.LineColor = Color.Red;
                    x.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.LabelStyle.ForeColor = Color.Red;
                    x.MinorGrid.Interval = 30;
                    x.MinorGrid.LineColor = Color.Red;
                    x.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.Interval = x.MinorGrid.Interval;
                    x.MinorGrid.Enabled = true;

                    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y.Maximum = max;
                    y.Minimum = min;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - min;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - min) / 7;
                    y.MinorGrid.LineColor = Color.Red;
                    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";

                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((max - lastclose) / lastclose, 4);
                    y2.Minimum = -y2.Maximum;
                    y2.Interval = y2.Maximum / 7;
                    y2.MajorGrid.Enabled = false;
                    y2.MinorGrid.Enabled = false;
                    y2.LabelStyle.Format = "0.00%";
                    y2.MajorTickMark.Enabled = false;
                    y2.MinorTickMark.Enabled = false;
                    y2.LineColor = Color.Red;
                    y2.LabelStyle.ForeColor = Color.Red;
                    chart_15s.ChartAreas[0].AxisY = y;
                    chart_15s.ChartAreas[0].AxisX = x;
                    chart_15s.ChartAreas[0].AxisY2 = y2;
                    chart_15s.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                    chart_15s.ChartAreas[0].BackColor = Color.White;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                #endregion
                #region 1分钟K
                try
                {
                    this.chart1.Series.Clear();
                    Series SeriesColumn = new Series("Bar");
                    SeriesColumn.IsVisibleInLegend = false;
                    chart1.Series.Add(SeriesColumn);
                    double max = 0;
                    double min = 10000;
                    int lastindex = -1;
                    
                    List<TimeSpan> ts = new List<TimeSpan>(bar1M.Bars.Keys);
                    for (int i = ts.Count - 1; i >= 0; i--)
                    {
                        double close = bar1M.Bars[ts[i]].Close;
                        if (close != 0)
                        {
                            lastindex = i;
                            break;
                        }
                    }

                    Series xpoint = new Series("XPoint");
                    chart1.Series.Add(xpoint);
                    xpoint.ChartType = SeriesChartType.Line;
                    xpoint.Color = Color.Black;
                    foreach(var xp in _riscPoints.RiscPointList)
                    {
                        int xline = _si.calculateMinutes(xp.Time);
                        xpoint.Points.AddXY(xline, xp.Price);
                    }

                    if (lastindex == -1)
                        return;
                    int line = 0;
                    double last = 0;
                    SeriesColumn.ChartType = SeriesChartType.Candlestick;
                    SeriesColumn["OpenCloseStyle"] = "Triangle";
                    SeriesColumn["ShowOpenClose"] = "Both";
                    SeriesColumn["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    SeriesColumn["PriceUpColor"] = "Red";
                    SeriesColumn["PriceDownColor"] = "Green";
                    foreach (var bar in bar1M.Bars)
                    {
                        if (line > lastindex)
                            break;

                        double close = bar.Value.Close;
                        double high = 0;
                        double open = 0;
                        double low = 0;
                        if (close == 0 && last == 0)
                        {
                            close = ldpa.LastClose;
                            high = close;
                            open = close;
                            low = close;
                        }
                        else if (close == 0 && last != 0)
                        {
                            close = last;
                            high = last;
                            open = last;
                            low = last;
                        }
                        else
                        {
                            close = bar.Value.Close;
                            open = bar.Value.Open;
                            high = bar.Value.High;
                            low = bar.Value.Low;
                            last = close;
                        }
                        if (high > max)
                            max = high;
                        if (low < min)
                            min = low;
                        string time = bar.Key.ToString().Substring(0, 5);
                        SeriesColumn.Points.AddXY(line, high);
                        SeriesColumn.Points[line].YValues[1] = low;
                        SeriesColumn.Points[line].YValues[2] = open;
                        SeriesColumn.Points[line].YValues[3] = close;
                        if (close > open)
                        {
                            SeriesColumn.Points[line].Color = Color.Red;
                        }
                        else if (close < open)
                        {
                            SeriesColumn.Points[line].Color = Color.Green;//.Blue;//.FromName("#54FFFF");
                        }
                        else
                        {
                            SeriesColumn.Points[line].Color = Color.Black;
                        }
                        SeriesColumn.Points[line].AxisLabel = time;
                        SeriesColumn.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\n量:{5}\n均5:{6}\n均10{7}\n类{8}", time, high, open, low, close, bar.Value.Volumn, "Ev5", "Ev10", Enum.GetName(typeof(BarType), bar.Value.RelativeRaiseType));


                        line++;


                    }

                    double lastclose = ldpa.LastClose;
                    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                    {
                        min = lastclose - (max - lastclose);
                    }
                    else
                    {
                        max = lastclose + (lastclose - min);
                    }

                    //min = Math.Round((lastclose - min) / 7, 2) * 7;

                    //max = lastclose - min + lastclose;


                    //Series seriesDropToPrice = new Series();

                    //seriesDropToPrice.ChartType = SeriesChartType.Line;
                    //DataPoint dp = new DataPoint();
                    //dp.SetValueXY(0, dropToPrice);
                    //seriesDropToPrice.Points.Add(dp);
                    //dp = new DataPoint();
                    //dp.SetValueXY(240, dropToPrice);
                    //seriesDropToPrice.Points.Add(dp);
                    //seriesDropToPrice.IsVisibleInLegend = false;
                    //this.chart1.Series.Add(seriesDropToPrice);


                    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    x.Minimum = 0;

                    x.Maximum = bar1M.Bars.Count -1;
                    x.IsMarginVisible = false;
                    x.LineColor = Color.Red;
                    x.MajorGrid.Interval = 120;
                    x.MajorGrid.LineColor = Color.Red;
                    x.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.LabelStyle.ForeColor = Color.Red;
                    x.MinorGrid.Interval = 30;
                    x.MinorGrid.LineColor = Color.Red;
                    x.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    x.Interval = x.MinorGrid.Interval;
                    x.MinorGrid.Enabled = true;

                    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y.Maximum = max;
                    y.Minimum = min;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - min;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - min) / 7;
                    y.MinorGrid.LineColor = Color.Red;
                    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";

                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((max - lastclose) / lastclose, 4);
                    y2.Minimum = -y2.Maximum;
                    y2.Interval = y2.Maximum / 7;
                    y2.MajorGrid.Enabled = false;
                    y2.MinorGrid.Enabled = false;
                    y2.LabelStyle.Format = "0.00%";
                    y2.MajorTickMark.Enabled = false;
                    y2.MinorTickMark.Enabled = false;
                    y2.LineColor = Color.Red;
                    y2.LabelStyle.ForeColor = Color.Red;
                    chart1.ChartAreas[0].AxisY = y;
                    chart1.ChartAreas[0].AxisX = x;
                    chart1.ChartAreas[0].AxisY2 = y2;
                    chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                    chart1.ChartAreas[0].BackColor = Color.White;

                    // Series seriesDropToPrice = new Series();
                    //seriesDropToPrice.ChartType = SeriesChartType.Line;
                    //DataPoint dp = new DataPoint();
                    //dp.SetValueXY(0, dropToPrice);
                    //seriesDropToPrice.Points.Add(dp);
                    //dp = new DataPoint();
                    //dp.SetValueXY(240, dropToPrice);
                    //seriesDropToPrice.Points.Add(dp);
                    //seriesDropToPrice.IsVisibleInLegend = false;
                    //this.chart1.Series.Add(seriesDropToPrice);
                    //Series Myseries = new Series();
                    ////设置图形为散点图
                    //Myseries.ChartType = SeriesChartType.Point;
                    ////设置散点图的颜色
                    //Myseries.Color = Color.Green;
                    ////设置不显示右上角的提示
                    //Myseries.IsVisibleInLegend = false;
                    ////设置散点图的数据点的样式
                    //Myseries.MarkerStyle = MarkerStyle.Cross;

                    ////遍历出openpoints中元素的个数
                    //for (int i = 0; i < openPoints.Count; i++)
                    //{
                    //    DataPoint datapoint = new DataPoint();
                    //    //设置标记的大小
                    //    datapoint.MarkerSize = 10;
                    //    //提示信息（买卖的类型（buy，sell）和价格）
                    //    datapoint.ToolTip = string.Format("{0}-{1}", Enum.GetName(typeof(OpenType), openPoints[i].OpenType), openPoints[i].OpenPrice);
                    //    //DateTime a = openPoints[i].OpenTime;
                    //    //TimeSpan t = a.TimeOfDay;
                    //    double t1 = (openPoints[i].OpenTime.TimeOfDay - SecurityMarket.MorningOpenTime).TotalMinutes;
                    //    if (t1 > 119)
                    //    {
                    //        t1 = t1 - 90;
                    //    }
                    //    //double dou = openPoints[i].OpenPrice;
                    //    datapoint.SetValueXY(t1, openPoints[i].OpenPrice);
                    //    Myseries.Points.Add(datapoint);
                    //}
                    //this.chart1.Series.Add(Myseries);
                }
                catch //(Exception ex)
                {
                    // MessageBox.Show(ex.ToString());
                }
                #endregion
            }
        }

        private void chart_fenshi_MouseMove(object sender, MouseEventArgs e)
        {

            int _currentPointX = e.X;
            int _currentPointY = e.Y;
            chart_fengshi.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(_currentPointX, _currentPointY), true);
            chart_fengshi.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(_currentPointX, _currentPointY), true);

        }

        private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
        {

        }
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            ShowHideCursor(e);
        }
        delegate void ShowHideCursorDelegate(MouseEventArgs e);
        private void ShowHideCursor(MouseEventArgs e)
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(new ShowHideCursorDelegate(ShowHideCursor), new object[] { e });
            }
            else
            {
                if (doubleClicked)
                {

                    this.SuspendLayout();
                    int CursorX = e.X;
                    int CursorY = e.Y;
                    chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                    chart1.ChartAreas[0].CursorY.Interval = 0.001;
                    chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                    chart1.ChartAreas[0].CursorX.Interval = 0.001;
                    chart1.ChartAreas[0].CursorX.LineColor = Color.Red;
                    chart1.ChartAreas[0].CursorY.LineColor = Color.Red;
                    this.ResumeLayout();
                }
            }
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            doubleClicked = !doubleClicked;
            if (!doubleClicked)
            {
                chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(0, 0), true);
                chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(0, 0), true);
            }
        }
        private void frm_TickReview_Shown(object sender, EventArgs e)
        {
            //获取裆前时间
            this.dt_date.Value = System.DateTime.Now.Date;
            //清除chart控件中的数据元素
            this.chart_tick.Series.Clear();
        }
        //tx_code按键事件，
        private void tx_code_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                //调用方法查找到code股票代码
                ShowChart();
            }
        }
        //鼠标放在chart中，会出现一个十字线
        private void chart_Tick_MouseMove(object sender, MouseEventArgs e)
        {
            if (doubleClicked&&dataget)
            {
                    this.SuspendLayout();
                int CursorX = e.X;
                //自定义
                int CursorY = e.Y;
                //显示X轴的线
                chart_tick.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                //与Y轴的间隔
                chart_tick.ChartAreas[0].CursorY.Interval = 0.01;
                //显示Y轴的线
                chart_tick.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                //获取X轴光标的位置
                int index = System.Convert.ToInt32(chart_tick.ChartAreas[0].CursorX.Position);
                    lastindex = index;
                
                    DataPoint dp = this.chart_tick.Series["PointSerial"].Points[index - 1];
                TimeSpan ts = TimeSpan.Parse(dp.AxisLabel);
                TickData td = th.ticks[ts];
                    //chart1.ChartAreas[0].AxisX.CustomLabels.Add(new CustomLabel(index, index, ts.ToString(), 0, LabelMarkStyle.Box));
                this.uc_stockdetail.Update(td);
                    this.lb_select.Text = string.Format("价格:{0},时间:{1},量:{2}",td.Last.ToString("0.00"),ts.ToString(),td.Volume);
                    this.ResumeLayout();
            }
            
        }
        //双击左键取消十字线
        private void chart_Tick_DoubleClick(object sender, EventArgs e)
        {
            doubleClicked = !doubleClicked;
            if(!doubleClicked)
            {
                chart_tick.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(0, 0), true);
                chart_tick.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(0, 0), true);
            }
        }
        
        private void frm_TickReview_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if(doubleClicked && dataget)
                {
                    if(e.KeyCode == Keys.Left)
                    {
                        if(lastindex >= 2)
                        {
                            lastindex--;
                            chart_tick.ChartAreas[0].CursorX.Position = lastindex;
                        }

                    }else if(e.KeyCode == Keys.Right)
                    {
                        if(lastindex < this.chart_tick.Series["PointSerial"].Points.Count)
                            lastindex++;
                        chart_tick.ChartAreas[0].CursorX.Position = lastindex;
                    }
                    DataPoint dp = this.chart_tick.Series["PointSerial"].Points[lastindex - 1];
                    TimeSpan ts = TimeSpan.Parse(dp.AxisLabel);
                    TickData td = th.ticks[ts];
                    this.uc_stockdetail.Update(td);
                    this.lb_select.Text = string.Format("价格:{0},时间:{1},量:{2}", td.Last.ToString("0.00"), ts.ToString(),td.Volume);
                }
            }
        }

        private void chart_tick_Click(object sender, EventArgs e)
        {
            this.tx_code.Focus();
        }

        private void panel5_Resize(object sender, EventArgs e)
        {
            this.chart_15s.Height = panel5.Height;
            this.chart_15s.Width = panel5.Width * 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.lb_select.Text = string.Empty;
            //this.chart_tick.Series.Clear();
            //清除缓存
            this.uc_stockdetail.Clear();
            //定义一个变量，保存
            string code = this.tx_code.Text.Trim();
            //定义一个当前时间
            DateTime datetime = this.dt_date.Value.Date;
            //判断股票代码是否为空
            if (code == string.Empty)
            {
                return;
            }
            List<SecurityInfo> lists = GlobalValue.GetFutureByCode(code);
            if (lists.Count == 0)
            {
                MessageBox.Show("找不到股票代码");
                return;
            }
            if (lists.Count == 1)
            {
                this._si = lists[0];
            }
            else
            {
                uc_StockSelect uc_select = new uc_StockSelect();
                uc_select.Top = this.tx_code.Top + this.tx_code.Height;
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
            MessageBox.Show(_si.calculateSeconds(new TimeSpan(13, 35, 05)).ToString());
        }

        private void uc_stockdetail_Load(object sender, EventArgs e)
        {

        }
        
    }
    //创建一个公共的类
    
}
