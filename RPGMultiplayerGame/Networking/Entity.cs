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
        [SyncVar(networkInterface = NetworkInterface.UDP)]
        protected int currentAnimationType;
        [SyncVar(networkInterface = NetworkInterface.UDP)]
        protected int direction;
        protected EntityID entityID;
        protected float speed;
        protected int timeSinceLastFrame;
        [SyncVar(networkInterface = NetworkInterface.UDP)]
        protected bool isMoving;
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnAnimationIndexSet")]
        protected int currentAnimationIndex;

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
            animationDelay = 100;
            currentAnimationType = (int) Animation.WalkDown;
            currentAnimationIndex = 0;
            speed = 0.05f;
            isMoving = false;
            direction = (int) Direction.Down;
        }

        public void OnAnimationIndexSet()
        {
            texture = animations[(Animation) currentAnimationType][currentAnimationIndex];
        }

        public override void Update(GameTime gameTime)
        {

            if (!hasAuthority || isServer)
            {
                return;
            }
            

            if (isMoving)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > animationDelay)
                {
                    timeSinceLastFrame = 0;
                    if (currentAnimationIndex + 1 >= animations[(Animation) currentAnimationType].Count)
                    {
                        currentAnimationIndex = 0;
                    }
                    else
                    {
                        currentAnimationIndex++;
                    }
                }
                double movment = speed * gameTime.ElapsedGameTime.Milliseconds;
                switch ((Direction) direction)
                {
                    case Direction.Up:
                        SyncY -= (float) movment;
                        break;
                    case Direction.Down:
                        SyncY += (float) movment;
                        break;
                    case Direction.Left:
                        SyncX -= (float)movment;
                        break;
                    case Direction.Right:
                        SyncX += (float) movment;
                        break;
                }
            }
        }
    }
}
