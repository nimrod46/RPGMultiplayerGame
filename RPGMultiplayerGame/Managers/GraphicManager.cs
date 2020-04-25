using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OffsetGeneratorLib;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Objects.Other;
using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

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



        public enum EffectId
        {
            FireBall
        }

        public const float GUI_LAYER = 0.1f;
        public const float OWN_PLAYER_LAYER = 0.6f;
        public const float ENTITY_LAYER = 0.7f;
        public const float MOVING_OJECT_LAYER = 0.8f;
        public const float CHARECTER_TEXT_LAYER = 0.9f;

        public Dictionary<EntityId, Dictionary<int, List<GameTexture>>> AnimationsByEntities { get; set; }
        public Dictionary<EffectId, Dictionary<int, List<GameTexture>>> AnimationsByEffects { get; set; }
        public Dictionary<ItemType, Texture2D> ItemTextures { get; set; }
        public List<Texture2D> Textures { get; set; }
        public Texture2D HealthBar { get; set; }
        public Texture2D HealthBarBackground { get; set; }
        public Texture2D UiHealthBar { get; set; }
        public Texture2D UiHealthBarBackground { get; set; }
        public SpriteFont PlayerNameFont { get; set; }
        public Texture2D DialogBackground { get; set; }
        public SpriteFont DialogTextFont { get; set; }
        public Texture2D InventorySlotBackground { get; set; }
        public Texture2D ItemDescriptionBackground { get; set; }
        private readonly List<IGameDrawable> grapichObjects = new List<IGameDrawable>();
        private readonly string dialogBackgroundPath;
        private readonly string questBackgroundPath;
        private Game1 game;

        private GraphicManager()
        {
            AnimationsByEntities = new Dictionary<EntityId, Dictionary<int, List<GameTexture>>>();
            AnimationsByEffects = new Dictionary<EffectId, Dictionary<int, List<GameTexture>>>();
            ItemTextures = new Dictionary<ItemType, Texture2D>();
            Textures = new List<Texture2D>();
            dialogBackgroundPath = "Content\\DialogBackground.svg";
            questBackgroundPath = "Content\\QuestBackground.svg";
        }

        public void Init(Game1 game)
        {
            this.game = game;
            Console.WriteLine("game: " + game);
        }

        public void LoadTextures(ContentManager content)
        {
            AnimationsByEntities = GetGameTextureByEnum<EntityId>(content);
            AnimationsByEffects = GetGameTextureByEnum<EffectId>(content);
            HealthBar = content.Load<Texture2D>("HealthBar");
            HealthBarBackground = content.Load<Texture2D>("HealthBarBackground");
            UiHealthBar = content.Load<Texture2D>("UiHealthBar");
            UiHealthBarBackground = content.Load<Texture2D>("UiHealthBarBackground");
            PlayerNameFont = content.Load<SpriteFont>("PlayerName");
            DialogTextFont = content.Load<SpriteFont>("DialogText");
            InventorySlotBackground = content.Load<Texture2D>("InventorySlot");
            ItemDescriptionBackground = content.Load<Texture2D>("DescriptionBackground");


            foreach (ItemType gameItemType in Enum.GetValues(typeof(ItemType)))
            {
                ItemTextures.Add(gameItemType, content.Load<Texture2D>(gameItemType.ToString()));
            }

            Texture2D spriteTextures = content.Load<Texture2D>("basictiles");
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



        internal Texture2D GetItemTextureByType(ItemType value)
        {
            return ItemTextures[value];
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

        public Texture2D GetDialogBackgroundByProperties(string name, string text, Color textColor, params string[] options)
        {
            return GetBackgroundByProperties(dialogBackgroundPath, name, text, textColor, options);
        }

        public Texture2D GetQuestBackgroundByProperties(string name, string text, Color textColor, params string[] options)
        {
            return GetBackgroundByProperties(questBackgroundPath, name, text, textColor, options);
        }

        public Texture2D GetBackgroundByProperties(string path, string name, string text, Color textColor, params string[] options)
        {
            return SVGToTexture2D(path, name, text, textColor, 0, 0, options);
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

        private Texture2D SVGToTexture2D(string path, string name, string text, Color? textColor = null, int width = 0, int height = 0, params string[] options)
        {
            textColor ??= Color.White;
            var svgDoc = SvgDocument.Open<SvgDocument>(path, null);
            if (width == 0)
            {
                width = (int)svgDoc.Bounds.Width;
            }
            if (height == 0)
            {
                height = (int)svgDoc.Bounds.Height;
            }


            ((SvgTextBase)svgDoc.GetElementById<SvgTextBase>("Name").Children[0]).Text = name;
            int cahrCount = 0;
            SvgTextBase svgText = (SvgTextBase)svgDoc.GetElementById("Text").Children[0].DeepCopy();
            string line = "";
            svgText.Dy.Add(new SvgUnit(-(svgText.Y[0].Value)));
            text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(w =>
            {
                cahrCount += w.Length;
                if ((cahrCount) > 60)
                {
                    cahrCount = 0;
                    svgText.Text = line + " " + w;
                    line = "";
                    svgText.Dy[0] = (svgText.Dy[0] + svgText.Y[0]);
                    svgDoc.GetElementById("Text").Children.Add(svgText);
                    svgText = (SvgTextBase)svgText.DeepCopy();
                }
                else
                {
                    line += " " + w + " ";
                }
            });
            svgText.Text = line;
            svgText.Fill = new SvgColourServer(System.Drawing.Color.FromArgb(textColor.Value.A, textColor.Value.R, textColor.Value.G, textColor.Value.B));
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
                game.GraphicsDevice, memoryStream);
            return texture;
        }
    }
}
