using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class SimpleDialog
    {
        public Npc Parent { get; set; }
        public Vector2 DrawOffset { get; set; }

        private readonly SpriteFont font;
        

        public string Text { get; private set; }

        public SimpleDialog(string text)
        {
            Text = text;
            font = GameManager.Instance.DialogTextFont;
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            sprite.DrawString(font, Text, Parent.Location + DrawOffset + new Vector2(-font.MeasureString(Text).X / 2, 0), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
