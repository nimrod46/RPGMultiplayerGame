using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class StormArrow : Arrow
    {
        public StormArrow() : base(GraphicManager.WeaponAmmunitionId.StormArrow, 1)
        {
            Scale *= 0.5f;
        }
    }
}
