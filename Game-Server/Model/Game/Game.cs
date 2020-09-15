using Game_Server.Controller.Database.Tables;
using Game_Server.Model.Misc;
using Game_Server.Network;
using Game_Server.Util;
using Game_Server.Util.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Game_Server.Model
{
    /// <summary>
    /// Session.cs is the game session of the quiz. 
    /// </summary>
    public class Game : GameObject, IBroadcaster, IDisconnect
    {
        private Room Parent;
        private Timer TurnTimer;
        // Game State to determine what to look out for when a player disconnect
        public readonly byte TotalQuestion; // Might be unnecessary here
        public Queue<Question> Questions;
        public PlayerManager PlayerManager;
        private Board Board;
        private RewardManager RewardManager;
        private DespawnDespatcher EventDespatcher;
        private Timer QuizTimer;
        public Guid Guid { get; set; }
        private int Duration = 45;

        private object _dclock = new object();

        private int QuizId;

        /// Database Entity
        private List<QuestionAttempted> Attempts = new List<QuestionAttempted>();
        private List<SessionQuestion> SQuestion = new List<SessionQuestion>();

        public string GetIdentifier()
        {
            return Guid.ToString();
        }

        internal Game(Room r)
        {
            this.Parent = r;
            this.TotalQuestion = (byte) r.NoOfQuestion;
            this.Questions = new Queue<Question>();
            this.RewardManager = new RewardManager();
            this.PlayerManager = new PlayerManager();
            this.Guid = Guid.NewGuid();
            this.TurnTimer = new Timer(EndCurrentPlayerTurn, this, Timeout.Infinite, Timeout.Infinite);
            QuizId = r.Quiz.Id;
            foreach (var client in r.Clients)
            {
                client.Character.Status.Update(this);
                PlayerManager.Add(client);
            }
            // no Wiping client
            // r.Clients.Clear();
            PlayerManager.Setup();
            this.EventDespatcher = new DespawnDespatcher(this);
            SetupBoard();
        }

        public Game(WaitingRoom r)
        {
            this.Parent = r;
            this.TotalQuestion = (byte) r.Quiz.Questions.Count();
            this.Questions = new Queue<Question>();
            this.RewardManager = new RewardManager();
            this.PlayerManager = new PlayerManager();
            this.TurnTimer = new Timer(EndCurrentPlayerTurn, this, Timeout.Infinite, Timeout.Infinite);
            foreach (var question in r.Quiz.Questions)
            {
                this.Questions.Enqueue(question);
            }
            this.QuizId = r.Quiz.Id;
            this.EventDespatcher = new DespawnDespatcher(this);
        }

        public void AcknowlegeInitialized(GameClient client)
        {
            PlayerManager.PlayerAcknowledge(client);
            if(PlayerManager.CanSendInitializeAck())
            {
                var ack = new InitializeGameAck()
                {
                    PlayerSequence = PlayerManager.GetPlayers().ToArray(),
                    GameTurn = TotalQuestion
                };
                Broadcast(ack.CreatePacket());
                var ack2 = new StartQuizAck()
                {
                    DurationInSec = Duration,
                    Question = Questions.Peek()
                };
                Broadcast(ack2.CreatePacket());
                // Start Quiz Timer here
                StartQuizTimer();
            }
        }

        private void StartQuizTimer()
        {
            QuizTimer = new Timer(EndQuiz, this, 1000*Duration, Timeout.Infinite);
        }

        private void EndQuiz(object state)
        {
            foreach(Player p in PlayerManager.GetPlayers())
            {
                if (p.Answered == false)
                {
                    Reward r = RewardManager.CreateReward(p.GetIdentifier(), false, p.Answered == false);
                    p.Receive(r);
                }
            }
            QuizTimeUpOrAnswered();
        }

        private void EndCurrentPlayerTurn(object state)
        {
            Game game = (Game)state;
            game.EndPlayerTurn();
        }

        // Double Command Initialized

        public void SetupBoard()
        {
            Board = new Board(8, 8);

            for (int i = 0; i < PlayerManager.PlayerCount(); i++) {
                PlayerManager.GetPlayer(i).PlayerColor = i + 1;
                PlayerManager.GetPlayer(i).Position = Board.GetSpawn(i + 1);
                PlayerManager.GetPlayer(i).FacingDirection = GameFactory.GetInitialBoardDirection(i);
            }
        }

        /// <summary>
        /// Update the player position and update the Board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Player MovePlayer(GameClient sender, BoardDirection direction, out bool success)
        {
            var player = PlayerManager.GetCurrentPlayer();
            success = false;
            if (IsCurrentPlayerCage())
                return player;
            if (player.GetIdentifier() != sender.Character.Token)
                return null;
            var position = player.Position;
            BoardTile tile = Utilities.ProcessDPad(position, direction, player.Effect);
            if (tile.X > 7 || tile.X < 0 || tile.Y > 7 || tile.Y < 0 || player.MoveLeft == 0)
            {
                player.Client.SendError("Invalid move");
                return player;
            }
            // Get a copy
            if(Board.IsTileInaccessible(tile))
            {
                player.Client.SendError("Inaccessible tile");
                return player;
            }
            // Update the Board Tile
            BoardTile boardTile = Board.UpdateTile(tile, player.PlayerColor);
            // Update player position
            player = PlayerManager.MovePlayer(boardTile, direction) ;
            Log.Debug("After: {0} {1}", player.Position.X, player.Position.Y);
            success = true;
            return player;
        }

        public bool IsCurrentPlayerCage()
        {
            return PlayerManager.GetCurrentPlayer().Effect == ItemType.Cage;
        }

        public bool HandleUseItem(CmdTurnPlayerUseItem packet, out BoardTile tile)
        {
            tile = PlayerManager.GetCurrentPlayer().Position;
            if (!IsCurrentPlayerCage())
            {
                if (PlayerManager.GetCurrentPlayer().UseItem(packet.Item.Item))
                {
                    Player curPlayer = PlayerManager.GetCurrentPlayer();
                    BoardTile playerTile = curPlayer.Position;
                    Player victim;
                    BoardTile targetTile = packet.Tile;
                    switch ((ItemType)packet.Item.Item)
                    {
                        case ItemType.Crown:
                            Board.Crown(curPlayer);
                            break;
                        case ItemType.Bed:
                            curPlayer.Position = Board.GetSpawn(curPlayer.PlayerColor);
                            break;
                        case ItemType.BoulderSpike:
                            victim = PlayerManager.GetPlayers().SingleOrDefault(plyr => plyr.GetIdentifier() == packet.Victim);
                            victim.Position = Board.GetSpawn(victim.PlayerColor);
                            break;
                        case ItemType.Cage:
                            victim = PlayerManager.GetPlayers().SingleOrDefault(plyr => plyr.GetIdentifier() == packet.Victim);
                            victim.Effect = ItemType.Cage;
                            break;
                        case ItemType.Coin:
                            curPlayer.MoveLeft += 2;
                            break;
                        case ItemType.Door:
                            curPlayer.Position = Board.UpdateTile(targetTile, curPlayer.PlayerColor);
                            tile = curPlayer.Position;
                            break;
                        case ItemType.Flag:
                            BoardTile flagTile = Board.UpdateTile(targetTile, curPlayer.PlayerColor);
                            tile = flagTile;
                            // Assign random tile
                            break;
                        case ItemType.GroundSpike:
                            BoardTile spikeTile = Board.SetInaccessible(targetTile);
                            tile = spikeTile;
                            break;
                        case ItemType.Lock:
                            Board.LockTile(curPlayer.PlayerColor);
                            break;
                        case ItemType.Pillar:
                            // Need debugging
                            Board.Pillar(curPlayer);
                            break;
                        case ItemType.SelfDestruct:
                            Board.SelfDestruct(curPlayer);
                            break;
                        case ItemType.WizardHat:
                            victim = PlayerManager.GetPlayers().SingleOrDefault(plyr => plyr.GetIdentifier() == packet.Victim);
                            BoardTile temp = victim.Position;
                            victim.Position = curPlayer.Position;
                            curPlayer.Position = Board.UpdateTile(temp, curPlayer.PlayerColor);
                            tile = curPlayer.Position;
                            break;
                    }
                    return true;
                }
                else
                {
                    PlayerManager.GetCurrentPlayer().Client.SendError("Item not found in inventory.");
                    return false;
                }
            }
            else
            {
                PlayerManager.GetCurrentPlayer().Client.SendError("Unable to use item due to cage.");
                return false;
            }
        }

        /// <summary>
        /// Temporary methods
        /// </summary>
        /// <returns></returns>
        public Player GetCurrentPlayer()
        {
            return PlayerManager.GetCurrentPlayer();
        }

        public bool HasAllAnswered()
        {
            return PlayerManager.CanSendCycleAck();
        }

        #region PACKET OPERATIONS

        public void BroadcastCurrentPlayer(Packet packet, int millis = -1)
        {
            if (millis == -1)
                PlayerManager.GetCurrentPlayer().Client.Send(packet);
            else
                PlayerManager.GetCurrentPlayer().Client.SendWithDelay(packet, 5000);
        }

        public void Broadcast(Packet packet, GameClient exclude = null)
        {
            foreach (Player player in PlayerManager.GetPlayers())
            {
                if(exclude == null || exclude != player.Client)
                    player.Client.Send(packet);
            }
        }

        public void Broadcast(Packet packet, int millis, GameClient exclude = null)
        {
            foreach (Player player in PlayerManager.GetPlayers())
            {
                if (exclude == null || exclude != player.Client)
                    player.Client.SendWithDelay(packet, millis);
            }
        }

        public void Broadcast(Packet packet, GameClient[] excludes)
        {
            foreach (Player player in PlayerManager.GetPlayers())
            {
                if (!excludes.Contains(player.Client))
                    player.Client.Send(packet);
            }
        }

        #endregion

        #region QUESTION OPERATIONS

        public void QuizTimeUpOrAnswered()
        {
            QuizTimer.Change(Timeout.Infinite, Timeout.Infinite);
            if (Parent.GetType() != typeof(WaitingRoom))
            {
                SQuestion.Add(new SessionQuestion()
                {
                    QuestionId = Questions.Peek().Id,
                    SessionId = this.GetIdentifier()
                });
            }
            if(!PlayerManager.IfThereStillPlayer()) 
            {
                new Task(() => EndGame()).Start();
                return;
            }
            var selectAnswerAck = new SelectAnswerAck()
            {
                Rewards = RewardManager.GetRewards().ToArray()
            };

            Broadcast(selectAnswerAck.CreatePacket());

            RewardManager.Flush();

            var cycleAck = new InitializeCycleAck()
            {
                PlayerSequence = PlayerManager.GetSequence()
            };
            //game.Broadcast(cycleAck.CreatePacket());
            Broadcast(cycleAck.CreatePacket(), 5000);
            var startTurnAck = new StartPlayerTurnAck()
            {
                Duration = 60,
                Token = PlayerManager.GetCurrentPlayer().GetIdentifier()
            };
            // Send the start turn ack to the current player only
            BroadcastCurrentPlayer(startTurnAck.CreatePacket(), 5000);
            Broadcast(new NotifyPlayerTurnAck()
            {
                Duration = 60,
                CurrentPlayer = PlayerManager.GetCurrentPlayer().GetIdentifier()
            }.CreatePacket(), 5000, PlayerManager.GetCurrentPlayer().Client);
            TurnTimer.Change(Timeout.Infinite, 75000);
        }

        public void Answer(GameClient client, int answerIndex)
        {

            var player = PlayerManager.GetPlayers().SingleOrDefault(plyr => plyr.GetIdentifier() == client.Character.Token);
            if(player.Answered == false) // if user haven't answer, then we allow the user to give his/her answer
            {
                player.Answered = true;
                var question = Questions.Peek();
                Reward reward = RewardManager.CreateReward(player.GetIdentifier(), answerIndex == question.Answers.SingleOrDefault(answer => answer.IsCorrect == true)?.Id, player.Answered == false);
                player.Receive(reward);
                var questionAttempted = new QuestionAttempted()
                {
                    AnswerId = answerIndex,
                    QuestionId = question.Id,
                    AttemptedOn = DateTime.Now,
                    AttemptedBy = player.Client.Character.CharacterDb.AccountId,
                    AttemptedFor = this.GetIdentifier()
                };
                Attempts.Add(questionAttempted);
            }
        }
        #endregion

        #region BOARD OPERATION

        public void PrintBoard()
        {
            Console.WriteLine(Board.ToString());
        }

        #endregion

        #region TURN OPERATION

        /// <summary>
        /// Reset all value to default after turn have been completed, a start of new question
        /// </summary>
        public void Reset()
        {
            Questions.Dequeue();
            PlayerManager.Setup();
        }

        public void StartNewCycle()
        {
            Reset();
            Board.ClearInaccessible();
            if (Questions.Count() == 0)
            {
                EndGame();
                return;
            }
            // Broadcast to ask all player to do quiz
            var quizAck = new StartQuizAck()
            {
                DurationInSec = Duration,
                Question = Questions.Peek()
            };
            Broadcast(quizAck.CreatePacket());
            StartQuizTimer();

        }

        private void StartTurnTimer()
        {
            this.TurnTimer.Change(Timeout.Infinite, 60 + 10);
        }

        private void StopTurnTimer()
        {
            this.TurnTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// End of all the player turn, we repeat the process of asking quiz question by sending a StartQuizAck to all the client and await for all reply
        /// </summary>
        public void EndPlayerTurn()
        {
            StopTurnTimer();
            BroadcastCurrentPlayer(new EndPlayerTurnAck().CreatePacket());
            if (IsCurrentPlayerCage())
                PlayerManager.GetCurrentPlayer().Effect = ItemType.NIL;
            PlayerManager.Next();
            if (!PlayerManager.IsCycleStillOngoing())
            {
                new Task(() => StartNewCycle()).Start();
            }
            else
            {
                new Task(() => NextTurn()).Start();
            }
        }

        private void NextTurn()
        {
            Board.Unlock();
            var startPlayerTurnAck = new StartPlayerTurnAck()
            {
                Token = PlayerManager.GetCurrentPlayer().GetIdentifier(),
                Duration = 60
            };
            // Broadcast to all player except current player
            Broadcast(new NotifyPlayerTurnAck()
            {
                Duration = 60,
                CurrentPlayer = PlayerManager.GetCurrentPlayer().GetIdentifier()
            }.CreatePacket(), PlayerManager.GetCurrentPlayer().Client);
            // Broadcast to current player
            BroadcastCurrentPlayer(startPlayerTurnAck.CreatePacket());
            StartTurnTimer();
        }

        /// <summary>
        /// Called at the end of the game, so we can save the session
        /// </summary>
        public void EndGame()
        {
            TurnTimer.Change(Timeout.Infinite, Timeout.Infinite);
            QuizTimer.Change(Timeout.Infinite, Timeout.Infinite);
            if (PlayerManager.GetPlayers().Count != 0)
            {
                SendEndGameAck(PlayerManager.GetPlayers());
                Parent.EndGame();
                foreach (Player p in PlayerManager.GetPlayers())
                {
                    // Call room function to add back the client
                    if (Parent.GetType() != typeof(WaitingRoom))
                    {
                        // Normal Room Session, join back the room once game end.
                        Parent.Join(p.Client);
                        ServerMain.Instance.Server.GameManager.Remove(this);
                    }
                    else
                    {
                        // Teacher session, once end, send everybody back to lobby selection
                        // Update client status on server side to allow the usage of lobby packets
                        p.Client.Character.Status.Update(null);
                    }
                }
            }
            // Need do a callback to update state
            Parent.SetAllUnready();
            PlayerManager.Clear();
            return;
        }

        public void SendEndGameAck(List<Player> players)
        {
            List<Player> updated = players.OrderByDescending(plyr => plyr.Score).ToList();
            List<GameScoreModel> models = new List<GameScoreModel>();
            var attempted = Attempts.Where(a => ServerMain.Instance.Database.GetAnswer(a.AnswerId).IsCorrect == 1).GroupBy(a => a.AttemptedBy);
            var tiles = Board.GetTiles().GroupBy(t => t.Color);
            List<Gloryboard> glories = ServerMain.Instance.Database.GetDbSet<Gloryboard>().ToList();
            for (int i = 0; i < updated.Count(); i++)
            {
                var player = updated.ElementAt(i);
                var tilesObtained = tiles.Where(t => t.Key == player.PlayerColor).FirstOrDefault();
                int tileCollected = 0;
                if (tilesObtained != null)
                    tileCollected = tilesObtained.Count();
                var mastery = player.Client.Character.CharacterDb.Account.Masteries.Where(m => m.TopicId == Parent.Parent.TopicId).FirstOrDefault();
                player.Score += (tileCollected * 150);
                int currentExp;
                player.ExperienceGained *= ServerMain.Instance.ExpRate;
                player.ExperienceGained += ServerMain.Instance.BaseExp;
                if (mastery == null)
                {
                    // Custom Session Mastery - no need calculate
                    mastery = new Mastery()
                    {
                        Exp = 0
                    };
                    player.ExperienceGained = 0;

                }
                int level = Utilities.ComputeLevel(mastery.Exp, out currentExp);
                int lootCount = ServerMain.Instance.LootRate * 1;
                var collection = attempted.SingleOrDefault(g => g.Key == player.Client.Character.CharacterDb.AccountId);
                var gameModelScore = new GameScoreModel()
                {
                    Token = player.GetIdentifier(),
                    Score = player.Score,
                    AnswerCorrectly = collection == null ? 0 : collection.Count(),
                    Experience = player.ExperienceGained,
                    CurrentExperience = currentExp,
                    CurrentLevel = level,
                    LootCount = (byte) lootCount
                };
                player.Client.Character.CharacterDb.ChestCount += lootCount;
                if (level < 10)
                {
                    mastery.Exp += player.ExperienceGained;
                }
                else
                {
                    int x;
                    // Since user is level 10, we'll check if all of his mastery is level 10
                    if(player.Client.Character.Experience.Masteries.Where(m => Utilities.ComputeLevel(m.Exp, out x) < 10).Count() == 0)
                    {
                        if(glories.Where(glory => glory.AccountId == player.Client.Character.CharacterDb.AccountId).Count() == 0)
                        {
                            ServerMain.Instance.Database.GetDbSet<Gloryboard>().Add(new Gloryboard()
                            {
                                AccountId = player.Client.Character.CharacterDb.AccountId,
                                CompletionDate = DateTime.Now
                            });
                        }
                    }
                }
                Attempts.Where(a => a.AttemptedBy == player.Client.Character.CharacterDb.AccountId).ToList().ForEach(player.Client.Character.CharacterDb.Account.QuestionAttempteds.Add);
                Score score = new Score()
                {
                    AccountId = player.Client.Character.CharacterDb.AccountId,
                    CreatedOn = DateTime.Now,
                    Point = player.Score,
                    TopicId = Parent.Parent.TopicId
                };
                Session s = new Session()
                {
                    Id = this.GetIdentifier(),
                    AccountId = player.Client.Character.CharacterDb.AccountId,
                    QuizId = this.QuizId
                };
                player.Client.Character.CharacterDb.Account.Sessions.Add(s);
                //this.GetDbSet<Score>().Add(score);
                player.Client.Character.CharacterDb.Account.Scores.Add(score);
                models.Add(gameModelScore);
            }
            //ServerMain.Instance.Database.RewardChests(updated);
            Log.Debug("Broadcasting");
            Broadcast(new EndGameAck()
            {
                Scores = models.OrderByDescending(model => model.Score).ToList()
            }.CreatePacket());
            ServerMain.Instance.Database.SaveSessionQuestion(SQuestion);
            ServerMain.Instance.Database.Save();

        }
        #endregion

        public void Disconnect(GameClient client)
        {
            PlayerManager.Leave(client);
            if (PlayerManager.IsCycleStillOngoing() && PlayerManager.GetCurrentPlayer().GetIdentifier() == client.Character.Token)
            {
                EndPlayerTurn();
            }
            EventDespatcher.Enqueue(client);

            if (Parent.Owner == client && PlayerManager.GetPlayers().Count > 0)
                Parent.Owner = PlayerManager.GetPlayers().ElementAt(Utilities.Random(PlayerManager.GetPlayers().Count)).Client;
            else if (PlayerManager.GetPlayers().Count == 0)
                new Task(() => EndGame()).Start();
            // In case disconnect and owner is equal to client.
            new Task(() => EventDespatcher.Invoke()).Start();

            // Escalate upward to Room
            Parent.Disconnect(client);

        }

        public void Leave(GameClient client)
        {
            this.PlayerManager.Leave(client);
        }

        public void Join(GameClient client)
        {
            throw new NotImplementedException();
        }
    }
}