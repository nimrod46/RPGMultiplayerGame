using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class RangedWeapon : Weapon
    {

        private readonly WeaponEffect weaponEffect;

        public RangedWeapon(ItemType itemType, Point size, float damage, string name, WeaponEffect weaponEffect) : base(itemType, size, damage, name)
        {
            this.weaponEffect = weaponEffect;
            weaponEffect.SyncDamage = damage;
        }

        public override void UpdateWeaponLocation(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Attack(Entity entity)
        {
            if (entity.isInServer)
            {
                weaponEffect.SyncCurrentDirection = entity.SyncCurrentDirection;
                ServerManager.Instance.Weapon_OnSpawnWeaponEffect(weaponEffect, entity);
            }
        }
    }
}
