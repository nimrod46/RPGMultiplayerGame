using Microsoft.Xna.Framework;
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
        public float SyncX { get; set; }

        public float SyncY { get; set; }
        public Point Size { get; }
        public float SyncDamage { get; set; }

        protected string SyncName { get; set; }

        public Weapon(ItemType itemType, Point size, float damage, string name) : base(itemType)
        {
            Size = size;
            SyncDamage = damage;
            SyncName = name;
        }

        public Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)SyncX, (int)SyncY, Size.X, Size.Y);
        }

        public abstract void Attack(Entity entity);

        public abstract void UpdateWeaponLocation(Entity entity);

    }
}
