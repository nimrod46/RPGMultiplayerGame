using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class QuestsMenu
    {
        private readonly Dictionary<Vector2, Quest> quests = new Dictionary<Vector2, Quest>();
        private Vector2 origin;
        private readonly OriginLocationType originType;

        public QuestsMenu(Vector2 origin, OriginLocationType originType)
        {
            this.origin = origin;
            this.originType = originType;
        }

        private Vector2 GetOriginBaseOnOriginType(Vector2 origin, Vector2 size, OriginLocationType originType)
        {
            switch (originType)
            {
                case OriginLocationType.Centered:
                    origin = origin - new Vector2(size.X / 2, size.Y / 2 * quests.Count());
                    break;
                case OriginLocationType.ButtomLeft:
                    origin = origin - new Vector2(0, size.Y * quests.Count());
                    break;
                case OriginLocationType.ButtomCentered:
                    origin = origin - new Vector2(size.X / 2, size.Y * quests.Count());
                    break;
                case OriginLocationType.TopLeft:
                    origin = origin - new Vector2(size.X, -size.Y * quests.Count());
                    break;
            }
            return origin;
        }

        public void AddQuest(Quest quest)
        {
            lock (quests)
            {
                Vector2 lastQuestPosition;
                if (quests.Count != 0)
                {
                    lastQuestPosition = GetOriginBaseOnOriginType(origin, quest.GetTextSize(), originType);
                }
                else
                {
                    lastQuestPosition = GetOriginBaseOnOriginType(origin, quest.GetTextSize(), originType);
                }
                quests.Add(lastQuestPosition, quest);
            }
        }

        public void Draw(SpriteBatch sprite)
        {
            lock (quests)
            {
                foreach (var v in quests)
                {
                    v.Value.DrawAt(sprite, v.Key);
                }
            }
        }
    }
}
