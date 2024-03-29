﻿using Microsoft.Xna.Framework;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;

namespace RPGMultiplayerGame.Objects.QuestsObjects
{
    public class QuestsMenu : UiComponent
    {
        private readonly List<Quest> quests = new List<Quest>();

        public QuestsMenu(Func<Point, Vector2> origin, PositionType positionType) : base(origin, positionType, UiManager.GUI_LAYER)
        {
            IsVisible = true;
        }

        public void AddQuest(Quest quest)
        {
            lock (quests)
            {
                quests.Add(quest);
                Size = new Vector2(quest.QuestUi.Size.X, Size.Y + quest.QuestUi.Size.Y);
                quest.QuestUi.OriginFunc = (screentSize) => Position + new Vector2(0, quest.QuestUi.Size.Y * quests.Count);
                quest.MakeVisible();
            }
        }

        public Quest GetQuestByType(Quest quest)
        {
            lock (quests)
            {
                foreach (var q in quests)
                {
                    if (q.GetType() == quest.GetType())
                    {
                        return q;
                    }
                }
            }
            return null;
        }

        internal void RemoveQuest(Quest quest)
        {
            lock (quests)
            {
                quests.Remove(quest);
                foreach (var aQuest in quests)
                {
                    aQuest.QuestUi.Resize();
                }
            }
        }
    }
}
