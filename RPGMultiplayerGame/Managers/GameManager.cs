using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OffsetGeneratorLib;
using RPGMultiplayerGame.MapObjects;
using RPGMultiplayerGame.Objects;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Managers
{
    public class GameManager
    {
        public enum EntityID
        {
            Player,
            Blacksmith
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
            AttackLeft,
            AttackUp,
            AttackRight,
            AttackDown,
        }

        public struct GameTexture
        {
            public Texture2D Texture { get; private set; }
            public Vector2 Offset { get; private set; }

            public GameTexture(Texture2D image, Vector2 offset)
            {
                Texture = image;
                Offset = offset;
            }
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

        public const float OWN_PLAYER_LAYER = 0.001f;
        public const float ENTITY_LAYER = 0.01f;
        public const float CHARECTER_TEXT_LAYER = 0.9f;

        static GameManager instance;


        public Dictionary<EntityID, Dictionary<Animation, List<GameTexture>>> animationsByEntities = new Dictionary<EntityID, Dictionary<Animation, List<GameTexture>>>();
        public List<Texture2D> textures = new List<Texture2D>();
        public Texture2D HealthBar;
        public Texture2D HealthBarBackground;
        public SpriteFont PlayerName;
        public GameMap map;
        public SpawnPoint spawnPoint;
        private readonly List<GameObject> gameObjects = new List<GameObject>();
        private readonly List<GraphicObject> grapichObjects = new List<GraphicObject>();
        private readonly List<UpdateObject> updateObjects = new List<UpdateObject>();
        private readonly List<Entity> entities = new List<Entity>();

        private GameManager()
        {
            map = new GameMap();
        }

        public void LoadTextures(GraphicsDevice graphicsDevice, ContentManager content)
        {
            for (int i = 0; i < (int)Enum.GetValues(typeof(EntityID)).Cast<EntityID>().Last() + 1; i++)
            {
                XmlManager<List<AnimationPropertiesLib>> xml = new XmlManager<List<AnimationPropertiesLib>>();
                List<AnimationPropertiesLib> animationProperties = null;
                try
                {
                    animationProperties = xml.Load("Content\\" + (EntityID)i + ".xml");
                }
                catch (Exception)
                {
                    Console.WriteLine("Warning: no xml found for " + (EntityID)i);
                }
                Dictionary<Animation, List<GameTexture>> animations = new Dictionary<Animation, List<GameTexture>>();
                for (int j = 0; j < (int)Enum.GetValues(typeof(Animation)).Cast<Animation>().Last() + 1; j++)
                {
                    List<GameTexture> animation = new List<GameTexture>();
                    for (int k = 1; k <= 32; k++)
                    {
                        string name ="" + (EntityID)i + "\\" + (Animation)j + "\\" + k ;
                        Vector2 offset = Vector2.Zero;
                        if (animationProperties?.Where(a => name.Contains(a.FullPath)).Count() > 0)
                        {
                            offset = new Vector2(animationProperties.Where(a => name.Contains(a.FullPath)).ToArray()[0].Offset.X, animationProperties.Where(a => name.Contains(a.FullPath)).ToArray()[0].Offset.Y);
                        }
                        try
                        {

                            animation.Add(new GameTexture(content.Load<Texture2D>("Entities\\" + name), offset));
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

        public List<Entity> GetEntitiesHitBy(Entity attacker)
        {
            List<Entity> damagedEntities = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (attacker == entity)
                {
                    continue;
                }

                if (attacker.Weapon.GetBaseBoundingRectangle().Intersects(entity.GetBoundingRectangle()))
                {
                    damagedEntities.Add(entity);
                }
            }
            return damagedEntities;
        }

        public void Update(GraphicsDevice graphicsDevice,GameTime gameTime)
        {
            for (int i = 0; i < updateObjects.Count; i++)
            {
                updateObjects[i].Update(gameTime);
            }
            int height = graphicsDevice.Viewport.Height;

            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                Rectangle rectangle = new Rectangle(entity.Location.ToPoint(), entity.BaseSize);
                float normalizedHieght = (float) rectangle.Bottom / height;
                if (normalizedHieght > 1)
                {
                    normalizedHieght = 1;
                }
                if(normalizedHieght < 0)
                {
                    normalizedHieght = 0;
                }
                entity.Layer = 1 - normalizedHieght;
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
