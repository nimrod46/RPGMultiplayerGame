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
                foreach (var t in text.Split('\n'))
                {
                    string textLine = t;
                    Vector2 textSize = Vector2.Zero;
                    float xSum = 0;
                    float yMax = 0;
                    try
                    {
                        if((textLine.Split('§').Length - 1) % 2 != 0)
                        {
                            throw new Exception("Invalid color code");
                        }
                            
                        while (!string.IsNullOrWhiteSpace(textLine))
                        {
                            int firstIndexOfCmd = textLine.IndexOf('§');
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
                                int lastIndexOfCmd = textLine.IndexOf('§', 1);
                                colorCode = textLine.Substring(0, lastIndexOfCmd + 1).Replace("§", "");
                                textLine = textLine.Substring(lastIndexOfCmd + 1, textLine.Length - lastIndexOfCmd - 1);
                                Console.WriteLine(colorCode);
                            }

                            int color = (int)System.Drawing.KnownColor.White;

                            {
                                textSize = font.MeasureString(subTextPart);
                                reachTexts.Add(new ReachText(subTextPart, nextColor, new Vector2(xSum, Size.Y)));
                                yMax = Math.Max(yMax, textSize.Y);
                                xSum += textSize.X;
                                nextColor = Color.White;
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
                        Console.WriteLine("Warning: failed to parse color text with exception: ");
                        Console.WriteLine(e);
                        textLine = textLine.Replace("§", "");
                        textSize = font.MeasureString(textLine);
                        reachTexts.Add(new ReachText(textLine, nextColor, new Vector2(0, Size.Y)));
                        yMax = Math.Max(yMax, textSize.Y);
                        nextColor = Color.White;
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
