﻿using System;
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
using RPGMultiplayerGame.Objects.Weapons;
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
            Idle, //TODO: Delete
        }

        public enum EntityState
        {
            Idle,
            Moving,
            Attacking
        }

        public Weapon Weapon { get; private set; }

        private Vector2 healthBarOffset;
        private Vector2 healthBarSize;
        private readonly float maxHealth;
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        [SyncVar(hook = "OnHealthSet")]
        protected float syncHealth; 
        protected float textLyer;
        protected Dictionary<Animation, List<GameTexture>> animations = new Dictionary<Animation, List<GameTexture>>();
        protected int currentAnimationType;
        protected int currentDirection;
        protected int currentEntityState;
        protected EntityID entityID;
        protected float speed;
        protected int animationDelay;
        protected int timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected int collisionOffsetX;
        protected int collisionOffsetY;
        protected bool shouldLoopAnimation;

        public Entity(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth)
        {
            this.entityID = entityID;
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
            this.maxHealth = maxHealth;
            syncHealth = maxHealth;
            healthBar = GameManager.Instance.HealthBar;
            healthBarBackground = GameManager.Instance.HealthBarBackground;
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
            animations = new Dictionary<Animation, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityID]);
            currentAnimationType = (int)Animation.IdleDown;
            currentDirection = (int)Direction.Down;
            currentEntityState = (int)EntityState.Idle;
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
            textLyer = CHARECTER_TEXT_LAYER + DefaultLayer;

        }

        public virtual void UpdateTexture()
        {
            GameTexture gameTexture = animations[(Animation)currentAnimationType][currentAnimationIndex];
            texture = gameTexture.Texture;
            offset = gameTexture.Offset * scale;
            Size = (texture.Bounds.Size.ToVector2() * scale).ToPoint();
            UpdateDrawOffset();
        }

        public void OnHealthSet()
        {
            healthBarSize.X = syncHealth * healthBar.Width / maxHealth;
        }

        protected virtual void UpdateDrawOffset()
        {
            healthBarOffset = new Vector2(BaseSize.X / 2 - healthBarBackground.Width / 2, -healthBarBackground.Height - 2);
        }

        [BroadcastMethod]
        public void EquipeWith(Weapon weapon)
        {
            this.Weapon = weapon;
        }

        public void OnCurrentAnimationTypeSet()
        {
            currentAnimationIndex = 0;
            UpdateTexture();
            timeSinceLastFrame = 0;
        }

        [BroadcastMethod(shouldInvokeSynchronously = true)]
        public void SetCurrentEntityState(int entityState, int direction)
        {
            currentEntityState = entityState;
            switch ((EntityState)currentEntityState)
            {
                case EntityState.Idle:
                    IdleAtDir((Direction)direction);
                    break;
                case EntityState.Moving:
                    MoveAtDir((Direction)direction);
                    break;
                case EntityState.Attacking:
                    AttackAtDir((Direction)direction);  
                    if(isInServer)
                    {
                        List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(this);
                        if (damagedEntities.Count > 0)
                        {
                           foreach(Entity damagedEntity in damagedEntities)
                            {
                                damagedEntity.OnAttackedBy(this);
                            }
                        }
                    }
                    break;
            }
            Location = new Vector2(SyncX, SyncY);
        }

        MapObjectLib block = null;
        Rectangle newLocationRect;
        System.Drawing.Rectangle rectt;
        public override void Update(GameTime gameTime)
        {
            if (GetCurrentEnitytState() == EntityState.Moving)
            {
                double movment = speed * gameTime.ElapsedGameTime.TotalMilliseconds;
                Vector2 newLocation = Location;
                switch ((Direction)currentDirection)
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

        [Command]
        public virtual void OnAttackedBy(Entity attacker)
        {
            syncHealth -= attacker.Weapon.SyncDamage;
            if(syncHealth == 0)
            {
                Console.WriteLine("Destroy");
                Destroy();
            }
        }

        public EntityState GetCurrentEnitytState()
        {
            return (EntityState)currentEntityState;
        }

        protected bool getIsLoopAnimationFinished()
        {
            return currentAnimationIndex + 1 >= animations[(Animation)currentAnimationType].Count;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer + 0.001f);
            sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0, 0, (int)healthBarSize.X, (int)healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer);
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
            OnCurrentAnimationTypeSet();
            if (Weapon != null)
            {
                switch ((Direction)currentDirection)
                {
                    case Direction.Left:
                        Weapon.SyncX = GetBoundingRectangle().Left ;
                        Weapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Up:
                        Weapon.SyncY = GetBoundingRectangle().Top;
                        Weapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Right:
                        Weapon.SyncX = GetBoundingRectangle().Right - Weapon.Size.X;
                        Weapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Down:
                        Weapon.SyncY = GetBoundingRectangle().Bottom - Weapon.Size.Y;
                        Weapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Idle:
                        break;
                }
            }
        }

        private void AnimationAtDir(Direction direction, int dirToAnimationIndex)
        {
            currentAnimationIndex = 0;
            currentDirection = (int)direction;
            currentAnimationType = (int)direction + dirToAnimationIndex;
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
