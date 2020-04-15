﻿using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;

namespace RPGMultiplayerGame.Objects.Items.Weapons
{
    public abstract class Weapon : InteractiveItem
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Point Size { get; }
        public float Damage { get; set; }
        protected string Name { get; set; }

        protected double coolDownTime;
        private double currentCoolDownTime;
        private bool inCoolDown;

        public Weapon(ItemType itemType, Point size, float damage, string name, double coolDownTime) : base(itemType)
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

    }
}