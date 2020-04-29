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
