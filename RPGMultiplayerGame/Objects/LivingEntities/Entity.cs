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
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Other;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

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
                }
                syncHealth = value;

                InvokeSyncVarNetworkly(nameof(SyncHealth), value);
                OnHealthSet();
            }
        }

        public Weapon EquippedWeapon { get; set; }

        public EntityId entityId { get; }
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected float textLyer;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected Vector2 healthBarOffset;
        protected bool damageable;
        private readonly float maxHealth;
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
            InvokeBroadcastMethodNetworkly(nameof(OnAttackedBy), attacker, damage);
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

        public void Kill(Entity attacker)
        {
            Console.WriteLine("KILL");
            if (!isInServer)
            {
                InvokeCommandMethodNetworkly(nameof(Kill), attacker);
            }
            else
            {
                attacker.OnEnitytKillEvent?.Invoke(this);
                Destroy();
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
                    break;
            }
        }

        protected void AttackAtDir(Direction direction)
        {
            AnimationAtDir(direction, 8, false);
        }

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

        protected void Attack()
        {
            if (EquippedWeapon.IsAbleToAttack())
            {
                SetCurrentEntityState((int)State.Attacking, SyncCurrentDirection);
                CmdAttack(this, (int)EquippedWeapon.ItemType);
            }
        }

        protected void CmdAttack(Entity attacker, int itemType)
        {
            if (!isInServer)
            {
                InvokeCommandMethodNetworkly(nameof(CmdAttack), attacker, itemType);
            }
            else
            {
                Weapon weapon = ItemFactory.GetItem<Weapon>((ItemType)itemType);
                weapon.Attack(attacker);
            }
        }
    }
}
