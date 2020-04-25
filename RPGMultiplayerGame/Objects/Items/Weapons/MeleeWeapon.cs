using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(ItemType itemType, string name, Point size, float damage, double coolDownTime) : base(itemType, name, size, damage, coolDownTime)
        {
        }

        public override void Attack(Entity attacker)
        {
            UpdateWeaponLocation(attacker);
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(this, attacker);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    damagedEntity.InvokeBroadcastMethodNetworkly(nameof(damagedEntity.OnAttackedBy), attacker, Damage);
                }
            }
        }

        public override void UpdateWeaponLocation(Entity entity)
        {
            switch ((Direction)entity.SyncCurrentDirection)
            {
                case Direction.Left:
                    X = entity.GetBoundingRectangle().Left;
                    Y = entity.GetCenter().Y;
                    break;
                case Direction.Up:
                    Y = entity.GetBoundingRectangle().Top;
                    X = entity.GetCenter().X;
                    break;
                case Direction.Right:
                    X = entity.GetBoundingRectangle().Right - Size.X;
                    Y = entity.GetCenter().Y;
                    break;
                case Direction.Down:
                    Y = entity.GetBoundingRectangle().Bottom - Size.Y;
                    X = entity.GetCenter().X;
                    break;
                case Direction.Idle:
                    break;
            }
        }

        public override string ToString()
        {
            return base.ToString() + "\n" + 
                "Subtype: " + "Melee weapon";
        }
    }
}
