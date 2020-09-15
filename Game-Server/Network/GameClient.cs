using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Game_Server.Model;
using Game_Server.Util;
using SADCrypt;

namespace Game_Server.Network
{
    /// <summary>
    /// GameClient for a Multi-Thread server
    /// This class contains every object that the server need to keep track of the Unity client
    /// Server will be using this to validate anything data sent from the client.
    ///
    /// We will be sending this over to client via the network stream. However, it will be a redacted version of GameClient,
    /// containing only vital information needed to render the required UI on the client side.
    /// </summary>
    public class GameClient : ISerializable, GameObject
    {
        private GameServer _parent;
        private TcpClient _tcp;
        private NetworkStream _ns;
        private Supercrypt _crypt;

        /// <summary>
        /// Queue to store packets which are queued before sending to user
        /// </summary>
        private Queue<Packet> _queue;

        /// <summary>
        /// flags to indicate whether there is any packet in the queue
        /// </summary>
        private bool awaitingQueue = false;

        /// <summary>
        /// Used for delaying packet sending
        /// </summary>
        private Timer delayTimer;

        /// <summary>
        /// flags for pinging the client
        /// </summary>
        private bool IsAckReceived;

        private bool _teacherApp;

        private bool _connected = false;
        private byte[] _buffer, packetId;
        private int _bytesToRead;

        private ushort _packetLength;

        public Character Character;

        private Timer _socketWorker;

        private bool _encrypt;

        private long TimeSinceLastPacket { get; set; }



        public GameClient(TcpClient tcp, GameServer server, bool exchangeRequired)
        {
            _tcp = tcp;
            _parent = server;
            _ns = tcp.GetStream();
            _crypt = new Supercrypt();
            _queue = new Queue<Packet>();
            TimeSinceLastPacket = DateTime.Now.Ticks;
            _socketWorker = new Timer(PingClient, this, 10000, 5000);
            IsAckReceived = true;
            _connected = true;
            _encrypt = exchangeRequired;
            Log.Info("Client {0} connected.", tcp.Client.RemoteEndPoint.ToString());
            try
            {
                if (ServerMain.Instance.Server.IsTeacherComponentEnabled || exchangeRequired)
                {
                    _buffer = new byte[4];
                    _bytesToRead = _buffer.Length;
                    _ns.BeginRead(_buffer, 0, 4, OnHandShake, null);
                }
                else
                {
                    _buffer = new byte[4];
                    _bytesToRead = _buffer.Length;
                    _ns.BeginRead(_buffer, 0, 4, OnHeader, null);
                }
            }
            catch (Exception ex)
            {
                KillConnection(ex);
            }
        }

        private void PingClient(object state)
        {
            //if (IsAckReceived == false)
                //this.KillConnection("Not Receiving Ack");
            if((DateTime.Now - new DateTime(TimeSinceLastPacket)).TotalSeconds > 10)
            {
                TimeSinceLastPacket = DateTime.Now.Ticks;
                this.Send(new NullPing().CreatePacket());
                this.IsAckReceived = false;
            }
        }

        public void Acknowledge()
        {
            this.IsAckReceived = true;
        }

        ~GameClient()
        {
            KillConnection();
        }

        public bool IsTeacherClient()
        {
            return _teacherApp;
        }

        public IPEndPoint EndPoint => _tcp?.Client?.RemoteEndPoint as IPEndPoint;

        private void OnHandShake(IAsyncResult result)
        {
            try
            {
                _bytesToRead -= _ns.EndRead(result);
                if (_bytesToRead > 0)
                {
                    _ns.BeginRead(_buffer, _buffer.Length - _bytesToRead, _bytesToRead, OnHandShake, null);
                    return;
                }
                bool isNormalClient = true;
                bool isTeacherClient = true;
                for (int i = 0; i < _buffer.Length; i++)
                {
                    if (_buffer[i] != Constant.NormalProtocolVersion[i])
                    {
                        isNormalClient = false;
                        break;
                    }
                }

                for (int i = 0; i < _buffer.Length; i++)
                {
                    if (_buffer[i] != Constant.TeacherProtocolVersion[i])
                    {
                        isTeacherClient = false;
                        break;
                    }
                }

                _teacherApp = isTeacherClient;

                if(!isNormalClient&&!isTeacherClient)
                {
                    KillConnection("Invalid client version.");
                    return;
                }



                Packet packet = new SymKeyAnswer()
                {
                    Key = _crypt.GetKey(),
                    IV = _crypt.GetIV()
                }.CreatePacket();

                this.Send(packet, false);


                _buffer = new byte[4];
                _bytesToRead = _buffer.Length;
                _ns.BeginRead(_buffer, 0, 4, OnHeader, null);
            }
            catch (Exception ex)
            {
                KillConnection(ex);
            }
        }

        private void OnHeader(IAsyncResult result)
        {
            try
            {
                _bytesToRead -= _ns.EndRead(result);
                if (_bytesToRead > 0)
                {
                    _ns.BeginRead(_buffer, _buffer.Length - _bytesToRead, _bytesToRead, OnHeader, null);
                    return;
                }

                _packetLength = BitConverter.ToUInt16(_buffer, 0);
                // Save the packetId to decrypt together with data
                packetId = _buffer.Skip(2).ToArray();
                // Read byte

                _bytesToRead = _packetLength - 4;
                _buffer = new byte[_bytesToRead];
                _ns.BeginRead(_buffer, 0, _bytesToRead, OnData, null);
            }
            catch (Exception ex)
            {
                KillConnection(ex);
            }
        }

