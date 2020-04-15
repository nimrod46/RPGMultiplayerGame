using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(nameof(Joe), "Kill 5 bats", new Action<Player>(player => {
            player.AddItemToInventory((int) ItemType.CommonWond);
            player.SyncHealth = 1000;
        }), EntityId.Bat, 5)
        {
        }
    }
}
