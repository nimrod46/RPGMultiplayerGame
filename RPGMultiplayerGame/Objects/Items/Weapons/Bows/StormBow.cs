using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class StormBow : RangedWeapon
    {
        public StormBow() : base(ItemType.StormBow, "Storm Bow", 10, new StormArrow(), 0.5f)
        {
            Scale = 0.46f;
            AddSpecielWeaponEffect<StormWeaponEffect>();
        }
    }
}
