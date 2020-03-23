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
        public virtual Vector2 Location { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnXSet")]
        public virtual float SyncX { get; set; }
        [SyncVar(networkInterface = NetworkInterface.UDP, hook = "OnYSet")]
        public virtual float SyncY { get; set; }
        public virtual Point Size { get; set; }
        public Point BaseSize { get; set; }


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

        public virtual Vector2 GetCenter()
        {
            return new Vector2(SyncX + Size.X / 2, SyncY + Size.Y / 2);
        }

        public virtual Vector2 GetBaseCenter()
        {
            return new Vector2(SyncX + BaseSize.X / 2.0f, SyncY + BaseSize.Y / 2.0f);
        }
       
        public virtual Rectangle GetBaseBoundingRectangle()
        {
            return new Rectangle((int)SyncX, (int)SyncY, BaseSize.X, BaseSize.Y);
        }

        public virtual void OnXSet()
        {
            if (MathHelper.Distance(Location.X, SyncX) >= 5f)
            {
                Location = new Vector2(SyncX, Location.Y);
            }
        }

        public virtual void OnYSet()
        {
            if (MathHelper.Distance(Location.Y, SyncY) >= 5f)
            {
                Location = new Vector2(Location.X, SyncY);
            }
        }

        public virtual void OnDestroyed(NetworkIdentity identity)
        {
            GameManager.Instance.RemoveGameObject(this);
        }
    }
}
