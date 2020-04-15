using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class QuestCompletedDialog : ComplexDialog
    {
        private readonly Quest quest;

        public QuestCompletedDialog(int index, string name, string text, Quest quest) : base(index, name, text, true)
        {
            this.quest = quest;
        }

        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            interactivePlayer.GetQuestByType(quest).RewardPlayer(interactivePlayer);
            interactivePlayer.GetQuestByType(quest).Unassign(interactivePlayer);
            return base.GetNextDialogByAnswer(interactivePlayer, answerIndex);
        }
    }
}
