using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base(Microsoft.Xna.Framework.Vector2.Zero, Ui.UiComponent.PositionType.ButtomCentered, nameof(Joe), "Kill 5 bats", new Action<Player>(player => {
            player.AddItemToInventory(ItemType.CommonWond);
            player.SyncHealth = 1000;
        }), EntityId.Bat, 5)
        {
        }
    }
}
