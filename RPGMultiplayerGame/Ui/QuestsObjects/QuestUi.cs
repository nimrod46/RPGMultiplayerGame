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
    class QuestUi : UiTextureComponent
    {
        private Color textColor;
        private readonly string npcName;
        private readonly string text;

        public QuestUi(Func<Point, Vector2> origin, PositionType originType, string npcName, string text, Color textColor) : base(origin, originType, true, GameManager.GUI_LAYER, GameManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor))
        {
            this.npcName = npcName;
            this.text = text;
            this.textColor = textColor;
        }


        public void MarkFinished()
        {
            textColor = Color.LawnGreen;
            Texture = GameManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor);
        }
    }
}
