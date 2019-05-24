using System;
using System.IO;
using System.Windows.Forms;

namespace DataBase
{
    public class DebugLog
    {
        static readonly string filename = string.Format("{0}\\{1}", Application.StartupPath, "debug.log");
        public static void Log(string logstring)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filename, true))
                {
                    file.WriteLine(string.Format("{0}>{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logstring));
                    file.Close();
                }
            }
            catch { }
        }
    }
}
