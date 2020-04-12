using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Objects.LivingEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Objects.Other
{
    public class ComplexDialog : SimpleDialog
    {
        private Texture2D background;

        public int Index { get; set; }

        public string Name { get; private set; }
        public bool IsProgressing { get; }

        private readonly Dictionary<string, ComplexDialog> dialogsByAnswers = new Dictionary<string, ComplexDialog>();

        public ComplexDialog(string name, string text, bool isProgressing) : base(text)
        {
            Index = 0;
            Name = name;
            IsProgressing = isProgressing;
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Keys.ToArray());
        }

        protected ComplexDialog(int index, string name, string text, bool isProgressing) : base(text)
        {
            Index = index;
            Name = name;
            IsProgressing = isProgressing;
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Keys.ToArray());
        }

        public ComplexDialog AddAnswerOption(string optionText, string text, bool isProgressing)
        {
            ComplexDialog dialog = new ComplexDialog(GetDialogsCount(this) + 1 + Index, Name, text, isProgressing);
            dialogsByAnswers.Add(optionText, dialog);
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Keys.ToArray());
            return dialog;
        }

        public T AddAnswerOption<T>(string optionText, params object[] args) where T : ComplexDialog
        {
            var v = args.ToList();
            v.Insert(0, GetDialogsCount(this) + 1 + Index);
            v.Insert(1, Name);
            T dialog = (T)Activator.CreateInstance(typeof(T), v.ToArray());
            dialogsByAnswers.Add(optionText, dialog);
            background = GameManager.Instance.GetDialogBackgroundByProperties(Name, Text, Color.White, dialogsByAnswers.Keys.ToArray());
            return dialog;
        }

        public virtual ComplexDialog GetNextDialogByAnswer(Player interactivePlayer, int answerIndex)
        {
            if(answerIndex >= dialogsByAnswers.Count || dialogsByAnswers.Count == 0)
            {
                return null;
            }

            return dialogsByAnswers.Values.ToList()[answerIndex];
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
            foreach (var item in dialogsByAnswers.Values)
            {
                result = item.GetDialogByIndex(index);
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
            foreach (var item in dialog.dialogsByAnswers.Values)
            {
                count += GetDialogsCount(item) + 1;
            }
            return count;
        }
    }
}
