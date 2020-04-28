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
        protected readonly int syncHealth;

        public HealthPotion(ItemType itemType, string name, int count, int health) : base(itemType, name, count, e => { e.SyncHealth += health; })
        {
            this.syncHealth = health;
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Gives:" + syncHealth + " hp\n";
        }
    }
}
