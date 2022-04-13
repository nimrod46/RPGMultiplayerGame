using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Bat : Monster
    {
        public Bat() : base(GraphicManager.EntityId.Bat, 3, 2, 100, GraphicManager.Instance.PlayerNameFont)
        {
            SyncName = "Bat";
            animationTimeDelay *= 0.7;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            CollisionSizeType = Ui.UiComponent.PositionType.TopLeft;
            CollisionSize = new Vector2(6, 6);
        }

        public override void Kill(Entity attacker)
        {
            base.Kill(attacker);
            if (!isInServer) 
                return;
            
            SyncEquippedWeapon?.SetAsMapItem(Location);
            SyncEquippedWeapon = null;
        }
    }
}
