using C1.Win.C1FlexGrid;
using C1.Win.C1Ribbon;
using DataAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EntropyRiseStockTP0
{
    public partial class frm_test : C1RibbonForm
    {
        int connectid = -1;
        string address;
        int port;
        public frm_test()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        
        
        

        private void c1Button1_Click(object sender, EventArgs e)
        {
            //ArrayList _al = new ArrayList();

            //_flex.Rows.Count = 4;
            //_flex.Cols.Count = 4;

            //_flex.Cols[1].Caption = "Column A";
            //_flex.Cols[2].Caption = "Column B";
            //_flex.Cols[3].Caption = "Delete";

            //_flex[1, 1] = "K01"; _flex[1, 2] = "New York";
            //_flex[2, 1] = "K02"; _flex[2, 2] = "Chicago";
            //_flex[3, 1] = "K03"; _flex[3, 2] = "New Delhi";

            //_flex.AllowResizing = AllowResizingEnum.Both;

            //for (int _row = _flex.Rows.Fixed; _row < _flex.Rows.Count; _row++)
            //{
            //    Button btn = new Button();
            //    btn.BackColor = SystemColors.Control;
            //    btn.Tag = _row.ToString();
            //    btn.Text = "Delete";

            //    btn.Click += (s1, e1) =>
            //    {
            //        var button = s1 as Button;
            //        _flex.Rows.Remove(Convert.ToInt32(button.Tag.ToString()));
            //        this.Controls.Remove(button);// 
            //        button.Dispose();
            //    };

            //    _al.Add(new HostedControl(_flex, btn, _row, 3));
            //}

            //_flex.Paint += (s1, e1) =>
            //{
            //    foreach (HostedControl hosted in _al)
            //        hosted.UpdatePosition();
            //};
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
    internal class HostedControl
    {
        internal C1FlexGrid _flex;
        internal Control _ctl;
        internal Row _row;
        internal Column _col;

        internal HostedControl(C1FlexGrid flex, Control hosted, int row, int col)
        {
            // save info
            _flex = flex;
            _ctl = hosted;
            _row = flex.Rows[row];
            _col = flex.Cols[col];

            // insert hosted control into grid
            _flex.Controls.Add(_ctl);
        }
        internal bool UpdatePosition()
        {
            // get row/col indices
            int r = _row.Index;
            int c = _col.Index;
            if (r < 0 || c < 0) return false;

            // get cell rect
            Rectangle rc = _flex.GetCellRect(r, c, false);

            // hide control if out of range
            if (rc.Width <= 0 || rc.Height <= 0 || !rc.IntersectsWith(_flex.ClientRectangle))
            {
                _ctl.Visible = false;
                return true;
            }

            // move the control and show it
            _ctl.Bounds = rc;
            _ctl.Visible = true;

            // done
            return true;
        }
    }

}
