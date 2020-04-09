using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Weapons
{
    public class RangedWeapon : Weapon
    {
        public delegate void SpwanWeaponEffectEventHandler(WeaponEffect weaponEffect, Entity entity);
        public event SpwanWeaponEffectEventHandler OnSpawnWeaponEffect;

        private readonly WeaponEffect weaponEffect;

        public RangedWeapon(Point size, float damage, string name, WeaponEffect weaponEffect) : base(size, damage, name)
        {
            this.weaponEffect = weaponEffect;
            weaponEffect.SyncDamage = damage;
        }

        internal override void Attack(Entity entity)
        {
            if(isInServer)
            {
                weaponEffect.SyncCurrentDirection = entity.SyncCurrentDirection;
                OnSpawnWeaponEffect?.Invoke(weaponEffect, entity);
            }
        }
    }
}
