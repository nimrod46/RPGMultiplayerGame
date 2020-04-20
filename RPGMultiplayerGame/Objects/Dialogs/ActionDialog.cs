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
