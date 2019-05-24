namespace PolicyClient
{
    partial class frm_SelectMonitor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bt_save = new System.Windows.Forms.Button();
            this.bt_load = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_stock = new System.Windows.Forms.Button();
            this.cb_policy = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.stock_lab = new System.Windows.Forms.Label();
            this.com_type = new System.Windows.Forms.ComboBox();
            this.btn_sure = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_import = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.grid_stocks = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_stocks)).BeginInit();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // bt_save
            // 
            this.bt_save.Location = new System.Drawing.Point(176, 7);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(75, 21);
            this.bt_save.TabIndex = 5;
            this.bt_save.Text = "保存列表";
            this.bt_save.UseVisualStyleBackColor = true;
            this.bt_save.Visible = false;
            // 
            // bt_load
            // 
            this.bt_load.Location = new System.Drawing.Point(12, 7);
            this.bt_load.Name = "bt_load";
            this.bt_load.Size = new System.Drawing.Size(75, 21);
            this.bt_load.TabIndex = 5;
            this.bt_load.Text = "调取列表";
            this.bt_load.UseVisualStyleBackColor = true;
            this.bt_load.Click += new System.EventHandler(this.bt_load_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(612, 7);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 23);
            this.btn_close.TabIndex = 3;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_stock
            // 
            this.btn_stock.Location = new System.Drawing.Point(520, 7);
            this.btn_stock.Name = "btn_stock";
            this.btn_stock.Size = new System.Drawing.Size(75, 23);
            this.btn_stock.TabIndex = 2;
            this.btn_stock.Text = "确定";
            this.btn_stock.UseVisualStyleBackColor = true;
            this.btn_stock.Click += new System.EventHandler(this.btn_stock_Click);
            // 
            // cb_policy
            // 
            this.cb_policy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_policy.FormattingEnabled = true;
            this.cb_policy.Location = new System.Drawing.Point(80, 12);
            this.cb_policy.Name = "cb_policy";
            this.cb_policy.Size = new System.Drawing.Size(238, 20);
            this.cb_policy.TabIndex = 6;
            this.cb_policy.SelectedIndexChanged += new System.EventHandler(this.cb_policy_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label1.Location = new System.Drawing.Point(43, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "策略";
            // 
            // stock_lab
            // 
            this.stock_lab.AutoSize = true;
            this.stock_lab.ForeColor = System.Drawing.SystemColors.WindowText;
            this.stock_lab.Location = new System.Drawing.Point(324, 18);
            this.stock_lab.Name = "stock_lab";
            this.stock_lab.Size = new System.Drawing.Size(53, 12);
            this.stock_lab.TabIndex = 5;
            this.stock_lab.Text = "股票代码";
            // 
            // com_type
            // 
            this.com_type.FormattingEnabled = true;
            this.com_type.Location = new System.Drawing.Point(385, 12);
            this.com_type.MaxLength = 15;
            this.com_type.Name = "com_type";
            this.com_type.Size = new System.Drawing.Size(121, 20);
            this.com_type.TabIndex = 0;
            this.com_type.KeyDown += new System.Windows.Forms.KeyEventHandler(this.com_type_KeyDown);
            // 
            // btn_sure
            // 
            this.btn_sure.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btn_sure.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btn_sure.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.btn_sure.Location = new System.Drawing.Point(525, 12);
            this.btn_sure.Name = "btn_sure";
            this.btn_sure.Size = new System.Drawing.Size(75, 23);
            this.btn_sure.TabIndex = 1;
            this.btn_sure.Text = "添加";
            this.btn_sure.UseVisualStyleBackColor = false;
            this.btn_sure.Click += new System.EventHandler(this.btn_sure_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.dateTimePicker2);
            this.panel4.Controls.Add(this.dateTimePicker1);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.cb_policy);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.stock_lab);
            this.panel4.Controls.Add(this.com_type);
            this.panel4.Controls.Add(this.bt_import);
            this.panel4.Controls.Add(this.btn_sure);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(704, 68);
            this.panel4.TabIndex = 11;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(385, 37);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(121, 21);
            this.dateTimePicker2.TabIndex = 7;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(80, 37);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(238, 21);
            this.dateTimePicker1.TabIndex = 7;
            this.dateTimePicker1.Value = new System.DateTime(2015, 1, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label3.Location = new System.Drawing.Point(325, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "开始日期";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.label2.Location = new System.Drawing.Point(19, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "开始日期";
            // 
            // bt_import
            // 
            this.bt_import.BackColor = System.Drawing.Color.CornflowerBlue;
            this.bt_import.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_import.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.bt_import.Location = new System.Drawing.Point(612, 12);
            this.bt_import.Name = "bt_import";
            this.bt_import.Size = new System.Drawing.Size(75, 23);
            this.bt_import.TabIndex = 1;
            this.bt_import.Text = "导入";
            this.bt_import.UseVisualStyleBackColor = false;
            this.bt_import.Click += new System.EventHandler(this.bt_import_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.bt_save);
            this.panel3.Controls.Add(this.bt_load);
            this.panel3.Controls.Add(this.btn_close);
            this.panel3.Controls.Add(this.btn_stock);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 446);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(704, 37);
            this.panel3.TabIndex = 6;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid1.Location = new System.Drawing.Point(431, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(273, 378);
            this.propertyGrid1.TabIndex = 11;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(428, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 378);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // grid_stocks
            // 
            this.grid_stocks.AllowUserToAddRows = false;
            this.grid_stocks.AllowUserToDeleteRows = false;
            this.grid_stocks.AllowUserToResizeColumns = false;
            this.grid_stocks.AllowUserToResizeRows = false;
            this.grid_stocks.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid_stocks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grid_stocks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_stocks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column5,
            this.Column4,
            this.Column6});
            this.grid_stocks.Dock = System.Windows.Forms.DockStyle.Left;
            this.grid_stocks.Location = new System.Drawing.Point(0, 0);
            this.grid_stocks.Name = "grid_stocks";
            this.grid_stocks.ReadOnly = true;
            this.grid_stocks.RowHeadersVisible = false;
            this.grid_stocks.RowTemplate.Height = 23;
            this.grid_stocks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid_stocks.Size = new System.Drawing.Size(428, 378);
            this.grid_stocks.TabIndex = 9;
            this.grid_stocks.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_stocks_CellClick);
            this.grid_stocks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_stocks_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column1.FillWeight = 101.5228F;
            this.Column1.HeaderText = "代码";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 60;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 103.0664F;
            this.Column2.HeaderText = "名称";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 70;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 72.32733F;
            this.Column3.HeaderText = "市场";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 40;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "策略";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 200;
            // 
            // Column4
            // 
            this.Column4.FillWeight = 123.0834F;
            this.Column4.HeaderText = "删除";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 50;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "dllname";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Visible = false;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Control;
            this.panel5.Controls.Add(this.propertyGrid1);
            this.panel5.Controls.Add(this.splitter1);
            this.panel5.Controls.Add(this.grid_stocks);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 68);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(704, 378);
            this.panel5.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(704, 446);
            this.panel1.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(704, 446);
            this.panel6.TabIndex = 7;
            // 
            // frm_SelectMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 483);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel3);
            this.Name = "frm_SelectMonitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加策略";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Load += new System.EventHandler(this.frm_SelectMonitor_Load);
            this.Shown += new System.EventHandler(this.frm_Statrtstrategy_Shown);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_stocks)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.Button bt_load;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_stock;
        private System.Windows.Forms.ComboBox cb_policy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label stock_lab;
        private System.Windows.Forms.ComboBox com_type;
        private System.Windows.Forms.Button btn_sure;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridView grid_stocks;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bt_import;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewButtonColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}