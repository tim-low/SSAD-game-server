using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Game_Server.Database;
using Game_Server.Model.Misc;
using Game_Server.Network;
using Game_Server.Util;
using Game_Server.Util.Database.Tables;
using Game_Server.Web;

namespace Game_Server
{
    /// <summary>
    /// ServerMain is a singleton instance that instantiate two different server for our application.
    ///     - GameServer
    ///         > GameServer is the SocketServer backend that communicate with our Unity application via TCP Socket Implementation
    ///           It will handle all actions pertaining to Authentication, World, Lobby, Room and Game.
    ///     - ApiServer
    ///         > ApiServer is the RESTful API backend & HTTPServer for Unity to query our endpoint to get the list of questions & answers via JSON;
    ///           It is also in charge of rendering results on a webpage which users of our application can share on Social Media platform.
    /// </summary>
    public class ServerMain
    {
        /// <summary>
        /// Singleton instance of our ServerMain class which other classes can call to access any exposed functions
        /// </summary>
        public static readonly ServerMain Instance = new ServerMain();
        /// <summary>
        /// This is flags for enabling Logs output on the ServerConsole - [0] Outgoing | [1] Incoming
        /// </summary>
        public bool[] LogEnabled = new bool[] { false, false };
        /// <summary>
        /// This is a list containing all IP address that we want to blacklist - can be 
        /// linked to database in the future for banning of unauthorised and unlawful user
        /// </summary>
        public List<string> IPBlacklist = new List<string>();

        /// <summary>
        /// Indication on whether the ServerMain instance is running
        /// </summary>
        private bool _running;

        /// <summary>
        /// GameServer instance
        /// </summary>
        public GameServer Server { get; set; }
        /// <summary>
        /// ApiServer/HttpServer instance
        /// </summary>
        public ApiServer ApiServer { get; set; }
        /// <summary>
        /// Database instance
        /// </summary>
        public GameDatabase Database;
        public Timer LogoutTimerScript;
        /// <summary>
        /// GameConf for GameServer
        /// </summary>
        public GameConf Config { get; set; }

        public Dictionary<int, Annoucement> Annoucement;

        public int LootRate
        {
            get
            {
                return Config.Game.LootBoxRate;
            }
            set
            {
                Config.Game.LootBoxRate = value;
            }
        }

        public int BaseExp
        {
            get
            {
                return Config.Game.BaseExp;
            }
            set
            {
                Config.Game.BaseExp = value;
            }
        }

        public int ExpRate
        {
            get
            {
                return Config.Game.ExpRate;
            }
            set
            {
                Config.Game.ExpRate = value;
            }
        }


        private ServerMain()
        {
        }

        public void Run()
        {
            if (_running)
                throw new Exception("Server is already running.");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ConsoleHelper.WriteHeader("Test", ConsoleColor.Green);
            Annoucement = new Dictionary<int, Annoucement>();
            Log.Info("Server startup requested");
            Log.Info($"Server Version {Util.Version.GetVersion()}");

            NavigateToRootFolder();

            LoadConf(Config = new GameConf());

            LoadErrorMessages();

            InitDatabase(Database = new GameDatabase(), Config);
            ServerMain.Instance.LoadAnnoucement();
            /*
            if(Config.Game.MockData)
            {
                AccountTable = new MockAccount();
                CharacterTable = new MockCharacter();
                Log.Info("Loaded Mock Data");
            }
            else
            {

                AccountTable = new Accounts();
                CharacterTable = new Characters();

            }
            */

            if (Config.Game.LoadPacketDatabase)
            {
                if (GameServer.PacketNameDatabase == null)
                {
                    GameServer.PacketNameDatabase = new Dictionary<ushort, string>();
                    if (File.Exists("system/conf/packets.txt"))
                    {
                        var src = File.ReadAllText("system/conf/packets.txt");

                        foreach (var line in src.Split('\n'))
                        {
                            if (line.Length <= 3) continue;
                            var lineSplit = line.Split(':');

                            var id = ushort.Parse(lineSplit[0]);

                            GameServer.PacketNameDatabase[id] = lineSplit[1].Trim().Split('_')[1];
                        }
                    }
                }
            }

            //AccountTable.Load(Database.Connection);
            //CharacterTable.Load(Database.Connection);

//            QuizTable = new Quizzes();
 //           QuizTable.Load(Database.Connection);

            Server = new GameServer(Config.Ip.ServerIp, Config.Ip.ServerPort, Config.Game.Encryption, Config.Game.IsTeacherComponentEnabled);
            Server.Start();

            ApiServer = ApiServer.Create(Config.Ip);
            ApiServer.Toggle();
            ConsoleHelper.RunningTitle();
            _running = true;

            watch.Stop();

            Log.Info("Ready after {0}ms", watch.ElapsedMilliseconds);

            // Start logging
            Log.Archive = "archive";
            Log.LogFile = "server.log";

            // LogoutTimerScript = new Timer(ExecuteLogout, null, 0, 60000);
            //var x = new ConsoleCommands();
            //x.Wait();


        }

