using StockPolicyContorl;
namespace GeneralForm
{
    partial class frm_kReview
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend8 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.end_date = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.start_date = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_ok = new System.Windows.Forms.Button();
            this.tx_code = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage2 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage3 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart_fengshi = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage4 = new C1.Win.C1Command.C1DockingTabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.chart_day = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.c1DockingTabPage5 = new C1.Win.C1Command.C1DockingTabPage();
            this.chart_5m = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lb_select = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.uc_stockdetail = new StockPolicyContorl.uc_StockBidAskDetailX();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.c1DockingTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_fengshi)).BeginInit();
            this.c1DockingTabPage4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_day)).BeginInit();
            this.c1DockingTabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_5m)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.bt_ok);
            this.panel1.Controls.Add(this.tx_code);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1183, 39);
            this.panel1.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.end_date);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.button2);
            this.panel6.Controls.Add(this.start_date);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Controls.Add(this.textBox1);
            this.panel6.Controls.Add(this.label3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1183, 39);
            this.panel6.TabIndex = 5;
            // 
            // end_date
            // 
            this.end_date.CustomFormat = "yyyy-MM-dd";
            this.end_date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.end_date.Location = new System.Drawing.Point(416, 10);
            this.end_date.Name = "end_date";
            this.end_date.Size = new System.Drawing.Size(94, 21);
            this.end_date.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(357, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "结束日期";
            // 
            // start_date
            // 
            this.start_date.CustomFormat = "yyyy-MM-dd";
            this.start_date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.start_date.Location = new System.Drawing.Point(242, 10);
            this.start_date.Name = "start_date";
            this.start_date.Size = new System.Drawing.Size(94, 21);
            this.start_date.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(183, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "开始日期";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 10);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "代码";
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
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage2);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage3);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage4);
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage5);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.SelectedIndex = 4;
            this.c1DockingTab1.Size = new System.Drawing.Size(965, 569);
            this.c1DockingTab1.TabIndex = 3;
            this.c1DockingTab1.TabsSpacing = 5;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
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
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend5.Name = "Legend1";
            this.chart1.Legends.Add(legend5);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Series1";
            this.chart1.Series.Add(series5);
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
            chartArea6.Name = "ChartArea1";
            this.chart_fengshi.ChartAreas.Add(chartArea6);
            this.chart_fengshi.Dock = System.Windows.Forms.DockStyle.Fill;
            legend6.Name = "Legend1";
            this.chart_fengshi.Legends.Add(legend6);
            this.chart_fengshi.Location = new System.Drawing.Point(0, 0);
            this.chart_fengshi.Name = "chart_fengshi";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chart_fengshi.Series.Add(series6);
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
            this.c1DockingTabPage4.Text = "日K";
            // 
            // panel5
            // 
            this.panel5.AutoScroll = true;
            this.panel5.Controls.Add(this.chart_day);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(963, 543);
            this.panel5.TabIndex = 4;
            this.panel5.Resize += new System.EventHandler(this.panel5_Resize);
            // 
            // chart_day
            // 
            chartArea7.Name = "ChartArea1";
            this.chart_day.ChartAreas.Add(chartArea7);
            this.chart_day.Dock = System.Windows.Forms.DockStyle.Fill;
            legend7.Name = "Legend1";
            this.chart_day.Legends.Add(legend7);
            this.chart_day.Location = new System.Drawing.Point(0, 0);
            this.chart_day.Name = "chart_day";
            series7.ChartArea = "ChartArea1";
            series7.Legend = "Legend1";
            series7.Name = "Series1";
            this.chart_day.Series.Add(series7);
            this.chart_day.Size = new System.Drawing.Size(963, 543);
            this.chart_day.TabIndex = 3;
            this.chart_day.Text = "chart2";
            this.chart_day.DoubleClick += new System.EventHandler(this.chart_day_DoubleClick);
            this.chart_day.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chart_Day_MouseMove);
            // 
            // c1DockingTabPage5
            // 
            this.c1DockingTabPage5.Controls.Add(this.chart_5m);
            this.c1DockingTabPage5.Location = new System.Drawing.Point(1, 25);
            this.c1DockingTabPage5.Name = "c1DockingTabPage5";
            this.c1DockingTabPage5.Size = new System.Drawing.Size(963, 543);
            this.c1DockingTabPage5.TabIndex = 4;
            this.c1DockingTabPage5.Text = "5分钟K";
            // 
            // chart_5m
            // 
            chartArea8.Name = "ChartArea1";
            this.chart_5m.ChartAreas.Add(chartArea8);
            this.chart_5m.Dock = System.Windows.Forms.DockStyle.Fill;
            legend8.Name = "Legend1";
            this.chart_5m.Legends.Add(legend8);
            this.chart_5m.Location = new System.Drawing.Point(0, 0);
            this.chart_5m.Name = "chart_5m";
            series8.ChartArea = "ChartArea1";
            series8.Legend = "Legend1";
            series8.Name = "Series1";
            this.chart_5m.Series.Add(series8);
            this.chart_5m.Size = new System.Drawing.Size(963, 543);
            this.chart_5m.TabIndex = 4;
            this.chart_5m.Text = "chart2";
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
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(559, 8);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "显示";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frm_kReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1183, 608);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "frm_kReview";
            this.Text = "图形查看";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Shown += new System.EventHandler(this.frm_TickReview_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_TickReview_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.c1DockingTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_fengshi)).EndInit();
            this.c1DockingTabPage4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_day)).EndInit();
            this.c1DockingTabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_5m)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tx_code;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private uc_StockBidAskDetailX uc_stockdetail;
        private System.Windows.Forms.Label lb_select;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage2;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_fengshi;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_day;
        private System.Windows.Forms.Panel panel5;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage5;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_5m;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DateTimePicker end_date;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker start_date;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
    }
}