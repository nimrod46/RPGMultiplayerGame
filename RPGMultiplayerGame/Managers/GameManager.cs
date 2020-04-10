﻿using System;
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
using RPGMultiplayerGame.Objects.LivingEntities;
using Svg;
using RPGMultiplayerGame.Objects.Other;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;
using static RPGMultiplayerGame.Objects.InventoryObjects.Inventory;
using RPGMultiplayerGame.Objects.InventoryObjects;
using RPGMultiplayerGame.Objects.Items.Weapons;

namespace RPGMultiplayerGame.Managers
{
    public class GameManager
    {
        public enum EntityId
        {
            Player,
            Blacksmith,
            Bat
        }

       

        public enum EffectId
        {
            FireBall
        }

        public enum OriginLocationType
        {
            Centered,
            ButtomLeft,
            ButtomCentered,
            TopLeft
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

        public const int INVENTORY_ROWS_NUMBER = 4, INVENTORY_COLUMNS_NUMBER = 5;
        public const float INVENTORY_LAYER = 0.0001f;
        public const float OWN_PLAYER_LAYER = 0.001f;
        public const float ENTITY_LAYER = 0.01f;
        public const float MOVING_OJECT_LAYER = 0.02f;
        public const float CHARECTER_TEXT_LAYER = 0.9f;

        static GameManager instance;


        public Dictionary<EntityId, Dictionary<int, List<GameTexture>>> animationsByEntities = new Dictionary<EntityId, Dictionary<int, List<GameTexture>>>();
        public Dictionary<EffectId, Dictionary<int, List<GameTexture>>> animationsByEffects = new Dictionary<EffectId, Dictionary<int, List<GameTexture>>>();
        public Dictionary<ItemType, Texture2D> itemTextures = new Dictionary<ItemType, Texture2D>();
        public List<Texture2D> textures = new List<Texture2D>();
        public Texture2D HealthBar;
        public Texture2D HealthBarBackground;
        public SpriteFont PlayerNameFont;
        public Texture2D DialogBackground;
        public SpriteFont DialogTextFont;
        public Texture2D InventorySlotBackground;
        public GameMap map; //TODO: Create GameMap with the relevent project class.
        private readonly List<GameObject> gameObjects = new List<GameObject>();
        private readonly List<GraphicObject> grapichObjects = new List<GraphicObject>();
        private readonly List<UpdateObject> updateObjects = new List<UpdateObject>();
        private readonly List<Entity> entities = new List<Entity>();
        private readonly List<Monster> monsters = new List<Monster>();
        private GraphicsDevice graphicsDevice;
        private readonly string dialogBackgroundPath;

        private GameManager()
        {
            map = new GameMap();
            dialogBackgroundPath = "Content\\DialogBackground.svg";
        }

        public void Init(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public Point GetMapSize()
        {
            return graphicsDevice.PresentationParameters.Bounds.Size;
        }

        public void LoadTextures(GraphicsDevice graphicsDevice, ContentManager content)
        {
            animationsByEntities = GetGameTextureByEnum<EntityId>(content);
            animationsByEffects = GetGameTextureByEnum<EffectId>(content);
            HealthBar = content.Load<Texture2D>("HealthBar");
            HealthBarBackground = content.Load<Texture2D>("HealthBarBackground");
            PlayerNameFont = content.Load<SpriteFont>("PlayerName");
            DialogTextFont = content.Load<SpriteFont>("DialogText");
            InventorySlotBackground = content.Load<Texture2D>("InventorySlot");


            foreach (ItemType gameItemType in Enum.GetValues(typeof(ItemType)))
            {
                itemTextures.Add(gameItemType, content.Load<Texture2D>(gameItemType.ToString()));
            }

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

        internal Texture2D GetItemByType(Inventory.ItemType value)
        {
            return itemTextures[value];
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

        public void AddMonster(Monster monster)
        {
            lock (monsters)
            {
                monsters.Add(monster);
            }
        }
        public void RemoveMonster(Monster monster)
        {
            lock (monsters)
            {
                monsters.Remove(monster);
            }
        }
        
        public Texture2D GetDialogBackGroundByProperties(string name, string text, params string[] options)
        {
            return SVGToTexture2D(dialogBackgroundPath, name, text, 0, 0, options);
        }

        private Dictionary<T, Dictionary<int, List<GameTexture>>> GetGameTextureByEnum<T>(ContentManager content) where T : Enum
        {
            Dictionary<T, Dictionary<int, List<GameTexture>>> gameTextureByEnum = new Dictionary<T, Dictionary<int, List<GameTexture>>>();
            for (int i = 0; i < (int)(object)Enum.GetValues(typeof(T)).Cast<object>().Cast<T>().Last() + 1; i++)
            {
                XmlManager<List<AnimationPropertiesLib>> xml = new XmlManager<List<AnimationPropertiesLib>>();
                List<AnimationPropertiesLib> animationProperties = null;
                try
                {
                    animationProperties = xml.Load("Content\\" + (T)(object)i + ".xml");
                }
                catch (Exception)
                {
                    Console.WriteLine("Warning: no xml found for " + (T)(object)i);
                }
                Dictionary<int, List<GameTexture>> animations = new Dictionary<int, List<GameTexture>>();
                for (int j = 0; j < (int)Enum.GetValues(typeof(EntityAnimation)).Cast<EntityAnimation>().Last() + 1; j++)
                {
                    List<GameTexture> animation = new List<GameTexture>();
                    for (int k = 1; k <= 32; k++)
                    {
                        string name = "" + (T)(object)i + "\\" + (EntityAnimation)j + "\\" + k;
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
                    animations.Add(j, animation);
                }

                gameTextureByEnum.Add((T)(object)i, animations);
            }
            return gameTextureByEnum;
        }
        private Texture2D SVGToTexture2D(string path, string name, string text, int width = 0, int height = 0, params string[] options)
        {
           
            var svgDoc = SvgDocument.Open<SvgDocument>(path, null);
            if (width == 0)
            {
                width = (int) svgDoc.Bounds.Width;
            }
            if (height == 0)
            {
                height = (int)svgDoc.Bounds.Height;
            }


            ((SvgTextBase) svgDoc.GetElementById("Name").Children[0]).Text = name;
            int cahrCount = 0;
            SvgTextBase svgText = (SvgTextBase)svgDoc.GetElementById("Text").Children[0].DeepCopy();
            string line = "";
            svgText.Dy.Add(new SvgUnit(-(svgText.Y[0].Value)));
            text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(w =>
            {
                cahrCount += w.Length;
                if ((cahrCount ) > 60)
                {
                    cahrCount = 0;
                    svgText.Text = line + " " + w;
                    line = "";
                    svgText.Dy[0] = (svgText.Dy[0] + svgText.Y[0]);
                    svgDoc.GetElementById("Text").Children.Add(svgText);
                    svgText = (SvgTextBase) svgText.DeepCopy();
                }
                else
                {
                    line += " " + w + " ";
                }
            });
            svgText.Text = line;
            svgText.Dy[0] = (svgText.Dy[0] + svgText.Y[0]);
            svgDoc.GetElementById("Text").Children.Add(svgText);    

            for (int i = 0; i < options.Length; i++)
            {

                ((SvgTextBase)svgDoc.GetElementById("Option" + (i + 1)).Children[0]).Text = options[i];

            }
            int bufferSize = width * height * 4;
            System.IO.MemoryStream memoryStream =
                new System.IO.MemoryStream(bufferSize);
            svgDoc.Draw().Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            Texture2D texture = Texture2D.FromStream(
                graphicsDevice, memoryStream);
            return texture;
        }
    }
}
