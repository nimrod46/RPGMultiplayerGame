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
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.Weapons;
using static RPGMultiplayerGame.Managers.GameManager;

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

        public delegate void EntityAttackedEventHandler(Entity entity);
        public event EntityAttackedEventHandler OnEntityAttcked;

        public Weapon SyncWeapon
        {
            get => syncWeapon; set
            {
                syncWeapon = value;
                InvokeSyncVarNetworkly(nameof(SyncWeapon), value);
            }
        }
        protected SpawnPoint SyncSpawnPoint
        {
            get => syncSpawnPoint; set
            {
                syncSpawnPoint = value;
                InvokeSyncVarNetworkly(nameof(SyncSpawnPoint), value);
            }
        }

        protected float SyncHealth
        {
            get => syncHealth; set
            {
                syncHealth = value;
                InvokeSyncVarNetworkly(nameof(SyncHealth), value);
                OnHealthSet();
            }
        }

       
        protected EntityId entityId;
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected float textLyer;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected Vector2 healthBarOffset;
        protected bool damageable;
        private readonly float maxHealth;
        private Weapon syncWeapon;
        private float syncHealth;
        private SpawnPoint syncSpawnPoint;
        private Vector2 healthBarSize;
        private int currentFlickerCount;
        private bool isBeingHit;
        private readonly double flickerTimeDelay = 0.2;
        private double currentFlickerTime = 0.5;

        public Entity(EntityId entityId, int collisionOffsetX, int collisionOffsetY, float maxHealth, bool damageable) : base(new Dictionary<int, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityId]), collisionOffsetX, collisionOffsetY)
        {
            this.entityId = entityId;
            this.maxHealth = maxHealth;
            this.damageable = damageable;
            isBeingHit = false;
            healthBar = GameManager.Instance.HealthBar;
            healthBarBackground = GameManager.Instance.HealthBarBackground;
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
            SyncHealth = maxHealth;
            SyncCurrentAnimationType = (int)EntityAnimation.IdleDown;
            syncCurrentEntityState = (int)State.Idle;
            Layer = GameManager.ENTITY_LAYER;
            isHidenCompletely = false;
            flickerCount = 5;
            healthBarOffset = Vector2.Zero;
        }

        protected override void InitAnimationsList()
        {
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddEntity(this);
            textLyer = CHARECTER_TEXT_LAYER + DefaultLayer;

        }

        public void OnHealthSet()
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

        public void EquipeWith(Weapon weapon)
        {
            this.SyncWeapon = weapon;
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
            
            base.Update(gameTime);
        }

        public virtual void OnAttackedBy(Entity attacker)
        {
            InvokeBroadcastMethodNetworkly(nameof(OnAttackedBy), attacker);
            if (hasAuthority)
            {
                SyncHealth -= attacker.SyncWeapon.SyncDamage;
                if(SyncHealth == 0)
                {
                    Destroy();

                }
            }
            MakeObjectFlicker();
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
                    sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer + 0.001f);
                    sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0, 0, (int)healthBarSize.X, (int)healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer);
                }
            }
        }


        public override void SetCurrentEntityState(int entityState, int direction)
        {
            base.SetCurrentEntityState(entityState, direction);
            switch ((State)syncCurrentEntityState)
            {
                case State.Attacking:
                    AttackAtDir((Direction)direction);
                    OnEntityAttcked?.Invoke(this);
                    break;
            }
        }

        protected void AttackAtDir(Direction direction)
        {
            AnimationAtDir(direction, 8, false);
            //OnCurrentAnimationTypeSet();
            UpdateWeaponLocation();
        }

        protected abstract void UpdateWeaponLocation();


        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            SyncSpawnPoint = spawnPoint;
            SyncX = spawnPoint.SyncX;
            SyncY = spawnPoint.SyncY;
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            base.OnDestroyed(identity);
            GameManager.Instance.RemoveEntity(this);
        }
    }
}
