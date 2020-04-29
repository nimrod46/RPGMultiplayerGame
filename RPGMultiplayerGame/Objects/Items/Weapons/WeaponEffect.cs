using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public abstract class WeaponEffect : MovingObject
    {
        public float SyncDamage
        {
            get => syncDamage; set
            {
                syncDamage = value;
                InvokeSyncVarNetworkly(nameof(SyncDamage), value);
            }
        }

        protected EffectId effectId;
        public Entity SyncAttacker { get; set; }
        private float syncDamage;
        private readonly List<IdentityId> hittedEntitiesId = new List<IdentityId>();

        public WeaponEffect(EffectId effectId) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEffects[effectId]), 0, 0)
        {
            this.effectId = effectId;
            speed *= 6;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            InvokeBroadcastMethodNetworkly(nameof(SetCurrentEntityState), (int)State.Moving, SyncCurrentDirection);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!hasAuthority)
            {
                return;
            }

            List<Entity> entities = GameManager.Instance.GetEntitiesIntersectsWith(this);
            foreach (Entity entity in entities)
            {
                Hit(entity);
            }
        }

        public void Hit(Entity entity)
        {
            if (!hittedEntitiesId.Contains(entity.Id))
            {
                entity.InvokeBroadcastMethodNetworkly(nameof(entity.OnAttackedBy), SyncAttacker, syncDamage);
                hittedEntitiesId.Add(entity.Id);
            }
        }

        public void SetLocation(Rectangle rectangle)
        {
            switch ((Direction)SyncCurrentDirection)
            {
                case Direction.Left:
                    SyncX = rectangle.Left - Size.X;
                    SyncY = rectangle.Center.Y - Size.Y / 2;
                    break;
                case Direction.Up:
                    SyncY = rectangle.Top - Size.Y;
                    SyncX = rectangle.Center.X - Size.X / 2;
                    break;
                case Direction.Right:
                    SyncX = rectangle.Right;
                    SyncY = rectangle.Center.Y - Size.Y / 2;
                    break;
                case Direction.Down:
                    SyncY = rectangle.Bottom;
                    SyncX = rectangle.Center.X - Size.X / 2;
                    break;
                case Direction.Idle:
                    break;
            }
        }
    }
}
