using C1.Win.C1Ribbon;
using StockData;
using StockPolicies;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using StockTradeAPI;

namespace PolicyClient
{
    public partial class frm_BackTestResult : C1RibbonForm
    {
        
        Assembly assembly;
        Object ObjectParameter;
        SecurityInfo si;
        int totalcount;
        int currentcount = 0;
        //List<TradePoints> tradePoints;
        //Dictionary<Guid, List<TradePoints>> tradePoints;
        //Dictionary<Guid, string> titles;

        ConcurrentDictionary<Guid, List<TradePoints>> ctradePoints;
        ConcurrentDictionary<Guid, string> ctitles;

         
        Object ObjectBackTest;
        //string title = string.Empty;
        TradeParameter tradeParameter;
        int  addRow;
        int totalwks;
        int firstwks;
        object lockGridX;
        DateTime firstDay;
        public frm_BackTestResult(SecurityInfo si,Assembly assembly,Object Parameter)
        {
            InitializeComponent();
            this.assembly = assembly;
            this.ObjectParameter = Parameter;
            this.si = si;
            //GUITools.DoubleBuffer(this.dataGridView1, true);
            //tradePoints = new List<TradePoints>();
            ctradePoints = new ConcurrentDictionary<Guid, List<TradePoints>>();
            ctitles = new ConcurrentDictionary<Guid, string>();
            lockGridX = new object();
        }

        private void tsb_policyview_Click(object sender, EventArgs e)
        {
            frm_PolicyParameter frm = new frm_PolicyParameter(ObjectParameter);
            //frm.MdiParent = this.MdiParent;
            frm.ShowDialog(this);
        }

        private void frm_BackTestResult_Load(object sender, EventArgs e)
        {
            string name = assembly.GetName().Name;
            Type ClassBackTest = assembly.GetType(string.Format("{0}.BackTest",name));
            ObjectBackTest = assembly.CreateInstance(ClassBackTest.FullName,true,BindingFlags.CreateInstance,null,new object[]{si,ObjectParameter,false},null,null);
            List<DataGridViewColumn> cols = ((RunningBackTest)ObjectBackTest).GridColumns();
            int xColCount = 1;
            //xColCount++;
            //this.grid_ResultX.Cols.Count = xColCount;
            //this.grid_ResultX.Cols[xColCount - 1].Caption = "序号";
            foreach(DataGridViewColumn col in cols)
            {
                //this.dataGridView1.Columns.Add(col);
                xColCount++;
                this.grid_ResultX.Cols.Count = xColCount;
                this.grid_ResultX.Cols[xColCount-1].Caption = col.HeaderText;
            }

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "开多胜率";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "开多次数";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "开空胜率";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "开空次数";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "总体胜率";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "平均收益";

            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "盈利合计";
            xColCount++;
            this.grid_ResultX.Cols.Count = xColCount;
            this.grid_ResultX.Cols[xColCount - 1].Caption = "耗时";

            //DataGridViewColumn coll = new DataGridViewColumn();
            //coll.HeaderText = "开多胜率";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            //coll = new DataGridViewColumn();
            //coll.HeaderText = "开多次数";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);


            //coll = new DataGridViewColumn();
            //coll.HeaderText = "开空胜率";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            //coll = new DataGridViewColumn();
            //coll.HeaderText = "开空次数";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            //coll = new DataGridViewColumn();
            //coll.HeaderText = "总体胜率";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            //coll = new DataGridViewColumn();
            //coll.HeaderText = "平均收益率";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            //coll = new DataGridViewColumn();
            //coll.HeaderText = "盈利合计";
            //coll.Width = 100;
            //coll.CellTemplate = new DataGridViewTextBoxCell();
            //coll.SortMode = DataGridViewColumnSortMode.Automatic;
            //this.dataGridView1.Columns.Add(coll);

            addRow = 8;
            totalcount = ((RunningBackTest)ObjectBackTest).getPolicyCount();
            this.lb_status.Text = string.Format("0/{0}", totalcount);
            ((RunningBackTest)ObjectBackTest).BackTestPolicy_Started += frm_BackTestResult_BackTestPolicy_Started;
            ((RunningBackTest)ObjectBackTest).BackTestPolicy_Loading += frm_BackTestResult_BackTestPolicy_Loading;
            ((RunningBackTest)ObjectBackTest).BackTestPolicy_DataFinished += frm_BackTestResult_BackTestPolicy_DataFinished;
            ((RunningBackTest)ObjectBackTest).BackTestPolicy_Finished += frm_BackTestResult_BackTestPolicy_Finished;
            ((RunningBackTest)ObjectBackTest).BackTestResult_Arrival += frm_BackTestResult_BackTestResult_Arrival;
            //((RunningBackTest)ObjectBackTest).BackTestMessage_Arrival += frm_BackTestResult_BackTestMessage_Arrival;
            ((RunningBackTest)ObjectBackTest).BackTest_Finished += frm_BackTestResult_BackTest_Finished;
            tradeParameter = ((RunningBackTest)ObjectBackTest).getTradeParameter();


            DateTime firstDay = firstWeekDay(tradeParameter.StartTime);
            //int startweek = GetWeekOfYear(tradeParameter.StartTime);
            //int endweek = GetWeekOfYear(tradeParameter.EndTime);

            //2016-12-31 -52   2017-2-1 1

            totalwks = System.Convert.ToInt32((tradeParameter.EndTime - firstDay).TotalDays) / 7 + 1;



            //totalwks = GetWeekOfYear(tradeParameter.EndTime) - GetWeekOfYear(tradeParameter.StartTime) + 1;
            //firstwks = GetWeekOfYear(tradeParameter.StartTime);
            for(int i=0;i<totalwks;i++)
            {
                //coll = new DataGridViewColumn();
                //coll.HeaderText = string.Format("{0}", firstDay.AddDays(i * 7).ToString("yyyy/MM/dd"));
                //coll.Width = 100;
                //coll.CellTemplate = new DataGridViewTextBoxCell();
                //coll.SortMode = DataGridViewColumnSortMode.Automatic;
                //this.dataGridView1.Columns.Add(coll);

                xColCount++;
                grid_ResultX.Cols.Count = xColCount;
                grid_ResultX.Cols[xColCount - 1].Caption = string.Format("{0}", firstDay.AddDays(i * 7).ToString("yyyy/MM/dd"));
            }
            addRow = addRow + totalwks;
            this.grid_ResultX.Hide();
            ((RunningBackTest)ObjectBackTest).Start();
        }


