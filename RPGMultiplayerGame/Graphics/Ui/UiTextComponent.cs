using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Graphics;
using System;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextComponent : UiComponent
    {
        private string text;
        private readonly Func<string> textFunc;

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

        private readonly ColoredText coloredText;

        public UiTextComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, SpriteFont textFont, Func<string> textFunc, Color textColor) : base(origin, originType, defaultVisibility, layer)
        {
            TextFont = textFont;
            coloredText = new ColoredText(TextFont, Text, DrawPosition, layer);
            this.textFunc = textFunc;
            TextColor = textColor;
            Text = textFunc.Invoke();
        }

        public override void Draw(SpriteBatch sprite)
        {
            coloredText.Position = DrawPosition;
            base.Draw(sprite);
            if (IsVisible)
            {
                Text = textFunc.Invoke();
                coloredText.Draw(sprite);
            }
        }
    }
}
