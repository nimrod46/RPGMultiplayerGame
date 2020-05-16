using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public class RangedWeapon : Weapon
    {

        private readonly WeaponAmmunition weaponAmmunition;

        public RangedWeapon(ItemType itemType, string name, float damage, WeaponAmmunition weaponEffect, double coolDownTime) : base(itemType, name, damage, coolDownTime)
        {
            this.weaponAmmunition = weaponEffect;
            weaponEffect.SyncWeapon = this;
        }

        public override void UpdateWeaponLocation(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void PreformeAttack()
        {
            weaponAmmunition.SyncCurrentDirection = Attacker.SyncCurrentDirection;
            weaponAmmunition.SyncAttacker = Attacker;
            ServerManager.Instance.Spawn(weaponAmmunition);
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Subtype: " + "Ranged weapon";
        }
    }
}
