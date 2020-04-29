using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponEffects
{
    public class FireBall : WeaponEffect
    {
        public FireBall() : base(GraphicManager.EffectId.FireBall)
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
