namespace EntropyRiseStockTP0
{
    partial class frm_RunPolicy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_RunPolicy));
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.panel10 = new System.Windows.Forms.Panel();
            this.grid_realpolicy = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.c1ToolBar1 = new C1.Win.C1Command.C1ToolBar();
            this.c1CommandHolder1 = new C1.Win.C1Command.C1CommandHolder();
            this.command_AddPolicy = new C1.Win.C1Command.C1Command();
            this.command_quit = new C1.Win.C1Command.C1Command();
            this.command_link_AddPolicy = new C1.Win.C1Command.C1CommandLink();
            this.command_link_Quit = new C1.Win.C1Command.C1CommandLink();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pb_min = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lb_title = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_realpolicy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_min)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.HeaderText = "";
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.Width = 217;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Width = 217;
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.GhostWhite;
            this.panel10.Controls.Add(this.grid_realpolicy);
            this.panel10.Controls.Add(this.c1ToolBar1);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(862, 445);
            this.panel10.TabIndex = 4;
            // 
            // grid_realpolicy
            // 
            this.grid_realpolicy.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_realpolicy.AllowEditing = false;
            this.grid_realpolicy.BackColor = System.Drawing.Color.White;
            this.grid_realpolicy.BorderStyle = C1.Win.C1FlexGrid.Util.BaseControls.BorderStyleEnum.None;
            this.grid_realpolicy.ColumnInfo = resources.GetString("grid_realpolicy.ColumnInfo");
            this.grid_realpolicy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_realpolicy.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F);
            this.grid_realpolicy.Location = new System.Drawing.Point(0, 26);
            this.grid_realpolicy.Name = "grid_realpolicy";
            this.grid_realpolicy.Rows.Count = 1;
            this.grid_realpolicy.Rows.DefaultSize = 21;
            this.grid_realpolicy.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Cell;
            this.grid_realpolicy.Size = new System.Drawing.Size(862, 419);
            this.grid_realpolicy.StyleInfo = resources.GetString("grid_realpolicy.StyleInfo");
            this.grid_realpolicy.TabIndex = 6;
            this.grid_realpolicy.VisualStyle = C1.Win.C1FlexGrid.VisualStyle.Office2010Blue;
            // 
            // c1ToolBar1
            // 
            this.c1ToolBar1.AccessibleName = "Tool Bar";
            this.c1ToolBar1.AutoSize = false;
            this.c1ToolBar1.CommandHolder = this.c1CommandHolder1;
            this.c1ToolBar1.CommandLinks.AddRange(new C1.Win.C1Command.C1CommandLink[] {
            this.command_link_AddPolicy,
            this.command_link_Quit});
            this.c1ToolBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.c1ToolBar1.Location = new System.Drawing.Point(0, 0);
            this.c1ToolBar1.Movable = false;
            this.c1ToolBar1.Name = "c1ToolBar1";
            this.c1ToolBar1.Size = new System.Drawing.Size(862, 26);
            this.c1ToolBar1.Text = "c1ToolBar1";
            this.c1ToolBar1.VisualStyle = C1.Win.C1Command.VisualStyle.Office2010Blue;
            this.c1ToolBar1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // c1CommandHolder1
            // 
            this.c1CommandHolder1.Commands.Add(this.command_AddPolicy);
            this.c1CommandHolder1.Commands.Add(this.command_quit);
            this.c1CommandHolder1.Owner = this;
            this.c1CommandHolder1.VisualStyle = C1.Win.C1Command.VisualStyle.Office2010Black;
            // 
            // command_AddPolicy
            // 
            this.command_AddPolicy.Image = ((System.Drawing.Image)(resources.GetObject("command_AddPolicy.Image")));
            this.command_AddPolicy.Name = "command_AddPolicy";
            this.command_AddPolicy.ShortcutText = "";
            this.command_AddPolicy.Text = "添加策略";
            this.command_AddPolicy.Click += new C1.Win.C1Command.ClickEventHandler(this.command_AddPolicy_Click);
            // 
            // command_quit
            // 
            this.command_quit.Image = ((System.Drawing.Image)(resources.GetObject("command_quit.Image")));
            this.command_quit.Name = "command_quit";
            this.command_quit.ShortcutText = "";
            this.command_quit.Text = "退出";
            this.command_quit.Click += new C1.Win.C1Command.ClickEventHandler(this.command_quit_Click);
            // 
            // command_link_AddPolicy
            // 
            this.command_link_AddPolicy.ButtonLook = ((C1.Win.C1Command.ButtonLookFlags)((C1.Win.C1Command.ButtonLookFlags.Text | C1.Win.C1Command.ButtonLookFlags.Image)));
            this.command_link_AddPolicy.Command = this.command_AddPolicy;
            this.command_link_AddPolicy.Text = "添加股票";
            // 
            // command_link_Quit
            // 
            this.command_link_Quit.ButtonLook = ((C1.Win.C1Command.ButtonLookFlags)((C1.Win.C1Command.ButtonLookFlags.Text | C1.Win.C1Command.ButtonLookFlags.Image)));
            this.command_link_Quit.Command = this.command_quit;
            this.command_link_Quit.SortOrder = 1;
            this.command_link_Quit.Text = "退出";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(862, 0);
            this.panel1.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pb_min);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(836, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(26, 0);
            this.panel2.TabIndex = 0;
            // 
            // pb_min
            // 
            this.pb_min.Image = global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Ext_Net_icons_page_white_text_width;
            this.pb_min.ImageLocation = "";
            this.pb_min.Location = new System.Drawing.Point(3, 3);
            this.pb_min.Name = "pb_min";
            this.pb_min.Size = new System.Drawing.Size(16, 16);
            this.pb_min.TabIndex = 0;
            this.pb_min.TabStop = false;
            this.pb_min.Click += new System.EventHandler(this.pb_min_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.lb_title);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(862, 0);
            this.panel3.TabIndex = 1;
            // 
            // lb_title
            // 
            this.lb_title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.lb_title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_title.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_title.ForeColor = System.Drawing.Color.Black;
            this.lb_title.Location = new System.Drawing.Point(0, 0);
            this.lb_title.Name = "lb_title";
            this.lb_title.Size = new System.Drawing.Size(862, 0);
            this.lb_title.TabIndex = 0;
            this.lb_title.Text = "策略监控";
            this.lb_title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lb_title.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_title_MouseDown);
            this.lb_title.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_title_MouseMove);
            this.lb_title.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_title_MouseUp);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(221)))), ((int)(((byte)(238)))));
            this.panel4.Controls.Add(this.panel10);
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(862, 445);
            this.panel4.TabIndex = 7;
            // 
            // frm_RunPolicy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 445);
            this.Controls.Add(this.panel4);
            this.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_RunPolicy";
            this.Text = "实时策略监控";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_RunPolicy_FormClosed);
            this.Load += new System.EventHandler(this.frm_Main_Load);
            this.panel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_realpolicy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1CommandHolder1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb_min)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn2;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.Panel panel10;
        private C1.Win.C1Command.C1CommandHolder c1CommandHolder1;
        private C1.Win.C1Command.C1ToolBar c1ToolBar1;
        private C1.Win.C1Command.C1Command command_AddPolicy;
        private C1.Win.C1Command.C1CommandLink command_link_Quit;
        private C1.Win.C1Command.C1CommandLink command_link_AddPolicy;
        private C1.Win.C1Command.C1Command command_quit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lb_title;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox pb_min;
        private C1.Win.C1FlexGrid.C1FlexGrid grid_realpolicy;
    }
}