using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DataBase
{
    public class IniFile
    {
        //INI文件名
        private string fileName;
        //声明读写INI文件的API函数 
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(
           string lpAppName,
           string lpKeyName,
           int nDefault,
           string lpFileName
           );
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpDefault,
           StringBuilder lpReturnedString,
           int nSize,
           string lpFileName
           );
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpString,
           string lpFileName
           );
        //类的构造函数，传递INI文件名  
        public IniFile(string filename)
        {
            fileName = filename;
        }
        //读整数
        public int GetInt(string section, string key, int def)
        {
            return GetPrivateProfileInt(section, key, def, fileName);
        }
        //读取INI文件指定  
        public string GetString(string section, string key, string def)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, temp, 1024, fileName);
            return temp.ToString();
        }
        //写入整数
        public void WriteInt(string section, string key, int iVal)
        {
            WritePrivateProfileString(section, key, iVal.ToString(), fileName);
        }
        //写INI文件  
        public void WriteString(string section, string key, string strVal)
        {
            WritePrivateProfileString(section, key, strVal, fileName);
        }
        //删除某个Section下的键  
        public void DelKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, fileName);
        }
        //清除某个Section 
        public void DelSection(string section)
        {
            WritePrivateProfileString(section, null, null, fileName);
        }
    }
}
