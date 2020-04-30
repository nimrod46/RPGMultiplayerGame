using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonBow : RangedWeapon
    {
        public CommonBow() : base(ItemType.CommonBow, "Common Bow", new Point(0, 0), 15, new CommonArrow(), 0.5f)
        {
            Scale = 0.46f;
        }
    }
}
