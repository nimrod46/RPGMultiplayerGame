using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPGMultiplayerGame.Managers;
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

        public string Name { get; private set; }
        public Dictionary<string, ComplexDialog> DialogsByAnswers { get; } = new Dictionary<string, ComplexDialog>();

        public ComplexDialog(string text) : base(text)
        {
            background = GameManager.Instance.GetDialogBackGroundByProperties(Name, Text, DialogsByAnswers.Keys.ToArray());
        }

        public ComplexDialog(string name, string text) : base(text)
        {
            Name = name;
            background = GameManager.Instance.GetDialogBackGroundByProperties(Name, Text, DialogsByAnswers.Keys.ToArray());
        }

        public ComplexDialog AddAnswerOption(string option, ComplexDialog dialog)
        {
            dialog.Name = Name;
            DialogsByAnswers.Add(option, dialog);
            background = GameManager.Instance.GetDialogBackGroundByProperties(Name, Text, DialogsByAnswers.Keys.ToArray());
            return this;
        }

        public ComplexDialog GetNextDialogByAnswer(int answerIndex)
        {
            return DialogsByAnswers.Values.ToList()[answerIndex];
        }

        public int AnswersCount()
        {
            return DialogsByAnswers.Count;
        }

        public new void DrawAt(SpriteBatch sprite, Vector2 location)
        {
            base.DrawAt(sprite, location);
            sprite.Draw(background, new Vector2(sprite.GraphicsDevice.Viewport.Width / 2 - background.Width / 2, sprite.GraphicsDevice.Viewport.Height / 4 - background.Height), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
    }
}
