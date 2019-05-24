using DataBase;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GeneralForm
{
    public partial class frm_PolicyUpdate : C1.Win.C1Ribbon.C1RibbonForm
    {

        public static bool isRunning;
        public frm_PolicyUpdate()
        {
            InitializeComponent();
        }

        private void bt_browser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.ShowDialog();
            StringBuilder strb = new StringBuilder();
            string filename = ofd.FileName;



            if (filename != string.Empty)
            {
                try
                {
                    //
                    FileInfo fi = new FileInfo(filename);
                    FileStream fs = fi.OpenRead();
                    string dllname = fi.Name.Substring(0, fi.Name.Length - 4);
                    //MessageBox.Show(dllname);
                    byte[] bfs = new byte[fs.Length];
                    fs.Read(bfs, 0, (int)fs.Length);
                    Assembly assembly = Assembly.Load(bfs);
                    Type ClassPolicy = assembly.GetType(string.Format("{0}.Policy", dllname));
                    //Object ObjectPolicy = assembly.CreateInstance(ClassPolicy.FullName, true, BindingFlags.CreateInstance, null, new object[] { si, o, pp }, null, null);
                    PropertyInfo pi = ClassPolicy.GetProperty("PName");
                    object o;
                    o = pi.GetValue(null, null);
                    string name = o.ToString();
                    this.tx_filename.Text = filename;
                    this.tx_name.Text = name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.tx_filename.Text = string.Empty;
                }

            }
        }

        private void bt_upload_Click(object sender, EventArgs e)
        {
            string filename = this.tx_filename.Text.Trim();
            string desc = this.tx_name.Text.Trim();
            if (desc == string.Empty)
            {
                MessageBox.Show("请输入策略名称");
                return;
            }
            if (filename != string.Empty)
            {
                try
                {
                    FileInfo fi = new FileInfo(filename);
                    FileStream fs = fi.OpenRead();
                    byte[] bfs = new byte[fs.Length];
                    fs.Read(bfs, 0, (int)fs.Length);
                    string fname = fi.Name;
                    if (PolicyProgram.exists(fname))
                    {
                        PolicyProgram.UpdateDLL(fname, bfs, desc);
                    }
                    else
                    {
                        PolicyProgram.InsertDLL(fname, bfs, desc);
                    }
                    fs.Close();
                    RefreshGrid();
                    MessageBox.Show("上传成功");
                }
                catch
                {
                    MessageBox.Show("上传失败");

                }
            }
        }

        private void RefreshGrid()
        {
            if (grid_policy.Rows.Count > grid_policy.Rows.Fixed)
            {
                grid_policy.Rows.RemoveRange(grid_policy.Rows.Fixed, grid_policy.Rows.Count - grid_policy.Rows.Fixed);//.Rows.Clear();
            }
            DataTable dt = PolicyProgram.getProgramList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                grid_policy.Rows.Add(1);
                grid_policy.Rows[i + grid_policy.Rows.Fixed][1] = (i + 1).ToString();

                //this.grid_programlist.Rows.Add();
                //this.grid_programlist[0, i].Value = (i + 1).ToString();
                grid_policy.Rows[i + grid_policy.Rows.Fixed][2] = dt.Rows[i][0].ToString();
                grid_policy.Rows[i + grid_policy.Rows.Fixed][3] = dt.Rows[i][1].ToString();

                grid_policy.SetCellImage(i + grid_policy.Rows.Fixed, 4, global::GeneralForm.Properties.Resources.Ext_Net_Build_Ext_Net_icons_cancel);
                //this.grid_programlist[3, i].Value = "Delete";
                //this.grid_programlist[4, i].Value = "Run";
                //this.grid_programlist[5, i].Value = "Download";
            }
        }

        private void frm_PolicyUpdate_Shown(object sender, EventArgs e)
        {
            RefreshGrid();
            this.grid_policy.MouseClick += grid_policy_MouseClick;
        }

        void grid_policy_MouseClick(object sender, MouseEventArgs e)
        {
            int col = grid_policy.Col;
            int row = grid_policy.Row;
            if (row > grid_policy.Rows.Fixed - 1)
            {
                if (col == 4)
                {
                    string programname = this.grid_policy.Rows[row][2].ToString();// .Rows[e.RowIndex].Cells[1].Value.ToString();
                    string programdesc = this.grid_policy.Rows[row][3].ToString();
                    //string strSql = string.Format("delete from dll_mstr where dll_name = '{0}'", programname);
                    if (MessageBox.Show(string.Format("是否要删除策略'{0}'?", programdesc), "确认", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            PolicyProgram.DeleteDLL(programname);
                            RefreshGrid();
                        }
                        catch
                        {
                            MessageBox.Show("有错误发生，请稍后再试");
                        }
                    }
                    else
                    {
                        grid_policy.Select(row, col - 1);
                    }
                }
            }
        }

        protected override void WndProc(ref Message m)
        {

            const int WM_SYSCOMMAND = 0x0112;


            const int SC_CLOSE = 0xF060;


            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                // 屏蔽传入的消息事件
                //this.WindowState = FormWindowState.Minimized;
                isRunning = false;
            }
            base.WndProc(ref m);
        }
    }
}
