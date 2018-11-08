using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Left,
            Up,
            Right,
            Down,
        }
        protected Dictionary<Animation, List<Texture2D>> animations = new Dictionary<Animation, List<Texture2D>>();
        protected int animationDelay;
        [SyncVar(networkInterface = NetworkInterface.TCP)]
        protected int syncCurrentAnimationType;
        [SyncVar(networkInterface = NetworkInterface.TCP)]
        protected int syncDirection;
        protected EntityID entityID;
        protected int idleIndex;
        protected float speed;
        protected int timeSinceLastFrame;
        [SyncVar(networkInterface = NetworkInterface.TCP)]
        protected bool syncIsMoving;
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnAnimationIndexSet")]
        protected int syncCurrentAnimationIndex;
        public Entity(EntityID entityID, int idleIndex)
        {
            this.entityID = entityID;
            this.idleIndex = idleIndex;
        }

        public override void OnNetworkInitialize()
        {
            if (isServer)
            {
                return;
            }
            base.OnNetworkInitialize();
            if (!hasFieldsBeenInitialized)
            {
                syncCurrentAnimationType = (int)Animation.WalkDown;
                syncCurrentAnimationIndex = idleIndex;
                syncIsMoving = false;
                syncDirection = (int)Direction.Down;
                Console.WriteLine("Init " + id + " " + "defaults");
            }
            else
            {
                Console.WriteLine("Init " + id + " " + syncCurrentAnimationType + " " + syncCurrentAnimationIndex);
            }
            speed = 0.5f / 10;
            animationDelay = 100;
            animations = GameManager.Instance.animationsByEntities[entityID];
            
            OnAnimationIndexSet();
        }

        public void OnAnimationIndexSet()
        {
            texture = animations[(Animation) syncCurrentAnimationType][syncCurrentAnimationIndex];
        }

        public override void Update(GameTime gameTime)
        {

            if (!hasAuthority || isServer)
            {
                return;
            }

            if (syncIsMoving)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > animationDelay)
                {
                    timeSinceLastFrame = 0;
                    if (syncCurrentAnimationIndex + 1 >= animations[(Animation) syncCurrentAnimationType].Count)
                    {
                        syncCurrentAnimationIndex = 0;
                    }
                    else
                    {
                        syncCurrentAnimationIndex++;
                    }
                }
                double movment = speed * gameTime.ElapsedGameTime.Milliseconds;
                switch ((Direction) syncDirection)
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
