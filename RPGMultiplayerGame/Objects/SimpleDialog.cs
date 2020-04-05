using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects
{
    public class SimpleDialog
    {
        private SpriteFont font;

        public string Text { get; private set; }

        public SimpleDialog(string text)
        {
            Text = text;
            font = GameManager.Instance.DialogTextFont;
        }

        public virtual void DrawAt(SpriteBatch sprite, Vector2 location)
        {
            sprite.DrawString(font, Text, location + new Vector2(-font.MeasureString(Text).X / 2, 0), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
