using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;

namespace RPGMultiplayerGame.Graphics.Ui.Dialogs
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
                (interactivePlayer, answerIndex) =>
                {
                    if (answerIndex == 0)
                    {
                        AssignPlayer(interactivePlayer, quest);
                    }
                }));
            ComplexDialog inProgressDialog = dialog.AddAnswerOption("Okay", new DialogByAnswerIndex(inQuestText,
                (_, answerIndex) =>
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
                }));
            inProgressDialog.AddAnswerOption("Ok finished", notFinishedText, false);
            return inProgressDialog.AddAnswerOption("", new ActionDialog(finishedText, (_, _) =>
            {
                quest.RewardPlayer();
                quest.Unassign();
            }));
        }
    }
}
