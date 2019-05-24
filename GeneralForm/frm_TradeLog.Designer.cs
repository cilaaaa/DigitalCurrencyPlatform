namespace GeneralForm
{
    partial class frm_TradeLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_TradeLog));
            this.list_log = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grid_checkX = new C1.Win.C1FlexGrid.C1FlexGrid();
            ((System.ComponentModel.ISupportInitialize)(this.grid_checkX)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // list_log
            // 
            this.list_log.BackColor = System.Drawing.Color.Black;
            this.list_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list_log.ForeColor = System.Drawing.Color.White;
            this.list_log.FormattingEnabled = true;
            this.list_log.ItemHeight = 16;
            this.list_log.Location = new System.Drawing.Point(0, 0);
            this.list_log.Name = "list_log";
            this.list_log.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.list_log.Size = new System.Drawing.Size(373, 343);
            this.list_log.TabIndex = 1;
            // 
            // panel1
            // 
            // 
            // grid_checkX
            // 
            this.grid_checkX.AllowDragging = C1.Win.C1FlexGrid.AllowDraggingEnum.None;
            this.grid_checkX.AllowEditing = false;
            this.grid_checkX.ColumnInfo = "3,0,0,0,0,105,Columns:0{Width:60;Caption:\"账号\";}\t1{Width:60;Caption:\"代码\";}\t2{Width" +
    ":60;Caption:\"头寸\";Style:\"Format:\"\"N0\"\";DataType:System.Int32;TextAlign:GeneralCen" +
    "ter;\";}\t";
            this.grid_checkX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_checkX.Location = new System.Drawing.Point(0, 0);
            this.grid_checkX.Name = "grid_checkX";
            this.grid_checkX.Rows.Count = 1;
            this.grid_checkX.Rows.DefaultSize = 21;
            this.grid_checkX.Size = new System.Drawing.Size(204, 343);
            this.grid_checkX.StyleInfo = resources.GetString("grid_checkX.StyleInfo");
            this.grid_checkX.TabIndex = 0;
            this.panel1.Controls.Add(this.grid_checkX);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(373, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 343);
            this.panel1.TabIndex = 2;
            // 
            // frm_TradeLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 343);
            this.Controls.Add(this.list_log);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(365, 0);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frm_TradeLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "交易信息";
            this.VisualStyleHolder = C1.Win.C1Ribbon.VisualStyle.Office2010Blue;
            this.Shown += new System.EventHandler(this.frm_TradeLog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.grid_checkX)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox list_log;
        private System.Windows.Forms.Panel panel1;
        private C1.Win.C1FlexGrid.C1FlexGrid grid_checkX;
    }
}