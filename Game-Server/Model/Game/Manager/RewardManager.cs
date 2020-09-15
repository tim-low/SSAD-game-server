 using System;
using System.Collections.Generic;

namespace Game_Server.Model
{
    public class RewardManager
    {

        private List<Reward> Rewards;

        public RewardManager()
        {
            Rewards = new List<Reward>();
        }

        public Reward CreateReward(string identifier, bool isCorrect, bool timeout)
        {
            Reward reward = new Reward()
            {
                IsCorrect = isCorrect,
                Item = new InventoryItem()
                {
                    Item = ItemType.NIL
                },
                Step = 2
            };
            if(isCorrect)
            {
                reward = GameFactory.CreateReward();
                reward.IsCorrect = isCorrect;
            }
            reward.Token = identifier;
            reward.Timeout = timeout;
            Rewards.Add(reward);
            return reward;
        }

        public List<Reward> GetRewards()
        {
            return Rewards;
        }

        public void Flush()
        {
            Rewards.Clear();
        }
    }
}
