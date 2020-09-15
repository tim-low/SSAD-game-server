using System;
using System.Numerics;
using Game_Server.Model.Misc;
using Game_Server.Util;
using Game_Server.Util.Database;

namespace Game_Server.Model
{
    public class Character : ISerializable, IDatabase<Game_Server.Controller.Database.Tables.Character>
    {
        public Vector3 Vec3;
        public Vector3 Direction;
        public Status Status;
        public string Name
        {
            get
            {
                return CharacterDb.Account.Username;
            }
        }
        public string Token;
        public bool IsReady;    // For game to start
        public DateTime IsReadyDateTime;
        public Controller.Database.Tables.Character CharacterDb;
        public Experience Experience
        {
            get
            {
                return new Experience(CharacterDb.Account.Masteries);
            }
        }

        public LeaderboardStats Statistic
        {
            get
            {
                return new LeaderboardStats(CharacterDb.Account.QuestionAttempteds);
            }
        }

        public int Permission
        {
            get
            {
                return CharacterDb.Account.Permission;
            }
        }
        public int GameCount
        {
            get
            {
                return CharacterDb.Account.Sessions.Count;
            }
        }



        public Character()
        {
        }

        public void Serialize(SerializeWriter writer)
        {
            // enable when further refactoring occur/
            //writer.WriteTextStatic(Token, 44);
            writer.WriteTextStatic(Name, 13);
            writer.Write(Vec3);
            writer.Write(Direction);
            // Write Attributes/Customization
            writer.Write((byte)CharacterDb.HeadEqp);
            writer.Write((byte)CharacterDb.TopEqp);
            writer.Write((byte)CharacterDb.BottomEqp);
            writer.Write((byte)CharacterDb.ShoeEqp);
        }

        /// <summary>
        /// Toggle IsReady flag
        /// If IsReady is toggled to true, store time receipt so they cannot spam ready/unready.
        /// </summary>
        public bool ToggleReady()
        {
            IsReady = !IsReady;
            if (IsReady)
            {
                // It has been less than 5 seconds since they clicked ready.
                if ((DateTime.Now - IsReadyDateTime).TotalSeconds < 5)
                {
                    return false;
                }
                IsReadyDateTime = DateTime.Now;
            }
            return true;
        }

        public void Unready()
        {
            IsReady = false;
        }

        public Controller.Database.Tables.Character ToEntity()
        {
            return CharacterDb;
        }

        public void FromEntity(Controller.Database.Tables.Character item)
        {
            this.Status = new Status();
            this.Vec3 = new Vector3(0, 0, 1);
            this.Direction = new Vector3(0, 0, 1);
            this.IsReady = false;
            this.IsReadyDateTime = DateTime.MinValue;
            this.CharacterDb = item;
            this.Token = Utilities.GenerateToken(item.Account.Username, item.Account.Salt);
        }
    }
}
