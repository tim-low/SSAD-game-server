using System;
namespace Game_Server.Util
{
    public class GameConf : BaseConf
    {
        public GameConf()
        {
            Game = new GameConfFile();
        }

        /// <summary>
        ///     login.conf
        /// </summary>
        public GameConfFile Game { get; protected set; }

        public override void Load()
        {
            LoadDefault();
            Game.Load();
        }
    }

    public class GameConfFile : ConfFile
    {
        public bool MockData { get; set; }
        public bool LoadPacketDatabase { get; set; }

        public bool Encryption { get; set; }

        public bool IsTeacherComponentEnabled { get; set; }
        public int LootBoxRate { get; set; }

        public int BaseExp { get; set; }

        public int ExpRate { get; set; }

        public void Load()
        {
            Require("system/conf/game.conf");

            MockData = GetBool("mock", false);
            LoadPacketDatabase = GetBool("load", false);
            Encryption = GetBool("encryption", true);
            IsTeacherComponentEnabled = GetBool("enable_teacher", false);
            LootBoxRate = GetInt("lootrate", 1);
            BaseExp = GetInt("baseexp", 500);
            ExpRate = GetInt("exprate", 1);
        }
    }
}
