using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.Items.Weapons.SpecielWeaponEffects;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public abstract class Weapon : InteractiveItem
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Damage { get; set; }

        protected List<Type> specielWeaponEffects;
        protected double coolDownTime;
        private double currentCoolDownTime;
        private bool inCoolDown;

        public Weapon(ItemType itemType, string name, float damage, double coolDownTime) : base(itemType, name)
        {
            Damage = damage;
            SyncName = name;
            this.coolDownTime = coolDownTime;
            currentCoolDownTime = 0;
            inCoolDown = false;
            specielWeaponEffects = new List<Type>();
            AddSpecielWeaponEffect<FlickerEffect>();
        }

        public void AddSpecielWeaponEffect<T>() where T : ISpecielWeaponEffect
        {
            specielWeaponEffects.Add(typeof(T));
        }

        public void Update(GameTime gameTime)
        {
            if (inCoolDown)
            {
                currentCoolDownTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (currentCoolDownTime >= coolDownTime)
                {
                    currentCoolDownTime = 0;
                    inCoolDown = false;
                }
            }
        }

        

        public bool IsAbleToAttack()
        {
            if (!inCoolDown)
            {
                inCoolDown = true;
                return true;
            }
            return false;
        }

        public abstract void Attack(Entity entity);

        public virtual void Hit(Entity attacker, Entity victim)
        {
            victim.InvokeBroadcastMethodNetworkly(nameof(victim.OnAttackedBy), attacker, Damage);
            InvokeBroadcastMethodNetworkly(nameof(ActivateEffectsOn), victim);
        }

        protected void ActivateEffectsOn(Entity victim)
        {
            lock (specielWeaponEffects)
            {
                foreach (var specielWeaponEffectType in specielWeaponEffects)
                {
                    Activator.CreateInstance(specielWeaponEffectType, victim);
                }
            }
        }

        public abstract void UpdateWeaponLocation(Entity entity);

        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Type: " + "Weapon" + "\n" +
                "Damage: " + Damage + "\n" +
                "Cooldown time: " + coolDownTime;
        }
    }
}
