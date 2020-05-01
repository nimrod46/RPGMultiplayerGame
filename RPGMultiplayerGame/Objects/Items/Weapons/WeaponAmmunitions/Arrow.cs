using RPGMultiplayerGame.Managers;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public abstract class Arrow : WeaponAmmunition
    {
        public Arrow(WeaponAmmunitionId weaponAmmunitionId, int maxHitCount) : base(weaponAmmunitionId, maxHitCount)
        {
            SyncSpeed *= 1.5f;
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
