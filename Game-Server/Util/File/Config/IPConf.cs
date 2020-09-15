using System;
namespace Game_Server.Util
{
    public class IPConf : ConfFile
    {
        public string ServerIp { get; protected set; }
        public int ServerPort { get; protected set; }

        public void Load()
        {
            Require("system/conf/ip.conf");

            ServerIp = GetString("serverIp", "127.0.0.1");
            ServerPort = GetInt("serverPort", 12041);
            
        }
    }
}
