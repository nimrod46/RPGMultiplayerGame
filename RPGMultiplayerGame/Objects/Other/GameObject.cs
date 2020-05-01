using Microsoft.Xna.Framework;
using Networking;
using static NetworkingLib.Server;

namespace RPGMultiplayerGame.Objects.Other
{
    public abstract class GameObject : NetworkIdentity
    {
        public virtual Vector2 Location { get; set; }
        public virtual float SyncX
        {
            get => syncX; set
            {
                syncX = value;
                InvokeSyncVarNetworkly(nameof(SyncX), value, NetworkInterfaceType.UDP);
                OnXSet();
            }
        }

        public virtual float SyncY
        {
            get => syncY; set
            {
                syncY = value;
                InvokeSyncVarNetworkly(nameof(SyncY), value, NetworkInterfaceType.UDP);
                OnYSet();
            }
        }
        public virtual Point Size { get; set; }
        public Point BaseSize { get; set; }
        protected object movmentLock = new object();
        private float syncX;
        private float syncY;

        public GameObject()
        {
            syncX = -9999;
            syncY = -9999;
            OnNetworkInitializeEvent += OnNetworkInitialize;
            OnDestroyEvent += OnDestroyed;
        }

        public virtual void OnNetworkInitialize()
        {
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

        public float GetDistanceFrom(GameObject gameObject)
        {
            return Vector2.Distance(gameObject.GetBaseCenter(), GetBaseCenter());
        }

        protected bool IsObjectInInteractingRadius(GameObject gameObject, float minDistanceForObjectInteraction)
        {
            return GetDistanceFrom(gameObject) <= minDistanceForObjectInteraction;
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
        }
    }
}
