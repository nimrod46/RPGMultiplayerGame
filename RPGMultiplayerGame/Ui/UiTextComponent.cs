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

        public SpriteFont TextFont { get; private set; }

        public string Text
        {
            get => text; set
            {
                text = value;
                Size = TextFont.MeasureString(text);
            }
        }


        public UiTextComponent(Func<Point, Vector2> origin, PositionType originType, bool defaultVisibility, float layer, SpriteFont textFont, string defaultString) : base(origin, originType, defaultVisibility, layer)
        {
            TextFont = textFont;
            Text = defaultString;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (IsVisible)
            {
                sprite.DrawString(TextFont, Text, Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, Layer);
            }
        }
    }
}
