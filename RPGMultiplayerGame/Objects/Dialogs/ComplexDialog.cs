using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using RPGMultiplayerGame.Objects.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Dialogs
{
    public class ComplexDialog : SimpleDialog //TODO: Make UI component
    {
        private Texture2D background;

        public int Index { get; set; }

        public string Name { get; private set; }
        public bool IsProgressing { get; }

        private readonly List<KeyValuePair<string, ComplexDialog>> dialogsByAnswers = new List<KeyValuePair<string, ComplexDialog>>();
        public ComplexDialog(string name, string text, bool isProgressing) : base(text)
        {
            Index = 0;
            Name = name;
            IsProgressing = isProgressing;
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Select(i => i.Key).ToArray());
        }

        public ComplexDialog(int index, string name, string text, bool isProgressing) : base(text)
        {
            Index = index;
            Name = name;
            IsProgressing = isProgressing;
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Select(i => i.Key).ToArray());
        }

        public ComplexDialog(string text, bool isProgressing) : base(text)
        {
            Index = 0;
            Name = "";
            IsProgressing = isProgressing;
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Select(i => i.Key).ToArray());
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
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Select(i => i.Key).ToArray());
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

        public new void DrawAt(SpriteBatch sprite, Vector2 location)
        {
            base.DrawAt(sprite, location);
            sprite.Draw(background, new Vector2(sprite.GraphicsDevice.Viewport.Width / 2 - background.Width / 2, sprite.GraphicsDevice.Viewport.Height / 4 - background.Height), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
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
