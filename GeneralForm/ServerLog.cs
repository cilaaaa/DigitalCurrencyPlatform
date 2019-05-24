
namespace GeneralForm
{
    public static class ServerLog
    {
        public static frm_ReceiveLog frm;
        public static void Log(string log,bool writeFile)
        {
            frm.Log(log,writeFile);
        }

        public static void LogServer(string log)
        {
            frm.LogServer(log);
        }

        public static void ClearList()
        {
            frm.ClearList();
        }

        public static void AddList(string name, string status, string server)
        {
            frm.AddList(name,status,server);
        }

        public static void UpdateConnecting(string name)
        {
            frm.UpdateConnecting(name);
        }

        public static void ErrorConnect(string name)
        {
            frm.ErrorConnect(name);
        }



        public static void Connected(string name, string server)
        {
            frm.Connected(name,server);
        }
    }
}
