﻿// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPGMultiplayerGame.Managers;
using RPGMultiplayerGame.Ui;

namespace MonoGame_Textbox
{
    public class GameTextBox : UiComponent
    {
        public GraphicsDevice GraphicsDevice { get; set; }

        public Rectangle Area
        {
            get { return Renderer.Area; }
            set { Renderer.Area = value; }
        }

        public readonly Text Text;
        public readonly TextRenderer Renderer;
        public readonly Cursor Cursor;
        public event EventHandler EnterDown;
        private readonly List<string> lastTexts = new List<string>();
        private int currentMemoryTextIndex;
        private string clipboard;

        public bool Active { get; set; }

        public GameTextBox(Rectangle area, int maxCharacters, string text, GraphicsDevice graphicsDevice,
            SpriteFont spriteFont,
            Color cursorColor, Color selectionColor, int ticksPerToggle)
        {
            GraphicsDevice = graphicsDevice;

            Text = new Text(maxCharacters)
            {
                String = text
            };

            Renderer = new TextRenderer(this)
            {
                Area = area,
                Font = spriteFont,
                Color = Color.Black
            };

            Cursor = new Cursor(this, cursorColor, selectionColor, new Rectangle(0, 0, 1, 1), ticksPerToggle);

            InputManager.Instance.OnCharPressed += CharacterTyped;
            InputManager.Instance.OnKeyPressed += OnKeyPressed;
            IsVisible = true;
        }

        public void Dispose()
        {
            Delete();
        }

        public void TextSended()
        {
            lastTexts.Add(Text.String);
            currentMemoryTextIndex = lastTexts.Count;
            Clear();
        }

        public void Clear()
        {
            Text.RemoveCharacters(0, Text.Length);
            Cursor.TextCursor = 0;
            Cursor.SelectedChar = null;
        }

