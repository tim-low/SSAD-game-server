using System;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class GameScoreModel : ISerializable
    {
        public string Token { get; set; }
        public int Score { get; set; }
        public int AnswerCorrectly { get; set; }
        public int Experience { get; set; }
        public int CurrentExperience { get; set; }
        public int CurrentLevel { get; set; }

        public byte LootCount { get; set; }

        public GameScoreModel()
        {

        }

        public GameScoreModel(SerializeReader reader)
        {
            Token = reader.ReadUnicodeStatic(44);
            Score = reader.ReadInt32();
            AnswerCorrectly = reader.ReadInt32();
            Experience = reader.ReadInt32();
            CurrentExperience = reader.ReadInt32();
            CurrentLevel = reader.ReadInt32();
        }

        public void Serialize(SerializeWriter writer)
        {
            writer.WriteTextStatic(Token, 44);
            writer.Write(Score);
            writer.Write(AnswerCorrectly);
            writer.Write(Experience);
            writer.Write(CurrentExperience);
            writer.Write(CurrentLevel);
            writer.Write(LootCount);
        }
    }
}
