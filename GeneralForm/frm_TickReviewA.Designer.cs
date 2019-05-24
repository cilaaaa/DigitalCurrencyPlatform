using StockPolicyContorl;
namespace GeneralForm
{
    partial class frm_TickReviewA
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea9 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend9 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea10 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend10 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea11 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend11 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend12 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.bt_ok = new System.Windows.Forms.Button();
            this.dt_date = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.tx_code = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart_tick = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage2 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage3 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart_fengshi = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage4 = new C1.Win.C1Command.C1DockingTabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.chart_15s = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lb_select = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.uc_stockdetail = new StockPolicyContorl.uc_StockBidAskDetailX();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_tick)).BeginInit();
            this.c1DockingTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.c1DockingTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_fengshi)).BeginInit();
            this.c1DockingTabPage4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_15s)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.bt_ok);
            this.panel1.Controls.Add(this.dt_date);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tx_code);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1183, 39);
            this.panel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(503, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 21);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bt_ok
            // 
            this.bt_ok.Location = new System.Drawing.Point(335, 8);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(75, 23);
            this.bt_ok.TabIndex = 4;
            this.bt_ok.Text = "显示";
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // dt_date
            // 
            this.dt_date.CustomFormat = "yyyy-MM-dd";
            this.dt_date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_date.Location = new System.Drawing.Point(224, 10);
            this.dt_date.Name = "dt_date";
            this.dt_date.Size = new System.Drawing.Size(94, 21);
            this.dt_date.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "日期";
            // 
            // tx_code
            // 
            this.tx_code.Location = new System.Drawing.Point(65, 10);
            this.tx_code.Name = "tx_code";
            this.tx_code.Size = new System.Drawing.Size(100, 21);
            this.tx_code.TabIndex = 1;
            this.tx_code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tx_code_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "代码";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1183, 569);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.c1DockingTab1);
            this.panel3.Controls.Add(this.lb_select);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(965, 569);
            this.panel3.TabIndex = 0;
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage2);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage3);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage4);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.SelectedIndex = 3;
            this.c1DockingTab1.Size = new System.Drawing.Size(965, 569);
            this.c1DockingTab1.TabIndex = 3;
            this.c1DockingTab1.TabsSpacing = 5;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.Controls.Add(this.chart_tick);
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(963, 543);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.TabStop = false;
            this.c1DockingTabPage1.Text = "Tick";
            // 
            // chart_tick
            // 
            chartArea9.Name = "ChartArea1";
            this.chart_tick.ChartAreas.Add(chartArea9);
            this.chart_tick.Dock = System.Windows.Forms.DockStyle.Fill;
            legend9.Name = "Legend1";
            this.chart_tick.Legends.Add(legend9);
            this.chart_tick.Location = new System.Drawing.Point(0, 0);
            this.chart_tick.Name = "chart_tick";
            series9.ChartArea = "ChartArea1";
            series9.Legend = "Legend1";
            series9.Name = "Series1";
            this.chart_tick.Series.Add(series9);
            this.chart_tick.Size = new System.Drawing.Size(963, 543);
            this.chart_tick.TabIndex = 0;
            this.chart_tick.Text = "chart1";
            this.chart_tick.Click += new System.EventHandler(this.chart_tick_Click);
            this.chart_tick.DoubleClick += new System.EventHandler(this.chart_Tick_DoubleClick);
            this.chart_tick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_Tick_MouseMove);
            // 
            // c1DockingTabPage2
            // 
            this.c1DockingTabPage2.Controls.Add(this.chart1);
            this.c1DockingTabPage2.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage2.Name = "c1DockingTabPage2";
            this.c1DockingTabPage2.Size = new System.Drawing.Size(963, 543);
            this.c1DockingTabPage2.TabIndex = 1;
            this.c1DockingTabPage2.TabStop = false;
            this.c1DockingTabPage2.Text = "1分钟K";
            // 
            // chart1
            // 
            chartArea10.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea10);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend10.Name = "Legend1";
            this.chart1.Legends.Add(legend10);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series10.ChartArea = "ChartArea1";
            series10.Legend = "Legend1";
            series10.Name = "Series1";
            this.chart1.Series.Add(series10);
            this.chart1.Size = new System.Drawing.Size(963, 543);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart2";
            this.chart1.DoubleClick += new System.EventHandler(this.chart1_DoubleClick);
            this.chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseMove);
            // 
            // c1DockingTabPage3
            // 
            this.c1DockingTabPage3.Controls.Add(this.chart_fengshi);
            this.c1DockingTabPage3.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage3.Name = "c1DockingTabPage3";
            this.c1DockingTabPage3.Size = new System.Drawing.Size(963, 543);
            this.c1DockingTabPage3.TabIndex = 2;
            this.c1DockingTabPage3.TabStop = false;
            this.c1DockingTabPage3.Text = "分时";
            // 
            // chart_fengshi
            // 
            chartArea11.Name = "ChartArea1";
            this.chart_fengshi.ChartAreas.Add(chartArea11);
            this.chart_fengshi.Dock = System.Windows.Forms.DockStyle.Fill;
            legend11.Name = "Legend1";
            this.chart_fengshi.Legends.Add(legend11);
            this.chart_fengshi.Location = new System.Drawing.Point(0, 0);
            this.chart_fengshi.Name = "chart_fengshi";
            series11.ChartArea = "ChartArea1";
            series11.Legend = "Legend1";
            series11.Name = "Series1";
            this.chart_fengshi.Series.Add(series11);
            this.chart_fengshi.Size = new System.Drawing.Size(963, 543);
            this.chart_fengshi.TabIndex = 2;
            this.chart_fengshi.Text = "chart1";
            this.chart_fengshi.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_fenshi_MouseMove);
            // 
            // c1DockingTabPage4
            // 
            this.c1DockingTabPage4.Controls.Add(this.panel5);
            this.c1DockingTabPage4.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage4.Name = "c1DockingTabPage4";
            this.c1DockingTabPage4.Size = new System.Drawing.Size(963, 543);
            this.c1DockingTabPage4.TabIndex = 3;
            this.c1DockingTabPage4.Text = "15秒K";
            // 
            // panel5
            // 
            this.panel5.AutoScroll = true;
            this.panel5.Controls.Add(this.chart_15s);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(963, 543);
            this.panel5.TabIndex = 4;
            this.panel5.Resize += new System.EventHandler(this.panel5_Resize);
            // 
            // chart_15s
            // 
            chartArea12.Name = "ChartArea1";
            this.chart_15s.ChartAreas.Add(chartArea12);
            legend12.Name = "Legend1";
            this.chart_15s.Legends.Add(legend12);
            this.chart_15s.Location = new System.Drawing.Point(11, 71);
            this.chart_15s.Name = "chart_15s";
            series12.ChartArea = "ChartArea1";
            series12.Legend = "Legend1";
            series12.Name = "Series1";
            this.chart_15s.Series.Add(series12);
            this.chart_15s.Size = new System.Drawing.Size(963, 546);
            this.chart_15s.TabIndex = 3;
            this.chart_15s.Text = "chart2";
            // 
            // lb_select
            // 
            this.lb_select.AutoSize = true;
            this.lb_select.BackColor = System.Drawing.Color.White;
            this.lb_select.Location = new System.Drawing.Point(83, 12);
            this.lb_select.Name = "lb_select";
            this.lb_select.Size = new System.Drawing.Size(0, 12);
            this.lb_select.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.uc_stockdetail);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(965, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(218, 569);
            this.panel4.TabIndex = 1;
            // 
            // uc_stockdetail
            // 
            this.uc_stockdetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uc_stockdetail.Location = new System.Drawing.Point(0, 0);
            this.uc_stockdetail.Name = "uc_stockdetail";
            this.uc_stockdetail.Size = new System.Drawing.Size(218, 569);
            this.uc_stockdetail.TabIndex = 0;
            // 
            // frm_TickReviewA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1183, 608);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "frm_TickReviewA";
            this.Text = "图形查看";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Shown += new System.EventHandler(this.frm_TickReview_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_TickReview_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_tick)).EndInit();
            this.c1DockingTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.c1DockingTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_fengshi)).EndInit();
            this.c1DockingTabPage4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_15s)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker dt_date;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tx_code;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_tick;
        private uc_StockBidAskDetailX uc_stockdetail;
        private System.Windows.Forms.Label lb_select;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage2;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_fengshi;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_15s;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button button1;
    }
}