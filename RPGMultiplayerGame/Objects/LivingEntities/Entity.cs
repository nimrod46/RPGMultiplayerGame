using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MapObjects;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.LivingEntities

{
    abstract class Entity : UpdateObject
    {
        public enum Direction
        {
            Left,
            Up,
            Right,
            Down,
            Idle,
        }
        protected Dictionary<Animation, List<GameTexture>> animations = new Dictionary<Animation, List<GameTexture>>();
        [SyncVar(hook = "OnCurrentAnimationTypeSet")]
        protected int syncCurrentAnimationType;
        [SyncVar]
        protected int syncDirection;
        [SyncVar(hook = "OnIsMovingSet")]
        protected bool syncIsMoving;
        protected EntityID entityID;
        protected float speed;
        protected int animationDelay;
        protected int timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected int collisionOffsetX;
        protected int collisionOffsetY;
        protected Point baseSize;

        public Entity(EntityID entityID, int collisionOffsetX, int collisionOffsetY)
        {
            this.entityID = entityID;
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
            animations = GameManager.Instance.animationsByEntities[entityID];
        }

        public override void OnNetworkInitialize()
        {
            if (!hasFieldsBeenInitialized && hasAuthority)
            {
                syncCurrentAnimationType = (int)Animation.IdleDown;
                syncIsMoving = false;
                syncDirection = (int)Direction.Down;
            }
            speed = 0.5f / 10;
            animationDelay = 100;
            layer -= 0.01f;
            UpdateTexture();
            base.OnNetworkInitialize();
        }

        public virtual void UpdateTexture()
        {
            texture = animations[(Animation) syncCurrentAnimationType][currentAnimationIndex].Texture;
            size = (texture.Bounds.Size.ToVector2() * scale).ToPoint();
            offset = animations[(Animation)syncCurrentAnimationType][currentAnimationIndex].Offset * scale;
        }

        public void OnCurrentAnimationTypeSet()
        {
            currentAnimationIndex = 0;
            UpdateTexture();
            timeSinceLastFrame = 0;
        }

        public void OnIsMovingSet()
        {
            if (!hasAuthority)
            {
                lock (movmentLock)
                {
                    Location = new Vector2(SyncX, SyncY);
                }
            }
        }

        MapObjectLib block = null;
        Rectangle rect;
        System.Drawing.Rectangle rectt;
        public override void Update(GameTime gameTime)
        {
            lock (movmentLock)
            {
                if (syncIsMoving)
                {
                    double movment = speed * gameTime.ElapsedGameTime.TotalMilliseconds;
                    Vector2 newLocation = Location;
                    switch ((Direction)syncDirection)
                    {
                        case Direction.Up:
                            newLocation.Y -= (float)movment;
                            break;
                        case Direction.Down:
                            newLocation.Y += (float)movment;
                            break;
                        case Direction.Left:
                            newLocation.X -= (float)movment;
                            break;
                        case Direction.Right:
                            newLocation.X += (float)movment;
                            break;
                    }
                    rect = GetCollisionRect(newLocation.X, newLocation.Y, texture.Width, texture.Height);
                    rectt = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                    for (int i = 0; i < GameManager.Instance.map.GraphicObjects.Count; i++)
                    {
                        if (GameManager.Instance.map.GraphicObjects[i] is BlockLib && GameManager.Instance.map.GraphicObjects[i].Layer > 0 && GameManager.Instance.map.GraphicObjects[i].Rectangle.IntersectsWith(rectt))
                        {
                            block = GameManager.Instance.map.GraphicObjects[i];
                            break;
                        }
                    }
                    if (block == null)
                    {
                        if (hasAuthority)
                        {
                            SyncX = newLocation.X;
                            SyncY = newLocation.Y;
                        }
                        Location = newLocation;
                    }
                    else
                    {
                        block = null;
                    }
                }
            }
            timeSinceLastFrame += (int)(speed * 500);
            if (timeSinceLastFrame >= animationDelay)
            {
                timeSinceLastFrame = 0;
                if (currentAnimationIndex + 1 >= animations[(Animation)syncCurrentAnimationType].Count)
                {
                    currentAnimationIndex = 0;
                }
                else
                {
                    currentAnimationIndex++;
                }
                UpdateTexture();
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
        }

        protected void StartMoving(Direction direction)
        {
            syncIsMoving = true;
            syncDirection = (int)direction;
            syncCurrentAnimationType = (int)direction;
        }

        public void StopMoving()
        {
            if (syncIsMoving)
            {
                syncIsMoving = false;
                syncCurrentAnimationType += (int) Enum.GetValues(typeof(Direction)).Cast<Direction>().Max();
            }
        }

        private Rectangle GetCollisionRect(float x, float y, int width, int height)
        {
            return new Rectangle((int)x + collisionOffsetX, (int)y + collisionOffsetY, width - collisionOffsetX, height - collisionOffsetY);
        }

        [BroadcastMethod]
        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            if (hasAuthority)
            {
                SyncX = spawnPoint.SyncX;
                SyncY = spawnPoint.SyncY;
            }
            SetSpawnPointLocaly(spawnPoint);
        }

        private void SetSpawnPointLocaly(SpawnPoint spawnPoint)
        {
            GameManager.Instance.spawnPoint = spawnPoint;
        }
    }
}
