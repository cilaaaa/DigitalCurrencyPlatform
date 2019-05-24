namespace PolicyClient
{
    partial class frm_BackTestResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_BackTestResult));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lb_running = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lb_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bt_export = new System.Windows.Forms.ToolStripButton();
            this.tsb_policyview = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsl_count = new System.Windows.Forms.ToolStripLabel();
            this.grid_ResultX = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.lb_load = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_ResultX)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lb_running,
            this.toolStripProgressBar1,
            this.lb_status,
            this.lb_load});
            this.statusStrip1.Location = new System.Drawing.Point(0, 177);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1470, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lb_running
            // 
            this.lb_running.Name = "lb_running";
            this.lb_running.Size = new System.Drawing.Size(44, 17);
            this.lb_running.Text = "运行中";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // lb_status
            // 
            this.lb_status.Name = "lb_status";
            this.lb_status.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bt_export,
            this.tsb_policyview,
            this.toolStripLabel1,
            this.tsl_count});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1470, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bt_export
            // 
            this.bt_export.Image = global::PolicyClient.Properties.Resources.Ext_Net_Build_Ext_Net_icons_page_excel;
            this.bt_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bt_export.Name = "bt_export";
            this.bt_export.Size = new System.Drawing.Size(76, 22);
            this.bt_export.Text = "导出结果";
            this.bt_export.Click += new System.EventHandler(this.bt_export_Click);
            // 
            // tsb_policyview
            // 
            this.tsb_policyview.Image = global::PolicyClient.Properties.Resources.Ext_Net_Build_Ext_Net_icons_page_edit;
            this.tsb_policyview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_policyview.Name = "tsb_policyview";
            this.tsb_policyview.Size = new System.Drawing.Size(76, 22);
            this.tsb_policyview.Text = "参数查看";
            this.tsb_policyview.Click += new System.EventHandler(this.tsb_policyview_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(47, 22);
            this.toolStripLabel1.Text = "线程数:";
            // 
            // tsl_count
            // 
            this.tsl_count.Name = "tsl_count";
            this.tsl_count.Size = new System.Drawing.Size(0, 22);
            // 
            // grid_ResultX
            // 
            this.grid_ResultX.AllowEditing = false;
            this.grid_ResultX.AllowFiltering = true;
            this.grid_ResultX.ColumnInfo = "1,1,0,0,0,95,Columns:";
            this.grid_ResultX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_ResultX.Location = new System.Drawing.Point(0, 25);
            this.grid_ResultX.Name = "grid_ResultX";
            this.grid_ResultX.Rows.Count = 2;
            this.grid_ResultX.Rows.DefaultSize = 19;
            this.grid_ResultX.Size = new System.Drawing.Size(1470, 152);
            this.grid_ResultX.TabIndex = 6;
            // 
            // lb_load
            // 
            this.lb_load.Name = "lb_load";
            this.lb_load.Size = new System.Drawing.Size(0, 17);
            // 
            // frm_BackTestResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1470, 199);
            this.Controls.Add(this.grid_ResultX);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_BackTestResult";
            this.Text = "回测结果";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BackTestResult_FormClosing);
            this.Load += new System.EventHandler(this.frm_BackTestResult_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_ResultX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel lb_running;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bt_export;
        private System.Windows.Forms.ToolStripButton tsb_policyview;
        private System.Windows.Forms.ToolStripStatusLabel lb_status;
        private C1.Win.C1FlexGrid.C1FlexGrid grid_ResultX;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel tsl_count;
        private System.Windows.Forms.ToolStripStatusLabel lb_load;
    }
}