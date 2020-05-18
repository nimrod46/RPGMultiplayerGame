using Map;
using Microsoft.Xna.Framework;
using Networking;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RPGMultiplayerGame.Managers
{
    public class GameManager
    {

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        public const int INVENTORY_ROWS_NUMBER = 4, INVENTORY_COLUMNS_NUMBER = 5;

        public bool IsMouseInteractable
        {
            get
            {
                return isMouseVisibleCounter > 0;
            }

            set
            {
                isMouseVisibleCounter = (value ? isMouseVisibleCounter + 1 : isMouseVisibleCounter - 1);
                if (isMouseVisibleCounter < 0)
                {
                    isMouseVisibleCounter = 0;
                }

            }
        }

        public event EventHandler OnStartGame;
        public bool HideMouse { get; set; }
        public Player Player { get; set; }
        public Camera Camera { get; set; }
        public GameChat GameChat { get; set; }

        public GameMap Map { get; set; }
        private readonly List<IGameUpdateable> updateObjects = new List<IGameUpdateable>();
        private readonly List<Entity> entities = new List<Entity>();
        private Game1 game;
        private int isMouseVisibleCounter;
        private string name;

        private GameManager()
        {
            Map = new GameMap();

            IsMouseInteractable = false;
            isMouseVisibleCounter = 0;
            HideMouse = false;
        }

        public void Init(Game1 game)
        {
            this.game = game;
            Camera = new Camera(game.GraphicsDevice.Viewport);
        }

        public void OnIdentityInitialize(NetworkIdentity identity)
        {
            if (identity.hasAuthority && identity is Player player)
            {
                Player = player;
                player.OnDestroyEvent += Player_OnDestroyEvent;
                if (name == null)
                {
                    player.OnLocalPlayerNameSetEvent += OnLocalPlayerNameSet;
                    player.InitName();
                }
                else
                {
                    player.Init(name);
                }
                this.GameChat.LocalPlayer = player;
            }
            if (identity is IGameUpdateable gameUpdateable)
            {
                AddUpdateObject(gameUpdateable);
            }
            if (identity is Entity entity)
            {
                AddEntity(entity);
            }
            if(identity is GameChat gameChat)
            {
                gameChat.Initialize(game);
                this.GameChat = gameChat;
            }
        }

        private void OnLocalPlayerNameSet(Player player)
        {
            OnStartGame?.Invoke(this, null);
            name = player.GetName();
        }

        private void Player_OnDestroyEvent(NetworkIdentity identity)
        {
            Player.IsInventoryVisible = false;
            Player = null;
        }

        readonly List<IGameUpdateable> updateObjectsToRemove = new List<IGameUpdateable>();
        public void Update(GameTime gameTime)
        {
            GameChat?.Update();
            if (Player != null)
            {
                Camera.Update(game.GraphicsDevice.Viewport, Player.Location);
            }
            game.IsMouseVisible = IsMouseInteractable && !HideMouse;

            lock (updateObjects)
            {
                foreach (var obj in updateObjects)
                {
                    if (obj.IsEnabled)
                    {
                        obj?.Update(gameTime);
                    }
                    if (obj.IsDestroyed)
                    {
                        updateObjectsToRemove.Add(obj);
                    }
                }

                foreach (var obj in updateObjectsToRemove)
                {
                    updateObjects.Remove(obj);
                }
            }
           
            updateObjectsToRemove.Clear();

            lock (entities)
            {
                foreach (var entity in entities)
                {
                    if (entity.IsDestroyed)
                    {
                        updateObjectsToRemove.Add(entity);
                        continue;
                    }
                }
            }

            foreach (var entity in updateObjectsToRemove)
            {
                entities.Remove(entity as Entity);
            }
            updateObjectsToRemove.Clear();          
        }

        public Point GeMapSize()
        {
            return new Point(1920, 1080);
        }

        public List<Entity> GetEntitiesHitBy(MeleeWeapon weapon, Entity attacker)
        {
            List<Entity> damagedEntities = new List<Entity>();
            lock (updateObjects)
            {
                foreach (var entity in entities)
                {
                    if (entity.SyncIsDead || attacker == entity || attacker.GetType().IsAssignableFrom(entity.GetType()))
                    {
                        continue;
                    }

                    if (weapon.GetBoundingRectangle().Intersects(entity.GetBoundingRectangle()))
                    {
                        damagedEntities.Add(entity);
                    }
                }
            }
            return damagedEntities;
        }

        public List<Entity> GetEntitiesIntersectsWith(GameObject gameObject)
        {
            List<Entity> entitiesIntersects = new List<Entity>();
            lock (updateObjects)
            {
                foreach (var entity in entities)
                {
                    if (!entity.SyncIsDead && gameObject.GetBaseBoundingRectangle().Intersects(entity.GetBoundingRectangle()))
                    {
                        entitiesIntersects.Add(entity);
                    }
                }
            }
            return (entitiesIntersects);
        }

        public List<Entity> GetEntities()
        {
            return new List<Entity>(entities);
        }

        private void AddEntity(Entity entity)
        {
            lock (entities)
            {
                entities.Add(entity);
            }
        }

        public void AddUpdateObject(IGameUpdateable obj)
        {
            new Thread(new ThreadStart(() =>
            {
                lock (updateObjects)
                {
                    updateObjects.Add(obj);
                }
            })).Start();
        }

        public void RemoveUpdateObject(IGameUpdateable obj)
        {
            new Thread(new ThreadStart(() =>
            {
                lock (updateObjects)
                {
                    updateObjects.Remove(obj);
                }
            })).Start();
        }
    }
}
