using Microsoft.Xna.Framework;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonSword : MeleeWeapon
    {
        public CommonSword() : base(ItemType.CommonSword, "Common sword", new Point(7, 6), 5, 0)
        {

        }
    }
}
