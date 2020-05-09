using Microsoft.Xna.Framework;
using Networking;
using System;

namespace RPGMultiplayerGame.Managers
{
    public class ClientManager : NetworkManager<ClientBehavior>
    {
        public static ClientManager instance;
        public static ClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientManager();
                }
                return instance;
            }
        }
        protected ClientManager()
        {
        }

        public void Update()
        {
            if(NetBehavior == null)
            {
                return;
            }
            if(NetBehavior.IsConnected)
            {
                NetBehavior.RunActionsSynchronously();
            }
        }

        public void Start()
        {
            NetBehavior.OnLocalIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            NetBehavior.OnRemoteIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            NetBehavior.Synchronize();
        }



        public bool Connect()
        {
            NetBehavior = lobby.Connect();
            if (NetBehavior != null)
            {
                return true;
            }
            return false;
        }

        
    }
}
