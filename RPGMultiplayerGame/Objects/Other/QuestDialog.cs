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

        public QuestDialog(string name, string text, Quest quest) : base(name, text)
        {
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
