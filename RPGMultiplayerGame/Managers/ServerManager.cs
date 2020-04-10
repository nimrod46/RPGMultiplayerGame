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
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;
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

        public bool IsRuning { get; private set; }
        public readonly List<Player> players = new List<Player>();

        public readonly List<UpdateObject> serverObjects = new List<UpdateObject>();
        public readonly List<Monster> monsters = new List<Monster>();
        public readonly List<WeaponEffect> weaponEffects = new List<WeaponEffect>();
        public SpawnPoint spawnPoint;

        public void StartServer()
        {
            ServerBehavior serverBehavior = new ServerBehavior(1331);
            serverBehavior.Run();
            serverBehavior.OnClientEventHandlerSynchronizedEvent += OnClientSynchronized;
            serverBehavior.OnRemoteIdentityInitialize += OnIdentityInitialize;
            serverBehavior.OnLocalIdentityInitialize += OnIdentityInitialize;
            NetBehavior = serverBehavior;
            IsRuning = true;
        }

        private void OnIdentityInitialize(NetworkIdentity identity)
        {
            if (identity is Monster)
            {
                lock (monsters)
                {
                    monsters.Add(identity as Monster);
                    identity.OnDestroyEvent += Monster_OnDestroyEvent;
                }
            }
        }

        private void Monster_OnDestroyEvent(NetworkIdentity identity)
        {
            lock (monsters)
            {
                monsters.Remove(identity as Monster);
            }
        }

        protected void OnClientSynchronized(int id)
        {
            lock (players)
            {
                Player player = (Player)NetBehavior.spawnWithClientAuthority(typeof(Player), id);
                players.Add(player);
                player.OnDestroyEvent += Player_OnDestroyEvent;
                player.AddItemToInventory((int) ItemType.CommonSword);
                player.AddItemToInventory((int)ItemType.CommonWond);
                player.AddItemToInventory((int)ItemType.CommonHealthPotion, 10);
                if (spawnPoint != null)
                {
                    player.SetSpawnPoint(spawnPoint);
                }
            }
        }

        public void Weapon_OnSpawnWeaponEffect(WeaponEffect weaponEffect, Entity entity)
        {
            weaponEffect = (WeaponEffect)NetBehavior.spawnWithServerAuthority(weaponEffect.GetType(), weaponEffect);
            weaponEffect.SetLocation(entity.GetBoundingRectangle());
            lock (weaponEffects)
            {
                weaponEffects.Add(weaponEffect);
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
                    for (int i = 0; i < 10; i++)
                    {
                        Bat bat = new Bat();
                        bat.SyncX = obj.Rectangle.X;
                        bat.SyncY = obj.Rectangle.Y;
                        Bat spawnedBat = NetBehavior.spawnWithServerAuthority(bat.GetType(), bat) as Bat;
                        spawnedBat.EquipeWith((int) ItemType.BatClaw);
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
            lock (serverObjects)
            {
                foreach (UpdateObject obj in serverObjects)
                {
                    obj.Update(gameTime);
                }
            }
            lock (weaponEffects)
            {
                foreach (WeaponEffect weaponEffect in weaponEffects)
                {
                   List<Entity> entities = GameManager.Instance.GetEntitiesIntersectsWith(weaponEffect);
                   foreach(Entity entity in entities)
                    {
                        weaponEffect.Hit(entity);
                    }
                }
            }
        }

        public void AddServerGameObject(UpdateObject updateObject)
        {
            lock (serverObjects)
            {
                serverObjects.Add(updateObject);
            }
        }

        public void RemoveServerGameObject(UpdateObject updateObject)
        {
            lock (serverObjects)
            {
                serverObjects.Remove(updateObject);
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
