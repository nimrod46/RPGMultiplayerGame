using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Ui
{
    public class UiTextComponent : UiComponent
    {
        private string text;
        private readonly Func<string> textFunc;

        public SpriteFont TextFont { get; private set; }

        public string Text
        {
            get => text; set
            {
                text = value;
                Size = TextFont.MeasureString(text);
            }
        }


        public UiTextComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, SpriteFont textFont, Func<string> textFunc) : base(origin, originType, defaultVisibility, layer)
        {
            TextFont = textFont;
            this.textFunc = textFunc;
            Text = textFunc.Invoke();
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
            if (isVisible)
            {
                Text = textFunc.Invoke();
                sprite.DrawString(TextFont, Text, DrawPosition, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, Layer);
            }
        }
    }
}
