using Game_Server.Util;
using System;

namespace Game_Server
{
    class Program
    {
        /// <summary>
        /// This is the application main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Run the singleton Server instance.
            ServerMain.Instance.Run();
            // Push the Server Command out onto Main Class to decouple certain features.
            var x = new ConsoleCommands();
            x.Wait();
        }
    }
}
