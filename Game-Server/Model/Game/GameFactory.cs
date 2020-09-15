using System;
using System.Collections.Generic;
using System.Linq;
using Game_Server.Controller.Database.Tables;
using Game_Server.Network;
using Game_Server.Util;

namespace Game_Server.Model
{
    public class GameFactory
    {
        /// <summary>
        /// Fill the board with board tile
        /// </summary>
        /// <param name="x">X axis of the Board</param>
        /// <param name="y">Y axis of the Board</param>
        /// <param name="Board">Pass in a board to fill the board with BoardTile</param>
        public static void FillBoard(int x, int y, out List<BoardTile> Board)
        {
            Board = new List<BoardTile>();
            for(byte i = 0; i < x; i++)
            {
                for(byte j = 0; j < y; j++)
                {
                    Board.Add(new BoardTile(i, j));
                }
            }
        }

        /// <summary>
        /// Generate initial board placement of player based on their index
        /// </summary>
        /// <param name="playerIndex">the player index</param>
        /// <returns>a array containing the x and y of the player on the board</returns>
        public static int[] GetInitialBoardTile(int playerIndex)
        {
            switch(playerIndex)
            {
                case 0:
                    return new int[] { 0, 0 };
                case 1:
                    return new int[] { 7, 7 };
                case 2:
                    return new int[] { 0, 7 };
                default:
                    return new int[] { 7, 0 };

            }
        }

        public static BoardDirection GetInitialBoardDirection(int playerIndex)
        {
            if(playerIndex % 2 == 0)
            {
                return BoardDirection.UP;
            }
            return BoardDirection.DOWN;
        }

        /// <summary>
        /// Generate and spawn an item to the player depending on the weight of the question
        /// </summary>
        /// <param name="weight">difficulty level of the question</param>
        /// <returns>InventoryItem</returns>
        public static Reward CreateReward(int weight = 0)
        {
            Reward reward = new Reward();
            reward.Item = InventoryItem.Spawn(Utilities.RandomItemEnum());
            reward.Step = 5;
            return reward;
        }

        /// <summary>
        /// Create an instance of a Game based off the Room
        /// </summary>
        /// <param name="quiz"></param>
        /// <param name="totalQnsCount"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Game CreateGame(Room r)
        {
            Game session = new Game(r);
            Quiz quiz = r.Quiz;
            quiz.Questions.Shuffle();
            List<Question> questions = quiz.Questions.Take(r.NoOfQuestion).ToList();
            foreach(Question question in questions)
            {
                session.Questions.Enqueue(question);
            }
            return session;
        }

        private static int GetSize(int count, out int numOfMinPlayerGame)
        {
            int remainder = count % 4;
            numOfMinPlayerGame = 0;
            if (remainder > 0 && remainder <= 3)
            {
                if (remainder == 1)
                    numOfMinPlayerGame = 3;
                else if (remainder == 2)
                    numOfMinPlayerGame = 2;
                else if (remainder == 3)
                    numOfMinPlayerGame = 1;
            }
            return Int32.Parse(Math.Ceiling(count / 4f).ToString());
        }

        public static Game[] CreateGame(WaitingRoom r)
        {
            Guid guid = Utilities.GenerateGuid();
            int gameCountWithMin;
            int size = GetSize(r.Clients.Count, out gameCountWithMin);
            Game[] game = new Game[size];
            r.Clients.Shuffle();
            Queue<GameClient> queue = r.Clients.ToQueue();
            int count = 0;
            while(count < game.Length)
            {
                game[count] = new Game(r);
                game[count].Guid = guid;
                int maxPlayerCount = (gameCountWithMin > 0) ? 3 : 4;
                int currentPlayerCount = 0;
                while(queue.Count() > 0 && currentPlayerCount != maxPlayerCount)
                {
                    var client = queue.Dequeue();
                    game[count].PlayerManager.Add(client);
                    client.Character.Status.Update(game[count]);
                    currentPlayerCount += 1;
                }
                gameCountWithMin -= 1;
                game[count].PlayerManager.Setup();
                game[count].SetupBoard();
                count++;
            }
            return game;
        }

    }
}
