using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

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
        private float syncDamage;
        List<int> hittedEntitiesId = new List<int>();

        public WeaponEffect(EffectId effectId) : base(new Dictionary<int, List<GameTexture>>(Instance.animationsByEffects[effectId]), 0, 0)
        {
            this.effectId = effectId;
            speed *= 6;
        }

        public override void OnNetworkInitialize()
        {
            base.OnNetworkInitialize();
            SetCurrentEntityState((int)State.Moving, SyncCurrentDirection);
        }

        public void Hit(Entity entity)
        {
            if (!hittedEntitiesId.Contains(entity.id))
            {
                entity.OnAttackedBy(syncDamage);
                hittedEntitiesId.Add(entity.id);
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
