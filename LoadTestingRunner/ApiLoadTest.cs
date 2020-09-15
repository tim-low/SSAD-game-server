using Game_Server;
using Game_Server.Util;
using LoadTestingRunner.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoadTestingRunner
{
    public class ApiLoadTest
    {
        HttpClient client;
        // hardcoded
        const string url = "http://" + Utilities.Host + ":8000/api/v1/question/get/mYthWbwbtf0T4wM9IU3fXUjJluZBX9qrKvMUi338i4s=";
        readonly Stopwatch stopwatch;
        readonly long testDurationInMs;

        /// <summary>
        /// Initialize load test for ApiServer.
        /// </summary>
        /// <param name="testDurationInMs">How long the test will run for.</param>
        public ApiLoadTest(long testDurationInMs)
        {
            client = new HttpClient();
            stopwatch = new Stopwatch();
            this.testDurationInMs = testDurationInMs;
        }

        public void Run()
        {
            LoginTeacherAccount();
            Thread.Sleep(5000);

            stopwatch.Start();
            StartLoop();

            stopwatch.Stop();
            Log.Info("Time elapsed: " + stopwatch.ElapsedMilliseconds + "ms");
        }

        private async void LoginTeacherAccount()
        {
            TestClient client = new TestClient();
            await client.ConnectAsync(Utilities.Host, Utilities.ServerPort);
            client.Listening();
            client.GetStream().Write(Constant.TeacherProtocolVersion, 0, 4);
            Thread.Sleep(2000);
            client.Send(new CmdLogin() { 
                Username = "demoteacher",
                Password = "s123456S!"
            }.CreatePacket());
        }

        private void StartLoop()
        {
            new Task(() => SendRequest()).Start();
            new Task(() => SendRequest()).Start();
            new Task(() => SendRequest()).Start();
            new Task(() => SendRequest()).Start();
            new Task(() => SendRequest()).Start();
        }

        public async void SendRequest()
        {
            while (stopwatch.ElapsedMilliseconds < testDurationInMs)
            {
                HttpResponseMessage response = await client.GetAsync(url);
                Log.Debug("Status Code (GET /questions): {0}", response.StatusCode);
            }
        }
    }
}
