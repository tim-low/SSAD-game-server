using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Game_Server.Controller.Manager;
using Game_Server.Manager;
using Game_Server.Model;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class GameServer : IBroadcaster, IDisconnect
    {
        /// <summary>
        /// A dictionary during debugging to ensure packet are received and send properly
        /// </summary>
        public static Dictionary<ushort, string> PacketNameDatabase = new Dictionary<ushort, string>();

        private readonly List<GameClient> _clients;
        private readonly bool _exchangeRequired;
        private readonly bool _teacherComponent;

        public bool IsTeacherComponentEnabled
        {
            get { return _teacherComponent; }
        }

        /// <summary>
        /// The listener for any tcp connection that connect to our server
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        /// Contains all parsers and their function
        /// </summary>
        private readonly Dictionary<ushort, Action<Packet>> _parsers;

        /// <summary>
        /// The port the server runs on
        /// </summary>
        private readonly int _port;

        private Dictionary<string, Quiz> _quizzes;

        public LobbyManager LobbyManager;

        public GameManager GameManager;

        /// <summary>
        /// List of packet that won't be display on the client but will be logged instead
        /// </summary>
        public static readonly List<ushort> PacketDumpBlacklist = new List<ushort>()
        {
            Packets.CmdNullPing,
            Packets.CmdMovePlayer
        };

        /// <summary>
        /// This is for blacklisting certain ip, so that we won't logged any record from this ip. DEBUG PURPOSE
        /// </summary>
        public static readonly List<IPEndPoint> IPDumpBlacklist = new List<IPEndPoint>()
        {
        };

        public Dictionary<string, GameClient> LoggedInClient = new Dictionary<string, GameClient>();

        /// <summary>
        /// GameServer Constructor
        /// Initialize all the instance variable
        /// </summary>
        /// <param name="port">portNo which the server will run on.</param>
        /// <param name="exchangeRequired">whether we do any verification on our client</param>
        public GameServer(string ipAddr, int port, bool exchangeRequired = false, bool isTeacherComponentEnable = false)
        {
            _port = port;
            _exchangeRequired = exchangeRequired;
            _teacherComponent = isTeacherComponentEnable;
            _quizzes = new Dictionary<string, Quiz>();
            _parsers = new Dictionary<ushort, Action<Packet>>();
            _clients = new List<GameClient>();
            LobbyManager = new LobbyManager();
            GameManager = new GameManager();
            _port = port;
            _listener = new TcpListener(IPAddress.Parse(ipAddr), port);
            var i = 0;
            i += AddAllMethodsFromType(Assembly.GetExecutingAssembly().GetTypes());
#if DEBUG
            Log.Info("Added {0} packets", i);
#endif
        }

        /// <summary>
        /// Start the listener for incoming TCP connections
        /// </summary>
        public void Start()
        {
            Log.Info("Starting network", _port);
            _listener.Start();
            Log.Info("Network started on port {0}", _port);
            Log.Info("Encryption Status: {0}", _exchangeRequired ? "Enabled" : "Disabled");
            _listener.BeginAcceptTcpClient(OnAccept, _listener);
        }
        #region PACKET_PROCESSING_AND_HANDLING

        /// <summary>
        /// This method loop through the entire solution for any possible method with the Packet Attribute and
        /// add it to the list of available packet data;
        /// </summary>
        /// <param name="types"></param>
        /// <returns>how many packets have been added to be parsed</returns>
        private int AddAllMethodsFromType(IEnumerable<Type> types)
        {
            var i = 0;
            // Go through each type
            foreach (var type in types)
                // Go through each method in the type
                foreach (var method in type.GetMethods())
                    // Check if there is a attribute call PacketAttribute in the method
                    foreach (var boxedAttrib in method.GetCustomAttributes(typeof(PacketAttribute), false))
                    {
                        // Convert the attribute into PacketAttribute so we can use attribute to our advantage
                        var attrib = boxedAttrib as PacketAttribute;

                        if (attrib == null) continue;
                        // Get the ID of the attribute which is the packet id
                        var id = attrib.Id;
                        // Set the method and the action for the packet.
                        var parser = (Action<Packet>)Delegate.CreateDelegate(typeof(Action<Packet>), method);

                        SetParser(id, parser);

#if DEBUG
                        i++;
#endif
                    }
            return i;
        }

        /// <summary>
        /// This setup the listener for any packet that we have al
        /// handled. This is a self-adding parser,
        /// as long as you setup a handler, it will automatically get added in.
        /// </summary>
        /// <param name="id">unique identifier of the packet</param>
        /// <param name="parser">the listener and processor of the packet</param>
        private void SetParser(ushort id, Action<Packet> parser)
        {
            if (_parsers.ContainsKey(id))
            {
                    Log.Error("Duplicated parser for packet ({0} {1} : 0x{1:X}).", Packets.GetName(id), id);
            }
#if DEBUG
            Log.Info("Loaded ({0} {1} : 0x{1:X}).", Packets.GetName(id), id);
#endif
            _parsers[id] = parser;
        }

        /// <summary>
        /// Process the packet by running the action according to the packet identifier
        /// </summary>
        /// <param name="packet">byte[] object contains the identifier and the buffer</param>
        public void Parse(Packet packet)
        {
            // Handle the packet.
            if (_parsers.ContainsKey(packet.Id))
            {
                // Implementing Error Handling
                _parsers[packet.Id](packet);

            }
            else
            {
                Log.Warning("Received unhandled packet {0} (id {1}, 0x{1:X}) on {2}.", Packets.GetName(packet.Id), packet.Id, _port);
            }
        }

#endregion

#region TcpClientHandler

        /// <summary>
        /// Async Code for accepting the TcpClient
        /// </summary>
        /// <param name="result"></param>
        private void OnAccept(IAsyncResult result)
        {
            // Get the TcpClient that was connected to our server
            var x = (TcpListener)result.AsyncState;
            var tcpClient = x.EndAcceptTcpClient(result);
            // Wrap the TcpClient into our managed client
            var client = new GameClient(tcpClient, this, _exchangeRequired);
            // Add the managed client into our list of connected client
            _clients.Add(client);
            // Listen for next incoming client connection
            _listener.BeginAcceptTcpClient(OnAccept, _listener);
        }

#endregion

#region GameServer Helper Method

        public GameClient GetClient(string token)
        {
            return _clients.FirstOrDefault(client => client?.Character?.Token == token);
        }
        /// <summary>
        /// Gets all clients connected to the server
        /// </summary>
        /// <returns>A collection of all clients connected to the server</returns>
        public IEnumerable<GameClient> GetClients() => _clients.ToArray();

        /// <summary>
        /// Broadcasts a packet to all clients on the server, excluding exclude if specified
        /// </summary>
        /// <param name="packet">The packet to broadcast</param>
        /// <param name="exclude">The client to exclude</param>
        public void Broadcast(Packet packet, GameClient exclude = null)
        {
            foreach (var client in GetClients())
                if (exclude == null || client != exclude)
                    client.Send(packet);
        }

        /// <summary>
        /// Broadcasts a packet to all clients on the server, excluding exclude if specified
        /// </summary>
        /// <param name="packet">The packet to broadcast</param>
        /// <param name="exclude">An array of clients that should be exluded</param>
        public void Broadcast(Packet packet, GameClient[] exclude)
        {
            foreach (var client in GetClients())
                if (!exclude.Contains(client))
                    client.Send(packet);
        }

        /// <summary>
        /// Broadcasts a packet to all clients in the same game the gameIdll
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="packet"></param>
        public void BroadcastChannel(string gameId, Packet packet)
        {
            foreach (var client in GetClients())
            {
                if (client.Character?.Status?.GetState() == typeof(Game) && client.Character?.Status?.GetIdentifier() == gameId)
                    client.Send(packet);
            }
        }

        /// <summary>
        /// Broadcasts a packet to the owner in a room
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="packet"></param>
        public void BroadcastRoom(string roomId, Packet packet)
        {
            foreach (var client in GetClients())
            {
                if (client.Character.Status.GetState() == typeof(Room) && client.Character?.Status.GetIdentifier() == roomId)
                    client.Send(packet);
            }
        }

        /// <summary>
        /// Broadcasts a packet to all clients in the lobby
        /// </summary>
        public void BroadcastLobby(string lobbyId, Packet packet)
        {
            foreach (var client in GetClients())
            {
                if (client.Character?.Status?.GetState() == typeof(Lobby) && client.Character?.Status?.GetIdentifier() == lobbyId)
                    client.Send(packet);
            }
        }

#endregion

#region Quiz
        public Quiz GetQuiz(string identifier)
        {
            Quiz quiz;
            if (_quizzes.TryGetValue(identifier, out quiz))
                return quiz;
            return null;
        }
#endregion

#region Client
        public void Disconnect(GameClient client)
        {
            this._clients.Remove(client);
        }
        #endregion
    }
}
// Game Server