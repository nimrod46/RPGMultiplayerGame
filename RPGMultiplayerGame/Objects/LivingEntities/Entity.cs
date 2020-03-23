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
    public abstract class Entity : UpdateObject
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
        [SyncVar(hook = "OnCurrentAnimationTypeSet", invokeThroughAction = true)]
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
        protected bool shouldLoopAnimation;

        public Entity(EntityID entityID, int collisionOffsetX, int collisionOffsetY)
        {
            this.entityID = entityID;
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
            animations = new Dictionary<Animation, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityID]);
            syncCurrentAnimationType = (int)Animation.IdleDown;
            syncIsMoving = false;
            syncDirection = (int)Direction.Down;
            speed = 0.5f / 10;
            animationDelay = 100;
            Layer = GameManager.ENTITY_LAYER;
            shouldLoopAnimation = true;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            UpdateTexture();
            GameManager.Instance.AddEntity(this);
        }

        public virtual void UpdateTexture()
        {
            GameTexture gameTexture = animations[(Animation)syncCurrentAnimationType][currentAnimationIndex];
            texture = gameTexture.Texture;
            offset = gameTexture.Offset * scale;
            Size = (texture.Bounds.Size.ToVector2() * scale).ToPoint();
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
        Rectangle newLocationRect;
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
                    newLocationRect = GetCollisionRect(newLocation.X, newLocation.Y, Size.X, Size.Y);
                    rectt = new System.Drawing.Rectangle(newLocationRect.X, newLocationRect.Y, newLocationRect.Width, newLocationRect.Height);
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
                if (getIsLoopAnimationFinished())
                {
                    if (shouldLoopAnimation)
                    {
                        currentAnimationIndex = 0;
                    }
                }
                else
                {
                    currentAnimationIndex++;
                }
                UpdateTexture();
            }
            base.Update(gameTime);
        }

        protected bool getIsLoopAnimationFinished()
        {
            return currentAnimationIndex + 1 >= animations[(Animation)syncCurrentAnimationType].Count;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
        }

        protected void StartMoving(Direction direction)
        {
            syncIsMoving = true;
            MoveAtDir(direction);
        }

        protected void IdleAtDir(Direction direction)
        {
            shouldLoopAnimation = true;
            AnimationAtDir(direction, 4);
        }

        protected void MoveAtDir(Direction direction)
        {
            shouldLoopAnimation = true;
            AnimationAtDir(direction, 0);
        }

        protected void AttackAtDir(Direction direction)
        {
            shouldLoopAnimation = false;
            AnimationAtDir(direction, 8);
        }

        private void AnimationAtDir(Direction direction, int dirToAnimationIndex)
        {
            currentAnimationIndex = 0;
            syncDirection = (int)direction;
            syncCurrentAnimationType = (int)direction + dirToAnimationIndex;
        }

        public void StopMoving()
        {
            if (syncIsMoving)
            {
                syncIsMoving = false;
                syncCurrentAnimationType = syncDirection + 4;
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

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            GameManager.Instance.RemoveEntity(this);
        }

        private void SetSpawnPointLocaly(SpawnPoint spawnPoint)
        {
            GameManager.Instance.spawnPoint = spawnPoint;
        }
    }
}
