using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGMultiplayerGame.Graphics
{
    public class ColoredTextRenderer : IGameDrawable
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

        public SpriteFont Font { get; }

        private readonly char colorCodeSplitter;
        private readonly Color defaultColor;
        private List<ReachText> reachTexts = new List<ReachText>();
        private string text;

        public ColoredTextRenderer(SpriteFont font, string text, char colorCodeSplitter, Vector2 position, Color defaultColor, float layer)
        {
            this.Font = font;
            Size = Vector2.Zero;
            Text = text;
            this.colorCodeSplitter = colorCodeSplitter;
            Position = position;
            this.defaultColor = defaultColor;
            Layer = layer;
        }

        private List<ReachText> GetGenerateTextParts(string text)
        {
            lock (reachTexts)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return new List<ReachText>();
                }
                Vector2 size = Vector2.Zero;
                List<ReachText> reachTexts = new List<ReachText>();
                Color nextColor = defaultColor;
                foreach (var t in text.Split('\n'))
                {
                    string textLine = t;
                    Vector2 textSize = Vector2.Zero;
                    float xSum = 0;
                    float yMax = 0;
                    try
                    {
                        if((textLine.Split(colorCodeSplitter).Length - 1) % 2 != 0)
                        {
                            throw new Exception("Invalid color code");
                        }
                            
                        while (!string.IsNullOrWhiteSpace(textLine))
                        {
                            int firstIndexOfCmd = textLine.IndexOf(colorCodeSplitter);
                            string subTextPart;
                            string colorCode;
                            if (firstIndexOfCmd == -1)
                            {
                                subTextPart = textLine;
                                textLine = "";
                                colorCode = "";
                            }
                            else
                            {
                                subTextPart = textLine.Substring(0, firstIndexOfCmd);
                                textLine = textLine.Substring(firstIndexOfCmd, textLine.Length - firstIndexOfCmd);
                                int lastIndexOfCmd = textLine.IndexOf(colorCodeSplitter, 1);
                                colorCode = textLine.Substring(0, lastIndexOfCmd + 1).Replace(colorCodeSplitter.ToString(), "");
                                textLine = textLine.Substring(lastIndexOfCmd + 1, textLine.Length - lastIndexOfCmd - 1);
                            }

                            int color = (int)System.Drawing.KnownColor.White;

                            {
                                textSize = Font.MeasureString(subTextPart);
                                reachTexts.Add(new ReachText(subTextPart, nextColor, new Vector2(xSum, size.Y)));
                                yMax = Math.Max(yMax, textSize.Y);
                                xSum += textSize.X;
                                nextColor = defaultColor;
                            }

                            if (int.TryParse(colorCode, out color))
                            {
                                System.Drawing.Color systemColor = System.Drawing.Color.FromKnownColor((System.Drawing.KnownColor)color);
                                nextColor = Color.FromNonPremultiplied(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Warning: failed to parse color text with exception:");
                        Console.WriteLine(e);
                        textLine = textLine.Replace(colorCodeSplitter.ToString(), "");
                        textSize = Font.MeasureString(textLine);
                        reachTexts.Add(new ReachText(textLine, nextColor, new Vector2(0, size.Y)));
                        yMax = Math.Max(yMax, textSize.Y);
                        nextColor = defaultColor;
                    }
                    size = new Vector2(Math.Max(textSize.X, size.X), size.Y + yMax);
                }
                Size = size;
                return reachTexts;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            lock(reachTexts)
            {
                foreach (var reachText in reachTexts)
                {
                    spriteBatch.DrawString(Font, reachText.Text, Position + reachText.Position, reachText.Color, 0, Vector2.Zero, 1, SpriteEffects.None, Layer);
                }
            }
        }
    }
}
