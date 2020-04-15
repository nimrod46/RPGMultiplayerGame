﻿using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class DialogQuestAssign : DialogQuest
    {

        public DialogQuestAssign(int index, string name, string text, Quest quest) : base(index, name, text, false, quest)
        {
        }

        public new T AddAnswerOption<T>(string optionText, params object[] args) where T : DialogQuestInProgress
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