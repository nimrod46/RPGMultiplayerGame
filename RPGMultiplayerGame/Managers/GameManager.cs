using System.Collections.Generic;
using Map;
using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using RPGMultiplayerGame.Objects.Items.Weapons;
using RPGMultiplayerGame.Ui;

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
                if(isMouseVisibleCounter < 0)
                {
                    isMouseVisibleCounter = 0;
                }

            }
        }

        public bool HideMouse { get; set; }
        public Player Player { get; set; }
        public Camera Camera { get; set; }

        public GameMap map; //TODO: Create GameMap with the relevent project class.
        private readonly List<GameObject> gameObjects = new List<GameObject>();
        private readonly List<IGameUpdateable> updateObjects = new List<IGameUpdateable>();
        private readonly List<Entity> entities = new List<Entity>();
        private Game1 game;
        private int isMouseVisibleCounter;

        private GameManager()
        {
            map = new GameMap();
          
            IsMouseInteractable = false;
            isMouseVisibleCounter = 0;
            HideMouse = false;
        }

        public void Init(Game1 game)
        {
            this.game = game;
            Camera  = new Camera(game.GraphicsDevice.Viewport);
        }

        public void Update(GameTime gameTime)
        {
            if (Player != null)
            {
                Camera.Update(game.GraphicsDevice.Viewport, Player.Location);
            }
            game.IsMouseVisible = IsMouseInteractable && !HideMouse;
            for (int i = 0; i < updateObjects.Count; i++)
            {
                updateObjects[i].Update(gameTime);
            }
            int height = game.GraphicsDevice.Viewport.Height;

            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                Rectangle rectangle = new Rectangle(entity.Location.ToPoint(), entity.BaseSize);
                float normalizedHieght = (float)rectangle.Bottom / height;
                if (normalizedHieght > 1)
                {
                    normalizedHieght = 1;
                }
                if (normalizedHieght < 0)
                {
                    normalizedHieght = 0;
                }
                entity.Layer = 1 - normalizedHieght;
            }
        } 

        public Point GeMapSize()
        {
            return new Point(1920, 1080);
        }

        public List<Entity> GetEntitiesHitBy(Weapon weapon, Entity attacker)
        {
            List<Entity> damagedEntities = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (attacker == entity || attacker.GetType().IsAssignableFrom(entity.GetType()))
                {
                    continue;
                }

                if (weapon.GetBoundingRectangle().Intersects(entity.GetBoundingRectangle()))
                {
                    damagedEntities.Add(entity);
                }
            }
            return damagedEntities;
        }

        public List<Entity> GetEntitiesIntersectsWith(GameObject gameObject)
        {
            List<Entity> entitiesIntersects = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (gameObject.GetBaseBoundingRectangle().Intersects(entity.GetBoundingRectangle()))
                {
                    entitiesIntersects.Add(entity);
                }
            }
            return (entitiesIntersects);
        }

       
        public void AddGameObject(GameObject obj)
        {
            lock (gameObjects)
            {
                gameObjects.Add(obj);
            }
        }

        public void RemoveGameObject(GameObject obj)
        {
            lock (gameObjects)
            {
                gameObjects.Remove(obj);
            }
        }

        public void AddEntity(Entity entity)
        {
            lock (entities)
            {
                entities.Add(entity);
            }
        }

        public void RemoveEntity(Entity entity)
        {
            lock (entities)
            {
                entities.Remove(entity);
            }
        }

        public void AddUpdateObject(IGameUpdateable obj)
        {
            lock (updateObjects)
            {
                updateObjects.Add(obj);
            }
        }

        public void RemoveUpdateObject(IGameUpdateable obj)
        {
            lock (updateObjects)
            {
                updateObjects.Remove(obj);
            }
        }
    }
}
