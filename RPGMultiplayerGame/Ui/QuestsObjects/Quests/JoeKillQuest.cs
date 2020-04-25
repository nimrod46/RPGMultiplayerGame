using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.QuestsObjects.Quests
{
    public class JoeKillQuest : KillQuest
    {
        public JoeKillQuest() : base((windowSize) => Microsoft.Xna.Framework.Vector2.Zero, Ui.UiComponent.PositionType.ButtomCentered, nameof(Joe), "Kill 5 bats", new Action<Player>(player => {
            player.AddItemToInventory(ItemType.CommonWond);
            player.SyncHealth = 1000;
        }), GraphicManager.EntityId.Bat, 5)
        {
        }
    }
}
