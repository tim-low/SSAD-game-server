using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game_Server.Controller;
using Game_Server.Controller.Database.Tables;
using Game_Server.Model;
using Game_Server.Network;
//using LoadTestingRunner;
using Microsoft.EntityFrameworkCore;

namespace Game_Server.Util
{
    public class ConsoleCommands : CommandManager<ConsoleCommand, ConsoleCommandFunc>
    {
        public ConsoleCommands()
        {
            Commands = new Dictionary<string, ConsoleCommand>();

            Add("help", "Displays this help", HandleHelp);
            Add("exit", "Closes application/server", HandleExit);
            Add("status", "Displays application status", HandleStatus);
            Add("connections", "Displays all currently active connections", HandleConnections);
            Add("api", "Start/Stop the WebServer", HandleApi);
            Add("log", "[output|input]", "Turn on/off the output and input log", HandleLog);
#if DEBUG
            Add("reset", "[DEBUG] Reset the server as if it just started", HandleReset);
            Add("sendpkt", "[packetid] [nullbyte count]", "[DEBUG] Sends an empty packet with the specified packet id",
                HandleSendPkt);
            Add("crash", "[DEBUG] Crashes the server", (command, args) => throw new Exception("Test Exception"));
            Add("send", "[DEBUG] Show latest game board", HandleSend);
            Add("event", "[exp|loot|giveloot] [value] [event name]", HandleEvent);
#endif
        }

