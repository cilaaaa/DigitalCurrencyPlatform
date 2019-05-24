using C1.Win.C1Ribbon;
using DataBase;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GeneralForm
{
    public partial class frm_kReview : C1RibbonForm
    {
        public static List<TickHistory> tickHistorys = new List<TickHistory>();
        Dictionary<string,TickData> lt;
        bool doubleClicked = false;
        bool dataget = false;
        int lastindex = 0;
        RiscPoints _riscPoints;
        SecurityInfo _si;
        DateTime DataSignTime;
        System.Windows.Forms.Timer chartTimer = new System.Windows.Forms.Timer();
        int DayPointX;
        int DayPointY;
        
        public frm_kReview()
        {
            InitializeComponent();
            GUITools.DoubleBuffer(this.chart1, true);
            GUITools.DoubleBuffer(this.uc_stockdetail, true);
            this.chart_day.Top = 0;
            this.chart_day.Left = 0;
            _riscPoints = new RiscPoints();
        }

        //private void StartGetLastData()
        //{
        //    _thread_GetLastData = new Thread(new ThreadStart(GetLastData));
        //    _thread_GetLastData.Start();
        //}

        void UpdateLastChart(LiveDataProcessor ldpa)
        {
            LiveBars bar1M = ldpa.Bar1M;
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

            }
            catch { }
            
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
            }
            catch { }
            #endregion
            #region 5分钟K
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
            }
            catch { }
            #endregion
        }
        //添加一个方法
        private void ShowChart()
        {
            _riscPoints = new RiscPoints();
            dataget = false;
            this.lb_select.Text = string.Empty;
            this.chart1.Series.Clear();
            //清除缓存
            this.uc_stockdetail.Clear();
            //定义一个变量，保存
            string code = this.textBox1.Text.Trim();
            DataSignTime = DateTime.Now.AddHours(-8);
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
                uc_select.Top = this.textBox1.Top + this.textBox1.Height;
                uc_select.Left = this.textBox1.Left;
                this.Controls.Add(uc_select);
                uc_select.BringToFront();
                uc_select.DataSource = lists;
                uc_select.DataBind();
                uc_select.Show();
                uc_select.Focus();
                uc_select.Stock_Selected += uc_select_Stock_Selected;
                uc_select.Leave += uc_select_Leave;
            }
            ShowCharts(_si);
        }

        void uc_select_Leave(object sender, EventArgs e)
        {
            this.Controls.Remove((uc_StockSelect)sender);
        }

        void uc_select_Stock_Selected(object sender, SecurityInfo si)
        {
            this._si = si;
            this.Controls.Remove((uc_StockSelect)sender);
            this.textBox1.Focus();
        }

        private void ShowCharts(SecurityInfo si)
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
            
            DateTime startTime = System.Convert.ToDateTime(this.start_date.Text);
            DateTime endTime = System.Convert.ToDateTime(this.end_date.Text);
            lt = new Dictionary<string, TickData>();
            List<LiveDataProcessor> lldp = new List<LiveDataProcessor>();
            while (startTime <= endTime)
            {
                DataTable tickdatas = CSVFileHelper.OpenCSV(ConfigFileName.HistoryDataFileName + "\\" + startTime.ToString("yyyyMMdd") + ".csv");
                if (tickdatas.Rows.Count > 0)
                {
                    List<int> intevals = new List<int>();
                    intevals.Add(300);
                    LiveDataProcessor ldpa = new LiveDataProcessor(intevals, si);
                    ldpa.OnLiveBarArrival += ldpa_OnLiveBarArrival;
                    lldp.Add(ldpa);
                    for (int i = 0; i < tickdatas.Rows.Count; i++)
                    {
                        DataRow dr = tickdatas.Rows[i];
                        DateTime tickTime = System.Convert.ToDateTime(dr["timestamp"].ToString());
                        if (dr["symbol"].ToString() != si.Code)
                        {
                            continue;
                        }
                        //TickData tickdata = TickData.ConvertFromDataRow(dr);

                        TickData tickdata = new TickData();
                        tickdata.Code = dr["symbol"].ToString();
                        tickdata.SecInfo = si;
                        tickdata.Time = tickTime;
                        tickdata.Preclose = 0;
                        tickdata.Open = 0;
                        tickdata.High = 0;
                        tickdata.Low = 0;
                        tickdata.Ask = System.Convert.ToDouble(dr["askPrice"]);
                        tickdata.Bid = System.Convert.ToDouble(dr["bidPrice"]);
                        tickdata.Last = Math.Floor((tickdata.Ask + tickdata.Bid) / 2 / si.PriceJingDu) * si.PriceJingDu;
                        tickdata.Volume = 0;
                        tickdata.Amt = 0;
                        tickdata.IsReal = false;
                        for (int j = 0; j < 10; j++)
                        {
                            tickdata.Asks[j] = tickdata.Ask;
                            tickdata.Bids[j] = tickdata.Bid;
                            tickdata.Asksizes[j] = System.Convert.ToDouble(dr["askSize"]);
                            tickdata.Bidsizes[j] = System.Convert.ToDouble(dr["bidSize"]);
                        }
                        
                        
                        if (!lt.ContainsKey(tickdata.Time.ToString()))
                        {
                            lt.Add(tickdata.Time.ToString(), tickdata);
                        }
                        ldpa.ReceiveTick(tickdata);
                    }
                }
                startTime = startTime.AddDays(1);
            }
            if (lt.Count == 0)
            {
                //提示
                MessageBox.Show("无历史纪录");
                //返回
                return;
            }
            else
            {
                this.uc_stockdetail.setStockName(si.Code, si.Name);
                UpdateChart(lldp);
                Series seriesColumn = new Series();
                seriesColumn.Name = "PointSerial";
                //不显示chart控件中的右上角提示的内容
                seriesColumn.IsVisibleInLegend = false;
                //preCloseSeries.IsVisibleInLegend = false;
                foreach (var x in lt)
                {
                    DataPoint dp = new DataPoint();
                    dp.SetValueXY(x.Key.ToString(), x.Value.Last.ToString());
                    seriesColumn.Points.Add(dp);
                }
            }
        }

        void ldpa_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int intevals = ((LiveBars)sender).Inteval;
            if (intevals == 60)
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
        delegate void UpdateDelegate(List<LiveDataProcessor> lldp);
        internal void UpdateChart(List<LiveDataProcessor> lldp)
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
                this.Invoke(new UpdateDelegate(UpdateChart), new object[] { lldp });
            }
            else
            {
                
                #region 分时
                //try
                //{
                //    //清除chart1中数据
                //    this.chart_fengshi.Series.Clear();



                //    //构建图表数据对象
                //    //chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                //    //chart1.ChartAreas[0].CursorY.IsUserEnabled = true; 
                //    Series seriesColumn = new Series();
                //    //十字线

                //    //允许网格线存在
                //    //chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                //    //chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                //    //chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
                //    //chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = true;
                //    //取消chart控件中右上角的标志
                //    seriesColumn.IsVisibleInLegend = false;

                //    //定义一个双精度浮点型最大值为0
                //    double max = 0;
                //    //定义一个双精度浮点型最小值为1000
                //    double min = 10000;
                //    double last = 0;
                //    int lastindex = -1;

                //    List<TimeSpan> ts = new List<TimeSpan>(bar1M.Bars.Keys);
                //    for (int i = ts.Count - 1; i >= 0; i--)
                //    {
                //        double close = bar1M.Bars[ts[i]].Close;
                //        if (close != 0)
                //        {
                //            lastindex = i;
                //            break;
                //        }
                //    }
                //    if (lastindex == -1)
                //        return;
                //    int line = 0;
                //    foreach (var bar in bar1M.Bars)
                //    {

                //        if (line >= lastindex)
                //            break;
                //        double close = bar.Value.Close;
                //        if (close == 0 && last == 0)
                //        {
                //            close = ldpa.LastClose;
                //        }
                //        else if (close == 0 && last != 0)
                //        {
                //            close = last;
                //        }
                //        else
                //        {
                //            last = close;
                //        }

                //        string time = bar.Key.ToString().Substring(0, 5);
                //        DataPoint dp = new DataPoint();
                //        dp.SetValueXY(line, close);
                //        dp.AxisLabel = time;
                //        seriesColumn.Points.Add(dp);
                //        if (max < close)
                //            max = close;
                //        if (min > close)
                //            min = close;
                //        line++;
                //    }
                //    //绘画出一个折线图
                //    seriesColumn.ChartType = SeriesChartType.Line;
                //    seriesColumn.Color = Color.Blue;
                //    chart_fengshi.Series.Add(seriesColumn);
                //    //获取或设置数据的名称
                //    chart_fengshi.Series[0].Name = string.Empty;
                //    //new一个y轴的方法
                //    double lastclose = ldpa.Bar1M.Bars.First().Value.Close;
                //    //if (max < openLine.highPrice)
                //    //{
                //    //    max = openLine.highPrice;
                //    //}
                //    //if (min > sellLine.highPrice)
                //    //{
                //    //    min = sellLine.highPrice;
                //    //}

                //    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                //    {
                //        min = lastclose - (max - lastclose)- 500;
                //        max += 500;
                //    }
                //    else
                //    {
                //        max = lastclose + (lastclose - min) + 500;
                //        min -= 500;
                //    }

                //    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    y.Maximum = max;
                //    y.Minimum = min;
                //    y.LineColor = Color.Red;
                //    y.MajorGrid.LineColor = Color.Red;
                //    y.MajorGrid.Interval = lastclose - min;
                //    y.LabelStyle.ForeColor = Color.Red;

                //    y.MinorGrid.Enabled = true;
                //    y.MinorGrid.Interval = (lastclose - min) / 7;
                //    y.MinorGrid.LineColor = Color.Red;
                //    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    y.MajorTickMark.Enabled = false;
                //    y.MinorGrid.LineWidth = 1;
                //    y.MinorGrid.Enabled = true;
                //    y.Interval = y.MinorGrid.Interval;
                //    y.LabelStyle.Format = "0.00";
                //    //y轴上的值
                //    chart_fengshi.ChartAreas[0].AxisY = y;


                //    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    y2.Maximum = Math.Round((max - lastclose) / lastclose, 4);
                //    y2.Minimum = -y2.Maximum;
                //    y2.Interval = y2.Maximum / 7;
                //    y2.MajorGrid.Enabled = false;
                //    y2.MinorGrid.Enabled = false;
                //    y2.LabelStyle.Format = "0.00%";
                //    y2.MajorTickMark.Enabled = false;
                //    y2.MinorTickMark.Enabled = false;
                //    y2.LineColor = Color.Red;
                //    y2.LabelStyle.ForeColor = Color.Red;

                //    chart_fengshi.ChartAreas[0].AxisY2 = y2;
                //    chart_fengshi.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;

                //    //new一个x轴的方法
                //    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    //设置x的最小值为0
                //    x.Minimum = 0;
                //    //设置x的最大值为240
                //    x.Maximum = bar1M.Bars.Count - 1;
                //    //表示取消x轴的网格线
                //    x.IsMarginVisible = false;
                //    x.LineColor = Color.Red;
                //    x.MajorGrid.Interval = 120;
                //    x.MajorGrid.LineColor = Color.Red;
                //    x.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    x.LabelStyle.ForeColor = Color.Red;
                //    x.MinorGrid.Interval = 30;
                //    x.MinorGrid.LineColor = Color.Red;
                //    x.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    x.Interval = x.MinorGrid.Interval;
                //    x.MinorGrid.Enabled = true;

                //    chart_fengshi.ChartAreas[0].AxisX = x;

 
                //    //背景颜色
                //    chart_fengshi.ChartAreas[0].BackColor = Color.White;
                //    //Y轴光标间隔
                //    chart_fengshi.ChartAreas[0].CursorY.Interval = 0.001;
                //    //X轴光标间隔
                //    chart_fengshi.ChartAreas[0].CursorX.Interval = 0.001;
                //    // Zoom into the X axis
                //    //SimpleChart.ChartAreas[0].AxisX.ScaleView.Zoom(1, 1);
                //    // Enable range selection and zooming end user interface
                //    chart_fengshi.ChartAreas[0].CursorX.IsUserEnabled = true;
                //    chart_fengshi.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                //    chart_fengshi.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                //    //将滚动内嵌到坐标轴中
                //    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                //    // 设置滚动条的大小
                //    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                //    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                //    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                //    // 设置自动放大与缩小的最小量
                //    chart_fengshi.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                //    chart_fengshi.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 1;
                //}
                //catch { }
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
                    
                    int line = 0;
                    double last = 0;
                    SeriesColumn.ChartType = SeriesChartType.Candlestick;
                    SeriesColumn["OpenCloseStyle"] = "Triangle";
                    SeriesColumn["ShowOpenClose"] = "Both";
                    SeriesColumn["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    SeriesColumn["PriceUpColor"] = "Red";
                    SeriesColumn["PriceDownColor"] = "Green";
                    foreach (var ldpa in lldp)
                    {
                        LiveBars bar1M = ldpa.Bar1M;
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
                        foreach (var bar in bar1M.Bars)
                        {

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
                            SeriesColumn.Points.AddXY(line, high, low, open, close);
                            //SeriesColumn.Points[line].YValues[1] = low;
                            //SeriesColumn.Points[line].YValues[2] = open;
                            //SeriesColumn.Points[line].YValues[3] = close;
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
                    }
                    double lastclose = lldp.First().Bar1M.Bars.First().Value.Close;
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
                    x.Maximum = line;
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

                    chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart1.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    // 设置自动放大与缩小的最小量
                    chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                    chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 1;


                    chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
                    chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart1.ChartAreas[0].AxisY.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart1.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    // 设置自动放大与缩小的最小量
                    chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = double.NaN;
                    chart1.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 1;
                }
                catch //(Exception ex)
                {
                    // MessageBox.Show(ex.ToString());
                }
                #endregion
                #region 5分钟K
                //try
                //{
                //    this.chart_5m.Series.Clear();
                //    Series SeriesColumn = new Series("Bar");
                //    SeriesColumn.IsVisibleInLegend = false;
                //    chart_5m.Series.Add(SeriesColumn);
                //    double max = 0;
                //    double min = 10000;
                //    int lastindex = -1;

                //    List<TimeSpan> ts = new List<TimeSpan>(bar5M.Bars.Keys);
                //    for (int i = ts.Count - 1; i >= 0; i--)
                //    {
                //        double close = bar5M.Bars[ts[i]].Close;
                //        if (close != 0)
                //        {
                //            lastindex = i;
                //            break;
                //        }
                //    }

                //    if (lastindex == -1)
                //        return;
                //    int line = 0;
                //    double last = 0;
                //    SeriesColumn.ChartType = SeriesChartType.Candlestick;
                //    SeriesColumn["OpenCloseStyle"] = "Triangle";
                //    SeriesColumn["ShowOpenClose"] = "Both";
                //    SeriesColumn["PointWdith"] = "0.2";
                //    //SeriesColumn.Color = Color.Red;
                //    SeriesColumn["PriceUpColor"] = "Red";
                //    SeriesColumn["PriceDownColor"] = "Green";
                //    foreach (var bar in bar5M.Bars)
                //    {
                //        if (line > lastindex)
                //            break;

                //        double close = bar.Value.Close;
                //        double high = 0;
                //        double open = 0;
                //        double low = 0;
                //        if (close == 0 && last == 0)
                //        {
                //            close = ldpa.LastClose;
                //            high = close;
                //            open = close;
                //            low = close;
                //        }
                //        else if (close == 0 && last != 0)
                //        {
                //            close = last;
                //            high = last;
                //            open = last;
                //            low = last;
                //        }
                //        else
                //        {
                //            close = bar.Value.Close;
                //            open = bar.Value.Open;
                //            high = bar.Value.High;
                //            low = bar.Value.Low;
                //            last = close;
                //        }
                //        if (high > max)
                //            max = high;
                //        if (low < min)
                //            min = low;
                //        string time = bar.Key.ToString().Substring(0, 5);
                //        SeriesColumn.Points.AddXY(line, high, low, open, close);
                //        //SeriesColumn.Points[line].YValues[1] = low;
                //        //SeriesColumn.Points[line].YValues[2] = open;
                //        //SeriesColumn.Points[line].YValues[3] = close;
                //        if (close > open)
                //        {
                //            SeriesColumn.Points[line].Color = Color.Red;
                //        }
                //        else if (close < open)
                //        {
                //            SeriesColumn.Points[line].Color = Color.Green;//.Blue;//.FromName("#54FFFF");
                //        }
                //        else
                //        {
                //            SeriesColumn.Points[line].Color = Color.Black;
                //        }
                //        SeriesColumn.Points[line].AxisLabel = time;
                //        SeriesColumn.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\n量:{5}\n均5:{6}\n均10{7}\n类{8}", time, high, open, low, close, bar.Value.Volumn, "Ev5", "Ev10", Enum.GetName(typeof(BarType), bar.Value.RelativeRaiseType));


                //        line++;


                //    }

                //    double lastclose = ldpa.Bar5M.Bars.First().Value.Close;
                //    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                //    {
                //        min = lastclose - (max - lastclose) - 500;
                //        max += 500;
                //    }
                //    else
                //    {
                //        max = lastclose + (lastclose - min) + 500;
                //        min -= 500;
                //    }


                //    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    x.Minimum = 0;

                //    x.Maximum = bar5M.Bars.Count - 1;
                //    x.IsMarginVisible = false;
                //    x.LineColor = Color.Red;
                //    x.MajorGrid.Interval = 120;
                //    x.MajorGrid.LineColor = Color.Red;
                //    x.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    x.LabelStyle.ForeColor = Color.Red;
                //    x.MinorGrid.Interval = 30;
                //    x.MinorGrid.LineColor = Color.Red;
                //    x.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    x.Interval = x.MinorGrid.Interval;
                //    x.MinorGrid.Enabled = true;

                //    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    y.Maximum = max;
                //    y.Minimum = min;
                //    y.LineColor = Color.Red;
                //    y.MajorGrid.LineColor = Color.Red;
                //    y.MajorGrid.Interval = lastclose - min;
                //    y.LabelStyle.ForeColor = Color.Red;

                //    y.MinorGrid.Enabled = true;
                //    y.MinorGrid.Interval = (lastclose - min) / 7;
                //    y.MinorGrid.LineColor = Color.Red;
                //    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                //    y.MajorTickMark.Enabled = false;
                //    y.MinorGrid.LineWidth = 1;
                //    y.MinorGrid.Enabled = true;
                //    y.Interval = y.MinorGrid.Interval;
                //    y.LabelStyle.Format = "0.00";

                //    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                //    y2.Maximum = Math.Round((max - lastclose) / lastclose, 4);
                //    y2.Minimum = -y2.Maximum;
                //    y2.Interval = y2.Maximum / 7;
                //    y2.MajorGrid.Enabled = false;
                //    y2.MinorGrid.Enabled = false;
                //    y2.LabelStyle.Format = "0.00%";
                //    y2.MajorTickMark.Enabled = false;
                //    y2.MinorTickMark.Enabled = false;
                //    y2.LineColor = Color.Red;
                //    y2.LabelStyle.ForeColor = Color.Red;
                //    chart_5m.ChartAreas[0].AxisY = y;
                //    chart_5m.ChartAreas[0].AxisX = x;
                //    chart_5m.ChartAreas[0].AxisY2 = y2;
                //    chart_5m.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                //    chart_5m.ChartAreas[0].BackColor = Color.White;

                //    // Zoom into the X axis
                //    //SimpleChart.ChartAreas[0].AxisX.ScaleView.Zoom(1, 1);
                //    // Enable range selection and zooming end user interface
                //    chart_5m.ChartAreas[0].CursorX.IsUserEnabled = true;
                //    chart_5m.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                //    chart_5m.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                //    //将滚动内嵌到坐标轴中
                //    chart_5m.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                //    // 设置滚动条的大小
                //    chart_5m.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                //    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                //    chart_5m.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                //    // 设置自动放大与缩小的最小量
                //    chart_5m.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                //    chart_5m.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 1;
                //}
                //catch //(Exception ex)
                //{
                //    // MessageBox.Show(ex.ToString());
                //}
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
            //清除chart控件中的数据元素
            this.chart1.Series.Clear();
        }
        //tx_code按键事件，
        private void tx_code_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                //调用方法查找到code股票代码
                ShowChart();
            }
        }
        //鼠标放在chart中，会出现一个十字线
        private void chart_Tick_MouseMove(object sender, MouseEventArgs e)
        {
            if (doubleClicked && dataget)
            {
                this.SuspendLayout();
                int CursorX = e.X;
                //自定义
                int CursorY = e.Y;
                //显示X轴的线
                chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                //与Y轴的间隔
                chart1.ChartAreas[0].CursorY.Interval = 0.01;
                //显示Y轴的线
                chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(CursorX, CursorY), true);
                //获取X轴光标的位置
                int index = System.Convert.ToInt32(chart1.ChartAreas[0].CursorX.Position);
                lastindex = index;

                DataPoint dp = this.chart1.Series["PointSerial"].Points[index - 1];
                TickData td = lt[dp.AxisLabel];
                //chart1.ChartAreas[0].AxisX.CustomLabels.Add(new CustomLabel(index, index, ts.ToString(), 0, LabelMarkStyle.Box));
                this.uc_stockdetail.Update(td);
                this.lb_select.Text = string.Format("价格:{0},时间:{1},量:{2}", td.Last.ToString("0.00"), td.Time.ToString(), td.Volume);
                this.ResumeLayout();
            }

        }
        //双击左键取消十字线
        private void chart_Tick_DoubleClick(object sender, EventArgs e)
        {
            doubleClicked = !doubleClicked;
            if (!doubleClicked)
            {
                chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(0, 0), true);
                chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(0, 0), true);
            }
        }

        private void frm_TickReview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if (doubleClicked && dataget)
                {
                    if (e.KeyCode == Keys.Left)
                    {
                        if (lastindex >= 2)
                        {
                            lastindex--;
                            chart1.ChartAreas[0].CursorX.Position = lastindex;
                        }

                    }
                    else if (e.KeyCode == Keys.Right)
                    {
                        if (lastindex < this.chart1.Series["PointSerial"].Points.Count)
                            lastindex++;
                        chart1.ChartAreas[0].CursorX.Position = lastindex;
                    }
                    DataPoint dp = this.chart1.Series["PointSerial"].Points[lastindex - 1];
                    TickData td = lt[dp.AxisLabel];
                    this.uc_stockdetail.Update(td);
                    this.lb_select.Text = string.Format("价格:{0},时间:{1},量:{2}", td.Last.ToString("0.00"), td.Time.ToString(), td.Volume);
                }
            }
        }

        private void chart_tick_Click(object sender, EventArgs e)
        {
            this.tx_code.Focus();
        }

        private void panel5_Resize(object sender, EventArgs e)
        {
            this.chart_day.Height = panel5.Height;
            this.chart_day.Width = panel5.Width * 2;
        }

        private void chart_day_DoubleClick(object sender, EventArgs e)
        {
            //显示X轴的线
            chart_day.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(DayPointX, DayPointY), true);
            //与Y轴的间隔
            chart_day.ChartAreas[0].CursorY.Interval = 0.01;
            //显示Y轴的线
            chart_day.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(DayPointX, DayPointY), true);
            //获取X轴光标的位置
            int index = System.Convert.ToInt32(chart_day.ChartAreas[0].CursorX.Position);
            lastindex = index;

            DataPoint dp = this.chart_day.Series["Bar"].Points[index];
            for (int i = 0; i < Application.OpenForms.Count;i++ )
            {
                string fName = string.Format("{0}-{1} 分时图", this._si.Name, dp.AxisLabel);
                if (Application.OpenForms[i].Text == fName)
                {
                    Application.OpenForms[i].Activate();
                    return;
                }

            }
            frm_kDetailReview DataDetail = new frm_kDetailReview(this._si, System.Convert.ToDateTime(dp.AxisLabel));
            DataDetail.Show();
        }

        //鼠标放在chart中，会出现一个十字线
        private void chart_Day_MouseMove(object sender, MouseEventArgs e)
        {
            if (dataget)
            {
                DayPointX = e.X;
                DayPointY = e.Y;
                //this.SuspendLayout();
                
                ////TimeSpan ts = TimeSpan.Parse(dp.AxisLabel);
                ////TickData td = th.ticks[ts];
                ////chart1.ChartAreas[0].AxisX.CustomLabels.Add(new CustomLabel(index, index, ts.ToString(), 0, LabelMarkStyle.Box));
                ////this.uc_stockdetail.Update(td);
                ////this.lb_select.Text = string.Format("价格:{0},时间:{1},量:{2}", td.Last.ToString("0.00"), ts.ToString(), td.Volume);
                //this.ResumeLayout();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowChart();
        }

    }
    //创建一个公共的类

}
