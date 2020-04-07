﻿using System;
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
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Weapons;
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

        public readonly List<Player> players = new List<Player>();
        public readonly List<UpdateObject> serverObjects = new List<UpdateObject>();
        public SpawnPoint spawnPoint;


        public void StartServer()
        {
            isServer = true;
            ServerBehavior serverBehavior = new ServerBehavior(1331);
            serverBehavior.Run();
            serverBehavior.OnClientEventHandlerSynchronizedEvent += OnClientSynchronized;
            serverBehavior.OnRemoteIdentityInitialize += OnIdentityInitialize;
            serverBehavior.OnLocalIdentityInitialize += OnIdentityInitialize;
            NetBehavior = serverBehavior;
        }

        private void OnIdentityInitialize(NetworkIdentity client)
        {
            if(client is Entity)
            {
                ((Entity)client).OnEntityAttcked += ServerManager_OnEntityAttcked;
            }
        }

        private void ServerManager_OnEntityAttcked(Entity entity)
        {
            List<Entity> damagedEntities = GameManager.Instance.GetEntitiesHitBy(entity);
            if (damagedEntities.Count > 0)
            {
                foreach (Entity damagedEntity in damagedEntities)
                {
                    damagedEntity.OnAttackedBy(entity);
                }
            }
        }

        protected void OnClientSynchronized(int id)
        {
            lock (players)
            {
                Player player = (Player)NetBehavior.spawnWithClientAuthority(typeof(Player), id);
                players.Add(player);
                player.OnDestroyEvent += Player_OnDestroyEvent;
                Weapon weapon = (Weapon)NetBehavior.spawnWithClientAuthority(typeof(OldSword), id);
                player.EquipeWith(weapon);
                if (spawnPoint != null)
                {
                    player.SetSpawnPoint(spawnPoint);
                }
            }
        }

        private void Player_OnDestroyEvent(NetworkIdentity identity)
        {
            lock(players)
            {
                players.Remove((Player) identity);
               // OnClientSynchronized(identity.ownerId);
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
                    PathEntity npcMark = identity as PathEntity;
                    foreach (WaypointLib waypoint in objP.waypoints)
                    {
                        npcMark.AddWaypoint(new Waypoint(new Point(waypoint.Point.X, waypoint.Point.Y), (float) waypoint.Time));
                    }
                }
                else if(identity is SpawnPoint spawnPoint)
                {
                    UpdatePlayersSpawnLocation(spawnPoint);
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
