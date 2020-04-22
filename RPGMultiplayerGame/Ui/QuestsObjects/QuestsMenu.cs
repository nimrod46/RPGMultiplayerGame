using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPGMultiplayerGame.Managers.GameManager;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class QuestsMenu : UiComponent
    {
        private readonly List<Quest> quests = new List<Quest>();

        public QuestsMenu(Func<Point, Vector2> origin, PositionType positionType) : base(origin, positionType, GameManager.GUI_LAYER)
        {
        }

        public void AddQuest(Quest quest)
        {
            lock (quests)
            {
                quest.PositionType = OriginType;
                quest.Origin = Origin;
                quest.Position += new Vector2(0, quest.Size.Y * quests.Count);
                quests.Add(quest);
                Size += new Vector2(0, quest.Size.Y);
            }
        }

        public Quest GetQuestByType(Quest quest)
        {
            lock (quests)
            {
                foreach (var q in quests)
                {
                    if(q.GetType() == quest.GetType())
                    {
                        return q;
                    }
                }
            }
            return null;
        }

        public override void Draw(SpriteBatch sprite)
        {
            lock (quests)
            {
                foreach (var quest in quests)
                {
                    quest.Draw(sprite);
                }
            }
        }

        internal void RemoveQuest(Quest quest)
        {
            lock (quests)
            {
                int removedIndex = quests.IndexOf(quest);
                for (int i = removedIndex + 1; i < quests.Count; i++)
                {
                    quests[i].Position -= new Vector2(0, -quest.Size.Y);
                }
                quests.Remove(quest);
            }
        }
    }
}
