using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class ExplodingBow : RangedWeapon
    {
        public ExplodingBow() : base(ItemType.ExplodingBow, "Exploding Bow", 10, new ExplodingArrow(), 0.5f)
        {
            Scale = 0.46f;
            AddSpecielWeaponEffect<ExplotionWeaponEffect>();
        }
    }
}
