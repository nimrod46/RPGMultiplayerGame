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
using RPGMultiplayerGame.Objects.Weapons;
using static RPGMultiplayerGame.Managers.GameManager;
using static RPGMultiplayerGame.Objects.AnimatedObject;

namespace RPGMultiplayerGame.Objects.LivingEntities

{
    public abstract class Entity : AnimatedObject
    {
        public enum EntityState
        {
            Idle,
            Moving,
            Attacking
        }

        public delegate void EntityAttackedEventHandler(Entity entity);
        public event EntityAttackedEventHandler OnEntityAttcked;
        public Weapon Weapon { get => syncWeapon; set => syncWeapon = value; }
        [SyncVar]
        private Weapon syncWeapon;
        [SyncVar(hook = "OnHealthSet")]
        protected float syncHealth;
        [SyncVar(shouldInvokeNetworkly = false)]
        protected int syncCurrentEntityState;
        [SyncVar]
        protected SpawnPoint syncSpawnPoint;
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        protected EntityID entityID;
        protected float textLyer;
        protected int collisionOffsetX;
        protected int collisionOffsetY;
        protected int flickerCount;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        private readonly float maxHealth;
        protected Vector2 healthBarOffset;
        private Vector2 healthBarSize;
        private int currentFlickerCount;
        private bool isBeingHit;
        private readonly double flickerTimeDelay = 0.2;
        private double currentFlickerTime = 0.5;

        public Entity(EntityID entityID, int collisionOffsetX, int collisionOffsetY, float maxHealth)
        {
            this.entityID = entityID;
            this.collisionOffsetX = collisionOffsetX;
            this.collisionOffsetY = collisionOffsetY;
            this.maxHealth = maxHealth;
            isBeingHit = false;
            healthBar = GameManager.Instance.HealthBar;
            healthBarBackground = GameManager.Instance.HealthBarBackground;
            healthBarSize = new Vector2(healthBar.Width, healthBar.Height);
            syncHealth = maxHealth;
            syncCurrentAnimationType = (int)EntityAnimation.IdleDown;
            syncCurrentEntityState = (int)EntityState.Idle;
            Layer = GameManager.ENTITY_LAYER;
            isHidenCompletely = false;
            flickerCount = 5;
        }

        protected override void InitAnimationsList()
        {
            animationsByType = new Dictionary<int, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityID]);
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            GameManager.Instance.AddEntity(this);
            textLyer = CHARECTER_TEXT_LAYER + DefaultLayer;

        }

        [BroadcastMethod(shouldInvokeSynchronously = true)]
        public void SetCurrentEntityState(int entityState, int direction)
        {
            syncCurrentEntityState = entityState;
            switch ((EntityState)syncCurrentEntityState)
            {
                case EntityState.Idle:
                    IdleAtDir((Direction)direction);
                    break;
                case EntityState.Moving:
                    MoveAtDir((Direction)direction);
                    break;
                case EntityState.Attacking:
                    AttackAtDir((Direction)direction);
                    OnEntityAttcked?.Invoke(this);
                    break;
            }
            Location = new Vector2(SyncX, SyncY);
        }

        public void OnHealthSet()
        {   
            healthBarSize.X = syncHealth * healthBar.Width / maxHealth;
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();
            UpdateDrawOffset();
        }

        protected virtual void UpdateDrawOffset()
        {
            healthBarOffset = new Vector2(BaseSize.X / 2 - healthBarBackground.Width / 2, -healthBarBackground.Height - 2);
        }

        public void EquipeWith(Weapon weapon)
        {
            this.syncWeapon = weapon;
        }

        public override void Update(GameTime gameTime)
        {
            if (GetCurrentEnitytState() == EntityState.Moving)
            {
                MapObjectLib block = null;
                Rectangle newLocationRect;
                System.Drawing.Rectangle rectt;
                double movment = speed * gameTime.ElapsedGameTime.TotalMilliseconds;
                Vector2 newLocation = Location;
                switch ((Direction)syncCurrentDirection)
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
            }

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

        [BroadcastMethod]
        public virtual void OnAttackedBy(Entity attacker)
        {
            if (hasAuthority)
            {
                syncHealth -= attacker.syncWeapon.SyncDamage;
                if(syncHealth == 0)
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

        public EntityState GetCurrentEnitytState()
        {
            return (EntityState)syncCurrentEntityState;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (!isHidenCompletely)
            {
                base.Draw(sprite);
                sprite.Draw(healthBarBackground, Location + healthBarOffset, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer + 0.001f);
                sprite.Draw(healthBar, Location + healthBarOffset, new Rectangle(0, 0, (int)healthBarSize.X, (int)healthBarSize.Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, textLyer);
            }
        }

        protected void IdleAtDir(Direction direction)
        {
            AnimationAtDir(direction, 4, true);
        }

        protected void MoveAtDir(Direction direction)
        {
            AnimationAtDir(direction, 0, true);
        }

        protected void AttackAtDir(Direction direction)
        {
            AnimationAtDir(direction, 8, false);
            //OnCurrentAnimationTypeSet();
            UpdateWeaponLocation();
        }

        protected void UpdateWeaponLocation()
        {
            if (syncWeapon != null)
            {
                switch ((Direction)syncCurrentDirection)
                {
                    case Direction.Left:
                        syncWeapon.SyncX = GetBoundingRectangle().Left;
                        syncWeapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Up:
                        syncWeapon.SyncY = GetBoundingRectangle().Top;
                        syncWeapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Right:
                        syncWeapon.SyncX = GetBoundingRectangle().Right - syncWeapon.Size.X;
                        syncWeapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Down:
                        syncWeapon.SyncY = GetBoundingRectangle().Bottom - syncWeapon.Size.Y;
                        syncWeapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Idle:
                        break;
                }
            }
        }
     
        private Rectangle GetCollisionRect(float x, float y, int width, int height)
        {
            return new Rectangle((int)x + collisionOffsetX, (int)y + collisionOffsetY, width - collisionOffsetX, height - collisionOffsetY);
        }

        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            syncSpawnPoint = spawnPoint;
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
