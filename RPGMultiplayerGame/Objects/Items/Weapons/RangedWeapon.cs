using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class RangedWeapon : Weapon
    {

        private readonly WeaponEffect weaponEffect;

        public RangedWeapon(ItemType itemType, string name, Point size, float damage, WeaponEffect weaponEffect, int coolDownTime) : base(itemType, name, size, damage, coolDownTime)
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
                weaponEffect.SyncAttacker = entity;
                ServerManager.Instance.Weapon_OnSpawnWeaponEffect(weaponEffect, entity);
            }
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Subtype: " + "Ranged weapon";
        }
    }
}
