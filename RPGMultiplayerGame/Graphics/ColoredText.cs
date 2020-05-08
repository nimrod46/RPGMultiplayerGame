using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Graphics
{
    public class ColoredText : IGameDrawable
    {

        private struct ReachText
        {
            public string Text { get; }

            public Color Color { get; }

            public Vector2 Position { get; }

            public ReachText(string text, Color color, Vector2 position)
            {
                this.Text = text;
                this.Color = color;
                Position = position;
            }
        }

        public string Text
        {
            get => text; 
            set
            {
                text = value;
                reachTexts = GetGenerateTextParts(text);
            }
        }

        public Vector2 Position { get; set; }

        public Vector2 Size { get; protected set; }
        public float Layer { get; }

        private readonly SpriteFont font;
        private List<ReachText> reachTexts = new List<ReachText>();
        private string text;

        public ColoredText(SpriteFont font, string text, Vector2 position, float layer)
        {
            this.font = font;
            Size = Vector2.Zero;
            Text = text;
            Position = position;
            Layer = layer;
        }

        //§15§asedfs§12§sdfgs
        /*
         * 15
         * asedfs
         * 12
         * sdfgs
         */
        private List<ReachText> GetGenerateTextParts(string text)
        {
            lock (reachTexts)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return new List<ReachText>();
                }
                Size = Vector2.Zero;
                List<ReachText> reachTexts = new List<ReachText>();
                Color nextColor = Color.White;
                foreach (var textPart in text.Split('\n'))
                {
                    Vector2 textSize = Vector2.Zero;
                    float xSum = 0;
                    float yMax = 0;
                    if (!textPart.Contains("§"))
                    {
                        textSize = font.MeasureString(textPart);
                        reachTexts.Add(new ReachText(textPart, nextColor, new Vector2(xSum, Size.Y)));
                        yMax = Math.Max(yMax, textSize.Y);
                        xSum += textSize.X;
                        Size = new Vector2(Math.Max(textSize.X, Size.X), Size.Y + yMax);
                        continue;
                    }
                    foreach (var subTextPart in textPart.Split('§'))
                    {
                        if (string.IsNullOrWhiteSpace(subTextPart))
                        {
                            continue;
                        }

                        int colorCode = (int)System.Drawing.KnownColor.White;
                        if (int.TryParse(subTextPart, out colorCode))
                        {
                            System.Drawing.Color systemColor = System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)colorCode);
                            nextColor = Color.FromNonPremultiplied(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
                        }
                        else
                        {
                            textSize = font.MeasureString(subTextPart);
                            reachTexts.Add(new ReachText(subTextPart, nextColor, new Vector2(xSum, Size.Y)));
                            yMax = Math.Max(yMax, textSize.Y);
                            xSum += textSize.X;
                            nextColor = Color.White;
                        }
                    }
                    Size = new Vector2(Math.Max(textSize.X, Size.X), Size.Y + yMax);
                }
                return reachTexts;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            lock(reachTexts)
            {
                foreach (var reachText in reachTexts)
                {
                    spriteBatch.DrawString(font, reachText.Text, Position + reachText.Position, reachText.Color, 0, Vector2.Zero, 1, SpriteEffects.None, Layer);
                }
            }
        }
    }
}
