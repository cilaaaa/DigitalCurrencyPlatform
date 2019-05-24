namespace EntropyRiseStockTP0
{
    partial class frm_MainX
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_MainX));
            this.c1Ribbon1 = new C1.Win.C1Ribbon.C1Ribbon();
            this.ribbonApplicationMenu1 = new C1.Win.C1Ribbon.RibbonApplicationMenu();
            this.ribbonBottomToolBar1 = new C1.Win.C1Ribbon.RibbonBottomToolBar();
            this.ribbonConfigToolBar1 = new C1.Win.C1Ribbon.RibbonConfigToolBar();
            this.ribbonQat1 = new C1.Win.C1Ribbon.RibbonQat();
            this.ribbonTab1 = new C1.Win.C1Ribbon.RibbonTab();
            this.ribbonGroup1 = new C1.Win.C1Ribbon.RibbonGroup();
            this.rbt_start = new C1.Win.C1Ribbon.RibbonButton();
            this.rbt_stop = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonButton5 = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonGroup2 = new C1.Win.C1Ribbon.RibbonGroup();
            this.rbt_list = new C1.Win.C1Ribbon.RibbonButton();
            this.rbt_refreshmarket = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonGroup3 = new C1.Win.C1Ribbon.RibbonGroup();
            this.rbt_policy = new C1.Win.C1Ribbon.RibbonButton();
            this.rbt_sdk = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonGroup5 = new C1.Win.C1Ribbon.RibbonGroup();
            this.rbt_account = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonGroup6 = new C1.Win.C1Ribbon.RibbonGroup();
            this.rbt_exit = new C1.Win.C1Ribbon.RibbonButton();
            this.ribbonTopToolBar1 = new C1.Win.C1Ribbon.RibbonTopToolBar();
            ((System.ComponentModel.ISupportInitialize)(this.c1Ribbon1)).BeginInit();
            this.SuspendLayout();
            // 
            // c1Ribbon1
            // 
            this.c1Ribbon1.ApplicationMenuHolder = this.ribbonApplicationMenu1;
            this.c1Ribbon1.AutoSizeElement = C1.Framework.AutoSizeElement.Width;
            this.c1Ribbon1.BottomToolBarHolder = this.ribbonBottomToolBar1;
            this.c1Ribbon1.ConfigToolBarHolder = this.ribbonConfigToolBar1;
            this.c1Ribbon1.Location = new System.Drawing.Point(0, 0);
            this.c1Ribbon1.Name = "c1Ribbon1";
            this.c1Ribbon1.QatHolder = this.ribbonQat1;
            this.c1Ribbon1.Size = new System.Drawing.Size(528, 153);
            this.c1Ribbon1.Tabs.Add(this.ribbonTab1);
            this.c1Ribbon1.TopToolBarHolder = this.ribbonTopToolBar1;
            // 
            // ribbonApplicationMenu1
            // 
            this.ribbonApplicationMenu1.Name = "ribbonApplicationMenu1";
            // 
            // ribbonBottomToolBar1
            // 
            this.ribbonBottomToolBar1.Name = "ribbonBottomToolBar1";
            // 
            // ribbonConfigToolBar1
            // 
            this.ribbonConfigToolBar1.Name = "ribbonConfigToolBar1";
            // 
            // ribbonQat1
            // 
            this.ribbonQat1.Name = "ribbonQat1";
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ribbonTab1.Groups.Add(this.ribbonGroup1);
            this.ribbonTab1.Groups.Add(this.ribbonGroup2);
            this.ribbonTab1.Groups.Add(this.ribbonGroup3);
            this.ribbonTab1.Groups.Add(this.ribbonGroup5);
            this.ribbonTab1.Groups.Add(this.ribbonGroup6);
            this.ribbonTab1.Name = "ribbonTab1";
            this.ribbonTab1.Text = "功能";
            // 
            // ribbonGroup1
            // 
            this.ribbonGroup1.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ribbonGroup1.Items.Add(this.rbt_start);
            this.ribbonGroup1.Items.Add(this.rbt_stop);
            this.ribbonGroup1.Items.Add(this.ribbonButton5);
            this.ribbonGroup1.Name = "ribbonGroup1";
            this.ribbonGroup1.Text = "数据接收";
            // 
            // rbt_start
            // 
            this.rbt_start.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_start.LargeImage")));
            this.rbt_start.Name = "rbt_start";
            this.rbt_start.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_start.SmallImage")));
            this.rbt_start.Text = "开始";
            this.rbt_start.Click += new System.EventHandler(this.rbt_start_Click);
            // 
            // rbt_stop
            // 
            this.rbt_stop.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_stop.LargeImage")));
            this.rbt_stop.Name = "rbt_stop";
            this.rbt_stop.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_stop.SmallImage")));
            this.rbt_stop.Text = "停止";
            this.rbt_stop.Click += new System.EventHandler(this.rbt_stop_Click);
            // 
            // ribbonButton5
            // 
            this.ribbonButton5.Name = "ribbonButton5";
            // 
            // ribbonGroup2
            // 
            this.ribbonGroup2.Items.Add(this.rbt_list);
            this.ribbonGroup2.Items.Add(this.rbt_refreshmarket);
            this.ribbonGroup2.Name = "ribbonGroup2";
            this.ribbonGroup2.Text = "信息接收";
            // 
            // rbt_list
            // 
            this.rbt_list.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_list.LargeImage")));
            this.rbt_list.Name = "rbt_list";
            this.rbt_list.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_list.SmallImage")));
            this.rbt_list.Text = "接收清单";
            this.rbt_list.Click += new System.EventHandler(this.rbt_list_Click);
            // 
            // rbt_refreshmarket
            // 
            this.rbt_refreshmarket.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_refreshmarket.LargeImage")));
            this.rbt_refreshmarket.Name = "rbt_refreshmarket";
            this.rbt_refreshmarket.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_refreshmarket.SmallImage")));
            this.rbt_refreshmarket.Text = "更新市场";
            this.rbt_refreshmarket.Click += new System.EventHandler(this.rbt_refreshmarket_Click);
            // 
            // ribbonGroup3
            // 
            this.ribbonGroup3.Items.Add(this.rbt_policy);
            this.ribbonGroup3.Items.Add(this.rbt_sdk);
            this.ribbonGroup3.Name = "ribbonGroup3";
            this.ribbonGroup3.Text = "模块管理";
            // 
            // rbt_policy
            // 
            this.rbt_policy.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_policy.LargeImage")));
            this.rbt_policy.Name = "rbt_policy";
            this.rbt_policy.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_policy.SmallImage")));
            this.rbt_policy.Text = "策略管理";
            this.rbt_policy.Click += new System.EventHandler(this.rbt_policy_Click);
            // 
            // rbt_sdk
            // 
            this.rbt_sdk.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_sdk.LargeImage")));
            this.rbt_sdk.Name = "rbt_sdk";
            this.rbt_sdk.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_sdk.SmallImage")));
            this.rbt_sdk.Text = "插件管理";
            this.rbt_sdk.Click += new System.EventHandler(this.rbt_sdk_Click);
            // 
            // ribbonGroup5
            // 
            this.ribbonGroup5.Items.Add(this.rbt_account);
            this.ribbonGroup5.Name = "ribbonGroup5";
            this.ribbonGroup5.Text = "账号";
            // 
            // rbt_account
            // 
            this.rbt_account.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_account.LargeImage")));
            this.rbt_account.Name = "rbt_account";
            this.rbt_account.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_account.SmallImage")));
            this.rbt_account.Text = "账号管理";
            this.rbt_account.Click += new System.EventHandler(this.rbt_account_Click);
            // 
            // ribbonGroup6
            // 
            this.ribbonGroup6.Items.Add(this.rbt_exit);
            this.ribbonGroup6.Name = "ribbonGroup6";
            this.ribbonGroup6.Text = "系统";
            // 
            // rbt_exit
            // 
            this.rbt_exit.LargeImage = ((System.Drawing.Image)(resources.GetObject("rbt_exit.LargeImage")));
            this.rbt_exit.Name = "rbt_exit";
            this.rbt_exit.SmallImage = ((System.Drawing.Image)(resources.GetObject("rbt_exit.SmallImage")));
            this.rbt_exit.Text = "退出";
            this.rbt_exit.Click += new System.EventHandler(this.rbt_exit_Click);
            // 
            // ribbonTopToolBar1
            // 
            this.ribbonTopToolBar1.Name = "ribbonTopToolBar1";
            // 
            // frm_MainX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 159);
            this.Controls.Add(this.c1Ribbon1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(743, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_MainX";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "策略及数据管理";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_MainX_FormClosed);
            this.Shown += new System.EventHandler(this.frm_MainX_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.c1Ribbon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private C1.Win.C1Ribbon.C1Ribbon c1Ribbon1;
        private C1.Win.C1Ribbon.RibbonApplicationMenu ribbonApplicationMenu1;
        private C1.Win.C1Ribbon.RibbonConfigToolBar ribbonConfigToolBar1;
        private C1.Win.C1Ribbon.RibbonQat ribbonQat1;
        private C1.Win.C1Ribbon.RibbonTab ribbonTab1;
        private C1.Win.C1Ribbon.RibbonGroup ribbonGroup1;
        private C1.Win.C1Ribbon.RibbonButton rbt_start;
        private C1.Win.C1Ribbon.RibbonButton rbt_stop;
        private C1.Win.C1Ribbon.RibbonGroup ribbonGroup2;
        private C1.Win.C1Ribbon.RibbonButton rbt_list;
        private C1.Win.C1Ribbon.RibbonButton rbt_refreshmarket;
        private C1.Win.C1Ribbon.RibbonGroup ribbonGroup3;
        private C1.Win.C1Ribbon.RibbonGroup ribbonGroup5;
        private C1.Win.C1Ribbon.RibbonButton rbt_sdk;
        private C1.Win.C1Ribbon.RibbonButton rbt_exit;
        private C1.Win.C1Ribbon.RibbonButton rbt_account;
        private C1.Win.C1Ribbon.RibbonGroup ribbonGroup6;
        private C1.Win.C1Ribbon.RibbonBottomToolBar ribbonBottomToolBar1;
        private C1.Win.C1Ribbon.RibbonButton ribbonButton5;
        private C1.Win.C1Ribbon.RibbonTopToolBar ribbonTopToolBar1;
        private C1.Win.C1Ribbon.RibbonButton rbt_policy;
    }
}