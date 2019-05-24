using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntropyRiseStockTP0
{
    public class MessageLog
    {
        static TextBox _systemLogTextBox;

        public static TextBox SystemLogBox
        {
            set { MessageLog._systemLogTextBox = value; }
        }

        static TextBox _tradeLogTextBox;

        public static TextBox TradeLogtBox
        {
            set { MessageLog._tradeLogTextBox = value; }
        }


        static TextBox _errorLogTextBox;

        public static TextBox ErrorLogBox
        {
            set { MessageLog._errorLogTextBox = value; }
        }

        delegate void LogDelegate(string msg);

        static TextBox _policyLogTextBox;

        public static TextBox PolicyLogTextBox
        {
            set { MessageLog._policyLogTextBox = value; }
        }

        public static void SystemLog(string msg)
        {
            if (_systemLogTextBox.InvokeRequired)
            {
                _systemLogTextBox.Invoke(new LogDelegate(SystemLog), new object[] { msg });
            }
            else
            {
                if (_systemLogTextBox.Lines.Length > 3000)
                {
                    _systemLogTextBox.Clear();
                }
                string logmsg = string.Format("{0}>{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, System.Environment.NewLine);
                _systemLogTextBox.AppendText(logmsg);
                _systemLogTextBox.Focus();
                _systemLogTextBox.Select(_systemLogTextBox.TextLength, 0);
                _systemLogTextBox.ScrollToCaret();
                //Logfile("system", logmsg);

            }
        }

        private static void Logfile(string file, string logmsg)
        {
            try
            {
                StreamWriter sw = new StreamWriter(string.Format("{0}\\{1}.txt", Application.StartupPath, file), true, Encoding.UTF8);
                sw.Write(logmsg);
                sw.Close();
            }
            catch { }
        }

        public static void TradeLog(string msg)
        {
            if (_tradeLogTextBox.InvokeRequired)
            {
                _tradeLogTextBox.Invoke(new LogDelegate(TradeLog), new object[] { msg });
            }
            else
            {
                if (_tradeLogTextBox.Lines.Length > 2000)
                    _tradeLogTextBox.Clear();
                string logmsg = string.Format("{0}>{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, System.Environment.NewLine);
                _tradeLogTextBox.AppendText(logmsg);
                _tradeLogTextBox.Focus();
                _tradeLogTextBox.Select(_tradeLogTextBox.TextLength, 0);
                _tradeLogTextBox.ScrollToCaret();
                //Logfile("trade", logmsg);
            }
        }


        public static void ErrorLog(string msg)
        {
            if (_errorLogTextBox.InvokeRequired)
            {
                _errorLogTextBox.Invoke(new LogDelegate(ErrorLog), new object[] { msg });
            }
            else
            {
                if (_errorLogTextBox.Lines.Length > 2000)
                    _errorLogTextBox.Clear();
                string logmsg = string.Format("{0}>{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, System.Environment.NewLine);
                _errorLogTextBox.AppendText(logmsg);
                _errorLogTextBox.Focus();
                _errorLogTextBox.Select(_errorLogTextBox.TextLength, 0);
                _errorLogTextBox.ScrollToCaret();
                // Logfile("error", logmsg);
            }
        }

        public static void PolicyLog(string msg)
        {
            if (_policyLogTextBox.InvokeRequired)
            {
                _policyLogTextBox.Invoke(new LogDelegate(PolicyLog), new object[] { msg });
            }
            else
            {
                if (_policyLogTextBox.Lines.Length > 2000)
                {
                    _policyLogTextBox.Clear();
                }
                string logmsg = string.Format("{0}>{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg, System.Environment.NewLine);
                _policyLogTextBox.AppendText(logmsg);
                _policyLogTextBox.Focus();
                _policyLogTextBox.Select(_policyLogTextBox.TextLength, 0);
                _policyLogTextBox.ScrollToCaret();
            }
        }
    }
}
