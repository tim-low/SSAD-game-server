using Game_Server;
using Game_Server.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LoadTestingRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServerMain.Instance.Run();
            Log.Info("Initialized LoadTestingRunner");
            Console.WriteLine("(1) GameServer Load Test: 100 clients spam NullPingAck packets");
            Console.WriteLine("(2) GameServer Load Test: 100 clients spam CmdUserAuth packets");
            Console.WriteLine("(3) ApiServer Load Test: Spam GET requests to /questions");

            Console.WriteLine("Enter option (1, 2, 3):");
            string input = Console.ReadLine();
            int testDurationInMs = 2000;

            int option;
            if (Int32.TryParse(input, out option))
            {
                switch (option)
                {
                    case 1:
                    case 2:
                        ServerLoadTest serverLoadTest = new ServerLoadTest(testDurationInMs);
                        if (option == 1) serverLoadTest.SpamNullPingAcks();
                        if (option == 2) serverLoadTest.SpamLoginCmds();
                        break;
                    case 3:
                        ApiLoadTest apiLoadTest = new ApiLoadTest(testDurationInMs);
                        apiLoadTest.Run();
                        break;
                    default:
                        Log.Error("No such option.");
                        break;
                }
            }
            else
            {
                Log.Error("No such option.");
            }

            var x = new ConsoleCommands();
            x.Wait();
        }
    }
}
