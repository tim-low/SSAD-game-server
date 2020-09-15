using System;
using System.Collections.Generic;
using System.Linq;

namespace Game_Server.Model
{
    public class Board
    {
        /// <summary>
        /// Instance of every single Tile on the board
        /// </summary>
        private List<BoardTile> Tiles;

        /// <summary>
        /// Constructor to fill up the number of block with the amount of x and y: Size = x*y
        /// </summary>
        /// <param name="x">amount of tile in x axis</param>
        /// <param name="y">amount of tile in y axis</param>
        public Board(int x = 8, int y = 8)
        {
            GameFactory.FillBoard(x, y, out Tiles);
        }

        /// <summary>
        /// <seealso cref="UpdateTile(BoardTile, int)"/>
        /// Update the tile in the board that exist on the x and y axis given by the parameter
        /// </summary>
        /// <param name="x">the x axis of the tile</param>
        /// <param name="y">the y axis of the tile</param>
        /// <param name="color">the color of the tile</param>
        /// <returns></returns>
        public BoardTile UpdateTile(int x, int y, int color)
        {
            return UpdateTile(new BoardTile(x, y), color);
        }

        /// <summary>
        /// Update the tile if it is not locked, if it is lock, return the tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public BoardTile UpdateTile(BoardTile tile, int color)
        {
            if (!IsTileLocked(tile))
                return Tiles.SingleOrDefault(t => t.X == tile.X && t.Y == tile.Y).SetColor(color);
            else
                return GetTile(tile);
        }

        /// <summary>
        /// Lock all the tiles on the board that is owned by this particular player color
        /// </summary>
        /// <param name="color">the player color</param>
        public void LockTile(int color)
        {
            foreach(BoardTile t in Tiles)
            {
                if (t.Color == color)
                    t.LockTile();
            }
        }

        /// <summary>
        /// Get the tile from the board given the tile x and y axis
        /// </summary>
        /// <seealso cref="GetTile(int, int)"/>
        /// <param name="tile"></param>
        /// <returns></returns>
        public BoardTile GetTile(BoardTile tile)
        {
            return GetTile(tile.X, tile.Y);
        }

        public List<BoardTile> GetTiles()
        {
            return this.Tiles;
        }

        /// <summary>
        /// Get the tile from the board given the x and y axis
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public BoardTile GetTile(int x, int y)
        {
            return Tiles.SingleOrDefault(t => t.X == x && t.Y == y);
        }

        /// <summary>
        /// Check if a particular tile is locked
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool IsTileLocked(BoardTile tile)
        {
            var tileSelected = GetTile(tile);
            return Tiles.Where(selectTile => selectTile.Color == tileSelected.Color).Where(selectTile => selectTile.IsLocked() == true).Count() > 0;
        }

        /// <summary>
        /// Check if a particular tile is inaccessible
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool IsTileInaccessible(BoardTile tile)
        {
            return GetTile(tile).IsInaccessible();
        }
        
        /// <summary>
        /// Set the particular tile to be inaccessible
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public BoardTile SetInaccessible(BoardTile tile)
        {
            return Tiles.SingleOrDefault(t => t.X == tile.X && t.Y == tile.Y).SetInaccessible(true);
        }

        /// <summary>
        /// Remove all inaccessible tile (call only at the end of each cycle)
        /// </summary>
        public void ClearInaccessible()
        {
            foreach(BoardTile t in Tiles)
            {
                if (t.Inaccessible)
                    t.SetInaccessible(false);
            }
        }

        /// <summary>
        /// Apply Pillar Item effect to the Board
        /// </summary>
        /// <param name="player"></param>
        public void Pillar(Player player)
        {
            var position = player.Position;
            var direction = player.FacingDirection;
            int curTilePos = (direction == BoardDirection.DOWN || direction == BoardDirection.UP) ? position.Y : position.X;
            int loopVal = (direction == BoardDirection.RIGHT || direction == BoardDirection.UP) ? 1 : -1;
            while (curTilePos >= 0 && curTilePos < 8)
            {
                if (direction == BoardDirection.DOWN || direction == BoardDirection.UP)
                {
                    UpdateTile(position.X, curTilePos, player.PlayerColor);
                }
                else
                {
                    UpdateTile(curTilePos, position.Y, player.PlayerColor);
                }
                curTilePos += loopVal;
            }
        }


        /// <summary>
        /// Apply Crown Item effect to the Board
        /// </summary>
        /// <param name="player"></param>
        public void Crown(Player player)
        {
            var position = player.Position;
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    if ((position.X + col >= 0 && position.X + col < 8) && (position.Y + row >= 0 && position.Y + row < 8))
                        UpdateTile(position.X + col, position.Y + row, player.PlayerColor);
                }
            }
        }

        /// <summary>
        /// Apply SelfDestruct Item effect to the board
        /// </summary>
        /// <param name="player"></param>
        public void SelfDestruct(Player player)
        {
            var position = player.Position;
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    if ((position.X + col >= 0 && position.X + col < 8) && (position.Y + row >= 0 && position.Y + row < 8))
                        UpdateTile(position.X + col, position.Y + row, 0);
                }
            }
        }

        /// <summary>
        /// Unlock the tile if the tile is locked
        /// </summary>
        public void Unlock()
        {
            foreach(BoardTile t in Tiles)
            {
                if(t.IsLocked())
                {
                    t.Unlock();
                }
            }
        }

        /// <summary>
        /// Setup player initial tile
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public BoardTile SetupTile(int playerIndex)
        {
            return GetSpawn(playerIndex + 1);
        }

        /// <summary>
        /// Get the spawn tile of the player
        /// </summary>
        /// <param name="playerColor">need to minus 1 to get player spawn point</param>
        /// <returns></returns>
        public BoardTile GetSpawn(int playerColor)
        {
            switch(playerColor)
            {
                case 1:
                    return UpdateTile(0, 0, playerColor);
                case 2:
                    return UpdateTile(7, 7, playerColor);
                case 3:
                    return UpdateTile(0, 7, playerColor);
                default:
                    return UpdateTile(7, 0, playerColor);
            }
        }

        /// <summary>
        /// Override the original <seealso cref="object.ToString"/> method to output the board state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "";
            for (int i = 7; i >= 0; i--)
            {
                output += String.Format("{0}: ", i);
                for (int j = 0; j < 8; j++)
                {
                    output += String.Format("{0} ", GetTile(j, i).Color);
                }
                output += "\n";
            }
            return output;
        }
    }
}
