using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(nameof(Joe), "Kill 5 bats", new Action<Player>(player =>
        {
            ServerManager.Instance.GivePlayerGameItem(player, new CommonWond());
            player.SyncHealth = 1000;
            player.SyncGold += 150;
        }), GraphicManager.EntityId.Bat, 5)
        {
        }
    }
}
