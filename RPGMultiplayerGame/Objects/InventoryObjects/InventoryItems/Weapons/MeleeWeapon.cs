using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.InventoryObjects.InventoryItems.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public MeleeWeapon(ItemType itemType, Point size, float damage, string name) : base(itemType, size, damage, name)
        {
        }

        internal override void Attack(Entity entity)
        {
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(entity);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    damagedEntity.OnAttackedBy(SyncDamage);
                }
            }
        }
    }
}
