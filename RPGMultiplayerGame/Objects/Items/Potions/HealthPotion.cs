using RPGMultiplayerGame.Objects.InventoryObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Potions
{
    public abstract class HealthPotion : Potion
    {
        public HealthPotion(Inventory.ItemType itemType, int count, int health) : base(itemType, count, e => { e.SyncHealth += health; })
        {
        }
    }
}
