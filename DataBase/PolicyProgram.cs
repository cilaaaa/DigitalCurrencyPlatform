using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;


//
//byte[ ] 转换为string
//byte[ ] image;
//string ll = Encoding.Default.GetString(image);
//string 转换为byte[ ]
//string ss;
//byte[] b = Encoding.Default.GetBytes(ss);
//
namespace DataBase
{
    public class PolicyProgram
    {
        static List<PolicyProgram> _programList;
        static XmlDocument _xmlDoc;
        //static string _filename = string.Format("{0}\\{1}", Application.StartupPath, "PolicyProgram.xml");
        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _desc;

        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }
        byte[] _program;

        public byte[] Program
        {
            get { return _program; }
            set { _program = value; }
        }

        public static void Load()
        {
            _programList = new List<PolicyProgram>();
            _xmlDoc = new XmlDocument();

            if (!File.Exists(ConfigFileName.PolicyProgramFileName))
            {
                StreamWriter sw = new StreamWriter(ConfigFileName.PolicyProgramFileName, true, Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<PolicyProgram>");
                sw.WriteLine("<Items>");
                sw.WriteLine("</Items>");
                sw.WriteLine("</PolicyProgram>");
                sw.Close();
            }
            _xmlDoc.Load(ConfigFileName.PolicyProgramFileName);
            XmlNode root = _xmlDoc.SelectSingleNode("PolicyProgram");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList programs = item.SelectNodes("Program");

            foreach (XmlNode program in programs)
            {

                string name = program.Attributes["name"].Value;
                string desc = program.SelectSingleNode("Desc").FirstChild.Value;
                string codestring = program.SelectSingleNode("Code").InnerText;
                PolicyProgram pp = new PolicyProgram();
                pp._name = name;
                pp._desc = desc;
                pp._program = Convert.FromBase64String(codestring);
                _programList.Add(pp);
            }

        }

        public static byte[] getProgram(string dllName)
        {
            for (int i = 0; i < _programList.Count; i++)
            {
                if (_programList[i]._name.ToUpper() == dllName.ToUpper())
                {
                    return _programList[i]._program;
                }
            }
            return new byte[1];
        }

        public static DataTable getProgramList()
        {
            DataTable list = new DataTable("Program");
            list.Columns.Add("dll_name");
            list.Columns.Add("dll_desc");
            for (int i = 0; i < _programList.Count; i++)
            {
                DataRow dr = list.NewRow();
                dr[0] = _programList[i]._name;
                dr[1] = _programList[i]._desc;
                list.Rows.Add(dr);
            }
            return list;
        }

        public static bool exists(string dllName)
        {
            for (int i = 0; i < _programList.Count; i++)
            {
                if (_programList[i]._name == dllName)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateDLL(string dllname, byte[] program, string desc)
        {
            for (int i = 0; i < _programList.Count; i++)
            {
                if (_programList[i]._name == dllname)
                {
                    _programList[i]._desc = desc;
                    _programList[i]._program = program;
                    UpdateXML(dllname, program, desc);
                    return;
                }
            }
        }

        public static void InsertDLL(string dllname, byte[] program, string desc)
        {
            PolicyProgram pp = new PolicyProgram();
            pp._name = dllname;
            pp._program = program;
            pp._desc = desc;
            _programList.Add(pp);
            InsertXML(dllname, program, desc);
        }

        private static void InsertXML(string dllname, byte[] program, string desc)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("PolicyProgram");
            XmlNode item = root.SelectSingleNode("Items");
            XmlElement ep = _xmlDoc.CreateElement("Program");
            ep.SetAttribute("name", dllname);
            XmlNode nd = _xmlDoc.CreateElement("Desc");
            nd.InnerText = desc;
            ep.AppendChild(nd);
            XmlNode np = _xmlDoc.CreateElement("Code");
            np.AppendChild(_xmlDoc.CreateCDataSection(Convert.ToBase64String(program)));
            ep.AppendChild(np);
            item.AppendChild(ep);
            _xmlDoc.Save(ConfigFileName.PolicyProgramFileName);

        }

        public static void DeleteDLL(string dllname)
        {
            for (int i = 0; i < _programList.Count; i++)
            {
                if (_programList[i]._name == dllname)
                {
                    _programList.RemoveAt(i);
                    DeleteXML(dllname);
                    return;
                }
            }
        }

        private static void DeleteXML(string dllname)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("PolicyProgram");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList programs = item.SelectNodes("Program");
            for (int i = 0; i < programs.Count; i++)
            {
                if (programs[i].Attributes["name"].Value == dllname)
                {
                    item.RemoveChild(programs[i]);
                    break;
                }
            }
            _xmlDoc.Save(ConfigFileName.PolicyProgramFileName);
        }


        private static void UpdateXML(string dllname, byte[] source, string desc)
        {
            XmlNode root = _xmlDoc.SelectSingleNode("PolicyProgram");
            XmlNode item = root.SelectSingleNode("Items");
            XmlNodeList programs = item.SelectNodes("Program");
            foreach (XmlNode program in programs)
            {
                if (program.Attributes["name"].Value == dllname)
                {
                    program.SelectSingleNode("Desc").FirstChild.Value = desc;
                    ((XmlCDataSection)program.SelectSingleNode("Code").FirstChild).Value = Convert.ToBase64String(source);
                    //XmlNode descnode = program.SelectSingleNode("Desc");
                    //descnode.InnerText = desc;
                    break;
                }
            }
            _xmlDoc.Save(ConfigFileName.PolicyProgramFileName);
        }

        public static string getPolicyName(string programname)
        {
            for (int i = 0; i < _programList.Count; i++)
            {
                if (_programList[i]._name == programname)
                {
                    return _programList[i]._desc;
                }
            }
            return string.Empty;
        }
    }
}
