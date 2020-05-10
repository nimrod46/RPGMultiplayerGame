using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace RPGMultiplayerGame.Graphics.Ui.QuestsObjects
{
    public class QuestReward
    {

        public QuestReward()
        {
        }

        public void RewardPlayerWithGold(Player player, long gold)
        {
           player.SyncGold += gold;
            ServerManager.Instance.SendGeneralMassageToPlayer("You have been rewarded with: " + 
                ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.Gold, gold.ToString()) 
                + " gold", player);
        }

        public void RewardPlayerWithHealth(Player player, float health)
        {
            player.SyncHealth += health;
        }

        public void RewardPlayerWithItem(Player player, GameItem item)
        {
            ServerManager.Instance.GivePlayerGameItem<GameItem>(player, item, "You have been rewarded with: ");
        }
    }
}
