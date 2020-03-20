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
        public event EventHandler OnStartGame;

        protected ClientManager()
        {
        }

        public void Start()
        {
            NetBehavior.OnLocalIdentityInitialize += OnLocalIdentityInitialize;
            NetBehavior.Synchronize();
        }

        private void OnLocalIdentityInitialize(NetworkIdentity client)
        {
            if (client is Player)
            {
                Player player = (client as Player);
                player.OnPlayerNameSet += NetworkManager_OnPlayerNameSet;

                player.InitName();
            }

        }

        private void NetworkManager_OnPlayerNameSet(object sender, EventArgs e)
        {
            OnStartGame?.Invoke(this, null);
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
