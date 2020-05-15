using Microsoft.Xna.Framework;
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
            switch (SyncCurrentDirection)
            {
                case Direction.Left:
                    CollisionSizeType = Ui.UiComponent.PositionType.CenteredLeft;
                    break;
                case Direction.Up:
                    CollisionSizeType = Ui.UiComponent.PositionType.TopCentered;
                    break;
                case Direction.Right:
                    CollisionSizeType = Ui.UiComponent.PositionType.CenteredRight;
                    break;
                case Direction.Down:
                    CollisionSizeType = Ui.UiComponent.PositionType.ButtomCentered;
                    break;
                case Direction.Idle:
                    break;
                default:
                    break;
            }
            CollisionSize = new Vector2(0.1f, 0.1f);
        }
    }
}
