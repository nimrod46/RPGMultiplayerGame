﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.LivingEntities

{
    public abstract class Entity : MovingObject
    {
        public new enum State
        {
            Idle,
            Moving,
            Attacking
        }

        public delegate void EntityKill(Entity killedEntity);
        public EntityKill OnEnitytKillEvent;
        protected SpawnPoint SyncSpawnPoint
        {
            get => syncSpawnPoint; set
            {
                syncSpawnPoint = value;
                InvokeSyncVarNetworkly(nameof(SyncSpawnPoint), value);
            }
        }

        public float SyncHealth
        {
            get => syncHealth; set
            {
                if (hasAuthority)
                {
                    if (value > maxHealth)
                    {
                        value = maxHealth;
                    }
                    InvokeSyncVarNetworkly(nameof(SyncHealth), value);
                }
                syncHealth = value;
                OnHealthSet();
            }
        }

        public Weapon EquippedWeapon { get; set; }

        public EntityId EntityId { get; }
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected float textLyer;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected Vector2 healthBarOffset;
        protected bool damageable;
        protected readonly float maxHealth;
        private float syncHealth;
        private SpawnPoint syncSpawnPoint;
        private Vector2 healthBarSize;
        private int currentFlickerCount;
        private bool isBeingHit;
        private readonly double flickerTimeDelay = 0.2;
        private double currentFlickerTime = 0.5;

        public Entity(EntityId entityId, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEntities[entityId]), collisionOffsetX, collisionOffsetY)
        {
            this.EntityId = entityId;
            this.maxHealth = maxHealth;
            this.damageable = damageable;
            isBeingHit = false;
            healthBar = GraphicManager.Instance.HealthBar;
            healthBarBackground = GraphicManager.Instance.HealthBarBackground;
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
            SyncHealth = maxHealth;
            SyncCurrentAnimationType = (int)EntityAnimation.IdleDown;
            syncCurrentEntityState = (int)State.Idle;
            Layer = GraphicManager.ENTITY_LAYER;
            isHidenCompletely = false;
            flickerCount = 5;
            healthBarOffset = Vector2.Zero;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            textLyer = CHARECTER_TEXT_LAYER + DefaultLayer;
        }


        public virtual void OnHealthSet()
        {
            healthBarSize.X = SyncHealth * healthBar.Width / maxHealth;
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();
            UpdateDrawOffset();
        }

        protected virtual void UpdateDrawOffset()
        {
            if (damageable)
            {
                healthBarOffset = new Vector2(BaseSize.X / 2 - healthBarBackground.Width / 2, -healthBarBackground.Height - 2);
            }
        }

        public virtual void EquipeWith(Weapon weapon)
        {
            EquippedWeapon = weapon;
        }

        public override void Update(GameTime gameTime)
        {
            if (isBeingHit)
            {
                currentFlickerTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (currentFlickerTime >= flickerTimeDelay)
                {
                    currentFlickerTime = 0;
                    isVisible = !isVisible;
                    currentFlickerCount++;
                    if (currentFlickerCount >= flickerCount)
                    {
                        isVisible = true;
                        isBeingHit = false;
                        currentFlickerCount = 0;
                    }
                }
            }
            if (hasAuthority)
            {
                EquippedWeapon?.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public virtual void OnAttackedBy(Entity attacker, float damage)
        {
            if (!damageable)
            {
                return;
            }

            if (hasAuthority)
            {
                SyncHealth -= damage;
                if (SyncHealth <= 0)
                {
                    Kill(attacker);
                }
            }
            MakeObjectFlicker();
        }

        public virtual void Kill(Entity attacker)
        {
            if (!isInServer)
            {
                InvokeCommandMethodNetworkly(nameof(Kill), attacker);
            }
            else
            {
                attacker.OnEnitytKillEvent?.Invoke(this);
                InvokeBroadcastMethodNetworkly(nameof(Destroy));
            }
        }

        private void MakeObjectFlicker()
        {
            isBeingHit = true;
            currentFlickerCount = 0;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (!isHidenCompletely)
            {
                base.Draw(sprite);
                if (damageable)
                {
                    sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, Layer + 0.001f);
                    sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0, 0, (int)healthBarSize.X, (int)healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, Layer);
                }
            }
        }

        public override void SetCurrentEntityState(int entityState, Direction direction)
        {
            base.SetCurrentEntityState(entityState, direction);
            switch ((State)syncCurrentEntityState)
            {
                case State.Attacking:
                    AttackAtDir(direction);
                    break;
            }
        }

        protected virtual void AttackAtDir(Direction direction)
        {
            AnimationAtDir(direction, 8, false);
        }

        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            SyncSpawnPoint = spawnPoint;

        }

        public void MoveToSpawnPoint()
        {
            SyncX = SyncSpawnPoint.SyncX;
            SyncY = SyncSpawnPoint.SyncY;
        }

        protected void Attack()
        {
            if (EquippedWeapon.IsAbleToAttack())
            {
                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (object)(int)State.Attacking, SyncCurrentDirection);
                InvokeCommandMethodNetworkly(nameof(CmdAttack), this, EquippedWeapon);
            }
        }

        protected void CmdAttack(Entity attacker, Weapon weapon)
        {
            weapon.Attack(attacker);
        }
    }
}
