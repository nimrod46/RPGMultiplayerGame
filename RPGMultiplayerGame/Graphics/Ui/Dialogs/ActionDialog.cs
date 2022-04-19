using System;
using RPGMultiplayerGame.Objects.LivingEntities;

namespace RPGMultiplayerGame.Graphics.Ui.Dialogs
{
    public class ActionDialog : ComplexDialog
    {
        private readonly Action<Player, int> action;

        public ActionDialog(string text, Action<Player, int> action) : base(text, false)
        {
            this.action = action;
        }

        public override ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            action.Invoke(interactivePlayer, answerIndex);
            return base.GetNextDialogByAnswer(interactivePlayer, answerIndex);
        }
    }
}
