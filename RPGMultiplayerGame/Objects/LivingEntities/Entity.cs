using Microsoft.Xna.Framework;
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
            Attacking,
            Death
        }

        public enum EntityAnimation
        {
            WalkLeft,
            WalkUp,
            WalkRight,
            WalkDown,
            IdleLeft,
            IdleUp,
            IdleRight,
            IdleDown,
            AttackLeft,
            AttackUp,
            AttackRight,
            AttackDown,
            DeathLeft,
            DeathUp,
            DeathRight,
            DeathDown
        }

        public delegate void EntityKill(Entity killedEntity);
        public EntityKill OnEnitytKillEvent;
        public SpawnPoint SyncSpawnPoint
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

        public Weapon SyncEquippedWeapon { get; set; }

        public EntityId EntityId { get; }

        public bool SyncIsDead
        {
            get => isDead; set
            {
                isDead = value;
                if (isDead)
                {
                    SetCurrentEntityState((int) State.Death,SyncCurrentDirection);
                }
                InvokeSyncVarNetworkly(nameof(SyncIsDead), isDead);
            }
        }

        public bool IsDamageable { get;}

        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected float textLyer;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected Vector2 healthBarOffset;
        protected readonly float maxHealth;
        protected bool isDead;
        private readonly List<ISpecielWeaponEffect> specielWeaponEffect;
        private float health;
        private SpawnPoint syncSpawnPoint;
        private Vector2 healthBarSize;

        public Entity(EntityId entityId, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEntities[entityId]), collisionOffsetX, collisionOffsetY)
        {
            this.EntityId = entityId;
            this.maxHealth = maxHealth;
            this.IsDamageable = damageable;
            healthBar = GraphicManager.Instance.HealthBar;
            healthBarBackground = GraphicManager.Instance.HealthBarBackground;
            specielWeaponEffect = new List<ISpecielWeaponEffect>();
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
            Layer = GraphicManager.ENTITY_LAYER;
            isHidenCompletely = false;
            flickerCount = 5;
            healthBarOffset = Vector2.Zero;
            SyncHealth = maxHealth;
            SyncIsDead = false;
            SyncCurrentAnimationType = (int)EntityAnimation.IdleDown;
            SyncCurrentEntityState = (int)State.Idle;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            textLyer = CHARECTER_TEXT_LAYER + DefaultLayer;
        }

        public void ScheduledNewAction(ISpecielWeaponEffect specielWeaponEffect)
        {
            if(SyncIsDead)
            {
                return;
            }

            lock (this.specielWeaponEffect)
            {
                this.specielWeaponEffect.ForEach(a => { if (a.GetType() == specielWeaponEffect.GetType()) a.IsDestroyed = true; } );
                specielWeaponEffect.Activate();
                this.specielWeaponEffect.Add(specielWeaponEffect);
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
            if (IsDamageable)
            {
                healthBarOffset = new Vector2(BaseSize.X / 2 - healthBarBackground.Width / 2, -healthBarBackground.Height - 2);
            }
        }

        public virtual void EquipeWith(Weapon weapon)
        {
            InvokeCommandMethodNetworkly(nameof(EquipeWith), weapon);
            SyncEquippedWeapon = weapon;
            SyncEquippedWeapon.Attacker = this;
        }

        public override void Update(GameTime gameTime)
        {
            lock (specielWeaponEffect)
            {
                specielWeaponEffect.RemoveAll(a => a.IsDestroyed);
            }

            if (hasAuthority)
            {
                SyncEquippedWeapon?.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public List<Entity> GetCurrentEntitiesInRadius(float radius)
        {
            List<Entity> currentInteractingEntities = new List<Entity>();
            List<Entity> entities = GameManager.Instance.GetEntities();
            foreach (var entity in entities)
            {
                if (!entity.SyncIsDead && IsObjectInInteractingRadius(entity, radius))
                {
                    currentInteractingEntities.Add(entity);
                }
            }
            return currentInteractingEntities;
        }

        public virtual void OnAttackedBy(Entity attacker, float damage)
        {
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
            if (isInServer)
            {
                SyncIsDead = true;
            }
            specielWeaponEffect.ForEach(a =>  a.IsDestroyed = true);
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (!isHidenCompletely)
            {
                base.Draw(sprite);
                if (IsDamageable)
                {
                    if (!isDead)
                    {
                        sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, Layer + 0.001f);
                        sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0, 0, (int)healthBarSize.X, (int)healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, Layer);
                    }
                }
            }
        }

        public override void SetCurrentEntityState(int entityState, Direction direction)
        {
            base.SetCurrentEntityState(entityState, direction);
            switch ((State)SyncCurrentEntityState)
            {
                case State.Attacking:
                    AttackAtDir(direction);
                    break;
                case State.Death:
                    DeathAtDir(direction);
                    break;
            }
        }

        protected virtual void AttackAtDir(Direction direction)
        {
            AnimationAtDir(direction, 8, false);
        }

        protected virtual void DeathAtDir(Direction direction)
        {
            AnimationAtDir(direction, 12, false);
        }

        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            SyncSpawnPoint = spawnPoint;

        }

        public void MoveToSpawnPoint()
        {
            if (SyncSpawnPoint != null)
            {
                SyncX = SyncSpawnPoint.SyncX;
                SyncY = SyncSpawnPoint.SyncY;
            }
        }

        protected void Attack()
        {
            if (SyncEquippedWeapon.IsAbleToAttack())
            {
                InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), NetworkingLib.Server.NetworkInterfaceType.UDP, false, (object)(int)State.Attacking, SyncCurrentDirection);
                CmdAttack(SyncEquippedWeapon);
            }
        }

        protected void CmdAttack(Weapon weapon)
        {
            InvokeCommandMethodNetworkly(nameof(CmdAttack), weapon);
            weapon.Attack();
        }

        public override void Respawn(float x, float y)
        {
            base.Respawn(x, y);
            SyncX = x;
            SyncY = y;
            SyncHealth = maxHealth;
            SyncIsDead = false;
            SyncIsVisible = true;
            InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (int)State.Idle, Direction.Down);
        }
    }
}
