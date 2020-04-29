using Microsoft.Xna.Framework;
namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class UpdateableObject : GameObject, IGameUpdateable
    {
        private double timeToDestroy;
        private bool isOnCountDown;

        public UpdateableObject()
        {
            timeToDestroy = 0;
            isOnCountDown = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (isOnCountDown)
            {
                timeToDestroy -= gameTime.ElapsedGameTime.TotalSeconds;
                if (timeToDestroy <= 0)
                {
                    InvokeBroadcastMethodNetworkly(nameof(Destroy));
                }
            }
        }

        public void SetTimeToDestroy(double timeToDestroy)
        {
            this.timeToDestroy = timeToDestroy;
            isOnCountDown = true;
        }
    }
}