        private void OnKeyPressed(Keys keyPressed)
        {
            if (Active)
            {
                int oldPos = Cursor.TextCursor;
                switch (keyPressed)
                {
                    case Keys.Up:
                        if (!lastTexts.Any())
                        {
                            return;
                        }
                        currentMemoryTextIndex--;
                        if(currentMemoryTextIndex < 0)
                        {
                            currentMemoryTextIndex = 0;
                        }
                        Text.String = lastTexts[currentMemoryTextIndex];
                        break;
                    case Keys.Down:
                        if (!lastTexts.Any())
                        {
                            return;
                        }
                        currentMemoryTextIndex++;
                        if (currentMemoryTextIndex > lastTexts.Count - 1)
                        {
                            currentMemoryTextIndex = lastTexts.Count - 1;
                        }
                        Text.String = lastTexts[currentMemoryTextIndex];
                        break;
                    case Keys.Enter:
                        EnterDown?.Invoke(this, null);
                        break;
                    case Keys.Left:
                        if (InputManager.Instance.CtrlDown)
                        {
                            Cursor.TextCursor = IndexOfLastCharBeforeWhitespace(Cursor.TextCursor, Text.Characters);
                        }
                        else
                        {
                            Cursor.TextCursor--;
                        }
                        ShiftMod(oldPos);
                        break;
                    case Keys.Right:
                        if (InputManager.Instance.CtrlDown)
                        {
                            Cursor.TextCursor = IndexOfNextCharAfterWhitespace(Cursor.TextCursor, Text.Characters);
                        }
                        else
                        {
                            Cursor.TextCursor++;
                        }
                        ShiftMod(oldPos);
                        break;
                    case Keys.Home:
                        Cursor.TextCursor = 0;
                        ShiftMod(oldPos);
                        break;
                    case Keys.End:
                        Cursor.TextCursor = Text.Length;
                        ShiftMod(oldPos);
                        break;
                    case Keys.Delete:
                        if (DelSelection() == null && Cursor.TextCursor < Text.Length)
                        {
                            Text.RemoveCharacters(Cursor.TextCursor, Cursor.TextCursor + 1);
                        }
                        break;
                    case Keys.Back:
                        if (DelSelection() == null && Cursor.TextCursor > 0)
                        {
                            Text.RemoveCharacters(Cursor.TextCursor - 1, Cursor.TextCursor);
                            Cursor.TextCursor--;
                        }
                        break;
                    case Keys.A:
                        if (InputManager.Instance.CtrlDown)
                        {
                            if (Text.Length > 0)
                            {
                                Cursor.SelectedChar = 0;
                                Cursor.TextCursor = Text.Length;
                            }
                        }
                        break;
                    case Keys.C:
                        if (InputManager.Instance.CtrlDown)
                        {
                            clipboard = DelSelection(true);
                        }
                        break;
                    case Keys.X:
                        if (InputManager.Instance.CtrlDown)
                        {
                            if (Cursor.SelectedChar.HasValue)
                            {
                                clipboard = DelSelection();
                            }
                        }
                        break;
                    case Keys.V:
                        if (InputManager.Instance.CtrlDown)
                        {
                            if (clipboard != null)
                            {
                                DelSelection();
                                foreach (char c in clipboard)
                                {
                                    if (Text.Length < Text.MaxLength)
                                    {
                                        Text.InsertCharacter(Cursor.TextCursor, c);
                                        Cursor.TextCursor++;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void ShiftMod(int oldPos)
        {
            if (InputManager.Instance.ShiftDown)
            {
                if (Cursor.SelectedChar == null)
                {
                    Cursor.SelectedChar = oldPos;
                }
            }
            else
            {
                Cursor.SelectedChar = null;
            }
        }

        private void CharacterTyped(char character)
        {
            if (Active && !InputManager.Instance.CtrlDown)
            {
                if (IsLegalCharacter(Renderer.Font, character) && !character.Equals('\r') &&
                    !character.Equals('\n'))
                {
                    DelSelection();
                    if (Text.Length < Text.MaxLength)
                    {
                        Text.InsertCharacter(Cursor.TextCursor, character);
                        Cursor.TextCursor++;
                    }
                }
            }
        }

        private string DelSelection(bool fakeForCopy = false)
        {
            if (!Cursor.SelectedChar.HasValue)
            {
                return null;
            }
            int tc = Cursor.TextCursor;
            int sc = Cursor.SelectedChar.Value;
            int min = Math.Min(sc, tc);
            int max = Math.Max(sc, tc);
            string result = Text.String.Substring(min, max - min);

            if (!fakeForCopy)
            {
                Text.Replace(Math.Min(sc, tc), Math.Max(sc, tc), string.Empty);
                if (Cursor.SelectedChar.Value < Cursor.TextCursor)
                {
                    Cursor.TextCursor -= tc - sc;
                }
                Cursor.SelectedChar = null;
            }
            return result;
        }

        public static bool IsLegalCharacter(SpriteFont font, char c)
        {
            return font.Characters.Contains(c) || c == '\r' || c == '\n';
        }

        public static int IndexOfNextCharAfterWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;
            char c = chars[pos];
            bool whiteSpaceFound = false;
            while (true)
            {
                if (c.Equals(' '))
                {
                    whiteSpaceFound = true;
                }
                else if (whiteSpaceFound)
                {
                    return pos;
                }

                ++pos;
                if (pos >= chars.Length)
                {
                    return chars.Length;
                }
                c = chars[pos];
            }
        }

        public static int IndexOfLastCharBeforeWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;

            bool charFound = false;
            while (true)
            {
                --pos;
                if (pos <= 0)
                {
                    return 0;
                }
                var c = chars[pos];

                if (c.Equals(' '))
                {
                    if (charFound)
                    {
                        return ++pos;
                    }
                }
                else
                {
                    charFound = true;
                }
            }
        }

        public void Update()
        {
            Renderer.Update();
            Cursor.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Renderer.Draw(spriteBatch);
            if (Active)
            {
                Cursor.Draw(spriteBatch);
            }
        }
    }
}