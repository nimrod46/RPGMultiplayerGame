using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.GameSaver;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Items.Potions;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.Items.Weapons.WeaponAmmunitions;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.QuestsObjects;
using RPGMultiplayerGame.Objects.VisualEffects;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<NetworkIdentity> gameIdentities = new List<NetworkIdentity>();
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
            players.Add(player);
            Console.WriteLine("Player {0} joined", player.GetName());
            bool isSaveloaded = gameSave.LoadObjectSave(player);
            if (isSaveloaded)
            {

            }
            else
            {
                GivePlayerGameItem(player, new CommonSword());
                GivePlayerGameItem(player, new CommonWond());
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 10 });
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 15 });
                GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 4 });
                GivePlayerGameItem(player, new CommonBow());
                player.SyncGold = 100;
                player.MoveToSpawnPoint();
            }
        }

        public void SaveGame()
        {
            lock (players)
            {
                foreach (var player in players)
                {
                    gameSave.SaveObjectData(player);
                }
            }

            lock (gameIdentities)
            {
                foreach (var identity in gameIdentities)
                {
                    if (identity is Npc npc)
                    {
                        gameSave.SaveObjectData(npc);
                    }
                }
            }
            XmlManager<GameSave> xml = new XmlManager<GameSave>();
            xml.Save(@"Content\game save.xml", gameSave);
        }

        public void LoadSaveGame()
        {
            XmlManager<GameSave> xml = new XmlManager<GameSave>();
            gameSave = xml.Load(@"Content\game save.xml");
            lock (players)
            {
                foreach (var player in players)
                {
                    gameSave.LoadObjectSave(player);
                }
            }
            lock (gameIdentities)
            {
                foreach (var identity in gameIdentities)
                {
                    if (identity is Npc npc)
                    {
                        gameSave.LoadObjectSave(npc);
                    }
                }
            }
        }

        public void SpawnWeaponAmmunition(WeaponAmmunition weaponEffect)
        {
            NetBehavior.spawnWithServerAuthority(weaponEffect.GetType(), weaponEffect);
        }

        public VisualEffect SpawnVisualEffect(VisualEffect visualEffect)
        {
            return NetBehavior.spawnWithServerAuthority(visualEffect.GetType(), visualEffect) as VisualEffect;
        }

        public void GivePlayerGameItem(Player player, GameItem gameItem)
        {
            player.AddItemToInventory(NetBehavior.spawnWithClientAuthority(gameItem.GetType(), player.OwnerId, gameItem) as GameItem);
        }

        public Quest AddQuest(Player player, Quest quest)
        {
            Quest netQuest = (Quest)NetBehavior.spawnWithClientAuthority(quest.GetType(), player.OwnerId, quest);
            netQuest.AssignTo(player);
            return netQuest;
        }

        private void Player_OnDestroyEvent(NetworkIdentity identity)
        {
            Player player = identity as Player;
            lock (players)
            {
                lock (gameIdentities)
                {
                    gameSave.SaveObjectData(player);
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
                    Blacksmith blacksmith = new Blacksmith
                    {
                        SyncX = obj.Rectangle.X + 15,
                        SyncY = obj.Rectangle.Y + 15
                    };
                    NetBehavior.spawnWithServerAuthority(blacksmith.GetType(), blacksmith);

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
        public List<NetworkIdentity> CopyIdentities()
        {
            return new List<NetworkIdentity>(gameIdentities);
        }

        public void UpdatePlayersSpawnLocation(SpawnPoint spawnPoint)
        {
            this.spawnPoint = spawnPoint;
            foreach (Player player in players)
            {
                player.SetSpawnPoint(spawnPoint);
                player.MoveToSpawnPoint();
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

