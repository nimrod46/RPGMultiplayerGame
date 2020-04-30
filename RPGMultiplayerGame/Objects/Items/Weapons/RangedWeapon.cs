using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class RangedWeapon : Weapon
    {

        private readonly WeaponAmmunition weaponEffect;

        public RangedWeapon(ItemType itemType, string name, Point size, float damage, WeaponAmmunition weaponEffect, double coolDownTime) : base(itemType, name, size, damage, coolDownTime)
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
                ServerManager.Instance.Weapon_OnSpawnWeaponEffect(weaponEffect);
            }
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Subtype: " + "Ranged weapon";
        }
    }
}
