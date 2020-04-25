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
        private readonly int health;

        public HealthPotion(ItemType itemType, string name, int count, int health) : base(itemType, name, count, e => { e.SyncHealth += health; })
        {
            this.health = health;
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Gives:" + health + " hp\n";
        }
    }
}
