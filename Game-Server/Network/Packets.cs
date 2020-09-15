using System;
using System.Reflection;

namespace Game_Server.Network
{
    /// <summary>
    /// This is the packets header class that indicate which packet it is.
    /// </summary>
    public class Packets
    {
        public const ushort CmdNullPing = 0;
        public const ushort CmdUserAuth = 1;
        public const ushort UserAuthAck = 2;
        public const ushort CmdLoadChar = 3;
        public const ushort LoadCharAck = 4;
        public const ushort LoadOverWorldAck = 5;

        public const ushort CmdUserReg = 6;
        public const ushort UserRegAck = 7;

        public const ushort CmdUpdateChar = 8;
        public const ushort UpdateCharAck = 9;

        public const ushort NullPingAck = 10;

        public const ushort CmdUserLogout = 11;


        public const ushort CmdGetNotices = 90;
        public const ushort GetNoticesAck = 91;
        public const ushort CmdNoticeData = 92;
        public const ushort NoticeDataAck = 93;


        // Avatar/Leaderboard Packet (100-199)
        public const ushort CmdGetChar = 100;
        public const ushort GetCharAck = 101;

        public const ushort CmdPlayerStats = 102;
        public const ushort PlayerStatsAck = 103;

        public const ushort CmdGetUnlocks = 104;
        public const ushort GetUnlocksAck = 105;

        public const ushort CmdUpdateUnlocks = 106;
        public const ushort UpdateUnlocksAck = 107;

        public const ushort CmdLeaderboard = 108;
        public const ushort LeaderboardAck = 109;

        public const ushort CmdGloryboard = 110;
        public const ushort GloryboardAck = 111;

        // Misc Packet

        public const ushort CmdSpawnPlayer = 200;
        public const ushort SpawnPlayerAck = 201;
        public const ushort SpawnPlayersAck = 202;
        public const ushort CmdMovePlayer = 203;
        public const ushort MovePlayerAck = 204;

        public const ushort CmdOpenChest = 205;
        public const ushort GetChestRewardAck = 206;

        public const ushort ErrorAck = 255;
        public const ushort ServerNoticeAck = 256;

        public const ushort CmdSendKaomoji = 296;
        public const ushort SendKaomojiAck = 297;
        public const ushort CmdChatMsg = 298;
        public const ushort ChatMsgAck = 299;

        // Room Packet (300 - 399)
        public const ushort CmdRoomDetails = 300;
        public const ushort RoomDetailsAck = 301;

        public const ushort CmdRoomPlayerMove = 302;
        public const ushort RoomPlayerMoveAck = 303;

        public const ushort CmdStartGame = 304;
        public const ushort ReadyStatusAck = 305;
        public const ushort StartGameAck = 306;
        public const ushort CanStartAck = 307;

        public const ushort UpdateRoomStatusAck = 308;

        public const ushort CmdJoinSession = 309;
        public const ushort JoinSessionAck = 310;
        public const ushort ClientPlayerJoinedSessionAck = 311;
        public const ushort CmdLeaveSession = 312;
        public const ushort LeaveSessionAck = 313;
        public const ushort ClientPlayerLeftSessionAck = 314;

        // Lobby Packet (400 - 499)
        public const ushort CmdWorldSelect = 400;
        public const ushort WorldSelectAck = 401;

        public const ushort CmdCreateRoom = 402;
        public const ushort CreateRoomAck = 403;
        public const ushort RoomCreatedAck = 404;

        public const ushort CmdJoinRoom = 405;
        public const ushort JoinRoomAck = 406;
        public const ushort NewPlayerInRoomAck = 407;

        public const ushort CmdLeaveRoom = 408;
        public const ushort LeaveRoomAck = 409;
        public const ushort PlayerHasLeftRoomAck = 410;

        public const ushort UpdateRoomPlayerCountAck = 411;

        public const ushort CmdLeaveLobby = 412;
        public const ushort LeaveLobbyAck = 413;

        // Game Packet (500 - 800)
        public const ushort CmdInitializeGame = 500;
        public static ushort InitializeGameAck = 501;
        public const ushort InitializeCycleAck = 502;
        public const ushort StartPlayerTurnAck = 503;
        public const ushort CmdTurnPlayerMove = 504;
        public const ushort TurnPlayerMoveAck = 505;

        public const ushort CmdTurnPlayerUseItem = 506;
        public const ushort TurnPlayerUseItemAck = 507;

        public const ushort StartQuizAck = 508;
        public const ushort CmdSelectAnswer = 509;
        public const ushort SelectAnswerAck = 510;

        public const ushort DespawnPlayerAck = 796;

        public const ushort NotifyPlayerTurnAck = 797;
        public const ushort CmdEndTurn = 798;
        public const ushort EndPlayerTurnAck = 799;
        public const ushort EndGameAck = 700;

        // Sample Ack
        public const ushort CmdSample = 999;
        public const ushort SampleAck = 1000;

        // Teacher App & Functionalities
        public const ushort CmdCreateSession = 1001;
        public const ushort CreateSessionAck = 1002;

        public const ushort TeacherPlayerJoinedSessionAck = 1003;
        public const ushort TeacherPlayerLeftSessionAck = 1004;

        public const ushort CmdStartSession = 1005;
        public const ushort StartSessionAck = 1006;
        public const ushort CmdCancelSession = 1007;
        public const ushort CancelSessionAck = 1008;


        public const ushort SymKeyAck = 99;

        public static string GetName(ushort packetId)
        {
            // Login/Channel
            foreach (var field in typeof(Packets).GetFields(BindingFlags.Public | BindingFlags.Static))
                if ((ushort) field.GetValue(null) == packetId)
                    return field.Name;

            return "?";
        }
    }
}
