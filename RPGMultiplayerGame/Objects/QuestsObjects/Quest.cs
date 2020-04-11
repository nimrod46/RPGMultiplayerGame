using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class Quest : NetworkIdentity
    {
        protected bool SyncIsFinished
        {
            get => isFinished;
            set
            {
                isFinished = value;
                InvokeSyncVarNetworkly(nameof(SyncIsFinished), isFinished);
                if (isFinished)
                {
                    textColor = Color.LawnGreen;
                    background = GameManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor);
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
            SyncIsFinished = false;
            textColor = Color.Blue;
            textFont = GameManager.Instance.PlayerNameFont;
            background = GameManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor);
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
