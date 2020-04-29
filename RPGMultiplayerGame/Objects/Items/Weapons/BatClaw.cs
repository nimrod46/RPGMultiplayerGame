using Microsoft.Xna.Framework;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class BatClaw : MeleeWeapon
    {
        public BatClaw() : base(ItemType.BatClaw, "Bat claw", new Point(5, 5), 5, 2.5)
        {
        }
    }
}
