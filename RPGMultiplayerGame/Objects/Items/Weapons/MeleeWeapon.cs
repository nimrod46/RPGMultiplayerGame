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

        public override void PreformeAttack()
        {
            UpdateWeaponLocation(Attacker);
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(this, Attacker);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    Hit(damagedEntity);
                }
            }
        }

        public override void UpdateWeaponLocation(Entity entity)
        {
            switch (entity.SyncCurrentDirection)
            {
                case Direction.Left:
                    SyncX = entity.GetBoundingRectangle().Left;
                    SyncY = entity.GetCenter().Y;
                    break;
                case Direction.Up:
                    SyncY = entity.GetBoundingRectangle().Top;
                    SyncX = entity.GetCenter().X;
                    break;
                case Direction.Right:
                    SyncX = entity.GetBoundingRectangle().Right - DamageAreaSize.X;
                    SyncY = entity.GetCenter().Y;
                    break;
                case Direction.Down:
                    SyncY = entity.GetBoundingRectangle().Bottom - DamageAreaSize.Y;
                    SyncX = entity.GetCenter().X;
                    break;
            }
        }

        public override Rectangle GetBoundingRectangle()
        {
            Rectangle rect =  base.GetBoundingRectangle();
            rect.Size = new Point(DamageAreaSize.X, DamageAreaSize.Y);
            return rect;
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Subtype: " + "Melee weapon";
        }
    }
}
