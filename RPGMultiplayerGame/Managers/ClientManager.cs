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
        public Player Player { get; private set; }
        public event EventHandler OnStartGame;
        public string name;
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
            if (client is Player player)
            {
                Player = player;
                player.OnDestroyEvent += Player_OnDestroyEvent;
                if (name == null)
                {
                    player.OnLocalPlayerNameSet += NetworkManager_OnPlayerNameSet;
                    player.InitName();
                }
                else
                {
                    player.Init(name);
                }
            }
        }

        private void Player_OnDestroyEvent(NetworkIdentity identity)
        {
            Player.IsInventoryVisible = false;
            Player = null;
        }

        private void NetworkManager_OnPlayerNameSet(Player player)
        {
            OnStartGame?.Invoke(this, null);
            name = player.GetName();
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
