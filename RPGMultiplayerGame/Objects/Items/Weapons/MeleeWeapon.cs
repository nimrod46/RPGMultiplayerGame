using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System.Collections.Generic;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public Point DamageAreaSize { get; }

        public MeleeWeapon(ItemType itemType, string name, Point damageAreaSize, float damage, double coolDownTime) : base(itemType, name, damage, coolDownTime)
        {
            DamageAreaSize = damageAreaSize;
        }

        public override void Attack(Entity attacker)
        {
            UpdateWeaponLocation(attacker);
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(this, attacker);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    Hit(attacker, damagedEntity);
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
                    X = entity.GetBoundingRectangle().Right - DamageAreaSize.X;
                    Y = entity.GetCenter().Y;
                    break;
                case Direction.Down:
                    Y = entity.GetBoundingRectangle().Bottom - DamageAreaSize.Y;
                    X = entity.GetCenter().X;
                    break;
                case Direction.Idle:
                    break;
            }
        }

        public Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)X, (int)Y, DamageAreaSize.X, DamageAreaSize.Y);
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Subtype: " + "Melee weapon";
        }
    }
}
