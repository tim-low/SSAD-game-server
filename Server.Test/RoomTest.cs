using Game_Server.Model;
using Game_Server.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Server.Test
{
    public class RoomTest
    {
        [Fact]
        public void CreateRoomAndCheckDefaultSize()
        {
            // Default room size is 4
            Room room = new Room();
            Assert.Equal(4, room.Size);
        }

        [Fact]
        public void CreateRoomWithSize1()
        {
            Room room = new Room(1);
            Assert.Equal(1, room.Size);
        }

    }
}
