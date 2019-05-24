using GeneralForm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace EntropyRiseStockTP0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ErrorHandler_ThreadException);
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //AppDomain.CurrentDomain.UnhandledException+=CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new frm_MainX());
            //Application.Run(new frm_test());
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
                MessageBox.Show("系统出现异常，请重新运行","错误");
            }
            if (e.IsTerminating)
            {
                Environment.Exit(0);
            }

        }

        

        private static void ErrorHandler_ThreadException(object sender,ThreadExceptionEventArgs t)
        {
            string me = t.Exception.Message;
            Debug.Print(me);
        }
    }
}