        private void LoadErrorMessages()
        {
            int i = 0;
            using (FileReader msgFile = new FileReader("system/conf/error.txt"))
            {
                foreach (var line in msgFile)
                {
                    var pos = -1;

                    // Check for seperator
                    if ((pos = line.Value.IndexOf(':')) < 0)
                        return;

                    string key = line.Value.Substring(0, pos).Trim();
                    string value = line.Value.Substring(pos + 1).Trim();

                    Constant.AddMessage(Int32.Parse(key), value);
                    i++;
                }
            }
            Log.Info("Loaded {0} messages.", i);
        }

        public async void LoadAnnoucement()
        {
            List<AnnoucementRecord> annoucements = await ServerMain.Instance.Database.LoadAnnoucement();
            foreach(var record in annoucements)
            {
                Annoucement annoucementModel = new Annoucement();
                annoucementModel.FromEntity(record);
                if (annoucementModel.IsActive())
                {
                    Annoucement.Add(record.Id, annoucementModel);
                }
            }
            // apply the latest annoucement event
            var annoucement = Annoucement.Values.OrderByDescending(a => a.StartDate).FirstOrDefault();
            if(annoucement != null)
            {
                foreach(var eventx in annoucement.Benefits)
                {
                    switch(eventx.Type)
                    {
                        case EventType.ExpRate:
                            this.ExpRate = eventx.Value;
                            break;
                        case EventType.BaseExp:
                            this.BaseExp = eventx.Value;
                            break;
                        case EventType.LootRate:
                            this.LootRate = eventx.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void ShowRate()
        {
            Log.Info("Exp Rate: {0}", ExpRate);
            Log.Info("Base Rate: {0}", BaseExp);
            Log.Info("Loot Rate: {0}", LootRate);
        }

        /// <summary>
        /// Navigating towards the root folder of our application, so we can access the config file.
        /// </summary>
        private static void NavigateToRootFolder()
        {
            for (var i = 0; i < 3; ++i)
            {
                if (Directory.Exists("system"))
                    return;

                Directory.SetCurrentDirectory("..");
            }

            Log.Error("Unable to find root directory.");
        }

        /// <summary>
        /// Load config file from the path stated in BaseConf
        /// </summary>
        /// <param name="conf">Config File containing the path of every file that need to be loaded</param>
        private static void LoadConf(BaseConf conf)
        {
            Log.Info("Reading configuration...");

            try
            {
                conf.Load();
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "Unable to read configuration. ({0})", ex.Message);
            }
        }

        /// <summary>
        /// Initialize the Database using parameters retrieved from BaseConf
        /// </summary>
        /// <param name="db">Database Connection</param>
        /// <param name="conf">Config File</param>
        protected static void InitDatabase(GameDatabase db, BaseConf conf)
        {
            Log.Info("Initializing database...");

            try
            {
                db.Init(conf.Database.Host, conf.Database.Port, conf.Database.User, conf.Database.Pass, conf.Database.Db);
            }
            catch (Exception ex)
            {
                db = null;
                Log.Error("Unable to open database connection. ({0})", ex.Message);
            }
        }
    }
}