        private CommandResult HandleEvent(string command, IList<string> args)
        {
            if(args.Count > 2)
            {
                if (args[1] == "exp" || args[1] == "loot" || args[1] == "giveloot")
                {
                    int value;
                    if(Int32.TryParse(args[2], out value))
                    {
                        switch(args[1])
                        {
                            case "exp":
                                ServerMain.Instance.ExpRate = value;
                                break;
                            case "giveloot":
                                foreach(GameClient c in ServerMain.Instance.Server.GetClients())
                                {
                                    if(c.Character != null)
                                    {
                                        c.Character.CharacterDb.ChestCount += value;
                                    }
                                }
                                break;
                            default:
                                ServerMain.Instance.LootRate = value;
                                break;
                        }
                        var ack = new ServerNoticeAck()
                        {
                            EventMessage = string.Join(' ', args.Skip(3).ToArray())
                        };
                        ServerMain.Instance.Server.Broadcast(ack.CreatePacket());
                    }
                    else
                    {
                        return CommandResult.InvalidArgument;
                    }
                    return CommandResult.Okay;
                }
                else
                {
                    return CommandResult.InvalidArgument;
                }
            }
            else
            {
                return CommandResult.InvalidArgument;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected CommandResult HandleReset(string command, IList<string> args)
        {
            int i = 0;
            if (args.Count() > 1)
            {
                var client = ServerMain.Instance.Server.GetClients().Where(c => c.Character.Name == args[0]).FirstOrDefault();
                if (client != null)
                {
                    client.KillConnection("Server dc you");
                    ++i;
                }
            }
            else
            {
                foreach (GameClient c in ServerMain.Instance.Server.GetClients())
                {
                    c.KillConnection("Server forced close");
                    ++i;
                }
            }
            Log.Info("{0} clients have been closed.", i);
            ServerMain.Instance.Database.ForcedLogout();
            return CommandResult.Okay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected CommandResult HandleSend(string command, IList<string> args)
        {
            Log.Debug("{0}", Newtonsoft.Json.JsonConvert.SerializeObject(ServerMain.Instance.Database.GetQuizStats()));
            return CommandResult.Okay;
        }

        protected CommandResult HandleLog(string command, IList<string> args)
        {
            if (args[1] == "output")
            {
                ServerMain.Instance.LogEnabled[0] = ServerMain.Instance.LogEnabled[0] != true;
                Log.Info("Packet Sent Log Enabled: {0}", ServerMain.Instance.LogEnabled[0]);
            }
            else if(args[1] == "input")
            {
                ServerMain.Instance.LogEnabled[1] = ServerMain.Instance.LogEnabled[1] != true;
                Log.Info("Packet Received Log Enabled: {0}", ServerMain.Instance.LogEnabled[1]);
            }
            else if(args[1] == "ip")
            {
                if(ServerMain.Instance.IPBlacklist.Contains(args[2]))
                {
                    ServerMain.Instance.IPBlacklist.Remove(args[2]);
                    Log.Info("IP({0}) have been whitelisted. You will start receiving packet dump from this IP.", args[2]);
                    //added logging function either to blacklist or add 
                }
                else
                {
                    ServerMain.Instance.IPBlacklist.Add(args[2]);
                    Log.Info("IP({0}) have been blacklisted. You will not received any packet dump from this IP.", args[2]);
                }
            }
            return CommandResult.Okay;
        }

        /// <summary>
        ///     Adds new command handler.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="handler"></param>
        public void Add(string name, string description, ConsoleCommandFunc handler)
        {
            Add(name, "", description, handler);
        }

        /// <summary>
        ///     Adds new command handler.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="usage"></param>
        /// <param name="description"></param>
        /// <param name="handler"></param>
        public void Add(string name, string usage, string description, ConsoleCommandFunc handler)
        {
            Commands[name] = new ConsoleCommand(name, usage, description, handler);
        }

        public void Wait()
        {
            // Just wait if not running in a console
            if (!ConsoleHelper.UserInteractive)
            {
                var reset = new ManualResetEvent(false);
                reset.WaitOne();
                return;
            }

            Log.Info("Type 'help' for a list of console commands.");

            while (true)
            {
                var line = Console.ReadLine();

                var args = ConsoleHelper.ParseLine(line);
                if (args.Count == 0)
                    continue;

                var command = GetCommand(args[0]);
                if (command == null)
                {
                    Log.Info("Unknown command '{0}'", args[0]);
                    continue;
                }

                var result = command.Func(line, args);
                if (result == CommandResult.Break)
                    break;
                if (result == CommandResult.Fail)
                    Log.Error("Failed to run command '{0}'.", command.Name);
                else if (result == CommandResult.InvalidArgument)
                    Log.Info("Usage: {0} {1}", command.Name, command.Usage);
            }
        }

        protected virtual CommandResult HandleApi(string command, IList<string> args)
        {
            ServerMain.Instance.ApiServer.Toggle();
            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleSendPkt(string command, IList<string> args)
        {
            Log.Debug("{0}", Newtonsoft.Json.JsonConvert.SerializeObject(ServerMain.Instance.Database.GetTopicMasteryStats())); //TODO is for testing, remove later
            return CommandResult.Fail;
        }

        protected virtual CommandResult HandleConnections(string command, IList<string> args)
        {
            int i = 0;
            foreach(GameClient c in ServerMain.Instance.Server.GetClients())
            {
                Log.Info("{0}\t|\t{1}\t|\t{2}", ++i, c.EndPoint.Address.ToString(), c.Character == null ? "-" : c.Character.Name);
            }
            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleHelp(string command, IList<string> args)
        {
            var maxLength = Commands.Values.Max(a => a.Name.Length);

            Log.Info("Available commands");
            foreach (var cmd in Commands.Values.OrderBy(a => a.Name))
                Log.Info("  {0,-" + (maxLength + 2) + "}{1}", cmd.Name, cmd.Description);

            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleStatus(string command, IList<string> args)
        {
            Log.Status("Memory Usage: {0:N0} KB", Math.Round(GC.GetTotalMemory(false) / 1024f));
            Log.Status("No. of Clients: {0}", ServerMain.Instance.Server.GetClients().Count());
            ServerMain.Instance.ShowRate();
            return CommandResult.Okay;
        }

        protected virtual CommandResult HandleExit(string command, IList<string> args)
        {
            ConsoleHelper.Exit(0, false);
            ServerMain.Instance.Database.Save();
            return CommandResult.Okay;
        }
    }

    public class ConsoleCommand : Command<ConsoleCommandFunc>
    {
        public ConsoleCommand(string name, string usage, string description, ConsoleCommandFunc func)
            : base(name, usage, description, func)
        {
        }
    }

    public delegate CommandResult ConsoleCommandFunc(string command, IList<string> args);
}
