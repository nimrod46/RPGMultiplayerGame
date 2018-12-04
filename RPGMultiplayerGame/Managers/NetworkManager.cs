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
using Microsoft.Xna.Framework;

namespace RPGMultiplayerGame.Managers
{
    public class NetworkManager
    {
        public static NetworkManager instance;
        List<Player> players = new List<Player>();
        List<UpdateObject> serverObjects = new List<UpdateObject>();
        public NetworkBehavior NetBehavior { get; private set; }
        public Lobby lobby;
        public ListView LobbyList;
        public event EventHandler OnStartGame;
        private Player client;
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

        private NetworkManager()
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
            new Block();
            new Player();
            new SpawnPoint();
            new Waypoint();
            new Joe();
        }

        public void Update(GameTime gameTime)
        {
            foreach (UpdateObject obj in serverObjects){
                obj.Update(gameTime);
            }
        }

        public void AddServerGameObject(UpdateObject gameObject)
        {
            lock (serverObjects)
            {
                serverObjects.Add(gameObject);
            }
        }

        public void LoadMap(GameMap gameMap)
        {
            GameManager.Instance.map = gameMap;
            foreach (MapObjectLib obj in gameMap.GraphicObjects)
            {
                GameObject gObject = null;
                if (obj is NpcLib)
                {
                    gObject = new Joe();

                }
                else if (obj is SpawnLib)
                {
                    gObject = new SpawnPoint();
                }
                else if (obj is BlockLib)
                {
                    gObject = new Block();
                    ((Block)gObject).SyncTextureIndex = (obj as BlockLib).ImageIndex;
                    ((Block)gObject).SyncLayer = obj.Layer;
                }
                gObject.SyncX = obj.Rectangle.X;
                gObject.SyncY = obj.Rectangle.Y;
                NetworkIdentity identity = NetBehavior.spawnWithServerAuthority(gObject.GetType(), gObject);
                if (obj is NpcLib objP)
                {
                    Waypoint point;
                    Npc npcMark = identity as Npc;
                    foreach (WaypointLib waypoint in objP.waypoints)
                    {
                        point = new Waypoint
                        {
                            SyncX = waypoint.Point.X,
                            SyncY = waypoint.Point.Y,
                            SyncTime = waypoint.Time,
                            SyncNpcId = npcMark.id,
                        };
                        NetworkManager.Instance.NetBehavior.spawnWithServerAuthority(point.GetType(), point);
                    }
                }

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

        public void UpdateSpawnLocation(SpawnPoint spawnPoint)
        {
            foreach (Player player in players)
            {
                player.SetSpawnPoint(spawnPoint);
            }
        }
    }
}
