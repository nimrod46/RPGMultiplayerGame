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
    public class QuestInProgressDialog : QuestDialog
    {
        public QuestInProgressDialog(int index, string name, string text, Quest questInProgress, string questStilInProgressText) : base(index, name, text, true, questInProgress)
        {
            base.AddAnswerOption("", questStilInProgressText, false);
        }

        public new T AddAnswerOption<T>(string optionText, params object[] args) where T : QuestCompletedDialog
        {
            return base.AddAnswerOption<T>(optionText, args);
        }

        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            if (answerIndex > 0)
            {
                return null;
            }

            if (!interactivePlayer.GetQuestByType(quest).SyncIsFinished)
            {
                return base.GetNextDialogByAnswer(interactivePlayer, answerIndex + 1);
            }
            return base.GetNextDialogByAnswer(interactivePlayer, answerIndex);
        }
    }
}

