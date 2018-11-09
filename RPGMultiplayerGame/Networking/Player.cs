using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RPGMultiplayerGame.Other;
using static RPGMultiplayerGame.Other.GameManager;

namespace RPGMultiplayerGame.Networking
{
    class Player : Entity
    {
        List<Keys> currentArrowsKeysPressed = new List<Keys>();
        Keys oldKey;
        public Player() : base(EntityID.Player, 1, 0, 10)
        {

        }

        public override void OnLocalPlayerInitialize()
        {
            speed *= 2;
            InputManager.Instance.OnArrowsKeysStateChange += Instance_OnArrowsKeysStateChange;
        }

        private void Instance_OnArrowsKeysStateChange(Keys key, bool isDown)
        {
            if (isDown)
            {
                Direction direction = (Direction)((int)key - (int)Keys.Left);
                StartMoving(direction);
                oldKey = key;
            }
            else
            {
                if (oldKey == key)
                {
                    StopMoving();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void StartMoving(Direction direction)
        {
            syncIsMoving = true;
            syncCurrentAnimationIndex = idleIndex;
            this.syncDirection = (int) direction;
            syncCurrentAnimationType = (int) direction;
        }


        private void StopMoving()
        {
            if (syncIsMoving)
            {
                syncIsMoving = false;
                syncCurrentAnimationIndex = idleIndex;
            }
        }

        public override void OnDestroyed()
        {
            base.OnDestroyed();
            InputManager.Instance.OnArrowsKeysStateChange -= Instance_OnArrowsKeysStateChange;
        }
    }
}
