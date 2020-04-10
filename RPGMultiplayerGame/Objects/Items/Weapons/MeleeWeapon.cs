using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(ItemType itemType, Point size, float damage, string name) : base(itemType, size, damage, name)
        {
        }

        public override void Attack(Entity entity)
        {
            UpdateWeaponLocation(entity);
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(entity);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    damagedEntity.OnAttackedBy(SyncDamage);
                }
            }
        }

        public override void UpdateWeaponLocation(Entity entity)
        {
            switch ((Direction)entity.SyncCurrentDirection)
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
                    SyncX = entity.GetBoundingRectangle().Right - Size.X;
                    SyncY = entity.GetCenter().Y;
                    break;
                case Direction.Down:
                    SyncY = entity.GetBoundingRectangle().Bottom - Size.Y;
                    SyncX = entity.GetCenter().X;
                    break;
                case Direction.Idle:
                    break;
            }
        }
    }
}
