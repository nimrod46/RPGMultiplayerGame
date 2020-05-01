using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System.Collections.Generic;
using static RPGMultiplayerGame.Managers.GraphicManager;

namespace RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions
{
    public abstract class WeaponAmmunition : MovingObject, IDamageInflicter
    {
        protected WeaponAmmunitionId effectId;
        public Entity SyncAttacker { get; set; }
        public Weapon SyncWeapon { get; set; }
        public float Damage { get => SyncWeapon.Damage; set => SyncWeapon.Damage = value; }
        Direction IDamageInflicter.Direction { get => SyncCurrentDirection; set => SyncCurrentDirection = value; }

        private readonly List<IdentityId> victimsEntitiesId = new List<IdentityId>();

        public WeaponAmmunition(WeaponAmmunitionId effectId) : base(new Dictionary<int, List<GameTexture>>(GraphicManager.Instance.AnimationsByEffects[effectId]), 0, 0)
        {
            this.effectId = effectId;
            SyncSpeed *= 6;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            SetCurrentEntityState((int)State.Moving, SyncCurrentDirection);
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
                Hit(SyncAttacker, entity);
            }
        }

        public void Hit(Entity attacker, Entity victim)
        {
            if (!victimsEntitiesId.Contains(victim.Id))
            {
                SyncWeapon.InvokeBroadcastMethodNetworkly(nameof(SyncWeapon.ActivateEffectsOn), victim, this);
                victim.InvokeBroadcastMethodNetworkly(nameof(victim.OnAttackedBy), attacker, Damage);
                victimsEntitiesId.Add(victim.Id);
            }
        }

        public void SetLocation()
        {
            Rectangle rectangle = SyncAttacker.GetBoundingRectangle();
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

        public void Attack(Entity attacker)
        {
            throw new System.NotImplementedException();
        }
    }
}
