using StockData;
using StockTradeAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StockPolicies
{
    public partial class frm_DataShow : Form
    {
        bool doubleClicked = false;
        public static bool isShow;
        public frm_DataShow()
        {
            isShow = true;
            InitializeComponent();
            this.FormClosed += frm_Monitor_FormClosed;
        }

        void frm_Monitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            isShow = false;
        }
        //声明委托
        delegate void UpdateDelegate(LiveShowData showData, List<OpenPoint> openPoints, string policyName);
        internal void UpdateChart(LiveShowData showData, List<OpenPoint> openPoints, string policyName)
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
                this.Invoke(new UpdateDelegate(UpdateChart), new object[] { showData, openPoints, policyName });
            }
            else
            {
                List<TimeSpan> keys = showData.Bar1M.Bars.Keys.ToList();
                #region 分时
                try
                {
                    //清除chart1中数据
                    this.chart_fengshi.Series.Clear();
                    Series seriesColumn = new Series();
                    //取消chart控件中右上角的标志
                    seriesColumn.IsVisibleInLegend = false;

                    //定义一个双精度浮点型最大值为0
                    double max = 0;
                    //定义一个双精度浮点型最小值为1000
                    double min = 10000;
                    double last = 0;
                    
                    int line = 0;
                    
                    foreach (var bar in showData.Bar1M.Bars)
                    {
                        double close = bar.Value.Close;
                        if (close == 0 && last == 0)
                        {
                            close = showData.LastClose;
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
                    seriesColumn.Color = Color.White;
                    chart_fengshi.Series.Add(seriesColumn);
                    //获取或设置数据的名称
                    chart_fengshi.Series[0].Name = string.Empty;
                    //new一个y轴的方法
                    double lastclose = showData.LastClose;

                    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                    {
                        min = lastclose - (max - lastclose);
                    }
                    else
                    {
                        max = lastclose + (lastclose - min);
                    }

                    Axis y = new Axis();
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
                    


                    Axis y2 = new Axis();
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
                    Axis x = new Axis();
                    //设置x的最小值为0
                    x.Minimum = 0;
                    //设置x的最大值为240
                    x.Maximum = showData.Bar1M.Bars.Count -1;
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
                   

                    Series MyseriesOpen = new Series();
                    //设置图形为散点图
                    MyseriesOpen.ChartType = SeriesChartType.Point;
                    //设置散点图的颜色
                    MyseriesOpen.Color = Color.Green;
                    //设置不显示右上角的提示
                    MyseriesOpen.IsVisibleInLegend = false;
                    //设置散点图的数据点的样式
                    MyseriesOpen.MarkerStyle = MarkerStyle.Circle;

                    Series MyseriesClose = new Series();
                    //设置图形为散点图
                    MyseriesClose.ChartType = SeriesChartType.Point;
                    //设置散点图的颜色
                    MyseriesClose.Color = Color.Green;
                    //设置不显示右上角的提示
                    MyseriesClose.IsVisibleInLegend = false;
                    //设置散点图的数据点的样式
                    MyseriesClose.MarkerStyle = MarkerStyle.Cross;
                    //遍历出openpoints中元素的个数
                    foreach (var op in openPoints)
                    {
                        DataPoint datapoint = new DataPoint();
                        //设置标记的大小
                        datapoint.MarkerSize = 10;
                        //提示信息（买卖的类型（buy，sell）和价格）
                        datapoint.ToolTip = string.Format("{0}-{1}", Enum.GetName(typeof(OpenType), op.OpenType), op.OpenPrice);
                        //DateTime a = openPoints[i].OpenTime;
                        //TimeSpan t = a.TimeOfDay;
                        int t1 = keys.IndexOf(new TimeSpan(op.OpenTime.Hour, op.OpenTime.Minute, 0));
                        //int t1 = keys.FindIndex();
                        //double t1 = (openPoints[i].OpenTime.TimeOfDay - SecurityMarket.MorningOpenTime).TotalMinutes - 1;
                        //if (t1 > 119)
                        //{
                        //    t1 = t1 - 90;
                        //}
                        //double dou = openPoints[i].OpenPrice;
                        datapoint.SetValueXY(t1, op.OpenPrice);
                        if (op.Openop)
                        {
                            MyseriesOpen.Points.Add(datapoint);
                        }
                        else
                        {
                            MyseriesClose.Points.Add(datapoint);
                        }
                        
                    }
                    this.chart_fengshi.Series.Add(MyseriesOpen);
                    this.chart_fengshi.Series.Add(MyseriesClose);
                    
                    //背景颜色
                    chart_fengshi.ChartAreas[0].BackColor = Color.Black;
                    //Y轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorY.Interval = 0.001;
                    //X轴光标间隔
                    chart_fengshi.ChartAreas[0].CursorX.Interval = 0.001;
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
                    double min = 1000000;
                    //int lastindex = -1;
                    List<TimeSpan> ts = new List<TimeSpan>(showData.Bar1M.Bars.Keys);
                    if (!object.Equals(null, showData.RiscPoints))
                    {
                        Series xpoint = new Series("XPoint");
                        xpoint.IsVisibleInLegend = false;
                        chart1.Series.Add(xpoint);
                        xpoint.ChartType = SeriesChartType.Line;
                        xpoint.Color = Color.White;
                        //xpoint.Color = Color.Black;
                        foreach (var xp in showData.RiscPoints.RiscPointList)
                        {
                           // int xline = (int)SecurityMarket.calculateMinutes(xp.Time);
                            int xline = keys.IndexOf(new TimeSpan(xp.Time.Hours, xp.Time.Minutes, 0));

                            xpoint.Points.AddXY(xline, xp.Price);
                        }
                    }

                    int line = 0;
                    double last = 0;
                    SeriesColumn.ChartType = SeriesChartType.Candlestick;
                    SeriesColumn["OpenCloseStyle"] = "Triangle";
                    SeriesColumn["ShowOpenClose"] = "Both";
                    SeriesColumn["PointWdith"] = "0.2";
                    //SeriesColumn.Color = Color.Red;
                    SeriesColumn["PriceUpColor"] = "Red";
                    SeriesColumn["PriceDownColor"] = "#54FFFF";
                    foreach (var bar in showData.Bar1M.Bars)
                    {
                        //if (line > lastindex)
                        //    break;

                        double close = bar.Value.Close;
                        double high = 0;
                        double open = 0;
                        double low = 0;
                        if (close == 0 && last == 0)
                        {
                            close = showData.LastClose;
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
                            SeriesColumn.Points[line].Color = Color.FromName("#54FFFF");
                        }
                        else
                        {
                            SeriesColumn.Points[line].Color = Color.White;
                        }
                        SeriesColumn.Points[line].AxisLabel = time;
                        SeriesColumn.Points[line].ToolTip = string.Format("{0}\n高:{1}\n开:{2}\n低:{3}\n收:{4}\n量:{5}\n均5:{6}\n均10{7}", time, high, open, low, close, bar.Value.Volumn, -1, -1);


                        line++;


                    }

                    double lastclose = showData.LastClose;
                    if (Math.Abs(max - lastclose) > Math.Abs(lastclose - min))
                    {
                        min = lastclose - (max - lastclose);
                    }
                    else
                    {
                        max = lastclose + (lastclose - min);
                    }


                    Axis x = new Axis();
                    x.Minimum = 0;

                    x.Maximum = showData.Bar1M.Bars.Count -1;
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

                    Axis y = new Axis();
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

                    Axis y2 = new Axis();
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
                    chart1.ChartAreas[0].BackColor = Color.Black;

                    Series MyseriesOpen = new Series();
                    //设置图形为散点图
                    MyseriesOpen.ChartType = SeriesChartType.Point;
                    //设置散点图的颜色
                    MyseriesOpen.Color = Color.Green;
                    //设置不显示右上角的提示
                    MyseriesOpen.IsVisibleInLegend = false;
                    //设置散点图的数据点的样式
                    MyseriesOpen.MarkerStyle = MarkerStyle.Circle;

                    Series MyseriesClose = new Series();
                    //设置图形为散点图
                    MyseriesClose.ChartType = SeriesChartType.Point;
                    //设置散点图的颜色
                    MyseriesClose.Color = Color.Green;
                    //设置不显示右上角的提示
                    MyseriesClose.IsVisibleInLegend = false;
                    //设置散点图的数据点的样式
                    MyseriesClose.MarkerStyle = MarkerStyle.Cross;
                    //遍历出openpoints中元素的个数
                    foreach (var op in openPoints)
                    {
                        DataPoint datapoint = new DataPoint();
                        //设置标记的大小
                        datapoint.MarkerSize = 10;
                        //提示信息（买卖的类型（buy，sell）和价格）
                        datapoint.ToolTip = string.Format("{0}-{1}", Enum.GetName(typeof(OpenType), op.OpenType), op.OpenPrice);
                        //DateTime a = openPoints[i].OpenTime;
                        //TimeSpan t = a.TimeOfDay;
                        int t1 = keys.IndexOf(new TimeSpan(op.OpenTime.Hour, op.OpenTime.Minute, 0));
                        //int t1 = keys.FindIndex();
                        //double t1 = (openPoints[i].OpenTime.TimeOfDay - SecurityMarket.MorningOpenTime).TotalMinutes - 1;
                        //if (t1 > 119)
                        //{
                        //    t1 = t1 - 90;
                        //}
                        //double dou = openPoints[i].OpenPrice;
                        datapoint.SetValueXY(t1, op.OpenPrice);
                        if (op.Openop)
                        {
                            MyseriesOpen.Points.Add(datapoint);
                        }
                        else
                        {
                            MyseriesClose.Points.Add(datapoint);
                        }

                    }
                    chart1.Series.Add(MyseriesOpen);
                    chart1.Series.Add(MyseriesClose);
                    //Y轴光标间隔
                    chart1.ChartAreas[0].CursorY.Interval = 0.001;
                    //X轴光标间隔
                    chart1.ChartAreas[0].CursorX.Interval = 0.001;
                }
                catch //(Exception ex)
                {
                    // MessageBox.Show(ex.ToString());
                }
                #endregion
                //DateTime now = DateTime.Now;
                //string savetime = "23:59:50";
                //DateTime dt =Convert.ToDateTime(savetime);
                //if (DateTime.Compare(now, dt) > 0)
                //{
                //    string path = Application.StartupPath + "/pics";
                //    System.IO.Directory.CreateDirectory(path);
                //    string fullFileName = path + "/" + string.Format("{0}-交易图-{1}", now.ToString("yyyy-MM-dd"), policyName) + ".png";
                //    chart_fengshi.SaveImage(fullFileName, ChartImageFormat.Png);
                //}
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
                    chart1.ChartAreas[0].CursorX.LineColor = Color.White;
                    chart1.ChartAreas[0].CursorY.LineColor = Color.White;
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
   
    }
    
}
