using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OffsetGeneratorLib;
using RPGMultiplayerGame.Objects.Items;
using RPGMultiplayerGame.Ui;
using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Objects.Other.AnimatedObject;

namespace RPGMultiplayerGame.Managers
{
    class UiManager
    {
        private static UiManager instance;

        public static UiManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UiManager();
                }
                return instance;
            }
        }
        public const float GUI_LAYER = 0.1f;

        public Dictionary<ItemType, Texture2D> ItemTextures { get; set; }
        public Texture2D UiHealthBar { get; set; }
        public Texture2D UiHealthBarBackground { get; set; }
        public Texture2D ItemDescriptionBackground { get; set; }
        public Texture2D InventorySlotBackground { get; set; }
        public Texture2D DialogBackground { get; set; }
        public SpriteFont DialogTextFont { get; set; }
        public SpriteFont HealthTextFont { get; internal set; }
        public SpriteFont StackableItemNumberFont { get; internal set; }
        public SpriteFont ItemDescriptionFont { get; internal set; }
        public SpriteFont GoldTextFont { get; internal set; }

        private readonly List<UiComponent> uiComponents = new List<UiComponent>();
        private readonly string dialogBackgroundPath;
        private readonly string questBackgroundPath;
        private Game1 game;

        private UiManager()
        {
            ItemTextures = new Dictionary<ItemType, Texture2D>();
            dialogBackgroundPath = "Content\\Ui\\DialogBackground.svg";
            questBackgroundPath = "Content\\Ui\\QuestBackground.svg";
        }

        public void Init(Game1 game)
        {
            this.game = game;
        }

        public void LoadTextures(ContentManager content)
        {
            UiHealthBar = content.Load<Texture2D>("Ui\\UiHealthBar");
            UiHealthBarBackground = content.Load<Texture2D>("Ui\\UiHealthBarBackground");
            InventorySlotBackground = content.Load<Texture2D>("Ui\\InventorySlot");
            ItemDescriptionBackground = content.Load<Texture2D>("Ui\\DescriptionBackground");
            DialogTextFont = content.Load<SpriteFont>("Ui\\DialogTextFont");
            HealthTextFont = content.Load<SpriteFont>("Ui\\HealthTextFont");
            StackableItemNumberFont = content.Load<SpriteFont>("Ui\\StackableItemNumberFont");
            ItemDescriptionFont = content.Load<SpriteFont>("Ui\\ItemDescriptionFont");
            GoldTextFont = content.Load<SpriteFont>("Ui\\GoldTextFont");

            foreach (ItemType gameItemType in Enum.GetValues(typeof(ItemType)))
            {
                ItemTextures.Add(gameItemType, content.Load<Texture2D>("Ui\\" + gameItemType.ToString()));
            }
        }

        internal Texture2D GetItemTextureByType(ItemType value)
        {
            return ItemTextures[value];
        }

        public Point GetScreenSize()
        {
            return game.GraphicsDevice.PresentationParameters.Bounds.Size;
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


        public void Draw(SpriteBatch sprite)
        {
            for (int i = 0; i < uiComponents.Count; i++)
            {
                uiComponents[i].Draw(sprite);
            }
        }

        public void OnResize()
        {
            lock (uiComponents)
            {
                foreach (var uiComponent in uiComponents)
                {
                    uiComponent.Resize();
                }
            }
        }

        public void AddUiComponent(UiComponent uiComponent)
        {
            lock (uiComponents)
            {
                uiComponents.Add(uiComponent);
            }
        }

        public void RemoveUiComponent(UiComponent uiComponent)
        {
            lock (uiComponents)
            {
                uiComponents.Remove(uiComponent);
            }
        }
    }
}