        private void OnData(IAsyncResult result)
        {
            try
            {
                _bytesToRead -= _ns.EndRead(result);
                if (_bytesToRead > 0)
                {
                    _ns.BeginRead(_buffer, _buffer.Length - _bytesToRead, _bytesToRead, OnData, null);
                    return;
                }
                // Combine the buffer for packetId and the data because we encrypt those together, we have to combine and decrypt them together
                _buffer = packetId.Concat(_buffer).ToArray();
                // Decrypt the buffer to retrieve the actual packetId and the data
                if(_encrypt)
                    _buffer = this._crypt.Decrypt(_buffer).ToArray();
                var packet = new Packet(this, BitConverter.ToUInt16(_buffer, 0), _buffer.Skip(2).ToArray());
                var hexDump = SerializeWriter.HexDump(packet.Buffer);
                if (ServerMain.Instance.LogEnabled[1] && ServerMain.Instance.IPBlacklist.Contains(this.EndPoint.Address.ToString()) == false)
                {
                    Log.Info("Handling packet {0} ({1} id {2}, 0x{2:X}) on {3} at {4}.", "",
                                    Packets.GetName(packet.Id), packet.Id, this.EndPoint.Port.ToString(), DateTime.Now.ToString());
                    Log.Info("HexDump {0}:{1}{2}", packet.Id, Environment.NewLine, hexDump);
                }
                _parent.Parse(packet);

                _buffer = new byte[4];
                _bytesToRead = _buffer.Length;
                _ns.BeginRead(_buffer, 0, 4, OnHeader, null);
            }
            catch (Exception ex)
            {
                KillConnection(ex);
            }
        }

        public void Send(Packet packet)
        {
            Send(packet, _encrypt);
        }

        private void Send(Packet packet, bool encrypt)
        {
            var buffer = packet.Writer.GetBuffer();
            if (encrypt)
                buffer = _crypt.Encrypt(packet.Writer.GetBuffer());
            var bufferLength = buffer.Length;
            var length = (ushort)(bufferLength + 2); // Length includes itself

            try
            {
                if (ServerMain.Instance.LogEnabled[0] && ServerMain.Instance.IPBlacklist.Contains(this?.EndPoint?.Address?.ToString()) == false)
                    Log.Info("[{2}] Attempting to send {0} to {1}.", Packets.GetName(packet.Id), this?._tcp?.Client?.RemoteEndPoint?.ToString(), this?.Character?.Name);
                _ns.Write(BitConverter.GetBytes(length), 0, 2);
                // Depend on what I want to do
                _ns.Write(buffer, 0, bufferLength);
                this.TimeSinceLastPacket = DateTime.Now.Ticks;
                
            }
            catch (Exception ex)
            {
                KillConnection(ex);
            }
        }

        /// <summary>
        /// Take first delay timer
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="delayInMillis"></param>
        public void SendWithDelay(Packet packet, int delayInMillis)
        {
            if(!awaitingQueue)
            {
                awaitingQueue = true;
                delayTimer = new Timer(SendDelayedPacket, this, delayInMillis, Timeout.Infinite);
            }
            _queue.Enqueue(packet);
                
        }

        private void SendDelayedPacket(object state)
        {
            awaitingQueue = false;
            GameClient client = (GameClient)state;
            while(client._queue.Count > 0)
            {
                Packet packet = client._queue.Dequeue();
                client.Send(packet);
            }
        }

        public void SendError(string format, params object[] args)
        {
            var err = new Packet(Packets.ErrorAck);
            string text = string.Format(format, args);
            err.Writer.WriteText(text);
            Send(err);
        }

        public void SendError(int messageId, params object[] args)
        {
            SendError(Constant.GetMessage(messageId), args);
        }

        public void SendChatMessage(string message)
        {

        }

        private void KillConnection(Exception ex)
        {
            if (ex is SocketException || ex is IOException)
            {
                KillConnection(ex.StackTrace);
                return;
            }
            //Log.Info(ex.Message + ": " + ex.StackTrace);
            // No killing of client unless is Socket/IOException
            KillConnection(ex.Message + ": " + ex.StackTrace);
        }

        public void KillConnection(string reason = "")
        {
            if (!_connected) return;
            _connected = false;
#if !DEBUG
            if(reason != "Socket or IO Exception")
#endif
            {
                Log.Info("Killing off client {1}. {0}", reason, _tcp?.Client?.RemoteEndPoint?.ToString());
            }
            _tcp.Close();
            // Cleanup and despawn character on the client
            if (Character != null)
            {
                if (Character.Status.GetState() != null)
                    Character.Status.GetObject<IDisconnect>().Disconnect(this);
                Logout();
            }
            _parent.Disconnect(this);
        }

        public void Logout()
        {
            Character.CharacterDb.Account.IsLoggedIn = 0;
            _parent.LoggedInClient.Remove(Character.Token);
            ServerMain.Instance.Database.Save();
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.Write(this.Character);
        }

        public string GetIdentifier()
        {
            if(Character == null)
            {
                return String.Empty;
            }
            return Character.Token;
        }
    }
}

// Game Client