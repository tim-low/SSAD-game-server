using System;
using System.Linq;
using System.Numerics;
using Game_Server.Controller.Database.Tables;
using Game_Server.Model;
using Game_Server.Network.Auth;
using Game_Server.Util;
using Game_Server.Util.Database;

namespace Game_Server.Network.Handler
{
    public class AuthHandler
    {
        /// <summary>
        /// This method is called when the client send a CmdUserAuth packet to the server
        /// </summary>
        /// <param name="packet"></param>
        [Packet(Packets.CmdUserAuth)]
        public static void HandleAuth(Packet packet)
        {
            if(packet.Sender.Character != null)
            {
                packet.Sender.Character.CharacterDb.Account.IsLoggedIn = 0;
                ServerMain.Instance.Database.Save();
                // Save data
            }
            // Read the packet into readable format
            UserAuthPacket authPkt = new UserAuthPacket(packet);
            // Login and Retrieve the User
            Controller.Database.Tables.Character character = ServerMain.Instance.Database.GetCharacter(authPkt.Username);
            if (character == null || Password.GenerateSaltedHash(authPkt.Password, character.Account.Salt) != character.Account.Password) 
            {
                packet.SendBackError(5);
                return;
            }
            if (character.Account.IsLoggedIn == 1)
            {
                packet.SendBackError(6);
                return;
            }
            if(character.Account.Permission == 0 && packet.Sender.IsTeacherClient())
            {
                packet.SendBackError(4);
                return;
            }
            // Prepare the Acknowledgment packet indicating success and token
            character.Account.IsLoggedIn = 1;
            character.Account.LastLoggedIn = DateTime.Now;
            ServerMain.Instance.Database.Save();
            Model.Character model = new Model.Character();
            model.FromEntity(character);
            packet.Sender.Character = model;
            var ack = new UserAuthAck()
            {
                Token = model.Token
            };
            packet.Sender.Send(ack.CreatePacket());
            if (ServerMain.Instance.Server.LoggedInClient.ContainsKey(model.Token))
            {
                ServerMain.Instance.Server.LoggedInClient.Remove(model.Token);
            }
            ServerMain.Instance.Server.LoggedInClient.Add(model.Token, packet.Sender);
            return;
        }

        [Packet(Packets.CmdUserReg)]
        public static void HandleRegistration(Packet packet)
        {
            if (packet.Sender.Character != null)
            {
                packet.Sender.Character.CharacterDb.Account.IsLoggedIn = 0;
                ServerMain.Instance.Database.Save();
            }
            CmdUserReg registrationPkt = new CmdUserReg(packet);
            Log.Debug("{0}", registrationPkt.ToString());
            var ack = new UserRegAck();
            int code = ServerMain.Instance.Database.CheckIfAccountExist(registrationPkt.Username, registrationPkt.Email);
            if (code == 1)
            {
                packet.SendBackError(7, registrationPkt.Username);
                return;
            }
            else if(code == 2)
            {
                packet.SendBackError(8, registrationPkt.Email);
                return;
            }
            else if(registrationPkt.Username.Trim() == "" || registrationPkt.Password == "da39a3ee5e6b4b0d3255bfef95601890afd80709")
            {
                packet.SendBackError(9);
                return;
            }
            else if(!registrationPkt.Email.Contains(".edu."))
            {
                packet.SendBackError(10);
                return;
            }
            var salt = Password.CreateSalt(Password.SaltSize);
            ServerMain.Instance.Database.CreateAccount(registrationPkt.Username, registrationPkt.Email, registrationPkt.Password, registrationPkt.StudentName, registrationPkt.Class, registrationPkt.Year, registrationPkt.Semester, salt);
            Controller.Database.Tables.Character character = ServerMain.Instance.Database.GetCharacter(registrationPkt.Username);
            Model.Character model = new Model.Character();
            model.FromEntity(character);
            ack.Token = model.Token;
            packet.Sender.Character = model;
            packet.Sender.Send(ack.CreatePacket());
            ServerMain.Instance.Server.LoggedInClient.Add(model.Token, packet.Sender);
            return;
        }

        [Packet(Packets.CmdUpdateChar)]
        public static void HandleCharUpdate(Packet packet)
        {
            CmdUpdateChar charPkt = new CmdUpdateChar(packet);
            var character = packet.Sender.Character;
            character.CharacterDb.HeadEqp = charPkt.Head;
            character.CharacterDb.TopEqp = charPkt.Shirt;
            character.CharacterDb.BottomEqp = charPkt.Pant;
            character.CharacterDb.ShoeEqp = charPkt.Shoe;
            var ack = new UpdateCharAck() {
                Token = charPkt.Token
            };
            packet.SendBack(ack.CreatePacket());
            return;
        }

        [Packet(Packets.CmdUserLogout)]
        public static void HandleLogout(Packet packet)
        {
            packet.Sender.Logout();
        }

    }
}
