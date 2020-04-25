using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public class InteractionText
    {
        public Npc Parent { get; set; }
        public Vector2 DrawOffset { get; set; }

        private readonly SpriteFont font;


        public string Text { get; private set; }

        public InteractionText(string text)
        {
            Text = text;
            font = UiManager.Instance.DialogTextFont;
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            sprite.DrawString(font, Text, Parent.Location + DrawOffset + new Vector2(-font.MeasureString(Text).X / 2, -font.MeasureString(Text).Y), Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
