using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other.Quests
{
    public class Quest
    {
        public bool IsFinished
        {
            get => isFinished;
            protected set
            {
                isFinished = value;
                if(isFinished)
                {
                    textColor = Color.LawnGreen;
                }
            }
        }

        protected Player player;
        private readonly string text;
        private readonly SpriteFont textFont;
        private Color textColor;
        private bool isFinished;

        public Quest(string text)
        {
            this.text = text;
            IsFinished = false;
            textColor = Color.Blue;
            textFont = GameManager.Instance.PlayerNameFont;
        }

        public virtual void AssignTo(Player player)
        {
            this.player = player;
            player.AddQuest(this);
        }

        public void DrawAt(SpriteBatch sprite, Vector2 position)
        {
            sprite.DrawString(textFont, text, position, textColor);
        }

        public Vector2 GetTextSize()
        {
            return textFont.MeasureString(text);
        }
    }
}
