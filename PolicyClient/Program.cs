using GeneralForm;
using System;
using System.Windows.Forms;

namespace PolicyClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new frm_Main());
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            if (ex != null)
            {
                ErrorLog.LogError(e.IsTerminating, ex);
                
                
                MessageBox.Show(string.Format("系统出现异常，请重新运行\r\n{0}", ex.Message), "错误");
            }
            else
            {
                MessageBox.Show("系统出现异常，请重新运行", "错误");
            }
            if (e.IsTerminating)
            {
                Environment.Exit(0);
            }

        }
    }
}
