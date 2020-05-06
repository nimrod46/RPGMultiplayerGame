using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class StormBow : RangedWeapon
    {
        public StormBow() : base(ItemType.StormBow, "Storm Bow", 40, new StormArrow(), 0.5f)
        {
            Scale = 0.46f;
            UiScale = 0.46f;
            AddSpecielWeaponEffect<StormWeaponEffect>();
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Speciel effect: blows away enemies for " + StormWeaponEffect.LASTING_TIME + " seconds";
        }
    }
}
