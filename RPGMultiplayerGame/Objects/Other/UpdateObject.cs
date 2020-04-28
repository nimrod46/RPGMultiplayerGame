using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class UpdateObject : GameObject, IGameUpdateable
    {
        private double timeToDestroy;
        private bool isOnCountDown;

        public UpdateObject()
        {
            timeToDestroy = 0;
            isOnCountDown = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(isOnCountDown)
            {
                timeToDestroy -= gameTime.ElapsedGameTime.TotalSeconds;
                if(timeToDestroy <= 0)
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
