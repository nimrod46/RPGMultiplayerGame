using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Ui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class ComplexDialog : UiTextureComponent
    {

        public string Text { get; set; }

        public int Index { get; set; }

        public string Name { get; private set; }
        public bool IsProgressing { get; }

        private readonly List<KeyValuePair<string, ComplexDialog>> dialogsByAnswers = new List<KeyValuePair<string, ComplexDialog>>();
        public ComplexDialog(string name, string text, bool isProgressing) : base((screenSize) => new Vector2(screenSize.X / 2, screenSize.Y / 4), PositionType.Centered, false, UiManager.GUI_LAYER, UiManager.Instance.GetDialogBackgroundByProperties(name, text, Color.White))
        {
            Index = 0;
            Name = name;
            Text = text;
            IsProgressing = isProgressing;
        }

        public ComplexDialog(int index, string name, string text, bool isProgressing) : base((screenSize) => new Vector2(screenSize.X / 2, screenSize.Y / 4), PositionType.Centered, false, UiManager.GUI_LAYER, UiManager.Instance.GetDialogBackgroundByProperties(name, text, Color.White))
        {
            Index = index;
            Name = name;
            Text = text;
            IsProgressing = isProgressing;
        }

        public ComplexDialog(string text, bool isProgressing) : base((screenSize) => new Vector2(screenSize.X / 2, screenSize.Y / 4), PositionType.Centered, false, UiManager.GUI_LAYER, UiManager.Instance.GetDialogBackgroundByProperties("", text, Color.White))
        {
            Index = 0;
            Name = "";
            Text = text;
            IsProgressing = isProgressing;
        }

        public virtual ComplexDialog GetLast()
        {
            return this;
        }

        public virtual ComplexDialog AddAnswerOption(string optionText, string text, bool isProgressing)
        {
            return AddAnswerOption<ComplexDialog>(optionText, text, isProgressing);
        }

        public ComplexDialog AddAnswerOption<T>(string optionText, params object[] args) where T : ComplexDialog
        {
            T dialog = (T)Activator.CreateInstance(typeof(T), args);
            return AddAnswerOption(optionText, dialog);
        }

        public ComplexDialog AddAnswerOption<T>(string optionText, T dialog) where T : ComplexDialog
        {
            dialog.Index = GetDialogsCount(this) + 1 + Index;
            dialog.Name = Name;
            dialogsByAnswers.Add(new KeyValuePair<string, ComplexDialog>(optionText, dialog));
            Texture = UiManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Select(i => i.Key).ToArray());
            return dialog.GetLast();
        }



        public virtual ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            if (answerIndex >= dialogsByAnswers.Count || dialogsByAnswers.Count == 0)
            {
                return null;
            }

            return dialogsByAnswers[answerIndex].Value;
        }

        public int AnswersCount()
        {
            return dialogsByAnswers.Count;
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);
        }

        public ComplexDialog GetDialogByIndex(int index)
        {
            if (Index == index) return this;
            ComplexDialog result;
            foreach (var item in dialogsByAnswers)
            {
                result = item.Value.GetDialogByIndex(index);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private static int GetDialogsCount(ComplexDialog dialog)
        {
            int count = 0;
            foreach (var item in dialog.dialogsByAnswers)
            {
                count += GetDialogsCount(item.Value) + 1;
            }
            return count;
        }
    }
}
