using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Potions
{
    public abstract class Potion : StackableItem
    {
        private readonly Action<Entity> action;
        public Potion(Inventory.ItemType itemType, int count, Action<Entity> action) : base(itemType, count)
        {
            this.action = action;
        }

        public void UseOn(Entity player)
        {
            action.Invoke(player);
            Use();
        }
    }
}
