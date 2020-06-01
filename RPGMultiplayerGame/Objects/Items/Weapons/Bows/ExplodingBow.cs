using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class ExplodingBow : RangedWeapon
    {
        public ExplodingBow() : base(ItemType.ExplodingBow, "Exploding Bow", 30, new ExplodingArrow(), 0.5f)
        {
            Scale = 0.25f;
            UiScale = 0.46f;
            AddSpecielWeaponEffect<ExplotionWeaponEffect>();
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Speciel effect: causing an explotion \n        that hits nearby enemies";
        }
    }
}
