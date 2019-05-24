using System.Windows.Forms;
namespace DataBase
{
    public class ConfigFileName
    {
        static IniFile inifile = new IniFile(string.Format("{0}\\{1}", Application.StartupPath, "setting.ini"));
        public static readonly string PolicyProgramFileName = string.Format("{0}\\{1}", Application.StartupPath, "PolicyProgram.xml");
        public static readonly string SDKFileName = string.Format("{0}\\{1}", Application.StartupPath, "TradeSDK.xml");
        public static readonly string FutureCodeFileName = string.Format("{0}\\{1}", Application.StartupPath, "FutureCode.xml");
        public static readonly string ReceiveCodeFileName = string.Format("{0}\\{1}", Application.StartupPath, "ReceiveCode.xml");
        public static readonly string AccountFileName = string.Format("{0}\\{1}", Application.StartupPath, "Account.xml");
        public static readonly string MarketCalendarFileName = string.Format("{0}\\{1}", Application.StartupPath, "MarketCalendar.xml");
        public static readonly string TradeLogDriectory = string.Format("{0}\\{1}", Application.StartupPath, "TradeLog");
        public static readonly string TickDataDirectory = string.Format("{0}\\{1}", Application.StartupPath, "TickData");
        public static readonly string PolicyItemsFileName = string.Format("{0}\\{1}", Application.StartupPath, "PolicyItems.xml");
        public static readonly string SysLogDirectory = string.Format("{0}\\{1}", Application.StartupPath, "SysLog");
        public static readonly string ReceiveLogFileName = string.Format("{0}\\{1}", SysLogDirectory, "ReceiveLog.txt");
        public static readonly string TradeLogFileName = string.Format("{0}\\{1}", SysLogDirectory, "TradeLog.txt");
        public static readonly string ErrorLogFileName = string.Format("{0}\\{1}", SysLogDirectory, "ErrorLog.txt");
        public static readonly string PolicyLogFileName = string.Format("{0}\\{1}", SysLogDirectory, "PolicyLog.txt");
        public static readonly string HistoryDataFileName = string.Format("{0}", inifile.GetString("System", "DataDir", ""));
    }
}
