﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
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
            get => health; set
            {
                if (hasAuthority)
                {
                    if (value > maxHealth)
                    {
                        value = maxHealth;
                    }
                    else if (value < 0)
                    {
                        value = 0;
                    }
                    InvokeSyncVarNetworkly(nameof(SyncHealth), value);
                }
                health = value;
                OnHealthSet();
            }
        }

        public Weapon EquippedWeapon { get; set; }

        public EntityId EntityId { get; }

        public bool SyncIsDead
        {
            get => isDead; set
            {
                isDead = value;
                InvokeSyncVarNetworkly(nameof(SyncIsDead), value);
            }
        }

        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected float textLyer;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected Vector2 healthBarOffset;
        protected bool damageable;
        protected readonly float maxHealth;
        private List<ISpecielWeaponEffect> scheduledActions;
        private float health;
        private SpawnPoint syncSpawnPoint;
        private Vector2 healthBarSize;
        private bool isDead;

        public Entity(EntityId entityId, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEntities[entityId]), collisionOffsetX, collisionOffsetY)
        {
            this.EntityId = entityId;
            this.maxHealth = maxHealth;
            this.damageable = damageable;
            SyncIsDead = false;
            healthBar = GraphicManager.Instance.HealthBar;
            healthBarBackground = GraphicManager.Instance.HealthBarBackground;
            scheduledActions = new List<ISpecielWeaponEffect>();
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

        public void ScheduledNewAction(ISpecielWeaponEffect specielWeaponEffect)
        {
            lock (scheduledActions)
            {
                scheduledActions.ForEach(a => { if (!a.AllowMultiple && a.GetType() == specielWeaponEffect.GetType()) a.IsDestroyed = true; } );
                scheduledActions.Add(specielWeaponEffect);
            }
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
            //if (isBeingHit)
            //{
            //    currentFlickerTime += gameTime.ElapsedGameTime.TotalSeconds;
            //    if (currentFlickerTime >= flickerTimeDelay)
            //    {
            //        currentFlickerTime = 0;
            //        isVisible = !isVisible;
            //        currentFlickerCount++;
            //        if (currentFlickerCount >= flickerCount)
            //        {
            //            isVisible = true;
            //            isBeingHit = false;
            //            currentFlickerCount = 0;
            //        }
            //    }
            //}
            lock(scheduledActions)
            {
                foreach (var scheduledAction in scheduledActions)
                {
                    scheduledAction.Update(gameTime);
                }
                scheduledActions.RemoveAll(a => a.IsDestroyed);
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
                    InvokeBroadcastMethodNetworkly(nameof(Kill), attacker);
                }
            }
        }

        public virtual void Kill(Entity attacker)
        {
            SyncIsDead = true;
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

        public virtual void Respawn()
        {
            SyncX = syncSpawnPoint.SyncX;
            SyncY = syncSpawnPoint.SyncY;
            SyncHealth = maxHealth;
            SyncIsDead = false;
        }
    }
}
