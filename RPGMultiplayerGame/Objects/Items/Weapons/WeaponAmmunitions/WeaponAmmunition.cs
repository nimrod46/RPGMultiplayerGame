using Map;
using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.MathExtention;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public abstract class WeaponAmmunition : MovingObject, IDamageInflicter
    {
        public Entity SyncAttacker { get; set; }
        public Weapon SyncWeapon { get; set; }
        public float Damage { get => SyncWeapon.Damage; set => SyncWeapon.Damage = value; }
        Direction IDamageInflicter.Direction { get => SyncCurrentDirection; set => SyncCurrentDirection = value; }
        public Entity Attacker { get => SyncAttacker; set => SyncAttacker = value; }

        private readonly List<IdentityId> victimsEntitiesId = new List<IdentityId>();
        private int maxHitCount;
        protected WeaponAmmunitionId effectId;

        public WeaponAmmunition(WeaponAmmunitionId effectId, int maxHitCount) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEffects[effectId]), 0, 0)
        {
            this.effectId = effectId;
            this.maxHitCount = maxHitCount;
            SyncSpeed *= 6;
            animationTimeDelay *= 12;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            SetCurrentEntityState((int)State.Moving, SyncCurrentDirection);
            switch (SyncCurrentDirection)
            {
                case Direction.Left:
                    CollisionSizeType = Ui.UiComponent.PositionType.Centered;
                    CollisionSize = new Vector2(Size.X, 1f);
                    break;
                case Direction.Up:
                    CollisionSizeType = Ui.UiComponent.PositionType.Centered;
                    CollisionSize = new Vector2(1f, Size.Y);
                    break;
                case Direction.Right:
                    CollisionSizeType = Ui.UiComponent.PositionType.Centered;
                    CollisionSize = new Vector2(Size.X, 1f);
                    break;
                case Direction.Down:
                    CollisionSizeType = Ui.UiComponent.PositionType.Centered;
                    CollisionSize = new Vector2(1f, Size.Y);
                    break;
                case Direction.Idle:
                    break;
                default:
                    break;
            }
            SetLocation();
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
                if (entity != Attacker)
                {
                    Hit(entity);
                }
            }
        }

        protected override void OnCollidingWithBlock(Block block)
        {
            base.OnCollidingWithBlock(block);
            if(hasAuthority)
            {
               BroadcastDestroy();
            }
        }

        public void Hit(Entity victim)
        {
            if (victim.IsDamageable && maxHitCount != 0)
            {
                if (!victimsEntitiesId.Contains(victim.Id))
                {
                    SyncWeapon.InvokeBroadcastMethodNetworkly(nameof(SyncWeapon.ActivateEffectsOn), victim, this);
                    victim.InvokeBroadcastMethodNetworkly(nameof(victim.OnAttackedBy), Attacker, Damage);
                    victimsEntitiesId.Add(victim.Id);
                    maxHitCount--;
                    if (maxHitCount == 0)
                    {
                        InvokeBroadcastMethodNetworkly(nameof(Destroy));
                    }
                }
            }
        }

        public void SetLocation()
        {
            Vector2 location = Operations.GetPositionByTopLeftPosition(Ui.UiComponent.PositionType.Centered, new Vector2(SyncAttacker.SyncX, SyncAttacker.SyncY), SyncAttacker.Size.ToVector2());
            location = Operations.GetTopLeftPositionByPorsitionType(Ui.UiComponent.PositionType.Centered, location, Size.ToVector2());
            SyncX = location.X;
            SyncY = location.Y;
        }
    }
}
