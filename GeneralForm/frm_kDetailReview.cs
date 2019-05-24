using C1.Win.C1Ribbon;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace GeneralForm
{
    public partial class frm_kDetailReview : C1RibbonForm
    {
        public static List<TickHistory> tickHistorys = new List<TickHistory>();
        TickHistory th;
        bool doubleClicked = false;
        bool dataget = false;
        int lastindex = 0;
        RiscPoints _riscPoints;
        SecurityInfo _si;
        DateTime sidate;
        Series SeriesRowColumn;
        Series Series1Column;
        Series Series5Column;
        Series SeriesFenShiColumn;
        double FenShiMax;
        double FenShiMin;
        double OneMax;
        double OneMin;
        double FiveMax;
        double FiveMin;
        public frm_kDetailReview(SecurityInfo si,DateTime datetime)
        {
            InitializeComponent();
            this._si = si;
            this.sidate = datetime;
            this.Text = string.Format("{0}-{1} 分时图", si.Name, datetime.ToShortDateString());
            GUITools.DoubleBuffer(this.chart_tick, true);
            GUITools.DoubleBuffer(this.uc_stockdetail, true);
            this.chart_5m.Top = 0;
            this.chart_5m.Left = 0;
            _riscPoints = new RiscPoints();
            dataget = false;
            this.chart_tick.Series.Clear();
            //清除缓存
            this.uc_stockdetail.Clear();
            Thread _thread_MonitorCanceledWeiTuoList = new Thread(new ThreadStart(LoadData));
            _thread_MonitorCanceledWeiTuoList.Start();
        }

        void ldpa_OnLiveBarArrival(object sender, LiveBarArrivalEventArgs args)
        {
            int intevals = ((LiveBars)sender).Inteval;
            if(intevals == 60)
            {
                _riscPoints.InsertBar(args.Bar);
            }
        }

        void LoadData()
        {
            List<int> intevals = new List<int>();
            intevals.Add(300);
            LiveDataProcessor ldpa = new LiveDataProcessor(intevals, _si);
            ldpa.OnLiveBarArrival += ldpa_OnLiveBarArrival;
            if (true)
            {
                DateTime startTime = sidate.Date.AddHours(-8);
                DateTime endTime = sidate.Date.AddHours(16);

                string sql = string.Format("select * from ideal_tick_mstr where tick_code = '{0}' and tick_time >= '{1}' and tick_time < '{2}' order by tick_time asc", _si.Code, startTime, endTime);
                DataTable dt = null;
                //DataTable dt = LocalSQL.QueryDataTable(sql);
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
                    th.code = _si.Code;
                    th.datetime = sidate;
                    th.lists = dt;
                    th.lastclose = System.Convert.ToDouble(dt.Rows[0]["tick_last"]);
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
                        td.Time = td.Time.AddHours(8);
                        if (_si.isLive(td.Time.TimeOfDay))// SecurityMarket.isLive(td.Time.TimeOfDay))
                        {
                            ldpa.ReceiveTick(td);
                            //dataAnalisys.ReceiveTick(td);
                        }
                        try
                        {
                            th.ticks.Add(td.Time.TimeOfDay, td);
                        }
                        catch { }
                    }
                    tickHistorys.Add(th);
                    dataget = true;
                    //updateChartTickX(dt);
                }
                SeriesRowColumn = new Series();
                SeriesRowColumn.Name = "PointSerial";

                //不显示chart控件中的右上角提示的内容
                SeriesRowColumn.IsVisibleInLegend = false;
                //preCloseSeries.IsVisibleInLegend = false;
                foreach (var x in th.ticks)
                {
                    DataPoint dp = new DataPoint();
                    dp.SetValueXY(x.Key.ToString(), x.Value.Last);
                    SeriesRowColumn.Points.Add(dp);

                }
                //绘画一个折线图
                SeriesRowColumn.ChartType = SeriesChartType.Line;
                LiveBars bar1M = ldpa.Bar1M;
                LiveBars bar5M = ldpa.Bar5M;

                #region 分时数据
                try
                {
                    SeriesFenShiColumn = new Series();

                    //取消chart控件中右上角的标志
                    SeriesFenShiColumn.IsVisibleInLegend = false;

                    //定义一个双精度浮点型最大值为0
                    FenShiMax = 0;
                    //定义一个双精度浮点型最小值为1000
                    FenShiMin = 10000;
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
                        SeriesFenShiColumn.Points.Add(dp);
                        if (FenShiMax < close)
                            FenShiMax = close;
                        if (FenShiMin > close)
                            FenShiMin = close;
                        line++;
                    }
                    //绘画出一个折线图
                    SeriesFenShiColumn.ChartType = SeriesChartType.Line;
                    SeriesFenShiColumn.Color = Color.Blue;
                }
                catch { }
                #endregion
                #region 1分钟数据
                try
                {
                    Series1Column = new Series("Bar");
                    Series1Column.IsVisibleInLegend = false;
                    
                    OneMax = 0;
                    OneMin = 10000;

                    int line = 0;
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
                    Series1Column.ChartType = SeriesChartType.Candlestick;
                    Series1Column["OpenCloseStyle"] = "Triangle";
                    Series1Column["ShowOpenClose"] = "Both";
                    Series1Column["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    Series1Column["PriceUpColor"] = "Red";
                    Series1Column["PriceDownColor"] = "Green";
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
                        if (high > OneMax)
                            OneMax = high;
                        if (low < OneMin)
                            OneMin = low;
                        string time = bar.Key.ToString().Substring(0, 5);
                        Series1Column.YValuesPerPoint = 4;
                        Series1Column.Points.AddXY(line, high);
                        Series1Column.Points[line].YValues[1] = low;
                        Series1Column.Points[line].YValues[2] = open;
                        Series1Column.Points[line].YValues[3] = close;
                        if (close > open)
                        {
                            Series1Column.Points[line].Color = Color.Red;
                        }
                        else if (close < open)
                        {
                            Series1Column.Points[line].Color = Color.Green;//.Blue;//.FromName("#54FFFF");
                        }
                        else
                        {
                            Series1Column.Points[line].Color = Color.Black;
                        }
                        Series1Column.Points[line].AxisLabel = time;
                        Series1Column.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\n量:{5}\n均5:{6}\n均10{7}\n类{8}", time, high, open, low, close, bar.Value.Volumn, "Ev5", "Ev10", Enum.GetName(typeof(BarType), bar.Value.RelativeRaiseType));


                        line++;


                    }

                    
                }
                catch(Exception e) {MessageBox.Show(System.Convert.ToString(e)); }

                #endregion
                #region 5分钟数据
                try
                {
                    Series5Column = new Series("Bar");
                    Series5Column.IsVisibleInLegend = false;

                    FiveMax = 0;
                    FiveMin = 10000;

                    int line = 0;
                    double last = 0;
                    int lastindex = -1;

                    List<TimeSpan> ts = new List<TimeSpan>(bar5M.Bars.Keys);
                    for (int i = ts.Count - 1; i >= 0; i--)
                    {
                        double close = bar5M.Bars[ts[i]].Close;
                        if (close != 0)
                        {
                            lastindex = i;
                            break;
                        }
                    }

                    if (lastindex == -1)
                        return;
                    Series5Column.ChartType = SeriesChartType.Candlestick;
                    Series5Column["OpenCloseStyle"] = "Triangle";
                    Series5Column["ShowOpenClose"] = "Both";
                    Series5Column["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    Series5Column["PriceUpColor"] = "Red";
                    Series5Column["PriceDownColor"] = "Green";
                    foreach (var bar in bar5M.Bars)
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
                        if (high > FiveMax)
                            FiveMax = high;
                        if (low < FiveMin)
                            FiveMin = low;
                        string time = bar.Key.ToString().Substring(0, 5);
                        Series5Column.YValuesPerPoint = 4;
                        Series5Column.Points.AddXY(line, high);
                        Series5Column.Points[line].YValues[1] = low;
                        Series5Column.Points[line].YValues[2] = open;
                        Series5Column.Points[line].YValues[3] = close;
                        if (close > open)
                        {
                            Series5Column.Points[line].Color = Color.Red;
                        }
                        else if (close < open)
                        {
                            Series5Column.Points[line].Color = Color.Green;//.Blue;//.FromName("#54FFFF");
                        }
                        else
                        {
                            Series5Column.Points[line].Color = Color.Black;
                        }
                        Series5Column.Points[line].AxisLabel = time;
                        Series5Column.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\n量:{5}\n均5:{6}\n均10{7}\n类{8}", time, high, open, low, close, bar.Value.Volumn, "Ev5", "Ev10", Enum.GetName(typeof(BarType), bar.Value.RelativeRaiseType));


                        line++;


                    }


                }
                catch (Exception e) { MessageBox.Show(System.Convert.ToString(e)); }

                #endregion
                UpdateChart(ldpa);
            }
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
                
                this.uc_stockdetail.setStockName(_si.Code, _si.Name);
                #region 原始数据
                try
                {
                    //添加
                    chart_tick.Series.Add(SeriesRowColumn);

                    double max = th.high;
                    double min = th.low;
                    if (Math.Abs(max - th.lastclose) > Math.Abs(th.lastclose - min))
                    {
                        min = th.lastclose - (max - th.lastclose) - 500;
                        max += 500;
                    }
                    else
                    {
                        max = th.lastclose + (th.lastclose - min) + 500;
                        min -= 500;
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    //设置Y轴的最大值
                    y.Maximum = max;
                    //设置Y轴的最小值
                    y.Minimum = min;
                    //显示网格线
                    y.MajorGrid.Enabled = true;
                    //设置网格之前的间隔
                    y.MajorGrid.Interval = th.lastclose - min;
                    //设置网格线的颜色
                    y.MajorGrid.LineColor = Color.Red;
                    //设置网格线的宽度
                    y.MajorGrid.LineWidth = 2;
                    //显示网格线
                    y.MinorGrid.Enabled = true;
                    //把chart中划分为7个区域
                    y.MinorGrid.Interval = (th.lastclose - min) / 7;
                    //设置网格线的颜色
                    y.MinorGrid.LineColor = Color.Red;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";
                    y.LabelStyle.Font = new Font("system", 11);
                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((max - th.lastclose) / th.lastclose, 4);
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
                    chart_tick.ChartAreas[0].AxisX.IsMarginVisible = false;
                    chart_tick.ChartAreas[0].CursorX.LineColor = Color.Blue;
                    chart_tick.ChartAreas[0].CursorY.LineColor = Color.Blue;

                    chart_tick.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart_tick.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart_tick.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
                    chart_tick.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart_tick.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart_tick.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart_tick.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart_tick.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart_tick.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    
                }
                catch { }
                #endregion
                
                #region 分时
                try
                {
                    //清除chart1中数据
                    this.chart_fengshi.Series.Clear();

                    chart_fengshi.Series.Add(SeriesFenShiColumn);
                    //获取或设置数据的名称
                    chart_fengshi.Series[0].Name = string.Empty;
                    //new一个y轴的方法
                    double lastclose = ldpa.Bar1M.Bars.First().Value.Close;

                    if (Math.Abs(FenShiMax - lastclose) > Math.Abs(lastclose - FenShiMin))
                    {
                        FenShiMin = lastclose - (FenShiMax - lastclose) - 500;
                        FenShiMax += 500;
                    }
                    else
                    {
                        FenShiMax = lastclose + (lastclose - FenShiMin) + 500;
                        FenShiMin -= 500;
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis y = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y.Maximum = FenShiMax;
                    y.Minimum = FenShiMin;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - FenShiMin;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - FenShiMin) / 7;
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
                    y2.Maximum = Math.Round((FenShiMax - lastclose) / lastclose, 4);
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
                    x.Maximum = SeriesFenShiColumn.Points.Count - 1;
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
                    //背景颜色
                    chart_fengshi.ChartAreas[0].BackColor = Color.White;
                    //Y轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorY.Interval = 0.001;
                    //X轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorX.Interval = 0.001;
                    chart_fengshi.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart_fengshi.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart_fengshi.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart_fengshi.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    // 设置自动放大与缩小的最小量
                    chart_fengshi.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                    chart_fengshi.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 1;
                }
                catch { }
                #endregion
                #region 5分钟K
                try
                {
                    this.chart_5m.Series.Clear();
                    chart_5m.Series.Add(Series5Column);
                    double lastclose = ldpa.Bar5M.Bars.First().Value.Close;
                    if (Math.Abs(FiveMax - lastclose) > Math.Abs(lastclose - FiveMin))
                    {
                        FiveMin = lastclose - (FiveMax - lastclose);
                    }
                    else
                    {
                        FiveMax = lastclose + (lastclose - FiveMin);
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    x.Minimum = 0;

                    x.Maximum = ldpa.Bar5M.Bars.Count - 1;
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
                    y.Maximum = FiveMax;
                    y.Minimum = FiveMin;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - FiveMin;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - FiveMin) / 7;
                    y.MinorGrid.LineColor = Color.Red;
                    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";

                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((FiveMax - lastclose) / lastclose, 4);
                    y2.Minimum = -y2.Maximum;
                    y2.Interval = y2.Maximum / 7;
                    y2.MajorGrid.Enabled = false;
                    y2.MinorGrid.Enabled = false;
                    y2.LabelStyle.Format = "0.00%";
                    y2.MajorTickMark.Enabled = false;
                    y2.MinorTickMark.Enabled = false;
                    y2.LineColor = Color.Red;
                    y2.LabelStyle.ForeColor = Color.Red;
                    chart_5m.ChartAreas[0].AxisY = y;
                    chart_5m.ChartAreas[0].AxisX = x;
                    chart_5m.ChartAreas[0].AxisY2 = y2;
                    chart_5m.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                    chart_5m.ChartAreas[0].BackColor = Color.White;

                    chart_5m.ChartAreas[0].CursorX.IsUserEnabled = true;
                    chart_5m.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                    chart_5m.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart_5m.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart_5m.ChartAreas[0].AxisX.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart_5m.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    // 设置自动放大与缩小的最小量
                    chart_5m.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = double.NaN;
                    chart_5m.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = 1;

                    chart_5m.ChartAreas[0].CursorY.IsUserEnabled = true;
                    chart_5m.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                    chart_5m.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
                    //将滚动内嵌到坐标轴中
                    chart_5m.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;
                    // 设置滚动条的大小
                    chart_5m.ChartAreas[0].AxisY.ScrollBar.Size = 10;
                    // 设置滚动条的按钮的风格，下面代码是将所有滚动条上的按钮都显示出来
                    chart_5m.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
                    // 设置自动放大与缩小的最小量
                    chart_5m.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = double.NaN;
                    chart_5m.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                #endregion
                #region 1分钟K
                try
                {
                    this.chart1.Series.Clear();
                    chart1.Series.Add(Series1Column);

                    double lastclose = ldpa.Bar1M.Bars.First().Value.Close;
                    if (Math.Abs(OneMax - lastclose) > Math.Abs(lastclose - OneMin))
                    {
                        OneMin = lastclose - (OneMax - lastclose) - 500;
                        OneMax += 500;
                    }
                    else
                    {
                        OneMax = lastclose + (lastclose - OneMin) + 500;
                        OneMin -= 500;
                    }

                    System.Windows.Forms.DataVisualization.Charting.Axis x = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    x.Minimum = 0;

                    x.Maximum = Series1Column.Points.Count -1;
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
                    y.Maximum = OneMax;
                    y.Minimum = OneMin;
                    y.LineColor = Color.Red;
                    y.MajorGrid.LineColor = Color.Red;
                    y.MajorGrid.Interval = lastclose - OneMin;
                    y.LabelStyle.ForeColor = Color.Red;

                    y.MinorGrid.Enabled = true;
                    y.MinorGrid.Interval = (lastclose - OneMin) / 7;
                    y.MinorGrid.LineColor = Color.Red;
                    y.MinorGrid.LineDashStyle = ChartDashStyle.Dot;
                    y.MajorTickMark.Enabled = false;
                    y.MinorGrid.LineWidth = 1;
                    y.MinorGrid.Enabled = true;
                    y.Interval = y.MinorGrid.Interval;
                    y.LabelStyle.Format = "0.00";

                    System.Windows.Forms.DataVisualization.Charting.Axis y2 = new System.Windows.Forms.DataVisualization.Charting.Axis();
                    y2.Maximum = Math.Round((OneMax - lastclose) / lastclose, 4);
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

        private void panel5_Resize(object sender, EventArgs e)
        {
            this.chart_5m.Height = panel5.Height;
            this.chart_5m.Width = panel5.Width * 2;
        }



        private void uc_stockdetail_Load(object sender, EventArgs e)
        {

        }
        
    }
    //创建一个公共的类
    
}
