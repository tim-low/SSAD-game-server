using System;
using System.Linq;
using Game_Server.Controller.Database.Tables;
using Game_Server.Model;
using Game_Server.Network.Auth;
using Game_Server.Util;

namespace Game_Server.Network
{
    public class SampleListener
    {
        /// <summary>
        /// This method will be read by the GameServer and processed it into a Action that can be called
        /// by GameServer when CmdSample packet is received by the Server
        /// </summary>
        /// <param name="packet"></param>
        [Packet(Packets.CmdSample)] // Indicate which packet this listener listen to
        public static void HandleCmdSample(Packet packet)
        {
            // Do your processing code here
            // So we created a class for the CmdSamplePacket, so we can process the packet easily
            CmdSamplePacket samplePkt = new CmdSamplePacket(packet);
            var ack = new SampleAck();
            // We simplify the conversion method within another class, so we can make this class look neater
            // samplePkt.IsTrue == packet.Reader.ReadBoolean();
            ack.StatusCode = samplePkt.IsTrue ? 200 : 404;
            ack.Message = samplePkt.IsTrue ? samplePkt.Message : "Error";
            // We convert the outgoing packet into packet which is sendable via our Socket Stream
            Packet ackPkt = ack.CreatePacket();
            // Retrieve the Socket Stream via the Sender of the Packet and send the packet over
            packet.SendBack(ackPkt);
        }
    }
}
