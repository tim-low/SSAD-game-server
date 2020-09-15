using System;
using System.Collections.Generic;
using System.Text;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class Ranking : ISerializable
    {
        public string Username;
        public int Score;

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(Username, 13);
            writer.Write(Score);
        }

        public override String ToString()
        {
            return String.Format("{0} {1}", Username, Score);
        }
    }
}
