using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class QuestAssignDialog : QuestDialog
    {

        public QuestAssignDialog(int index, string name, string text, Quest quest) : base(index, name, text, false, quest)
        {
        }

        public new T AddAnswerOption<T>(string optionText, params object[] args) where T : QuestInProgressDialog
        {
            return base.AddAnswerOption<T>(optionText, args);
        }
        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            if (answerIndex == 0)
            {
                ServerManager.Instance.AddQuest(quest, interactivePlayer);
            }
            return base.GetNextDialogByAnswer(interactivePlayer, answerIndex);
        }
    }
}
