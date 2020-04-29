using RPGMultiplayerGame.Objects.LivingEntities;
using System;

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