        public  delegate void TestFinished(object sender,EventArgs e);
        public event TestFinished Test_Finished;
        public void RaiseFinished(EventArgs e)
        {
            if (Test_Finished != null)
            {
                Test_Finished(this, e);
            }
        }

        public delegate void BackTest_FinishDelegate(object sender, EventArgs e);
        void frm_BackTestResult_BackTest_Finished(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //委托
                this.Invoke(new BackTest_FinishDelegate(frm_BackTestResult_BackTest_Finished), new object[] { sender, e });
            }
            else
            {
                this.grid_ResultX.Show();
            }
            SaveResult();
            RaiseFinished(e);
        }

        delegate void SaveResultDelegate();
        private void SaveResult()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new SaveResultDelegate(SaveResult));
            }else
            {
                try
                {
                    string fileName = string.Format("{0}\\{1}-优化结果-{2}.xls",Application.StartupPath, this.Text, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

                    this.grid_ResultX.SaveExcel(fileName, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                        
                }
                catch { }
            }
        }


        private DateTime firstWeekDay(DateTime dateTime)
        {
            //星期一为第一天  
            int weeknow = Convert.ToInt32(dateTime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。  
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天  
            return  dateTime.AddDays(daydiff);
            //return Convert.ToDateTime(FirstDay);  
        }
        private int GetWeekOfYear(DateTime dt)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
        void frm_BackTestResult_BackTestPolicy_Finished(object sender,BackTestFinishedArgs arg)
        {
            
                
                //MessageBox.Show(tradePoints.Count.ToString());
                Calculate(arg.guid,arg.Elapsed);
                ClearPoints(arg.guid);
            
        }
        void frm_BackTestResult_BackTestPolicy_DataFinished(object sender)
        {
            UpdateProgress();
        }

        void frm_BackTestResult_BackTestPolicy_Loading(object sender, string text)
        {
            UpdateLoadProgress(text);
        }
        delegate void UpdateProgressDelegate();
        private void UpdateProgress()
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdateProgressDelegate(UpdateProgress));
                }
                catch { }
            }
            else
            {
                lock (lockGridX)
                {
                    currentcount++;
                }
                this.lb_status.Text = string.Format("{0}/{1}", currentcount, totalcount);
                if (currentcount == totalcount)
                {
                    this.toolStripProgressBar1.Value = 100;
                    this.lb_load.Text = "已结束";
                }
                else
                {
                    this.toolStripProgressBar1.Value = System.Convert.ToInt32(((double)currentcount / totalcount * 100));
                }
            }
        }

        delegate void UpdateLoadProgressDelegate(string text);
        private void UpdateLoadProgress(string text)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdateLoadProgressDelegate(UpdateLoadProgress), new object[] { text });
                }
                catch { }
            }
            else
            {
                currentcount = 0;
                this.lb_status.Text = string.Format("0/{0}", totalcount);
                this.lb_load.Text = string.Format("正在加载：{0}", text);
            }
        }

        delegate void ClearPointDelegate(Guid g);
        private void ClearPoints(Guid g)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new ClearPointDelegate(ClearPoints), new object[] { g });
                }
                catch { }
            }
            else
            {
                try
                {
                    string title = string.Empty;
                    List<TradePoints> points = new List<TradePoints>();
                    this.ctradePoints.TryRemove(g, out points);
                    this.ctitles.TryRemove(g,out title);
                    this.tsl_count.Text = ctradePoints.Count.ToString();
                    //this.tradePoints.Remove(g);// this.tradePoints.Clear();
                    
                }
                catch { }
            }
        }

        delegate void CalculateDelegate(Guid g, TimeSpan elaps);
        private void Calculate(Guid g, TimeSpan elaps)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new CalculateDelegate(Calculate),new object[]{g,elaps});
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); MessageBox.Show(ex.StackTrace); }
            }
            else
            {
                try
                {
                    //double endValue = tradeParameter.Startvalue;
                    double endValue = 0;
                    double buytimes = 0;
                    double selltimes = 0;
                    double buysuccess = 0;
                    double sellsuccess = 0;
                    double[] weekprofit = new double[totalwks];
                    //MessageBox.Show(weekprofit.Length.ToString());
                    int count = 0;
                    //MessageBox.Show(tradepo)
                    List<TradePoints> tps = ctradePoints[g];
                    foreach (TradePoints tp in tps)
                    {
                        //MessageBox.Show(string.Format("{0},{1},{2}", tp.EnterPoint.OpenPrice, tp.OutPoint.OpenPrice,tp.EnterPoint.OpenTime.ToString("yyyy-MM-dd HH:mm:ss")));
                        double t = 0;
                        double startPrice = tp.EnterPoint.OpenPrice;
                        double endPrice = tp.OutPoint.OpenPrice;
                        if (startPrice == 0)
                            continue;
                        if (tp.EnterPoint.OpenType == OpenType.Buy)
                        {
                            //endValue = endValue + (endPrice - startPrice) * tradeParameter.Amount; // -startPrice * tradeParameter.Amount * tradeParameter.Fee / 100 - endPrice * tradeParameter.Amount * tradeParameter.Fee;
                            t = Math.Round(((endPrice - startPrice) - tradeParameter.Fee * (startPrice + endPrice)) * tp.EnterPoint.OpenQty / tp.EnterPoint.OpenPrice, 8, MidpointRounding.AwayFromZero);
                            if (t > 0)
                            {
                                buysuccess++;
                            }
                            buytimes++;
                        }
                        if (tp.EnterPoint.OpenType == OpenType.KaiDuo)
                        {
                            t = Math.Round((1 / startPrice - 1 / endPrice - tradeParameter.Fee * (1 / startPrice + 1 / endPrice)) * tp.EnterPoint.OpenQty * System.Convert.ToInt16(si.ContractVal), 8, MidpointRounding.AwayFromZero);
                            if (t > 0)
                            {
                                buysuccess++;
                            }
                            buytimes++;
                        }
                        if (tp.EnterPoint.OpenType == OpenType.Sell)
                        {
                            //endValue = endValue + (startPrice - endPrice) * tradeParameter.Amount; //-startPrice * tradeParameter.Amount * tradeParameter.Fee / 100 - endPrice * tradeParameter.Amount * tradeParameter.Fee;
                            t = Math.Round(((startPrice - endPrice) - tradeParameter.Fee * (startPrice + endPrice)) * tp.EnterPoint.OpenQty / tp.EnterPoint.OpenPrice, 8, MidpointRounding.AwayFromZero);
                            if (t > 0)
                            {
                                sellsuccess++;
                            }
                            selltimes++;
                        }
                        if (tp.EnterPoint.OpenType == OpenType.KaiKong)
                        {
                            t = Math.Round((1 / endPrice - 1 / startPrice - tradeParameter.Fee * (1 / startPrice + 1 / endPrice)) * tp.EnterPoint.OpenQty * System.Convert.ToInt16(si.ContractVal), 8, MidpointRounding.AwayFromZero);
                            if (t > 0)
                            {
                                sellsuccess++;
                            }
                            selltimes++;
                        }
                        Debug.Print("{0}-{1}-{2}-{3}-{4}", tp.EnterPoint.OpenPrice,tp.EnterPoint.OpenTime,tp.OutPoint.OpenPrice,tp.OutPoint.OpenTime,t);
                        endValue += t;
                        weekprofit[System.Convert.ToInt32((tp.EnterPoint.OpenTime - tradeParameter.StartTime).TotalDays) / 7] += t;
                        count++;
                    }

                    string[] ts = ctitles[g].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    lock (lockGridX)
                    {
                        
                        this.grid_ResultX.Rows.Insert(1);

                        this.grid_ResultX.Rows[1][1] = tradeParameter.Startvalue;
                        this.grid_ResultX.Rows[1][2] = Math.Round(endValue, 3, MidpointRounding.AwayFromZero);
                        for (int i = 0; i < ts.Length; i++)
                        {
                            this.grid_ResultX.Rows[1][i + 3] = ts[i];
                        }
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow - 1] = count;
                        if (buytimes > 0)
                        {
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow] = string.Format("{0}%", Math.Round(buysuccess / buytimes * 100, 2, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow] = 0;

                        }
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 1] = buytimes;
                        if (selltimes > 0)
                        {
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 2] = string.Format("{0}%", Math.Round(sellsuccess / selltimes * 100, 2, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 2] = 0;
                        }
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 3] = selltimes;

                        if (buytimes + selltimes > 0)
                        {
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 4] = string.Format("{0}%", Math.Round((buysuccess + sellsuccess) / (buytimes + selltimes) * 100, 2, MidpointRounding.AwayFromZero));

                        }
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 5] = Math.Round(endValue / count, 3, MidpointRounding.AwayFromZero);
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 6] = Math.Round(endValue, 2, MidpointRounding.AwayFromZero);
                        this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - addRow + 7] = string.Format("T{0}",elaps.TotalSeconds);
                        for (int i = 0; i < totalwks; i++)
                        {
                            //this.dataGridView1.Rows[0].Cells[this.dataGridView1.Columns.Count - totalwks + i].Value = weekprofit[i].ToString("0.000");
                            this.grid_ResultX.Rows[1][this.grid_ResultX.Cols.Count - totalwks + i] = weekprofit[i].ToString("0.00000000");

                        }
                        
                    }
                    Debug.WriteLine("Calculating...");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("{0}\n{1}",ex.Message,ex.StackTrace));
                }
            }
        }

        void frm_BackTestResult_BackTestPolicy_Started(object sender, BackTestStartEventArgs args)
        {
            
                this.ctitles.TryAdd(args.PolicyGuid,args.Title);// = args.Title;
                this.ctradePoints.TryAdd(args.PolicyGuid, new List<TradePoints>());
            
        }

        delegate void UpdateGridDelegate(string title);
        private void UpdateGrid(string title)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new UpdateGridDelegate(UpdateGrid), new object[] { title });
                }
                catch { }
            }
            else
            {
                try
                {
                    
                }
                catch { }
            }
        }

        void frm_BackTestResult_BackTestResult_Arrival(object sender, PolicyResultEventArgs args)
        {
            if (args.PairePoint.Closed)
            {

                Guid g = ((RunningPolicy)sender).PolicyGuid;
                ctradePoints[g].Add(args.PairePoint);
            }
        }

        private void frm_BackTestResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((RunningBackTest)ObjectBackTest).Stop();
            GC.Collect();
        }

        private void bt_export_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel工作表(*.xls)|*.xls";
            sfd.FileName = string.Format("{0}-回测结果-{1}", this.Text, System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;
            sfd.CreatePrompt = false;
            sfd.OverwritePrompt = true;
            sfd.Title = "输出结果";
            sfd.ShowDialog();
            string filename = sfd.FileName;
            if (filename != string.Empty)
            {
                try
                {
                    this.grid_ResultX.SaveExcel(filename, C1.Win.C1FlexGrid.FileFlags.IncludeFixedCells);
                }
                catch
                {
                    MessageBox.Show("保存文件错误");
                }
            }
            //sfd.ShowDialog();
            //Stream myStream;
            //try
            //{
            //    myStream = sfd.OpenFile();
            //}
            //catch
            //{
            //    MessageBox.Show("错误", "保存失败，请稍后再试");
            //    return;
            //}
            //StreamWriter sw;
            //try
            //{
            //    sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
            //}
            //catch
            //{
            //    MessageBox.Show("对不起，有错误发生，请稍后再试");
            //    return;
            //}
            //try
            //{
            //    StringBuilder stb = new StringBuilder();
            //    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            //    {
            //        stb.Append(string.Format("{0},", this.dataGridView1.Columns[i].HeaderText));
            //    }
            //    stb.Append(Environment.NewLine);
            //    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //    {
            //        for (int j = 0; j < this.dataGridView1.Columns.Count; j++)
            //        {
            //            try
            //            {
            //                stb.Append(string.Format("{0},", this.dataGridView1.Rows[i].Cells[j].Value.ToString()));
            //            }
            //            catch
            //            {
            //                stb.Append(",");
            //            }
            //        }
            //        stb.Append(Environment.NewLine);
            //    }
            //    sw.Write(stb.ToString());
            //}
            //catch
            //{
            //    MessageBox.Show("对不起，有错误发生，请稍后再试");
            //}
            //finally
            //{
            //    sw.Close();
            //    myStream.Close();
            //}

        }
    }
}
