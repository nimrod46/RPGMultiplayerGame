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
        [SyncVar(networkInterface = NetworkInterface.UDP)]
        public float SyncX { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP)]
        public float SyncY { get; set; }
        protected Point size;
        protected bool controling = false;
        protected object movmentLock = new object();

        public GameObject()
        {
            OnNetworkInitializeEvent += OnNetworkInitialize;
            OnDestroyEvent += OnDestroyed;
        }
        public virtual void OnNetworkInitialize()
        {
            GameManager.Instance.AddGameObject(this);
        }

        public virtual void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveGameObject(this);
        }
    }
}
