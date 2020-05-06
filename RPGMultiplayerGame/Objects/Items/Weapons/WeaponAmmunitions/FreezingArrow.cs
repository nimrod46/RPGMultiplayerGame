using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class FreezingArrow : Arrow
    {
        public FreezingArrow() : base(GraphicManager.WeaponAmmunitionId.FreezingArrow, 1)
        {
            Scale *= 0.5f;
        }
    }
}
