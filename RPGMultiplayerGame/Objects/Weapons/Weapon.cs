using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Weapons
{
    public abstract class Weapon : GameObject
    {
        public override float SyncX
        {
            get => syncX; set
            {
                syncX = value;
                OnYSet();
            }
        }

        public override float SyncY
        {
            get => syncY; set
            {
                syncY = value;
                OnYSet();
            }
        }

        public float SyncDamage
        {
            get => syncDamage; set
            {
                syncDamage = value;
                InvokeSyncVarNetworkly(nameof(SyncDamage), value);
            }
        }

        protected string SyncName
        {
            get => syncName; set
            {
                syncName = value;
                InvokeSyncVarNetworkly(nameof(SyncName), value);
            }
        }

        private string syncName;
        private float syncX;
        private float syncY;
        private float syncDamage;

        public Weapon(Point size, float damage, string name)
        {
            Size = size;
            BaseSize = size;
            SyncDamage = damage;
            SyncName = name;
        }
    }
}
