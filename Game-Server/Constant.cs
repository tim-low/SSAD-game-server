using System;
using System.Collections.Generic;

namespace Game_Server
{
    public class Constant
    {
        public static byte[] NormalProtocolVersion = { 0x00, 0x00, 0x01, 0x0A };

        public static byte[] TeacherProtocolVersion = { 0x80, 0x00, 0x01, 0x0A };

        public const int EVENT_TRIGGER_MILLIS = 2000;

        private static Dictionary<int, string> ErrorMessages = new Dictionary<int, string>();

        public static string GetMessage(int messageId)
        {
            if(ErrorMessages.ContainsKey(messageId))
            {
                return ErrorMessages[messageId];
            }
            else
            {
                return "404";
            }
        }

        public static void AddMessage(int messageId, string message)
        {
            if(ErrorMessages.ContainsKey(messageId))
            {
                ErrorMessages.Remove(messageId);
            }
            ErrorMessages.Add(messageId, message);
        }
    }
}
