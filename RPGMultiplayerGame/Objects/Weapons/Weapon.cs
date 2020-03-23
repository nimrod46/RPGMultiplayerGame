using Microsoft.Xna.Framework;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Weapons
{
    public abstract class Weapon : GameObject
    {
        [SyncVar(isDisabled = true)]
        public override float SyncX { get; set; }
        [SyncVar(isDisabled = true)]
        public override float SyncY { get; set; }
        [SyncVar]
        public float SyncDamage { get; set; }
        [SyncVar]
        protected string syncName;

        public Weapon(Point size, float damage, string name)
        {
            Size = size;
            BaseSize = size;
            SyncDamage = damage;
            syncName = name;
        }
    }
}
