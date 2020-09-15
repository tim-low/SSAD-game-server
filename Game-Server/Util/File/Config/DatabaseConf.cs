using System;
namespace Game_Server.Util
{
    public class DatabaseConf : ConfFile
    {
        public string Host { get; protected set; }
        public int Port { get; protected set; }
        public string User { get; protected set; }
        public string Pass { get; protected set; }
        public string Db { get; protected set; }


        public void Load()
        {
            Require("system/conf/database.conf");

            Host = GetString("host", "127.0.0.1");
            Port = GetInt("port", 3306);
            User = GetString("user", "root");
            Pass = GetString("pass", "xxxx");
            Db = GetString("database", "xxx");
        }
    }
}
