using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Potions
{
    public abstract class Potion : StackableGameItem
    {
        private readonly Action<Entity> action;
        public Potion(ItemType itemType, string name, int count, Action<Entity> action) : base(itemType, name, count)
        {
            this.action = action;
        }

        public void UseOn(Entity player)
        {
            action.Invoke(player);
            Use();
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Type:" + "potion";
        }
    }
}
