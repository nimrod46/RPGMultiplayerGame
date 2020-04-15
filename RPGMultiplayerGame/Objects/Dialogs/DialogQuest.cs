using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.Dialogs;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.QuestsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class DialogQuest : ComplexDialog
    {
        protected readonly Quest quest;

        public DialogQuest(int index, string name, string text, bool isProgressing, Quest quest) : base(index, name, text, isProgressing)
        {
            this.quest = quest;
        }

        public new T AddAnswerOption<T>(string optionText, params object[] args) where T : ComplexDialog
        {
            var v = args.ToList();
            v.Insert(1, quest);
           
            return base.AddAnswerOptionAt<T>(optionText, 0, v.ToArray());
        }
    }
}
