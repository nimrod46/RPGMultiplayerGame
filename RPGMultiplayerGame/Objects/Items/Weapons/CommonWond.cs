using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponEffects;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class CommonWond : RangedWeapon
    {
        public CommonWond() : base(ItemType.CommonWond, "Common Wand", new Point(0, 0), 50, new FireBall(), 2)
        {
        }
    }
}
