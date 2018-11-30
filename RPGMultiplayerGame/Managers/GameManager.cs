using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Managers
{
    class GameManager
    {
        public enum EntityID
        {
            Player,
        }

        public enum Animation
        {
            WalkLeft,
            WalkUp,
            WalkRight,
            WalkDown,
            IdleLeft,
            IdleUp,
            IdleRight,
            IdleDown,
        }

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

        static GameManager instance;

        public Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>> animationsByEntities = new Dictionary<EntityID, Dictionary<Animation, List<Texture2D>>>();
        public List<Texture2D> textures = new List<Texture2D>();
        public Texture2D HealthBar;
        public Texture2D HealthBarBackground;
        public SpriteFont PlayerName;
        public GameMap map;
        public SpawnMark spawnPoint;
        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GraphicObject> grapichObjects = new List<GraphicObject>();
        private List<UpdateObject> updateObjects = new List<UpdateObject>();

        private GameManager()
        {
            map = new GameMap();
        }

        public void LoadTextures(GraphicsDevice graphicsDevice, ContentManager content)
        {
            for (int i = 0; i < (int)Enum.GetValues(typeof(EntityID)).Cast<EntityID>().Last() + 1; i++)
            {
                Dictionary<Animation, List<Texture2D>> animations = new Dictionary<Animation, List<Texture2D>>();
                for (int j = 0; j < (int)Enum.GetValues(typeof(Animation)).Cast<Animation>().Last() + 1; j++)
                {
                    List<Texture2D> animation = new List<Texture2D>();
                    for (int k = 1; k <= 32; k++)
                    {
                        try
                        {

                            animation.Add(content.Load<Texture2D>("Entities\\" + (EntityID)i + (Animation)j + k));
                            Console.WriteLine("Loaded: " + (EntityID)i + (Animation)j + k);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    animations.Add((Animation)j, animation);
                }
                animationsByEntities.Add((EntityID) i, animations);
            }

            HealthBar = content.Load<Texture2D>("HealthBar");
            HealthBarBackground = content.Load<Texture2D>("HealthBarBackground");
            PlayerName = content.Load<SpriteFont>("PlayerName");

            Texture2D spriteTextures = content.Load<Texture2D>("basictiles");
            int count = 0;
            for (int y = 0; y < spriteTextures.Height; y += 16)
            {
                for (int x = 0; x < spriteTextures.Width; x += 16)
                {
                    Texture2D texture = new Texture2D(graphicsDevice, 16, 16);
                    Rectangle cloneRect = new Rectangle(x, y, 16, 16);
                    int c = cloneRect.Width * cloneRect.Height;
                    Color[] data = new Color[c];
                    spriteTextures.GetData(0, cloneRect, data, 0, c);
                    texture.SetData(data);
                    textures.Add(texture);
                    count++;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < updateObjects.Count; i++)
            {
                updateObjects[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            for (int i = 0; i < grapichObjects.Count; i++)
            {
                grapichObjects[i].Draw(sprite);
            }
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

        public void AddGraphicObject(GraphicObject obj)
        {
            lock (grapichObjects)
            {
                grapichObjects.Add(obj);
            }
        }

        public void RemoveGraphicObject(GraphicObject obj)
        {
            lock (grapichObjects)
            {
                grapichObjects.Remove(obj);
            }
        }

        public void AddUpdateObject(UpdateObject obj)
        {
            lock (updateObjects)
            {
                updateObjects.Add(obj);
            }
        }

        public void RemoveUpdateObject(UpdateObject obj)
        {
            lock (updateObjects)
            {
                updateObjects.Remove(obj);
            }
        }
    }
}
