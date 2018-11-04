using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Other;
using static RPGMultiplayerGame.Other.GameManager;

namespace RPGMultiplayerGame.Networking
{
    abstract class Entity : GameObject
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        protected Dictionary<Animation, List<Texture2D>> animations = new Dictionary<Animation, List<Texture2D>>();
        protected int animationDelay;
        protected Animation currentAnimationType;
        protected Direction direction;
        [SyncVar(hook = "OnAnimationIndexSet")]
        protected int currentAnimationIndex;
        protected EntityID entityID;
        protected float speed;
        private int timeSinceLastFrame;
        private bool isMoving;
        public Entity(EntityID entityID)
        {
            this.entityID = entityID;
        }

        public override void OnNetworkInitialize()
        {
            if (isServer)
            {
                return;
            }
            base.OnNetworkInitialize();
            animations = GameManager.Instance.animationsByEntities[entityID];
            animationDelay = 50;
            currentAnimationType = Animation.Walk;
            currentAnimationIndex = 0;
            speed = 0.3f;
            isMoving = false;
            direction = Direction.Down;
        }

        public void OnAnimationIndexSet()
        {
            texture = animations[currentAnimationType][currentAnimationIndex];
        }

        public override void Update(GameTime gameTime)
        {

            if (!hasAuthority || isServer)
            {
                return;
            }
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > animationDelay)
            {
                timeSinceLastFrame = 0;
                if (currentAnimationIndex + 1 >= animations[currentAnimationType].Count)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex++;
                }
            }

            if (isMoving)
            {
                switch (direction)
                {
                    case Direction.Up:
                        SyncY -= speed;
                        break;
                    case Direction.Down:
                        SyncY += speed;
                        break;
                    case Direction.Left:
                        SyncX -= speed;
                        break;
                    case Direction.Right:
                        SyncX += speed;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
