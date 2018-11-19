﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        [SyncVar(networkInterface = NetworkInterface.TCP, hook = "OnCurrentAnimationTypeSet", invokeInServer = false)]
        protected int syncCurrentAnimationType;
        [SyncVar(networkInterface = NetworkInterface.TCP)]
        protected int syncDirection;
        protected EntityID entityID;
        protected float speed;
        protected int timeSinceLastFrame;
        [SyncVar(networkInterface = NetworkInterface.TCP)]
        protected bool syncIsMoving;
        protected int currentAnimationIndex;
        protected int collisionOffsetX;
        protected int collisionOffsetY;

        public Entity(EntityID entityID, int collisionOffsetX, int collisionOffsetY)
        {
            this.entityID = entityID;
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
            if (!hasFieldsBeenInitialized && hasAuthority)
            {
                hasInitialized = true;
                syncCurrentAnimationType = (int)Animation.IdleDown;
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
            texture = animations[(Animation) syncCurrentAnimationType][currentAnimationIndex];
        }

        public void OnCurrentAnimationTypeSet()
        {
            currentAnimationIndex = 0;
            OnAnimationIndexSet();
            timeSinceLastFrame = animationDelay;
        }

        public override void Update(GameTime gameTime)
        {
            if (syncIsMoving)
            {
                if (hasAuthority && !isServerAuthority)
                {
                    double movment = speed * gameTime.ElapsedGameTime.Milliseconds;
                    Vector2 newLocation = new Vector2(Location.X, Location.Y);
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
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            timeSinceLastFrame += (int)(speed * 100);
            if (timeSinceLastFrame > animationDelay)
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
                texture = animations[(Animation)syncCurrentAnimationType][currentAnimationIndex];
            }
        }

        protected void StartMoving(Direction direction)
        {
            syncIsMoving = true;
            syncDirection = (int)direction;
            syncCurrentAnimationType = (int)direction;
        }

        protected void StopMoving()
        {
            if (syncIsMoving)
            {
                syncIsMoving = false;
                syncCurrentAnimationType += (int) Enum.GetValues(typeof(Direction)).Cast<Direction>().Max() + 1;
            }
        }

        private Rectangle GetCollisionRect(float x, float y, int width, int height)
        {
            return new Rectangle((int)x + collisionOffsetX, (int)y + collisionOffsetY, width - collisionOffsetX, height - collisionOffsetY);
        }

        [BroadcastMethod]
        public void SetSpawnPoint(NetBlock spawnPoint)
        {
            if (hasAuthority)
            {
                SyncX = spawnPoint.SyncX;
                SyncY = spawnPoint.SyncY;
            }
            SetSpawnPointLocaly(spawnPoint);
        }

        private void SetSpawnPointLocaly(NetBlock spawnPoint)
        {
            MapManager.Instance.spawnPoint = spawnPoint;
        }
    }
}
