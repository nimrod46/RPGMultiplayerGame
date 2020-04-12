using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public class QuestDialog : ComplexDialog
    {
        private readonly Quest quest;

        public QuestDialog(int index, string name, string text, Quest quest, string nextDialogText) : base(index, name, text, false)
        {
            AddAnswerOption("Okay", nextDialogText, true);
            this.quest = quest;
        }

        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            if (ServerManager.Instance.IsRuning)
            {
                ServerManager.Instance.AddQuest(quest, interactivePlayer);
            }
            return base.GetNextDialogByAnswer(interactivePlayer, answerIndex);
        }
    }
}
