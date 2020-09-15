using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Server.Network
{
    public class CmdNoticeData
    {
        public readonly int[] AnnoucementIndexes;

        public CmdNoticeData(Packet packet)
        {
            AnnoucementIndexes = new int[packet.Reader.ReadInt32()];
            for(int i = 0; i < AnnoucementIndexes.Length; i++)
            {
                AnnoucementIndexes[i] = packet.Reader.ReadInt32();
            }
        }

    }
}
