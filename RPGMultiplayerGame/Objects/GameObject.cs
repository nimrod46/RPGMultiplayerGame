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
        protected object movmentLock = new object();

        public GameObject()
        {
            OnNetworkInitializeEvent += OnNetworkInitialize;
            OnDestroyEvent += OnDestroyed;
        }

        public virtual void OnNetworkInitialize()
        {
            GameManager.Instance.AddGameObject(this);
            Location = new Vector2(SyncX, SyncY);
        }

        public virtual void OnXSet()
        {
                lock (movmentLock) {
                if (MathHelper.Distance(Location.X, SyncX) >= 5f)
                {
                    Location = new Vector2(SyncX, Location.Y);
                }
            }
        }

        public virtual void OnYSet()
        {
            lock (movmentLock)
            {
                if (MathHelper.Distance(Location.Y, SyncY) >= 5f)
                {
                    Location = new Vector2(Location.X, SyncY);
                }
            }
        }

        public virtual void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveGameObject(this);
        }
    }
}
