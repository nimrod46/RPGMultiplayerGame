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
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.MapObjects;

namespace RPGMultiplayerGame.Managers
{
    public class NetworkManager
    {
        List<Player> players = new List<Player>();
        public NetworkBehavior NetBehavior { get; private set; }
        public Lobby lobby;
        public ListView LobbyList;
        public event EventHandler OnStartGame;
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
        Player client;
        NetworkManager()
        {
        }

        public void Init(ref ListView lobbyList)
        {
            LobbyList = lobbyList;
            client = new Player();
            lobby = new Lobby(client, ref LobbyList, 1331, "RPG Game");
            RegisterNetworkElements();
        }

        private void RegisterNetworkElements()
        {
            new NetBlock();
            new Player();
            new SpawnMark();
            new NpcMark();
        }

        public void LoadMap(GameMap gameMap)
        {
            GameManager.Instance.map = gameMap;
            foreach (MapObjectLib obj in gameMap.GraphicObjects)
            {
                GameObject gObject = null;
                if (obj is NpcLib)
                {
                    gObject = new NpcMark();
                }
                else if (obj is SpawnLib)
                {
                    gObject = new SpawnMark();
                }
                else if (obj is BlockLib)
                {
                    gObject = new NetBlock();
                    ((NetBlock)gObject).SyncTextureIndex = (obj as BlockLib).ImageIndex;
                    ((NetBlock)gObject).SyncLayer = obj.Layer;
                }
                gObject.SyncX = obj.Rectangle.X;
                gObject.SyncY = obj.Rectangle.Y;
                NetBehavior.spawnWithServerAuthority(gObject.GetType(), gObject);
            }
        }

        public void Start()
        {
            NetBehavior.player.Synchronize();
            ((Player)NetBehavior.player).OnPlayerNameSet += NetworkManager_OnPlayerNameSet;
        }

        private void NetworkManager_OnPlayerNameSet(object sender, EventArgs e)
        {
            OnStartGame?.Invoke(this, null);
        }

        public void StartServer()
        {
            NetBehavior = new NetworkBehavior(new Player(), 1331);
            NetBehavior.StartServer();
            NetBehavior.OnPlayerSynchronized += NetBehavior_OnPlayerSynchronized;
        }

        private void NetBehavior_OnPlayerSynchronized(NetworkIdentity client)
        {
            lock (players)
            {
                players.Add(((Player)client));
            }
            if (GameManager.Instance.spawnPoint != null)
            {
                ((Player)client).SetSpawnPoint(GameManager.Instance.spawnPoint);
            }
        }

        public bool IsNameLegal(string name)
        {
            lock (players)
            {
                return !players.Any(player => player.GetName().Equals(name));
            }
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

        public void UpdateSpawnLocation(SpawnMark spawnPoint)
        {
            foreach (Player player in players)
            {
                player.SetSpawnPoint(spawnPoint);
            }
        }
    }
}
