using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OffsetGeneratorLib;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using static RPGMultiplayerGame.Objects.LivingEntities.Entity;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;
using static RPGMultiplayerGame.Objects.VisualEffects.VisualEffect;

namespace RPGMultiplayerGame.Managers
{
    public class GraphicManager
    {
        private static GraphicManager instance;

        public static GraphicManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GraphicManager();
                }
                return instance;
            }
        }

        public enum EntityId
        {
            Player,
            Blacksmith,
            Bat
        }


        public enum WeaponAmmunitionId
        {
            FireBall,
            CommonArrow,
            FreezingArrow,
            ExplodingArrow,
            StormArrow
        }

        public enum VisualEffectId
        {
            WindStorm,
            Explotion,
        }

        public const float OWN_PLAYER_LAYER = 0.6f;
        public const float ITEM_LAYER = 0.6f;
        public const float ENTITY_LAYER = 0.7f;
        public const float MOVING_OJECT_LAYER = 0.8f;
        public const float CHARECTER_TEXT_LAYER = 0.9f;

        public Dictionary<EntityId, Dictionary<int, List<GameTexture>>> AnimationsByEntities { get; set; }
        public Dictionary<WeaponAmmunitionId, Dictionary<int, List<GameTexture>>> AnimationsByEffects { get; set; }
        public Dictionary<VisualEffectId, Dictionary<int, List<GameTexture>>> AnimationsByVisualEffect { get; set; }
        public List<Texture2D> Textures { get; set; }
        public Texture2D HealthBar { get; set; }
        public Texture2D HealthBarBackground { get; set; }
        public SpriteFont PlayerNameFont { get; set; }
        private readonly List<IGameDrawable> grapichObjects = new List<IGameDrawable>();

        private Game1 game;

        private GraphicManager()
        {
            AnimationsByEntities = new Dictionary<EntityId, Dictionary<int, List<GameTexture>>>();
            AnimationsByEffects = new Dictionary<WeaponAmmunitionId, Dictionary<int, List<GameTexture>>>();
            AnimationsByVisualEffect = new Dictionary<VisualEffectId, Dictionary<int, List<GameTexture>>>();
            Textures = new List<Texture2D>();

        }

        public void Init(Game1 game)
        {
            this.game = game;
        }

        public void LoadTextures(ContentManager content)
        {
            AnimationsByEntities = GetGameTextureByEnum<EntityId, EntityAnimation>(content);
            AnimationsByEffects = GetGameTextureByEnum<WeaponAmmunitionId, EntityAnimation>(content);
            AnimationsByVisualEffect = GetGameTextureByEnum<VisualEffectId, VisualEffectAnimation>(content);
            HealthBar = content.Load<Texture2D>("Graphics\\HealthBar");
            HealthBarBackground = content.Load<Texture2D>("Graphics\\HealthBarBackground");
            PlayerNameFont = content.Load<SpriteFont>("Graphics\\PlayerNameFont");

            Texture2D spriteTextures = content.Load<Texture2D>("Graphics\\basictiles");
            int count = 0;
            for (int y = 0; y < spriteTextures.Height; y += 16)
            {
                for (int x = 0; x < spriteTextures.Width; x += 16)
                {
                    Texture2D texture = new Texture2D(game.GraphicsDevice, 16, 16);
                    Rectangle cloneRect = new Rectangle(x, y, 16, 16);
                    int c = cloneRect.Width * cloneRect.Height;
                    Color[] data = new Color[c];
                    spriteTextures.GetData(0, cloneRect, data, 0, c);
                    texture.SetData(data);
                    Textures.Add(texture);
                    count++;
                }
            }
        }

        private Dictionary<T, Dictionary<int, List<GameTexture>>> GetGameTextureByEnum<T, V>(ContentManager content) where T : Enum where V : Enum
        {
            Dictionary<T, Dictionary<int, List<GameTexture>>> gameTextureByEnum = new Dictionary<T, Dictionary<int, List<GameTexture>>>();
            for (int i = 0; i < (int)(object)Enum.GetValues(typeof(T)).Cast<object>().Cast<T>().Last() + 1; i++)
            {
                XmlManager<List<AnimationPropertiesLib>> xml = new XmlManager<List<AnimationPropertiesLib>>();
                List<AnimationPropertiesLib> animationProperties = null;
                try
                {
                    animationProperties = xml.Load("Content\\Graphics\\Entities\\" + (T)(object)i + "\\" + (T)(object)i + ".xml");
                }
                catch (Exception)
                {
                    Console.WriteLine("Warning: no xml found for " + (T)(object)i);
                }
                Dictionary<int, List<GameTexture>> animations = new Dictionary<int, List<GameTexture>>();
                for (int j = 0; j < (int)(object)Enum.GetValues(typeof(V)).Cast<object>().Cast<V>().Last() + 1; j++)
                {
                    List<GameTexture> animation = new List<GameTexture>();
                    for (int k = 1; k <= 100; k++)
                    {
                        string name = "" + (T)(object)i + "\\" + (V)(object)j + "\\" + k;
                        string partPath = (EntityAnimation)j + "\\" + k;
                        Vector2 offset = Vector2.Zero;
                        if (animationProperties?.Where(a => partPath.Equals(a.FullPath)).Count() > 0)
                        {
                            offset = new Vector2(animationProperties.Where(a => partPath.Equals(a.FullPath)).ToArray()[0].Offset.X, animationProperties.Where(a => partPath.Equals(a.FullPath)).ToArray()[0].Offset.Y);
                        }
                        try
                        {
                            animation.Add(new GameTexture(content.Load<Texture2D>("Graphics\\Entities\\" + name), offset));
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

        public void Draw(SpriteBatch sprite)
        {
            lock (grapichObjects)
            {
                for (int i = 0; i < grapichObjects.Count; i++)
                {
                    grapichObjects[i].Draw(sprite);
                }

                int height = game.GraphicsDevice.Viewport.Height;
                if (GameManager.Instance.Player != null)
                {
                    lock (grapichObjects)
                    {
                        foreach (var graphicObj in grapichObjects)
                        {
                            if (graphicObj is Entity entity) {
                                float playerY = GameManager.Instance.Player.SyncY + GameManager.Instance.Player.BaseSize.Y;
                                float entityY = entity.SyncY + entity.BaseSize.Y;
                                if (MathHelper.Distance(playerY, entityY) < height / 2)
                                {
                                    if (playerY > entityY)
                                    {
                                        entityY = height / 2 + entityY - playerY;
                                    }
                                    else
                                    {
                                        entityY = height / 2 + entityY - playerY;
                                    }
                                    float normalizedHieght = (float)Math.Abs(entityY) / height;
                                    if (normalizedHieght > 1)
                                    {
                                        normalizedHieght = 1;
                                    }
                                    if (normalizedHieght < 0)
                                    {
                                        normalizedHieght = 0;
                                    }
                                    normalizedHieght = entityY < 0 ? normalizedHieght : 1 - normalizedHieght;
                                    normalizedHieght += Math.Sign(entityY) * (UiManager.GUI_LAYER + 0.01f);
                                    
                                    entity.Layer = normalizedHieght;
                                }
                                else
                                {
                                    entity.Layer = 1;
                                }
                            }
                        }
                    }
                }
            }
        }


        public void AddGraphicObject(IGameDrawable obj)
        {
            lock (grapichObjects)
            {
                grapichObjects.Add(obj);
            }
        }

        public void RemoveGraphicObject(IGameDrawable obj)
        {
            lock (grapichObjects)
            {
                grapichObjects.Remove(obj);
            }
        }
    }
}
