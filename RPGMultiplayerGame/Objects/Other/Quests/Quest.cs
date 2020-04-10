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
                    background = GameManager.Instance.GetQuestBackgroundByProperties(npcName,text, textColor);
                }
            }
        }

        protected Player player;
        private readonly string npcName;
        private readonly string text;
        private readonly SpriteFont textFont;
        private Color textColor;
        private bool isFinished;
        private Texture2D background;

        public Quest(string npcName, string text)
        {
            this.npcName = npcName;
            this.text = text;
            IsFinished = false;
            textColor = Color.Blue;
            textFont = GameManager.Instance.PlayerNameFont;
            background = GameManager.Instance.GetQuestBackgroundByProperties(npcName,text, textColor);
        }

        public virtual void AssignTo(Player player)
        {
            this.player = player;
            player.AddQuest(this);
        }

        public void DrawAt(SpriteBatch sprite, Vector2 position)
        {
            sprite.Draw(background, position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, GameManager.GUI_LAYER);
        }

        public Vector2 GetTextSize()
        {
            return background.Bounds.Size.ToVector2();
        }
    }
}
