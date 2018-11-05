using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static RPGMultiplayerGame.Other.GameManager;

namespace RPGMultiplayerGame.Networking
{
    class Player : Entity
    {

        public Player() : base(EntityID.Player)
        {

        }

        private KeyboardState mainState;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority || isServer)
            {
                return;
            }
            mainState = Keyboard.GetState();
            if (mainState.GetPressedKeys().Contains(Keys.Up))
            {
                Move(Direction.Up);
            }
            else if(mainState.GetPressedKeys().Contains(Keys.Down))
            {
                Move(Direction.Down);
            }
            else if (mainState.GetPressedKeys().Contains(Keys.Right))
            {
                Move(Direction.Right);
            }
            else if (mainState.GetPressedKeys().Contains(Keys.Left))
            {
                Move(Direction.Left);
            }
            else
            {
                StopMoving();
            }
        }

        private void StartMoving(Direction direction)
        {
            isMoving = true;
            currentAnimationIndex = 0;
            this.direction = (int) direction;
            currentAnimationType = (int) direction;
        }

        private void Move(Direction direction)
        {
            if (!isMoving)
            {
                StartMoving(direction);
            }
        }

        private void StopMoving()
        {
            if (isMoving)
            {
                isMoving = false;
                currentAnimationIndex = 0;
            }
        }
    }
}
