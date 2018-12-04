using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;

namespace RPGMultiplayerGame.Objects
{
    public abstract class GameObject : NetworkIdentity
    {
        public Vector2 Location { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnXSet")]
        public float SyncX { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnYSet")]
        public float SyncY { get; set; }
        protected Point size;
        protected bool controling = false;

        public override void OnNetworkInitialize()
        {
            Location = new Vector2(SyncX, SyncY);
            GameManager.Instance.AddGameObject(this);
        }

        public void OnXSet()
        {
            if (!controling)
                Location = new Vector2(SyncX, Location.Y);
        }

        public void OnYSet()
        {
            if (!controling)
                Location = new Vector2(Location.X, SyncY);
        }

        public override void OnDestroyed()
        {
            GameManager.Instance.RemoveGameObject(this);
        }
    }
}
