namespace EntropyRiseStockTP0
{
    partial class frm_Account
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Account));
            this.panel1 = new System.Windows.Forms.Panel();
            this.c1DockingTab1 = new C1.Win.C1Command.C1DockingTab();
            this.c1DockingTabPage1 = new C1.Win.C1Command.C1DockingTabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.c1FlexGrid1 = new C1.Win.C1FlexGrid.C1FlexGrid();
            this.account = new System.Windows.Forms.TextBox();
            this.c1Label1 = new C1.Win.C1Input.C1Label();
            this.c1Label2 = new C1.Win.C1Input.C1Label();
            this.api_key = new System.Windows.Forms.TextBox();
            this.secret_key = new System.Windows.Forms.TextBox();
            this.c1Label3 = new C1.Win.C1Input.C1Label();
            this.c1Label4 = new C1.Win.C1Input.C1Label();
            this.environment = new System.Windows.Forms.ComboBox();
            this.c1Label5 = new C1.Win.C1Input.C1Label();
            this.ccb_new = new C1.Win.C1Input.C1Button();
            this.cbt_delete = new C1.Win.C1Input.C1Button();
            this.cbt_save = new C1.Win.C1Input.C1Button();
            this.cck_r = new C1.Win.C1Input.C1CheckBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).BeginInit();
            this.c1DockingTab1.SuspendLayout();
            this.c1DockingTabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccb_new)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbt_delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbt_save)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cck_r)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.c1DockingTab1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(715, 150);
            this.panel1.TabIndex = 0;
            // 
            // c1DockingTab1
            // 
            this.c1DockingTab1.Controls.Add(this.c1DockingTabPage1);
            this.c1DockingTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1DockingTab1.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c1DockingTab1.Location = new System.Drawing.Point(0, 0);
            this.c1DockingTab1.Name = "c1DockingTab1";
            this.c1DockingTab1.Size = new System.Drawing.Size(715, 150);
            this.c1DockingTab1.TabIndex = 0;
            this.c1DockingTab1.TabsSpacing = 5;
            this.c1DockingTab1.TabStyle = C1.Win.C1Command.TabStyleEnum.Office2010;
            this.c1DockingTab1.VisualStyleBase = C1.Win.C1Command.VisualStyle.Office2010Blue;
            // 
            // c1DockingTabPage1
            // 
            this.c1DockingTabPage1.Controls.Add(this.cck_r);
            this.c1DockingTabPage1.Controls.Add(this.ccb_new);
            this.c1DockingTabPage1.Controls.Add(this.cbt_delete);
            this.c1DockingTabPage1.Controls.Add(this.cbt_save);
            this.c1DockingTabPage1.Controls.Add(this.c1Label5);
            this.c1DockingTabPage1.Controls.Add(this.environment);
            this.c1DockingTabPage1.Controls.Add(this.c1Label4);
            this.c1DockingTabPage1.Controls.Add(this.secret_key);
            this.c1DockingTabPage1.Controls.Add(this.c1Label3);
            this.c1DockingTabPage1.Controls.Add(this.api_key);
            this.c1DockingTabPage1.Controls.Add(this.c1Label2);
            this.c1DockingTabPage1.Controls.Add(this.c1Label1);
            this.c1DockingTabPage1.Controls.Add(this.account);
            this.c1DockingTabPage1.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c1DockingTabPage1.Location = new System.Drawing.Point(1, 26);
            this.c1DockingTabPage1.Name = "c1DockingTabPage1";
            this.c1DockingTabPage1.Size = new System.Drawing.Size(713, 123);
            this.c1DockingTabPage1.TabIndex = 0;
            this.c1DockingTabPage1.TabStop = false;
            this.c1DockingTabPage1.Text = "账号信息";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.c1FlexGrid1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 150);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(715, 222);
            this.panel2.TabIndex = 1;
            // 
            // c1FlexGrid1
            // 
            this.c1FlexGrid1.AllowEditing = false;
            this.c1FlexGrid1.ColumnInfo = resources.GetString("c1FlexGrid1.ColumnInfo");
            this.c1FlexGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c1FlexGrid1.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F);
            this.c1FlexGrid1.Location = new System.Drawing.Point(0, 0);
            this.c1FlexGrid1.Name = "c1FlexGrid1";
            this.c1FlexGrid1.Rows.Count = 1;
            this.c1FlexGrid1.Rows.DefaultSize = 21;
            this.c1FlexGrid1.SelectionMode = C1.Win.C1FlexGrid.SelectionModeEnum.Row;
            this.c1FlexGrid1.Size = new System.Drawing.Size(715, 222);
            this.c1FlexGrid1.StyleInfo = resources.GetString("c1FlexGrid1.StyleInfo");
            this.c1FlexGrid1.TabIndex = 7;
            // 
            // account
            // 
            this.account.Location = new System.Drawing.Point(97, 13);
            this.account.Name = "account";
            this.account.Size = new System.Drawing.Size(100, 22);
            this.account.TabIndex = 1;
            // 
            // c1Label1
            // 
            this.c1Label1.AutoSize = true;
            this.c1Label1.BackColor = System.Drawing.Color.Transparent;
            this.c1Label1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label1.ForeColor = System.Drawing.Color.Black;
            this.c1Label1.Location = new System.Drawing.Point(20, 16);
            this.c1Label1.Name = "c1Label1";
            this.c1Label1.Size = new System.Drawing.Size(30, 16);
            this.c1Label1.TabIndex = 2;
            this.c1Label1.Tag = null;
            // 
            // c1Label2
            // 
            this.c1Label2.AutoSize = true;
            this.c1Label2.BackColor = System.Drawing.Color.Transparent;
            this.c1Label2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label2.ForeColor = System.Drawing.Color.Black;
            this.c1Label2.Location = new System.Drawing.Point(20, 54);
            this.c1Label2.Name = "c1Label2";
            this.c1Label2.Size = new System.Drawing.Size(51, 16);
            this.c1Label2.TabIndex = 3;
            this.c1Label2.Tag = null;
            // 
            // api_key
            // 
            this.api_key.Location = new System.Drawing.Point(97, 51);
            this.api_key.Name = "api_key";
            this.api_key.Size = new System.Drawing.Size(289, 22);
            this.api_key.TabIndex = 4;
            // 
            // secret_key
            // 
            this.secret_key.Location = new System.Drawing.Point(97, 79);
            this.secret_key.Name = "secret_key";
            this.secret_key.Size = new System.Drawing.Size(289, 22);
            this.secret_key.TabIndex = 6;
            // 
            // c1Label3
            // 
            this.c1Label3.AutoSize = true;
            this.c1Label3.BackColor = System.Drawing.Color.Transparent;
            this.c1Label3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label3.ForeColor = System.Drawing.Color.Black;
            this.c1Label3.Location = new System.Drawing.Point(20, 82);
            this.c1Label3.Name = "c1Label3";
            this.c1Label3.Size = new System.Drawing.Size(71, 32);
            this.c1Label3.TabIndex = 5;
            this.c1Label3.Tag = null;
            // 
            // c1Label4
            // 
            this.c1Label4.AutoSize = true;
            this.c1Label4.BackColor = System.Drawing.Color.Transparent;
            this.c1Label4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label4.ForeColor = System.Drawing.Color.Black;
            this.c1Label4.Location = new System.Drawing.Point(416, 16);
            this.c1Label4.Name = "c1Label4";
            this.c1Label4.Size = new System.Drawing.Size(30, 16);
            this.c1Label4.TabIndex = 7;
            this.c1Label4.Tag = null;
            // 
            // environment
            // 
            this.environment.FormattingEnabled = true;
            this.environment.Location = new System.Drawing.Point(452, 13);
            this.environment.Name = "environment";
            this.environment.Size = new System.Drawing.Size(121, 24);
            this.environment.TabIndex = 8;
            // 
            // c1Label5
            // 
            this.c1Label5.AutoSize = true;
            this.c1Label5.BackColor = System.Drawing.Color.Transparent;
            this.c1Label5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.c1Label5.ForeColor = System.Drawing.Color.Black;
            this.c1Label5.Location = new System.Drawing.Point(416, 51);
            this.c1Label5.Name = "c1Label5";
            this.c1Label5.Size = new System.Drawing.Size(30, 16);
            this.c1Label5.TabIndex = 9;
            this.c1Label5.Tag = null;
            // 
            // ccb_new
            // 
            this.ccb_new.Image = global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Ext_Net_icons_add;
            this.ccb_new.Location = new System.Drawing.Point(456, 92);
            this.ccb_new.Name = "ccb_new";
            this.ccb_new.Size = new System.Drawing.Size(66, 21);
            this.ccb_new.TabIndex = 12;
            this.ccb_new.Text = "新增";
            this.ccb_new.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ccb_new.UseVisualStyleBackColor = true;
            // 
            // cbt_delete
            // 
            //this.cbt_delete.Image = global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Images_delete;
            this.cbt_delete.Location = new System.Drawing.Point(618, 93);
            this.cbt_delete.Name = "cbt_delete";
            this.cbt_delete.Size = new System.Drawing.Size(84, 21);
            this.cbt_delete.TabIndex = 10;
            this.cbt_delete.Text = "删除";
            this.cbt_delete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cbt_delete.UseVisualStyleBackColor = true;
            // 
            // cbt_save
            // 
            this.cbt_save.Image = global::EntropyRiseStockTP0.Properties.Resources.Ext_Net_Build_Ext_Net_icons_database_save;
            this.cbt_save.Location = new System.Drawing.Point(528, 92);
            this.cbt_save.Name = "cbt_save";
            this.cbt_save.Size = new System.Drawing.Size(84, 21);
            this.cbt_save.TabIndex = 11;
            this.cbt_save.Text = "保存";
            this.cbt_save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cbt_save.UseVisualStyleBackColor = true;
            // 
            // cck_r
            // 
            this.cck_r.AutoSize = true;
            this.cck_r.BackColor = System.Drawing.Color.Transparent;
            this.cck_r.BorderColor = System.Drawing.Color.Transparent;
            this.cck_r.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cck_r.ForeColor = System.Drawing.Color.Black;
            this.cck_r.Location = new System.Drawing.Point(452, 51);
            this.cck_r.Name = "cck_r";
            this.cck_r.Padding = new System.Windows.Forms.Padding(1);
            this.cck_r.Size = new System.Drawing.Size(17, 16);
            this.cck_r.TabIndex = 13;
            this.cck_r.UseVisualStyleBackColor = true;
            this.cck_r.Value = null;
            // 
            // frm_Account
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 372);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_Account";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "账号管理";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Load += new System.EventHandler(this.frm_Account_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1DockingTab1)).EndInit();
            this.c1DockingTab1.ResumeLayout(false);
            this.c1DockingTabPage1.ResumeLayout(false);
            this.c1DockingTabPage1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.c1Label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccb_new)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbt_delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbt_save)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cck_r)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private C1.Win.C1Command.C1DockingTab c1DockingTab1;
        private C1.Win.C1Command.C1DockingTabPage c1DockingTabPage1;
        private C1.Win.C1Input.C1CheckBox cck_r;
        private C1.Win.C1Input.C1Button ccb_new;
        private C1.Win.C1Input.C1Button cbt_delete;
        private C1.Win.C1Input.C1Button cbt_save;
        private C1.Win.C1Input.C1Label c1Label5;
        private System.Windows.Forms.ComboBox environment;
        private C1.Win.C1Input.C1Label c1Label4;
        private System.Windows.Forms.TextBox secret_key;
        private C1.Win.C1Input.C1Label c1Label3;
        private System.Windows.Forms.TextBox api_key;
        private C1.Win.C1Input.C1Label c1Label2;
        private C1.Win.C1Input.C1Label c1Label1;
        private System.Windows.Forms.TextBox account;
        private C1.Win.C1FlexGrid.C1FlexGrid c1FlexGrid1;
    }
}