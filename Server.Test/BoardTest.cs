using Game_Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Server.Test
{
    /// <summary>
    /// This BoardTest.cs contain all the unit testing of all the functionalities and features of <seealso cref="Board">Board</seealso> 
    /// class in <seealso cref="Game">Game</seealso>. All pushes and update need to pass the check before it can be merge.
    /// </summary>
    public class BoardTest
    {
        public Board board = new Board();
        public Dictionary<int, List<Position>> BoardColorPosition = new Dictionary<int, List<Position>>();

        /// <summary>
        /// Setup the test case (run everytime a new fact is executed)
        /// </summary>
        public BoardTest()
        {
            Random random = new Random();
            // Initialize the board
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int color = random.Next(1, 5);
                    if (!BoardColorPosition.ContainsKey(color))
                        BoardColorPosition[color] = new List<Position>();
                    BoardColorPosition[color].Add(new Position(x, y));
                    board.UpdateTile(x, y, color);
                }
            }
        }

        [Fact]
        public void CanWriteOverLockTile()
        {
            Random random = new Random();
            int selectedColor = random.Next(1, 5);
            board.LockTile(selectedColor);
            var randomTilePos = BoardColorPosition[selectedColor].ElementAt(random.Next(0, BoardColorPosition[selectedColor].Count));
            int overrideColor = selectedColor;
            while(overrideColor == selectedColor)
            {
                overrideColor = random.Next(1, 5);
            }
            var oldTileColor = board.GetTile(randomTilePos.x, randomTilePos.y).Color;
            var tile = board.UpdateTile(randomTilePos.x, randomTilePos.y, overrideColor);
            Assert.Equal(oldTileColor, tile.Color);
        }

        [Fact]
        public void UpdateTile()
        {
            Random random = new Random();
            int selectedColor = random.Next(1, 5);
            var randomTilePos = BoardColorPosition[selectedColor].ElementAt(random.Next(0, BoardColorPosition[selectedColor].Count));
            int overrideColor = selectedColor;
            while (overrideColor == selectedColor)
            {
                overrideColor = random.Next(1, 5);
            }
            var oldTileColor = board.GetTile(randomTilePos.x, randomTilePos.y).Color;
            var tile = board.UpdateTile(randomTilePos.x, randomTilePos.y, overrideColor);
            Assert.True(oldTileColor != tile.Color, "Tile has not been updated.");
        }

        [Fact]
        public void LockTileEvenAfterUserGainMoreTile()
        {
            Random random = new Random();
            int selectedColor = random.Next(1, 5);
            board.LockTile(selectedColor);
            int overrideColor = selectedColor;
            while (overrideColor == selectedColor)
            {
                overrideColor = random.Next(1, 5);
            }
            var randomTilePos = BoardColorPosition[overrideColor].ElementAt(random.Next(0, BoardColorPosition[overrideColor].Count));
            var tile = board.UpdateTile(randomTilePos.x, randomTilePos.y, selectedColor);
            Assert.True(board.IsTileLocked(tile), "Tile is not locked");
        }

        [Fact]
        public void RunUnlockTile()
        {
            Random random = new Random();
            int selectedColor = random.Next(1, 5);
            board.LockTile(selectedColor);
            var randomTilePos = BoardColorPosition[selectedColor].ElementAt(random.Next(0, BoardColorPosition[selectedColor].Count));
            int overrideColor = selectedColor;
            while (overrideColor == selectedColor)
            {
                overrideColor = random.Next(1, 5);
            }
            var oldTileColor = board.GetTile(randomTilePos.x, randomTilePos.y).Color;
            // Unlock() is supposed to run twice, next turn
            board.Unlock();
            var tile = board.UpdateTile(randomTilePos.x, randomTilePos.y, overrideColor);
            Assert.Equal(oldTileColor, tile.Color); // Not supposed to override
            // next turn
            board.Unlock();
            var newTile = board.UpdateTile(randomTilePos.x, randomTilePos.y, overrideColor);
            Assert.Equal(overrideColor, newTile.Color);
        }

        public class Position
        {
            public int x;
            public int y;

            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
