using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Managers;
using System;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextComponent : UiComponent
    {
        private string text;

        public Func<string> TextFunc { get; set; }
        public SpriteFont TextFont { get; private set; }
        public Color TextColor { get; set; }

        public string Text
        {
            get => text; set
            {
                text = value;
                coloredText.Text = text;
                Size = coloredText.Size;
            }
        }


        private readonly ColoredTextRenderer coloredText;

        public UiTextComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, SpriteFont textFont, Func<string> textFunc, Color textColor) : base(origin, originType, defaultVisibility, layer)
        {
            TextFont = textFont;
            coloredText = new ColoredTextRenderer(TextFont, Text, UiManager.COLOR_CODE_SPLITTER, DrawPosition, textColor, layer);
            this.TextFunc = textFunc;
            TextColor = textColor;
            Text = textFunc.Invoke();
        }

        public string ColorToColorCode(System.Drawing.KnownColor color)
        {
            return coloredText.ColorToColorCode(color);
        }

        public string ColorToColorCode(System.Drawing.KnownColor color, string text)
        {
            return coloredText.ColorToColorCode(color, text);
        }

        public override void Draw(SpriteBatch sprite)
        {
            coloredText.Position = DrawPosition;
            base.Draw(sprite);
            if (IsVisible)
            {
                Text = TextFunc.Invoke();
                coloredText.Draw(sprite);
            }
        }
    }
}
