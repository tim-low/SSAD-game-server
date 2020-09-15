using System;
namespace Game_Server.Util
{
    public abstract class BaseConf : ConfFile
    {
        protected BaseConf()
        {
            Ip = new IPConf();
            Database = new DatabaseConf();
        }


        /// <summary>
        ///     ip.conf
        /// </summary>
        public IPConf Ip { get; protected set; }

        /// <summary>
        ///     database.conf
        /// </summary>
        public DatabaseConf Database { get; }

        /// <summary>
        ///     Loads several conf files that are generally required,
        ///     like log, database, etc.
        /// </summary>
        protected void LoadDefault()
        {
            Ip.Load();
            Database.Load();
        }

        public abstract void Load();
    }
}
