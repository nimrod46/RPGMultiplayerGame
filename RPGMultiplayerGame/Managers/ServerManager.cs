using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.QuestsObjects;
using static NetworkingLib.Server;
using static RPGMultiplayerGame.Objects.LivingEntities.PathEntity;

namespace RPGMultiplayerGame.Managers
{
    public class ServerManager : NetworkManager<ServerBehavior>
    {
        private static ServerManager instance;
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

        public bool IsRunning { get; private set; }
        public readonly List<Player> players = new List<Player>();
        private List<NetworkIdentity> gameIdentities = new List<NetworkIdentity>();
        private readonly Dictionary<string, List<Quest>> questByPlayersName = new Dictionary<string, List<Quest>>();
        public GameSave gameSave;
        public SpawnPoint spawnPoint;

        public void StartServer()
        {
            ServerBehavior serverBehavior = new ServerBehavior(1331);
            serverBehavior.Run();
            serverBehavior.OnClientEventHandlerSynchronizedEvent += OnClientSynchronized;
            serverBehavior.OnRemoteIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            serverBehavior.OnLocalIdentityInitialize += GameManager.Instance.OnIdentityInitialize;
            serverBehavior.OnRemoteIdentityInitialize += OnIdentityInitialize;
            serverBehavior.OnLocalIdentityInitialize += OnIdentityInitialize;
            NetBehavior = serverBehavior;
            IsRunning = true;
            gameSave = new GameSave();
        }

        private void OnIdentityInitialize(NetworkIdentity identity)
        {
            lock (gameIdentities)
            {
                gameIdentities.Add(identity);
            }
            identity.OnDestroyEvent += Identity_OnDestroyEvent;
        }

        private void Identity_OnDestroyEvent(NetworkIdentity identity)
        {
            lock (gameIdentities)
            {
                gameIdentities.Remove(identity);
            }
        }

        protected void OnClientSynchronized(EndPointId endPointId)
        {
            lock (players)
            {
                Player player = (Player)NetBehavior.spawnWithClientAuthority(typeof(Player), endPointId);
                players.Add(player);
                player.OnDestroyEvent += Player_OnDestroyEvent;
                player.OnSyncPlayerSaveEvent += Player_OnSyncPlayerSaveEvent;
                if (spawnPoint != null)
                {
                    player.SetSpawnPoint(spawnPoint);
                }
            }
        }

        private void Player_OnSyncPlayerSaveEvent(Player player)
        {
            Console.WriteLine("New player {0} joined", player.GetName());
            bool isSaveloaded = gameSave.LoadPlayerSave(player);
            if (isSaveloaded)
            {
                lock (questByPlayersName)
                {
                    if (questByPlayersName.ContainsKey(player.GetName()))
                    {
                        foreach (var quest in questByPlayersName[player.GetName()])
                        {
                            quest.AssignTo(player);
                        }
                    }
                }
            }
            else
            {
                GivePlayerGameItem(player, new CommonSword());
                GivePlayerGameItem(player, new CommonWond());
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 10 });
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 15 });
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 4 });
                player.SyncGold = 100;
            }
        }

        public void Weapon_OnSpawnWeaponEffect(WeaponEffect weaponEffect, Entity entity)
        {
            weaponEffect = (WeaponEffect)NetBehavior.spawnWithServerAuthority(weaponEffect.GetType(), weaponEffect);
            weaponEffect.SetLocation(entity.GetBoundingRectangle());
        }

        public void GivePlayerGameItem(Player player, GameItem gameItem)
        {
            player.AddItemToInventory(NetBehavior.spawnWithClientAuthority(gameItem.GetType(), player.OwnerId, gameItem) as GameItem);
        }

        public Quest AddQuest(Quest quest, Player player)
        {
            Quest netQuest = (Quest)NetBehavior.spawnWithServerAuthority(quest.GetType(), quest);
            if (!questByPlayersName.ContainsKey(player.GetName()))
            {
                questByPlayersName.Add(player.GetName(), new List<Quest>(new Quest[] { netQuest }));
            }
            else
            {
                questByPlayersName[player.GetName()].Add(netQuest);
            }
            netQuest.AssignTo(player);
            return netQuest;
        }

        public void RemoveQuest(Player player, Quest quest)
        {
            questByPlayersName[player.GetName()].Remove(quest);
        }

        private void Player_OnDestroyEvent(NetworkIdentity identity)
        {
            Player player = identity as Player;
            lock (players)
            {
                lock (gameIdentities)
                {
                    gameSave.SavePlayerData(player, gameIdentities.Where(o => o is GameItem gameItem && gameItem.OwnerId == player.OwnerId).Cast<GameItem>().ToArray());
                }
                players.Remove((Player)identity);
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
                    Blacksmith blacksmith = new Blacksmith();
                    blacksmith.SyncX = obj.Rectangle.X;
                    blacksmith.SyncY = obj.Rectangle.Y;
                    NetBehavior.spawnWithServerAuthority(blacksmith.GetType(), blacksmith);

                    Joe j = new Joe();
                    j.SyncX = obj.Rectangle.X;
                    j.SyncY = obj.Rectangle.Y;
                    NetBehavior.spawnWithServerAuthority(j.GetType(), j);
                    gObject = new Joe();

                    for (int i = 0; i < 10; i++)
                    {
                        Bat bat = new Bat
                        {
                            SyncX = obj.Rectangle.X,
                            SyncY = obj.Rectangle.Y
                        };
                        Bat spawnedBat = NetBehavior.spawnWithServerAuthority(bat.GetType(), bat) as Bat;
                        BatClaw batClaw = NetBehavior.spawnWithServerAuthority(typeof(BatClaw)) as BatClaw;
                        spawnedBat.EquipeWith(batClaw);
                    }
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
                    PathEntity npcMark = identity as PathEntity;
                    foreach (WaypointLib waypoint in objP.waypoints)
                    {
                        npcMark.AddWaypoint(new Waypoint(new Point(waypoint.Point.X, waypoint.Point.Y), (float)waypoint.Time));
                    }
                }
                else if (identity is SpawnPoint spawnPoint)
                {
                    UpdatePlayersSpawnLocation(spawnPoint);
                }

            }
        }

        public void UpdatePlayersSpawnLocation(SpawnPoint spawnPoint)
        {
            this.spawnPoint = spawnPoint;
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

