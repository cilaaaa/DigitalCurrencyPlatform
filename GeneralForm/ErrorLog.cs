using System;
using System.IO;
using System.Windows.Forms;

namespace GeneralForm
{
    public class ErrorLog
    {
        
        public static void LogError(bool isTerminating, Exception ex)
        {
            using (StreamWriter file = new StreamWriter(string.Format("{0}\\system.log", Application.StartupPath), true))
            {

                file.WriteLine(string.Format("*****************{0}******************", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                file.WriteLine(string.Format("isTerminating={0}", System.Convert.ToInt32(isTerminating)));
                file.WriteLine("Exception:");
                file.WriteLine(string.Format("Message:{0}", ex.Message));
                file.WriteLine(string.Format("StackTrace:{0}", ex.StackTrace));
                file.WriteLine(string.Format("Source:{0}", ex.Source));
                file.WriteLine("Inner Exception");
                Exception iex = ex.InnerException;
                while (iex != null)
                {

                    file.WriteLine(string.Format("Inner Message:{0}", ex.InnerException.Message));
                    file.WriteLine(string.Format("Inner StackTrace:{0}", ex.InnerException.StackTrace));
                    file.WriteLine(string.Format("Inner Source:{0}", ex.InnerException.Source));
                    iex = iex.InnerException;

                }
                file.Close();
                frm_error frm = new frm_error();
                frm.SetInformation(ex);
                frm.Show();
            }
        }
    }
}
