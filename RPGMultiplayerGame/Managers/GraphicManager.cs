using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OffsetGeneratorLib;
using RPGMultiplayerGame.Objects.Other;
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
            CommonArrow
        }

        public enum VisualEffectId
        {
            WindStorm,
        }

        public const float OWN_PLAYER_LAYER = 0.6f;
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
                    for (int k = 1; k <= 32; k++)
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
            for (int i = 0; i < grapichObjects.Count; i++)
            {
                grapichObjects[i].Draw(sprite);
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
