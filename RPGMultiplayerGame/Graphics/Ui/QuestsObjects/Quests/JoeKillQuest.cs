using RPGMultiplayerGame.Graphics.Ui.QuestsObjects;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(nameof(Joe), "Kill 5 bats", new Action<QuestReward, Player>((questReward,player) =>
        {
            questReward.RewardPlayerWithItem(player, new IceBow());
            questReward.RewardPlayerWithHealth(player,1000);
            questReward.RewardPlayerWithGold(player,150);
        }), GraphicManager.EntityId.Bat, 5)
        {
        }
    }
}
