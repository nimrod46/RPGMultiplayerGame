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
        public Player() : base(EntityID.Player, 1)
        {

        }

        public override void OnLocalPlayerInitialize()
        {
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
            if (!hasAuthority || isServer)
            {
                return;
            }
        }

        private void StartMoving(Direction direction)
        {
            isMoving = true;
            currentAnimationIndex = idleIndex;
            this.direction = (int) direction;
            currentAnimationType = (int) direction;
        }


        private void StopMoving()
        {
            if (isMoving)
            {
                isMoving = false;
                currentAnimationIndex = idleIndex;
            }
        }
    }
}
