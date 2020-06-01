using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonWond : RangedWeapon
    {
        public CommonWond() : base(ItemType.CommonWond, "Common Wand", 15, new FireBall(), 2)
        {
            Scale = 0.5f;
        }
    }
}
