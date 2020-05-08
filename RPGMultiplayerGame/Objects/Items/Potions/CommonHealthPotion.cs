namespace RPGMultiplayerGame.Objects.Items.Potions
{
    public class CommonHealthPotion : HealthPotion
    {
        public CommonHealthPotion() : base(ItemType.CommonHealthPotion, "Common health potion", 1, 20)
        {
            Scale = 0.4f;
        }
    }
}
