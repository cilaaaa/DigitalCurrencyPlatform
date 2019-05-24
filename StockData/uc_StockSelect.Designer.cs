namespace StockData
{
    partial class uc_StockSelect
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grid_list = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid_list)).BeginInit();
            this.SuspendLayout();
            // 
            // grid_list
            // 
            this.grid_list.AllowUserToAddRows = false;
            this.grid_list.AllowUserToDeleteRows = false;
            this.grid_list.AllowUserToResizeColumns = false;
            this.grid_list.AllowUserToResizeRows = false;
            this.grid_list.BackgroundColor = System.Drawing.Color.White;
            this.grid_list.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.grid_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_list.ColumnHeadersVisible = false;
            this.grid_list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid_list.DefaultCellStyle = dataGridViewCellStyle1;
            this.grid_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_list.Location = new System.Drawing.Point(0, 0);
            this.grid_list.Name = "grid_list";
            this.grid_list.ReadOnly = true;
            this.grid_list.RowHeadersVisible = false;
            this.grid_list.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.grid_list.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid_list.Size = new System.Drawing.Size(210, 255);
            this.grid_list.TabIndex = 0;
            this.grid_list.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_list_CellDoubleClick);
            this.grid_list.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_list_KeyDown);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "代码";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 60;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "描述";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "市场";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 50;
            // 
            // uc_StockSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid_list);
            this.Name = "uc_StockSelect";
            this.Size = new System.Drawing.Size(210, 255);
            ((System.ComponentModel.ISupportInitialize)(this.grid_list)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid_list;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
    }
}
