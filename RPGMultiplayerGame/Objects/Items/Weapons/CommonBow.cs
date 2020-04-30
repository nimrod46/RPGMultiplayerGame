using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonBow : RangedWeapon
    {
        public CommonBow() : base(ItemType.CommonBow, "Common Bow", 10, new CommonArrow(), 0.5f)
        {
            Scale = 0.46f;
            AddSpecielWeaponEffect<FreezingEffect>();
        }
    }
}
