using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Game_Server.Util;
using Game_Server;
using LoadTestingRunner.Util;
using Game_Server.Network;
using Game_Server.Network.Handler;
using Game_Server.Model;
using Game_Server.Controller.Manager;

namespace LoadTestingRunner
{
    public class ServerLoadTest
    {
        readonly TestClient[,] clients = new TestClient[5, 20];
        readonly Random random = new Random();
        readonly Stopwatch stopwatch;
        readonly long testDurationInMs;

        /// <summary>
        /// Initialize load test for GameServer.
        /// </summary>
        /// <param name="testDurationInMs">How long the test will run for.</param>
        public ServerLoadTest(long testDurationInMs)
        {
            this.stopwatch = new Stopwatch();
            this.testDurationInMs = testDurationInMs;
        }

        /// <summary>
        /// Connect 100 TCP clients that send CmdUserAuth to the server continuously.
        /// Option 2
        /// </summary>
        public async void SpamLoginCmds()
        {
            await ConnectTcpClients(100);
            Log.Info("{0} clients loaded. Time elapsed: {1}ms", 100, stopwatch.ElapsedMilliseconds);

            Log.Info("Wait for 10 seconds...");
            // Sleep 10s to ensure that all encryption key have been read from the server and stored
            Thread.Sleep(10000);
            // Start the 5 thread to spam packets
            stopwatch.Start();
            StartLoop(new CmdLogin() { Username = "ophis", Password = "wrongpassword" });
        }

        /// <summary>
        /// Connect 100 TCP clients that send NullPingAcks to the server continuously.
        /// Option 1
        /// </summary>
        public async void SpamNullPingAcks()
        {
            await ConnectTcpClients(100);

            Log.Info("{0} clients loaded. Time elapsed: {1}ms", 100, stopwatch.ElapsedMilliseconds);

            Console.WriteLine("Wait for 10 seconds...");
            // Sleep 10s to ensure that all encryption key have been read from the server and stored
            Thread.Sleep(10000);
            // Start the 5 thread to spam packets
            stopwatch.Start();
            StartLoop(new NullPingAck());
        }

        public void StartLoop(OutPacket pkt)
        {
            // Start 5 threads spamming packets from 5 random clients every ms
            // Each thread will access different indexes of the 100 clients
            new Task(() => SendPacket(0, pkt)).Start(); // First 20 clients
            new Task(() => SendPacket(1, pkt)).Start(); // 20 - 39
            new Task(() => SendPacket(2, pkt)).Start(); // 40 - 59
            new Task(() => SendPacket(3, pkt)).Start(); // 60 - 79
            new Task(() => SendPacket(4, pkt)).Start(); // 80 - 99
        }

        public void SendPacket(int count, OutPacket pkt)
        {
            while (stopwatch.ElapsedMilliseconds < testDurationInMs)
            {
                int index = random.Next(0, 20);
                Log.Debug("Client {0} sending...", (count*index)+index);
                clients[count, index].Send(pkt.CreatePacket());
                Thread.Sleep(50);
                //Log.Debug(stopwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
        }

        #region CONNECT_TCP_CLIENTS

        private async Task<TestClient> ConnectTcpClient()
        {
            TestClient client = new TestClient();
            await client.ConnectAsync(Utilities.Host, Utilities.ServerPort);
            client.Listening();
            // Send authentication header
            client.GetStream().Write(Constant.NormalProtocolVersion, 0, 4);
            return client;
        }

        private async Task ConnectTcpClients(int numClients)
        {
            for (int i = 0; i < numClients; i++)
            {
                int a = i / 20;
                int b = i % 20;
                clients[a, b] = await ConnectTcpClient();
            }
        }
        #endregion
    }
}