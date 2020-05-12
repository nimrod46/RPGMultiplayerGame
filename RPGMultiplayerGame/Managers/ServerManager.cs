using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.GameSaver;
using RPGMultiplayerGame.Graphics;
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
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            NetBehavior.SpawnWithServerAuthority<GameChat>();
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
        public void Update()
        {
            if (NetBehavior == null)
            {
                return;
            }
            if (NetBehavior.IsRunning)
            {
                NetBehavior.RunActionsSynchronously();
            }
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
                Player player = NetBehavior.SpawnWithClientAuthority<Player>(endPointId);
                player.OnDestroyEvent += Player_OnDestroyEvent;
                player.OnSyncPlayerSaveEvent += Player_OnSyncPlayerSaveEvent;
                player.OnRemotePlayerTryToPickUpItemEvent += Player_OnPlayerPickUpItemEvent;
                if (spawnPoint != null)
                {
                    player.SetSpawnPoint(spawnPoint);
                }
            }
        }

        private void Player_OnPlayerPickUpItemEvent(Player player)
        {
            GameItem gameItem = GetGameItems().FirstOrDefault(g => player.IsIntersectingWith(g));
            if (gameItem != null && gameItem.isServerAuthority)
            {
                GivePlayerExistingItem(player, gameItem, "You picked up: ");
            }
        }

        public List<GameItem> GetGameItems()
        {
            return gameIdentities.Where(i => i is GameItem).Cast<GameItem>().ToList();
        }
        public T Spawn<T>(T identity = null) where T : NetworkIdentity
        {
            return NetBehavior.SpawnWithServerAuthority<T>(identity);
        }

        private void Player_OnSyncPlayerSaveEvent(Player player)
        {
            lock (players)
            {
                players.Add(player);
                bool isSaveloaded = gameSave.LoadObjectSave(player);
                if (isSaveloaded)
                {
                    Console.WriteLine("Known player joined {0}", player.GetName());
                    SendGeneralMassageToPlayer(player, "Welcome back {0}", ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.DarkBlue, player.GetName()));
                    BroadcastGeneralMassage("{0} joined the server", ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.DarkBlue, player.GetName()));
                }
                else
                {
                    Console.WriteLine("New player {0} joined", player.GetName());
                    SendGeneralMassageToPlayer(player, "Welcome {0}", ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.DarkBlue, player.GetName()));
                    BroadcastGeneralMassage("{0} joined the server", ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.DarkBlue, player.GetName()));
                    GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 10 }, "");
                    GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 15 }, "");
                    GivePlayerGameItem(player, new CommonHealthPotion() { SyncCount = 4 }, "");
                    //GivePlayerGameItem(player, new CommonSword(), "");
                    //GivePlayerGameItem(player, new CommonWond(), "");
                    //GivePlayerGameItem(player, new CommonBow(), "");
                    //GivePlayerGameItem(player, new IceBow(), "");
                    //GivePlayerGameItem(player, new ExplodingBow(), "");
                    //GivePlayerGameItem(player, new StormBow(), "");
                    player.SyncGold = 100;
                    player.MoveToSpawnPoint();
                }
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
            
            lock (players)
            {
                foreach (var player in players)
                {
                    gameSave.LoadObjectSave(player);
                }
            }
        }

        public void GivePlayerGameItem<T>(Player player, T gameItem, string itemRecivedTextPefix) where T : GameItem
        {
            GivePlayerExistingItem(player, NetBehavior.SpawnWithServerAuthority((dynamic)gameItem), itemRecivedTextPefix);
        }

        private void GivePlayerExistingItem(Player player, GameItem gameItem, string itemRecivedTextPefix)
        {
            player.AddItemToInventory(gameItem);
            if (!string.IsNullOrWhiteSpace(itemRecivedTextPefix))
            {
                SendGeneralMassageToPlayer(player, itemRecivedTextPefix + ColoredTextRenderer.ColorToColorCode(System.Drawing.KnownColor.Gold, gameItem.SyncName.ToString()));
            }
        }

        public T AddQuest<T>(Player player, T quest) where T : Quest
        {
            T netQuest = NetBehavior.SpawnWithClientAuthority(player.OwnerId, quest);
            netQuest.AssignTo(player);
            return netQuest;
        }

        public void ReAssignPlayer(Player player, Quest quest)
        {
            GetNpcByName(quest.NpcName).AssignQuestTo(player, quest);
        }

        private Npc GetNpcByName(string name)
        {
            lock(gameIdentities)
            {
                foreach (var identity in gameIdentities)
                {
                    if(identity is Npc npc && npc.GetName() == name)
                    {
                        return npc;
                    }
                }
                return null;
            }
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
            lock (players)
            {
                foreach (MapObjectLib obj in gameMap.GraphicObjects)
                {
                    if (obj is NpcLib objP)
                    {
                        Blacksmith blacksmith = new Blacksmith
                        {
                            SyncX = obj.Rectangle.X + 15,
                            SyncY = obj.Rectangle.Y + 15
                        };
                        NetBehavior.SpawnWithServerAuthority(blacksmith);

                        Joe gObject = new Joe
                        {
                            SyncX = obj.Rectangle.X,
                            SyncY = obj.Rectangle.Y
                        };
                        for (int i = 0; i < 10; i++)
                        {
                            Bat bat = new Bat
                            {
                                SyncX = obj.Rectangle.X,
                                SyncY = obj.Rectangle.Y
                            };
                            Bat spawnedBat = NetBehavior.SpawnWithServerAuthority(bat);
                            BatClaw batClaw = NetBehavior.SpawnWithServerAuthority<BatClaw>();
                            spawnedBat.EquipeWith(batClaw);
                        }
                        NetworkIdentity identity = NetBehavior.SpawnWithServerAuthority(gObject);
                        PathEntity npcMark = identity as PathEntity;
                        foreach (WaypointLib waypoint in objP.waypoints)
                        {
                            npcMark.AddWaypoint(new Waypoint(new Point(waypoint.Point.X, waypoint.Point.Y), (float)waypoint.Time));
                        }
                    }
                    else if (obj is SpawnLib)
                    {
                        SpawnPoint gObject = new SpawnPoint
                        {
                            SyncX = obj.Rectangle.X,
                            SyncY = obj.Rectangle.Y
                        };
                        gObject = NetBehavior.SpawnWithServerAuthority(gObject);
                        UpdatePlayersSpawnLocation(gObject);
                    }
                    else if (obj is BlockLib)
                    {
                        Block gObject = new Block
                        {
                            SyncX = obj.Rectangle.X,
                            SyncY = obj.Rectangle.Y,
                            SyncTextureIndex = (obj as BlockLib).ImageIndex,
                            SyncLayer = obj.Layer
                        };
                        NetBehavior.SpawnWithServerAuthority(gObject);
                    }
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

        public void SendGeneralMassageToPlayer(Player player, string massage, params object[] args)
        { 
            GameManager.Instance.GameChat.InvokeCommandMethodNetworkly(nameof(GameManager.Instance.GameChat.LocallyAddGeneralMassage), player.OwnerId, FormatText(massage, args));
        }

        public void BroadcastGeneralMassage(string massage, params object[] args)
        {
           
            GameManager.Instance.GameChat.BoardcastlyAddGeneralMassage(FormatText(massage, args));
        }

        private string FormatText(string massage, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                massage = massage.Replace("{" + i + "}", args[i].ToString());
            }
            return massage;
        }
    }
}

