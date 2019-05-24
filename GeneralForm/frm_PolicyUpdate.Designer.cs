namespace GeneralForm
{
    partial class frm_PolicyUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_PolicyUpdate));
            this.c1ThemeController1 = new C1.Win.C1Themes.C1ThemeController();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bt_upload = new C1.Win.C1Input.C1Button();
            this.bt_browser = new C1.Win.C1Input.C1Button();
            this.tx_name = new System.Windows.Forms.TextBox();
            this.tx_filename = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.grid_policy = new C1.Win.C1FlexGrid.C1FlexGrid();
            ((System.ComponentModel.ISupportInitialize)(this.c1ThemeController1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bt_upload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bt_browser)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_policy)).BeginInit();
            this.SuspendLayout();
            // 
            // c1ThemeController1
            // 
            this.c1ThemeController1.Theme = "VS2013Dark";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel1.Controls.Add(this.bt_upload);
            this.panel1.Controls.Add(this.bt_browser);
            this.panel1.Controls.Add(this.tx_name);
            this.panel1.Controls.Add(this.tx_filename);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(737, 44);
            this.panel1.TabIndex = 0;
            this.c1ThemeController1.SetTheme(this.panel1, "(default)");
            // 
            // bt_upload
            // 
            this.bt_upload.Location = new System.Drawing.Point(625, 8);
            this.bt_upload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_upload.Name = "bt_upload";
            this.bt_upload.Size = new System.Drawing.Size(78, 25);
            this.bt_upload.TabIndex = 1;
            this.bt_upload.Text = "上传";
            this.c1ThemeController1.SetTheme(this.bt_upload, "(default)");
            this.bt_upload.UseVisualStyleBackColor = true;
            this.bt_upload.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.bt_upload.Click += new System.EventHandler(this.bt_upload_Click);
            // 
            // bt_browser
            // 
            this.bt_browser.Location = new System.Drawing.Point(423, 8);
            this.bt_browser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.bt_browser.Name = "bt_browser";
            this.bt_browser.Size = new System.Drawing.Size(37, 25);
            this.bt_browser.TabIndex = 1;
            this.bt_browser.Text = "...";
            this.c1ThemeController1.SetTheme(this.bt_browser, "(default)");
            this.bt_browser.UseVisualStyleBackColor = true;
            this.bt_browser.VisualStyleBaseStyle = C1.Win.C1Input.VisualStyle.Office2010Blue;
            this.bt_browser.Click += new System.EventHandler(this.bt_browser_Click);
            // 
            // tx_name
            // 
            this.tx_name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tx_name.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx_name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tx_name.Location = new System.Drawing.Point(468, 9);
            this.tx_name.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tx_name.Name = "tx_name";
            this.tx_name.Size = new System.Drawing.Size(151, 22);
            this.tx_name.TabIndex = 0;
            this.c1ThemeController1.SetTheme(this.tx_name, "(default)");
            // 
            // tx_filename
            // 
            this.tx_filename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tx_filename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx_filename.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tx_filename.Location = new System.Drawing.Point(3, 9);
            this.tx_filename.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tx_filename.Name = "tx_filename";
            this.tx_filename.Size = new System.Drawing.Size(415, 22);
            this.tx_filename.TabIndex = 0;
            this.c1ThemeController1.SetTheme(this.tx_filename, "(default)");
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel2.Controls.Add(this.grid_policy);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.ForeColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(0, 44);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(737, 373);
            this.panel2.TabIndex = 1;
            this.c1ThemeController1.SetTheme(this.panel2, "(default)");
            // 
            // grid_policy
            // 
            this.grid_policy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.grid_policy.BorderStyle = C1.Win.C1FlexGrid.Util.BaseControls.BorderStyleEnum.FixedSingle;
            this.grid_policy.ColumnInfo = resources.GetString("grid_policy.ColumnInfo");
            this.grid_policy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_policy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.grid_policy.Location = new System.Drawing.Point(0, 0);
            this.grid_policy.Name = "grid_policy";
            this.grid_policy.Rows.Count = 1;
            this.grid_policy.Rows.DefaultSize = 21;
            this.grid_policy.ShowThemedHeaders = C1.Win.C1FlexGrid.ShowThemedHeadersEnum.None;
            this.grid_policy.Size = new System.Drawing.Size(737, 373);
            this.grid_policy.StyleInfo = resources.GetString("grid_policy.StyleInfo");
            this.grid_policy.TabIndex = 0;
            this.c1ThemeController1.SetTheme(this.grid_policy, "(default)");
            this.grid_policy.Tree.Indent = 16;
            this.grid_policy.Tree.LineColor = System.Drawing.Color.Transparent;
            this.grid_policy.Tree.NodeImageCollapsed = ((System.Drawing.Image)(resources.GetObject("grid_policy.Tree.NodeImageCollapsed")));
            this.grid_policy.Tree.NodeImageExpanded = ((System.Drawing.Image)(resources.GetObject("grid_policy.Tree.NodeImageExpanded")));
            // 
            // frm_PolicyUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 417);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_PolicyUpdate";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "策略管理";
            this.c1ThemeController1.SetTheme(this, "(default)");
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Custom;
            this.Shown += new System.EventHandler(this.frm_PolicyUpdate_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.c1ThemeController1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bt_upload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bt_browser)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_policy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private C1.Win.C1Themes.C1ThemeController c1ThemeController1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tx_filename;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Input.C1Button bt_upload;
        private C1.Win.C1Input.C1Button bt_browser;
        private System.Windows.Forms.TextBox tx_name;
        private C1.Win.C1FlexGrid.C1FlexGrid grid_policy;
    }
}
