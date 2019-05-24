namespace GeneralForm
{
    partial class frm_ReceiveLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_ReceiveLog));
            this.tx_log = new System.Windows.Forms.TextBox();
            this.tx_serverLog = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.grid_list = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_list)).BeginInit();
            this.SuspendLayout();
            // 
            // tx_log
            // 
            this.tx_log.BackColor = System.Drawing.Color.Black;
            this.tx_log.Dock = System.Windows.Forms.DockStyle.Top;
            this.tx_log.ForeColor = System.Drawing.Color.White;
            this.tx_log.Location = new System.Drawing.Point(0, 0);
            this.tx_log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tx_log.Multiline = true;
            this.tx_log.Name = "tx_log";
            this.tx_log.ReadOnly = true;
            this.tx_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tx_log.Size = new System.Drawing.Size(364, 105);
            this.tx_log.TabIndex = 0;
            // 
            // tx_serverLog
            // 
            this.tx_serverLog.BackColor = System.Drawing.Color.Black;
            this.tx_serverLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tx_serverLog.ForeColor = System.Drawing.Color.White;
            this.tx_serverLog.Location = new System.Drawing.Point(0, 108);
            this.tx_serverLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tx_serverLog.Multiline = true;
            this.tx_serverLog.Name = "tx_serverLog";
            this.tx_serverLog.ReadOnly = true;
            this.tx_serverLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tx_serverLog.Size = new System.Drawing.Size(364, 93);
            this.tx_serverLog.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tx_serverLog);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.tx_log);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 201);
            this.panel1.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 105);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(364, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 201);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(364, 3);
            this.splitter3.TabIndex = 5;
            this.splitter3.TabStop = false;
            // 
            // grid_list
            // 
            this.grid_list.ColumnInfo = "4,1,0,0,0,105,Columns:1{Width:80;AllowSorting:False;Caption:\"任务名称\";AllowDragging:" +
    "False;AllowResizing:False;AllowEditing:False;}\t2{Width:69;Caption:\"状态\";}\t3{Width" +
    ":156;Caption:\"连接地址\";}\t";
            this.grid_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_list.Location = new System.Drawing.Point(0, 204);
            this.grid_list.Name = "grid_list";
            this.grid_list.Rows.Count = 1;
            this.grid_list.Rows.DefaultSize = 21;
            this.grid_list.Size = new System.Drawing.Size(364, 139);
            this.grid_list.StyleInfo = resources.GetString("grid_list.StyleInfo");
            this.grid_list.TabIndex = 6;
            // 
            // frm_ReceiveLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 343);
            this.Controls.Add(this.grid_list);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frm_ReceiveLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "数据接收日志";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_list)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tx_log;
        private System.Windows.Forms.TextBox tx_serverLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter3;
        private C1.Win.C1FlexGrid.C1FlexGrid grid_list;
    }
}