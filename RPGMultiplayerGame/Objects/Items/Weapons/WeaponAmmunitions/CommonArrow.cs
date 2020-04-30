using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public class CommonArrow : WeaponAmmunition
    {
        public CommonArrow() : base(GraphicManager.WeaponAmmunitionId.CommonArrow)
        {
            SyncSpeed *= 1.5f;
            scale *= 0.5f;
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
