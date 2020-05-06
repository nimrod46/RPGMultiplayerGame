using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class IceBow : RangedWeapon
    {
        public IceBow() : base(ItemType.IceBow, "Ice Bow", 40, new FreezingArrow(), 0.5f)
        {
            Scale = 0.46f;
            UiScale = 0.46f;
            AddSpecielWeaponEffect<FreezingWeaponEffect>();
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Speciel effect: slows down enemies for " + FreezingWeaponEffect.LASTING_TIME + " seconds";
        }
    }
}
