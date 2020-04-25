using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public abstract class Weapon : InteractiveItem
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Point Size { get; }
        public float Damage { get; set; }

        protected double coolDownTime;
        private double currentCoolDownTime;
        private bool inCoolDown;

        public Weapon(ItemType itemType, string name, Point size, float damage, double coolDownTime) : base(itemType, name)
        {
            Size = size;
            Damage = damage;
            Name = name;
            this.coolDownTime = coolDownTime;
            currentCoolDownTime = 0;
            inCoolDown = false;
        }

        public void Update(GameTime gameTime)
        {
            if(inCoolDown)
            {
                currentCoolDownTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (currentCoolDownTime >= coolDownTime)
                {
                    currentCoolDownTime = 0;
                    inCoolDown = false;
                }
            }
        }

        public Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)X, (int)Y, Size.X, Size.Y);
        }

        public bool IsAbleToAttack()
        {
            if(!inCoolDown)
            {
                inCoolDown = true;
                return true;
            }
            return false;
        }

        public abstract void Attack(Entity entity);

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
