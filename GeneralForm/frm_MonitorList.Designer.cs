namespace GeneralForm
{
    partial class frm_MonitorList
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
            this.grid_stocklist = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bt_ok = new System.Windows.Forms.Button();
            this.tx_code = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grid_stocklist)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid_stocklist
            // 
            this.grid_stocklist.AllowUserToAddRows = false;
            this.grid_stocklist.AllowUserToDeleteRows = false;
            this.grid_stocklist.AllowUserToResizeColumns = false;
            this.grid_stocklist.AllowUserToResizeRows = false;
            this.grid_stocklist.ColumnHeadersHeight = 28;
            this.grid_stocklist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid_stocklist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.grid_stocklist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_stocklist.Location = new System.Drawing.Point(0, 37);
            this.grid_stocklist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_stocklist.Name = "grid_stocklist";
            this.grid_stocklist.ReadOnly = true;
            this.grid_stocklist.RowHeadersWidth = 10;
            this.grid_stocklist.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grid_stocklist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid_stocklist.Size = new System.Drawing.Size(363, 542);
            this.grid_stocklist.TabIndex = 2;
            this.grid_stocklist.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_stocklist_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "序号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 40;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "代码";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "名称";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "市场";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 60;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Text = "";
            this.Column5.Width = 50;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bt_ok);
            this.panel1.Controls.Add(this.tx_code);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(363, 37);
            this.panel1.TabIndex = 3;
            // 
            // bt_ok
            // 
            this.bt_ok.Location = new System.Drawing.Point(222, 5);
            this.bt_ok.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(69, 28);
            this.bt_ok.TabIndex = 2;
            this.bt_ok.Text = "添加";
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // tx_code
            // 
            this.tx_code.Location = new System.Drawing.Point(71, 7);
            this.tx_code.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tx_code.Name = "tx_code";
            this.tx_code.Size = new System.Drawing.Size(143, 22);
            this.tx_code.TabIndex = 1;
            this.tx_code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tx_code_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "股票代码";
            // 
            // frm_MonitorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 579);
            this.Controls.Add(this.grid_stocklist);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_MonitorList";
            this.ShowIcon = false;
            this.Text = "接收清单";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_MonitorList_FormClosed);
            this.Load += new System.EventHandler(this.frm_MonitorList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grid_stocklist)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid_stocklist;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.TextBox tx_code;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewButtonColumn Column5;
    }
}