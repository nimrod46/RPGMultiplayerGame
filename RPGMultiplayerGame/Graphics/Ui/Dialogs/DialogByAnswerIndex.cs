using RPGMultiplayerGame.Objects.LivingEntities;
using System;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class DialogByAnswerIndex : ComplexDialog
    {
        private readonly Func<Player, int, int> getIndex;

        public DialogByAnswerIndex(string text, Func<Player, int, int> getIndex) : base(text, true)
        {
            this.getIndex = getIndex;
        }

        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {

            return base.GetNextDialogByAnswer(interactivePlayer, getIndex.Invoke(interactivePlayer, answerIndex));
        }
    }
}

