using System;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Graphics.Ui.Dialogs
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

