using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Managers
{
    public class ServerManager : NetworkManager<ServerBehavior>
    {
        public static ServerManager instance;
        public static ServerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerManager();
                }
                return instance;
            }
        }

        readonly List<Player> players = new List<Player>();
        readonly List<UpdateObject> serverObjects = new List<UpdateObject>();


        public void StartServer()
        {
            isServer = true;
            ServerBehavior serverBehavior = new ServerBehavior(1331);
            serverBehavior.Run();
            serverBehavior.OnClientEventHandlerSynchronizedEvent += OnClientSynchronized;
            NetBehavior = serverBehavior;
        }

        protected void OnClientSynchronized(int id)
        {
            lock (players)
            {
                Player player = (Player) NetBehavior.spawnWithClientAuthority(typeof(Player), id);
                players.Add(player);

                if (GameManager.Instance.spawnPoint != null)
                {
                    player.SetSpawnPoint(GameManager.Instance.spawnPoint);
                }
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
                    gObject = new Blacksmith();

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
                        NetBehavior.spawnWithServerAuthority(point.GetType(), point);
                    }
                }

            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (UpdateObject obj in serverObjects)
            {
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

        public void UpdateSpawnLocation(SpawnPoint spawnPoint)
        {
            foreach (Player player in players)
            {
                player.SetSpawnPoint(spawnPoint);
            }
        }

        public bool IsNameLegal(string name)
        {
            lock (players)
            {
                return !players.Any(player => player.GetName().Equals(name));
            }
        }

    }
}
