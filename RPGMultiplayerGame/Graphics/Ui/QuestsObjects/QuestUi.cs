using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using System;

namespace RPGMultiplayerGame.Ui
{
    public class QuestUi : UiTextureComponent
    {
        private Color textColor;
        private readonly string npcName;
        private readonly string text;

        public QuestUi(Func<Point, Vector2> origin, PositionType originType, string npcName, string text, Color textColor) : base(origin, originType, false, UiManager.GUI_LAYER, UiManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor))
        {
            this.npcName = npcName;
            this.text = text;
            this.textColor = textColor;
        }


        public void MarkFinished()
        {
            textColor = Color.LawnGreen;
            Texture = UiManager.Instance.GetQuestBackgroundByProperties(npcName, text, textColor);
        }
    }
}
