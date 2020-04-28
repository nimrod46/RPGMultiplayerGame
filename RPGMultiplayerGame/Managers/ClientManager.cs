using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using Networking;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;

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
