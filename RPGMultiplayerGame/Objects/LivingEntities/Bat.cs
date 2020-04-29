using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects.LivingEntities
{
    public class Bat : Monster
    {
        public Bat() : base(GraphicManager.EntityId.Bat, 0, 0, 100, GraphicManager.Instance.PlayerNameFont)
        {
            SyncName = "Bat";
            animationTimeDelay *= 0.7;
        }
    }
}
