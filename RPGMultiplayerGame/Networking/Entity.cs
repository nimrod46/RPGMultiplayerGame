using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
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
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnAnimationIndexSet", invokeInServer = false)]
        protected int syncCurrentAnimationIndex;
        protected int collisionOffsetX;
        protected int collisionOffsetY;
        public Entity(EntityID entityID, int idleIndex, int collisionOffsetX, int collisionOffsetY)
        {
            this.entityID = entityID;
            this.idleIndex = idleIndex;
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
        }

        public override void OnNetworkInitialize()
        {
            if (isServerAuthority)
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

            if (!hasAuthority || isServerAuthority)
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
                Vector2 newLocation = new Vector2(Location.X, Location.Y);
                switch ((Direction) syncDirection)
                {
                    case Direction.Up:
                        newLocation.Y -= (float) movment;
                        break;
                    case Direction.Down:
                        newLocation.Y += (float) movment;
                        break;
                    case Direction.Left:
                        newLocation.X -= (float)movment;
                        break;
                    case Direction.Right:
                        newLocation.X += (float) movment;
                        break;
                }
                Rectangle rect = GetCollisionRect(newLocation.X, newLocation.Y, texture.Width, texture.Height);
                Block block = MapManager.Instance.map.blocks.FirstOrDefault(b => b.Layer > 0 && 
                b.Rectangle.IntersectsWith(new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height)));
                if (block == null)
                {
                    SyncX = newLocation.X;
                    SyncY = newLocation.Y;
                }
            }
        }
        private Rectangle GetCollisionRect(float x, float y, int width, int height)
        {
            return new Rectangle((int)x + collisionOffsetX, (int)y + collisionOffsetY, width - collisionOffsetX, height - collisionOffsetY);
        }
    }
}
