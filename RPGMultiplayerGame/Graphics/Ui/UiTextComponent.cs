using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using RPGMultiplayerGame.Graphics.Ui;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextComponent : UiComponent
    {

        public Func<string> TextFunc { get; set; }
        public SpriteFont TextFont { get; private set; }
        public Color TextColor { get; set; }

        public ColoredTextRenderer ColoredText { get; }
        public string Text
        {
            get => ColoredText.Text; set
            {
                ColoredText.Text = value;
                Size = ColoredText.Size;
            }
        }

        private string lastTextFronFunc;

        public UiTextComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, SpriteFont textFont, Func<string> textFunc, Color textColor) : base(origin, originType, layer)
        {
            TextFont = textFont;
            ColoredText = new ColoredTextRenderer(TextFont, "", DrawPosition, textColor, layer);
            this.TextFunc = textFunc;
            TextColor = textColor;
            Text = textFunc.Invoke();
            lastTextFronFunc = "";
            IsVisible = defaultVisibility;
            IsEnabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            string newText = TextFunc.Invoke();
            if (newText != lastTextFronFunc)
            {
                Text = newText;
                lastTextFronFunc = newText;
            }
        }

        public override void UpdatePosition()
        {
            base.UpdatePosition();
            if (ColoredText != null)
            {
                ColoredText.Position = DrawPosition;
            }
        }

        public override void Resize()
        {
            base.Resize();
            ColoredText.Position = DrawPosition;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            ColoredText.Draw(sprite);
        }
    }
}
