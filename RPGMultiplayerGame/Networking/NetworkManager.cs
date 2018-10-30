using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;
using ServerLobby;
using System.Windows.Forms;
using System.Threading;
using Map;

namespace RPGMultiplayerGame.Networking
{
    public class NetworkManager
    {
        public NetworkBehavior NetBehavior { get; private set; }
        public Lobby lobby;
        public ListView LobbyList;
        public static NetworkManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }
        static NetworkManager instance;
        Client client;
        NetworkManager()
        {
        }

        public void Init(ref ListView lobbyList)
        {
            LobbyList = lobbyList;
            client = new Client();
            lobby = new Lobby(client, ref LobbyList, 1331, "Map editor");
            RegisterNetworkElements();
        }

        private void RegisterNetworkElements()
        {
            new NetBlock();
        }

        public void LoadMap(GameMap gameMap)
        {
            foreach (Block block in gameMap.blocks)
            {
                string[] objs = new string[4];
                objs[0] = block.ImageIndex.ToString();
                objs[1] = block.Location.X.ToString();
                objs[2] = block.Location.Y.ToString();
                objs[3] = block.Layer.ToString();
                NetBehavior.spawnWithServerAuthority(typeof(NetBlock), objs);
            }
        }

        public void Start()
        {
            NetBehavior.player.Synchronize();
        }

        public void StartServer()
        {
            NetBehavior = new NetworkBehavior(new Client(), 1331);
            NetBehavior.StartServer();
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

        public void AddServer()
        {
            lobby.AddServer();
        }

        public void Refersh() 
        {
            lobby.RefreshAll();
        }

        public void Remove(ListViewItem item)
        {
            lobby.Remove(item);
        }
    }
}
