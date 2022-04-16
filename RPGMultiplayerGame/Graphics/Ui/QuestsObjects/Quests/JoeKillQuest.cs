using System;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;

namespace RPGMultiplayerGame.Graphics.Ui.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(nameof(Joe), "Kill 5 bats", (questReward, player) =>
        {
            questReward.RewardPlayerWithItem(player, new IceBow());
            questReward.RewardPlayerWithHealth(player,1000);
            questReward.RewardPlayerWithGold(player,150);
        }, GraphicManager.EntityId.Bat, 5)
        {
        }
    }
}
