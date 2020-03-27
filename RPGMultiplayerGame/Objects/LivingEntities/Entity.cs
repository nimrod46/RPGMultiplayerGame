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

        public delegate void EntityAttackedEventHandler(Entity entity);
        public event EntityAttackedEventHandler OnEntityAttcked;
        [SyncVar]
        public Weapon SyncWeapon { get; set; }

        private Vector2 healthBarOffset;
        private Vector2 healthBarSize;
        private bool isBeingHit;
        private readonly float maxHealth;
        protected readonly Texture2D healthBar;
        protected readonly Texture2D healthBarBackground;
        [SyncVar(hook = "OnHealthSet")]
        protected float syncHealth; 
        protected float textLyer;
        protected Dictionary<Animation, List<GameTexture>> animations = new Dictionary<Animation, List<GameTexture>>();
        [SyncVar(shouldInvokeNetworkly = false, hook = "UpdateTexture")] //Live sync through BroadcastMethod, when first time connecting through SyncVar
        protected int syncCurrentAnimationType;
        [SyncVar(shouldInvokeNetworkly = false)]
        protected int syncCurrentDirection;
        [SyncVar(shouldInvokeNetworkly = false)]
        protected int syncCurrentEntityState;
        protected EntityID entityID;
        protected float speed;
        protected int animationDelay;
        protected int timeSinceLastFrame;
        protected int currentAnimationIndex;
        protected int collisionOffsetX;
        protected int collisionOffsetY;
        protected bool shouldLoopAnimation;
        protected bool isHidenCompletely;
        protected bool startedFlickeringAnim;
        protected int flickerCount;
        private int currentFlickerCount;
        [SyncVar]
        protected SpawnPoint syncSpawnPoint;

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
            animations = new Dictionary<Animation, List<GameTexture>>(GameManager.Instance.animationsByEntities[entityID]);
            syncCurrentAnimationType = (int)Animation.IdleDown;
            syncCurrentDirection = (int)Direction.Down;
            syncCurrentEntityState = (int)EntityState.Idle;
            speed = 0.5f / 10;
            animationDelay = 100;
            Layer = GameManager.ENTITY_LAYER;
            shouldLoopAnimation = true;
            isHidenCompletely = false;
            flickerCount = 5;
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
            GameTexture gameTexture = animations[(Animation)syncCurrentAnimationType][currentAnimationIndex];
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

        public void EquipeWith(Weapon weapon)
        {
            this.SyncWeapon = weapon;
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

        MapObjectLib block = null;
        Rectangle newLocationRect;
        System.Drawing.Rectangle rectt;

        public override void Update(GameTime gameTime)
        {
            if (GetCurrentEnitytState() == EntityState.Moving)
            {
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
                if (isBeingHit)
                {
                    isVisible = !isVisible;
                    currentFlickerCount++;
                    if(currentFlickerCount >= flickerCount)
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
                syncHealth -= attacker.SyncWeapon.SyncDamage;
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

        protected bool getIsLoopAnimationFinished()
        {
            return currentAnimationIndex + 1 >= animations[(Animation)syncCurrentAnimationType].Count;
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
            UpdateWeaponLocation();
        }

        protected void UpdateWeaponLocation()
        {
            if (SyncWeapon != null)
            {
                switch ((Direction)syncCurrentDirection)
                {
                    case Direction.Left:
                        SyncWeapon.SyncX = GetBoundingRectangle().Left;
                        SyncWeapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Up:
                        SyncWeapon.SyncY = GetBoundingRectangle().Top;
                        SyncWeapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Right:
                        SyncWeapon.SyncX = GetBoundingRectangle().Right - SyncWeapon.Size.X;
                        SyncWeapon.SyncY = GetCenter().Y;
                        break;
                    case Direction.Down:
                        SyncWeapon.SyncY = GetBoundingRectangle().Bottom - SyncWeapon.Size.Y;
                        SyncWeapon.SyncX = GetCenter().X;
                        break;
                    case Direction.Idle:
                        break;
                }
            }
        }

        private void AnimationAtDir(Direction direction, int dirToAnimationIndex)
        {
            currentAnimationIndex = 0;
            syncCurrentDirection = (int)direction;
            syncCurrentAnimationType = (int)direction + dirToAnimationIndex;
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
