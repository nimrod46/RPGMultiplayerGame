﻿using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class QuestDialog : ComplexDialog
    {
        private Quest quest;
        private readonly string questText;
        private readonly string inQuestText;
        private readonly string notFinishedText;
        private readonly string finishedText;

        public QuestDialog(string text, Quest quest, string questText, string inQuestText, string notFinishedText, string finishedText) : base(text, true)
        {
            this.quest = quest;
            this.questText = questText;
            this.inQuestText = inQuestText;
            this.notFinishedText = notFinishedText;
            this.finishedText = finishedText;
        }

        public void AssignPlayer(Player player, Quest quest)
        {
            this.quest = ServerManager.Instance.AddQuest(player, (dynamic) quest);
        }

        public override ComplexDialog GetLast()
        {
            ComplexDialog dialog = AddAnswerOption("Got it", new ActionDialog(questText,
                new Action<Player, int>((interactivePlayer, answerIndex) =>
            {
                if (answerIndex == 0)
                {
                    AssignPlayer(interactivePlayer, quest);
                }
            })));
            ComplexDialog inProgressDialog = dialog.AddAnswerOption("Okay", new DialogByAnswerIndex(inQuestText,
                new Func<Player, int, int>((interactivePlayer, answerIndex) =>
            {
                if (answerIndex > 0)
                {
                    return -1;
                }

                if (!quest.SyncIsFinished)
                {
                    return answerIndex;
                }
                return answerIndex + 1;
            })));
            inProgressDialog.AddAnswerOption("Ok finished", notFinishedText, false);
            return inProgressDialog.AddAnswerOption("", new ActionDialog(finishedText, new Action<Player, int>((interactivePlayer, answerIndex) =>
           {
               quest.RewardPlayer();
               quest.Unassign();
           })));
        }
    }
}
