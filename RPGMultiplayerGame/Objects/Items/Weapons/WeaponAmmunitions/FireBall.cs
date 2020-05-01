using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class FireBall : WeaponAmmunition
    {
        public FireBall() : base(GraphicManager.WeaponAmmunitionId.FireBall, int.MaxValue)
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            if (hasAuthority)
            {
                SetTimeToDestroy(5);
            }
        }
    }
}
