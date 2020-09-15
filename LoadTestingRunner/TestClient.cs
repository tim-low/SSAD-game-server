using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Game_Server.Util;
using Game_Server.Network;
using SADCrypt;
using System.Linq;
using System.IO;
using Game_Server.Model;

namespace LoadTestingRunner.Util
{
    public class TestClient : TcpClient
    {
        private NetworkStream _ns;
        private Supercrypt _crypt;
        private byte[] _buffer;
        private int _bytesToRead;
        private ushort _packetLength;
        private byte[] packetId;

        public void Listening()
        {
            // Get the network stream if it is null
            if(_ns == null)
            {
                _ns = this.GetStream();
            }
            _buffer = new byte[4];
            _bytesToRead = _buffer.Length;
            _ns.BeginRead(_buffer, 0, 4, OnKey, null);
        }

        private void OnKey(IAsyncResult ar)
        {
            try
            {
                _bytesToRead -= _ns.EndRead(ar);
                if (_bytesToRead > 0)
                {
                    _ns.BeginRead(_buffer, _buffer.Length - _bytesToRead, _bytesToRead, OnKey, null);
                    return;
                }

                _packetLength = BitConverter.ToUInt16(_buffer, 0);
                // Save the packetId to decrypt together with data
                packetId = _buffer.Skip(2).ToArray();
                // Read byte

                _bytesToRead = _packetLength - 4;
                _buffer = new byte[_bytesToRead];
                _ns.BeginRead(_buffer, 0, _bytesToRead, OnKeyData, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }



        private void OnKeyData(IAsyncResult ar)
        {
            try
            {
                _bytesToRead -= _ns.EndRead(ar);
                if (_bytesToRead > 0)
                {
                    _ns.BeginRead(_buffer, _buffer.Length - _bytesToRead, _bytesToRead, OnKeyData, null);
                    return;
                }
                var reader = new SerializeReader(new MemoryStream(_buffer));
                byte[] key = reader.ReadBytes(32);
                byte[] iv = reader.ReadBytes(16);
                _crypt = new Supercrypt(key, iv);
                _buffer = new byte[4];
                _bytesToRead = _buffer.Length;
                _ns.BeginRead(_buffer, 0, _bytesToRead, OnHeader, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
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
                Console.Error.WriteLine(ex);
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
                if (_crypt != null)
                    _buffer = this._crypt.Decrypt(_buffer).ToArray();

                var id = BitConverter.ToUInt16(_buffer, 0);
                var data = new SerializeReader(new MemoryStream(_buffer.Skip(2).ToArray()));
                var hexDump = SerializeWriter.HexDump(_buffer.Skip(2).ToArray());

                //Console.WriteLine("{0}", hexDump);

                _buffer = new byte[4];
                _bytesToRead = _buffer.Length;
                _ns.BeginRead(_buffer, 0, 4, OnHeader, null);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        public void Send(Packet packet)
        {
            var buffer = packet.Writer.GetBuffer();
            if (_crypt != null)
                buffer = _crypt.Encrypt(packet.Writer.GetBuffer());
            var bufferLength = buffer.Length;
            var length = (ushort)(bufferLength + 2); // Length includes itself

            try
            {
                _ns.Write(BitConverter.GetBytes(length), 0, 2);
                // Depend on what I want to do
                _ns.Write(buffer, 0, bufferLength);

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
